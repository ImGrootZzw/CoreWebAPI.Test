using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreWebAPI.Common.Helper;
using NPOI.SS.Formula.Functions;
using Newtonsoft.Json;
using MongoDB.Bson.Serialization;
using System;
using MongoDB.Driver.GridFS;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;
using System.Data;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace CoreWebAPI.Common.DB.MongoDB
{
    public class MongoDBHelper
    {
        ///// <summary>
        ///// 数据库连接
        ///// </summary>
        private readonly string conn = Appsettings.App("Startup", "MongoDB", "Connection");
        ///// <summary>
        ///// 指定的数据库
        ///// </summary>
        private readonly string dbName = Appsettings.App("Startup", "MongoDB", "DataBase");
        ///// <summary>
        ///// 数据库集合名称
        ///// </summary>
        private readonly string collectionName = Appsettings.App("Startup", "MongoDB", "CollectionName");

        // 定义客户端
        public IMongoClient _client;
        // 定义接口
        public IMongoDatabase _database;
        // 定义集合名称
        public string _collectionName;
        // 定义集合
        public IMongoCollection<BsonDocument> _collection;

        public MongoDBHelper(string connStr, string dbNameStr, string collectionNameStr)
        {
            // 创建并实例化客户端
            _client = new MongoClient(connStr.GetIsEmptyOrNull() ? conn : connStr);
            //  根据数据库名称实例化数据库
            _database = _client.GetDatabase(dbNameStr.GetIsEmptyOrNull() ? dbName : dbNameStr);
            _collectionName = collectionNameStr.GetIsEmptyOrNull() ? collectionName: collectionNameStr;
            // 根据集合名称获取集合
            _collection = _database.GetCollection<BsonDocument>(_collectionName);
        }

        public void Clear()
        {
            _database.DropCollection(_collectionName);
        }

        public void Dispose()
        {
            _database.DropCollection(_collectionName);
            _collection = null;
        }

        public FilterBsonDocument MongoDbDynamicBsonDocumentGet(object searchModel, Dictionary<string, string> fieldMapping)
        {
            FilterBsonDocument filterBsonDocument = new FilterBsonDocument();
            BsonDocument filterBsonDocumentMain = new BsonDocument();
            BsonDocument filterBsonDocumentDetail = new BsonDocument();

            var dynamicSearchModel = JsonHelper.JsonToObject<DynamicSearchModel>(JsonHelper.ObjectToJson(searchModel));
            if (dynamicSearchModel == null || dynamicSearchModel.ConditionList == null)
                return new FilterBsonDocument();

            foreach (var fieldCondition in dynamicSearchModel.ConditionList)
            {
                foreach (var condition in fieldCondition.Conditions)
                {
                    if (condition.FirstValue.GetIsEmptyOrNull())
                        continue;

                    if (!fieldMapping.ContainsKey(fieldCondition.Field))
                        SetFilterBsonDocument(fieldCondition.Field, condition.Type, condition.MongoDbType, condition.FirstValue, condition.SecondValue, ref filterBsonDocumentMain);
                    else
                        SetFilterBsonDocument(fieldMapping[fieldCondition.Field], condition.Type, condition.MongoDbType, condition.FirstValue, condition.SecondValue, ref filterBsonDocumentDetail);
                }
            }
            filterBsonDocument.Main = filterBsonDocumentMain;
            filterBsonDocument.Detail = filterBsonDocumentDetail;
            return filterBsonDocument;
        }

        public FilterDefinition<BsonDocument> MongoDbDynamicFilterDefinitionGet(object searchModel)
        {
            var filterBuilder = Builders<BsonDocument>.Filter;
            var filterList = new List<FilterDefinition<BsonDocument>>();

            var dynamicSearchModel = JsonHelper.JsonToObject<DynamicSearchModel>(JsonHelper.ObjectToJson(searchModel));
            if (dynamicSearchModel == null || dynamicSearchModel.ConditionList == null)
                return filterBuilder.Empty;

            foreach (var fieldCondition in dynamicSearchModel.ConditionList)
            {
                foreach (var condition in fieldCondition.Conditions)
                {
                    //if (condition.FirstValue.GetIsEmptyOrNull())
                    //    continue;

                    FilterDefinition<BsonDocument> fieldFilter = GetFieldFilter(fieldCondition.Field, condition.Type, condition.MongoDbType, condition.FirstValue, condition.SecondValue);
                    if (fieldFilter != null)
                    {
                        filterList.Add(fieldFilter);
                    }
                }
            }

            if (filterList.Count > 0)
            {
                var finalFilter = filterBuilder.And(filterList);
                return finalFilter;
            }
            else
            {
                return filterBuilder.Empty;
            }
        }

        /// <summary>
        /// 根据字段、MongoDbType、值产生FilterDefinition
        /// </summary>
        /// <returns></returns>
        private FilterDefinition<BsonDocument> GetFieldFilter(string fieldName, string logicType, string mongoDbType, object firstValue, object secondValue)
        {
            var filterBuilder = Builders<BsonDocument>.Filter;
            FilterDefinition<BsonDocument> fieldFilter = null;
            switch (logicType.ToLower())
            {
                case "eq":
                    switch (mongoDbType.ToLower())
                    {
                        case "string":
                        case "decimal":
                            if (firstValue.GetIsEmptyOrNull())
                            {
                                fieldFilter = filterBuilder.Eq(fieldName, firstValue.GetCString());
                            }
                            else
                            {
                                fieldFilter = filterBuilder.Regex(fieldName, new BsonRegularExpression("^"+ firstValue.GetCString() + "$", "i"));
                            }
                            break;
                        case "int":
                            fieldFilter = filterBuilder.Eq(fieldName, firstValue.GetCInt());
                            break;
                        case "date":
                            DateTime dt = firstValue.GetCDate();
                            fieldFilter = filterBuilder.Eq(fieldName, new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0, DateTimeKind.Utc));
                            break;
                        case "boolean":
                            fieldFilter = filterBuilder.Eq(fieldName, firstValue.GetCBool());
                            break;
                        case "id":
                            fieldFilter = filterBuilder.Eq(fieldName, ObjectId.Parse(firstValue.GetCString()));
                            break;
                    }
                    break;
                case "ne":
                    switch (mongoDbType.ToLower())
                    {
                        case "string":
                        case "decimal":
                            if (firstValue.GetIsEmptyOrNull())
                            {
                                fieldFilter = filterBuilder.Ne(fieldName, firstValue.GetCString());
                            }
                            else
                            {
                                fieldFilter = filterBuilder.Regex(fieldName, new BsonRegularExpression("^(?!" + firstValue + ")", "i"));
                            }
                            break;
                        case "int":
                            fieldFilter = filterBuilder.Ne(fieldName, firstValue.GetCInt());
                            break;
                        case "date":
                            DateTime dt = firstValue.GetCDate();
                            fieldFilter = filterBuilder.Ne(fieldName, new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0, DateTimeKind.Utc));
                            break;
                        case "boolean":
                            fieldFilter = filterBuilder.Ne(fieldName, firstValue.GetCBool());
                            break;
                        case "id":
                            fieldFilter = filterBuilder.Eq(fieldName, ObjectId.Parse(firstValue.GetCString()));
                            break;
                    }
                    break;
                case "lt":
                    switch (mongoDbType.ToLower())
                    {
                        case "string":
                        case "decimal":
                            fieldFilter = filterBuilder.Lt(fieldName, firstValue.GetCString());
                            break;
                        case "int":
                            fieldFilter = filterBuilder.Lt(fieldName, firstValue.GetCInt());
                            break;
                        case "date":
                            fieldFilter = filterBuilder.Lt(fieldName, firstValue.GetCDate());
                            break;
                        case "boolean":
                            fieldFilter = filterBuilder.Lt(fieldName, firstValue.GetCBool());
                            break;
                        case "id":
                            fieldFilter = filterBuilder.Eq(fieldName, ObjectId.Parse(firstValue.GetCString()));
                            break;
                    }
                    break;
                case "lte":
                    switch (mongoDbType.ToLower())
                    {
                        case "string":
                        case "decimal":
                            fieldFilter = filterBuilder.Lte(fieldName, firstValue.GetCString());
                            break;
                        case "int":
                            fieldFilter = filterBuilder.Lte(fieldName, firstValue.GetCInt());
                            break;
                        case "date":
                            fieldFilter = filterBuilder.Lte(fieldName, firstValue.GetCDate());
                            break;
                        case "boolean":
                            fieldFilter = filterBuilder.Lte(fieldName, firstValue.GetCBool());
                            break;
                        case "id":
                            fieldFilter = filterBuilder.Eq(fieldName, ObjectId.Parse(firstValue.GetCString()));
                            break;
                    }
                    break;
                case "gt":
                    switch (mongoDbType.ToLower())
                    {
                        case "string":
                        case "decimal":
                            fieldFilter = filterBuilder.Gt(fieldName, firstValue.GetCString());
                            break;
                        case "int":
                            fieldFilter = filterBuilder.Gt(fieldName, firstValue.GetCInt());
                            break;
                        case "date":
                            fieldFilter = filterBuilder.Gt(fieldName, firstValue.GetCDate());
                            break;
                        case "boolean":
                            fieldFilter = filterBuilder.Gt(fieldName, firstValue.GetCBool());
                            break;
                        case "id":
                            fieldFilter = filterBuilder.Eq(fieldName, ObjectId.Parse(firstValue.GetCString()));
                            break;
                    }
                    break;
                case "gte":
                    switch (mongoDbType.ToLower())
                    {
                        case "string":
                        case "decimal":
                            fieldFilter = filterBuilder.Gte(fieldName, firstValue.GetCString());
                            break;
                        case "int":
                            fieldFilter = filterBuilder.Gte(fieldName, firstValue.GetCInt());
                            break;
                        case "date":
                            fieldFilter = filterBuilder.Gte(fieldName, firstValue.GetCDate());
                            break;
                        case "boolean":
                            fieldFilter = filterBuilder.Gte(fieldName, firstValue.GetCBool());
                            break;
                        case "id":
                            fieldFilter = filterBuilder.Eq(fieldName, ObjectId.Parse(firstValue.GetCString()));
                            break;
                    }
                    break;
                case "like":
                    switch (mongoDbType.ToLower())
                    {
                        case "string":
                        case "decimal":
                            fieldFilter = filterBuilder.Regex(fieldName, new BsonRegularExpression(".*" + Regex.Escape(firstValue.GetCString().Trim('*')) + ".*", "i"));
                            break;
                    }
                    break;
                case "in":
                    BsonArray bsonArray = new BsonArray();
                    switch (mongoDbType.ToLower())
                    {
                        case "id":
                            List<string> listId = JsonHelper.JsonToObject<List<string>>(JsonHelper.ObjectToJson(firstValue));
                            foreach (var item in listId)
                            {
                                bsonArray.Add(ObjectId.Parse(item));
                            }
                            fieldFilter = filterBuilder.In(fieldName, bsonArray);
                            break;
                        case "lower":
                            var regexList = new List<BsonRegularExpression>();
                            List<string> list = JsonHelper.JsonToObject<List<string>>(JsonHelper.ObjectToJson(firstValue));
                            foreach (var item in list)
                            {
                                var regexPattern = new BsonRegularExpression(item, "i");
                                regexList.Add(regexPattern);
                            }
                            fieldFilter = filterBuilder.In(fieldName, regexList);
                            break;
                        default:
                            bsonArray = BsonSerializer.Deserialize<BsonArray>(JsonHelper.ObjectToJson(firstValue));
                            fieldFilter = filterBuilder.In(fieldName, bsonArray);
                            break;
                    }
                    break;
                case "between":
                    FilterDefinition<BsonDocument> gteFilter = null;
                    FilterDefinition<BsonDocument> lteFilter = null;
                    switch (mongoDbType.ToLower())
                    {
                        case "string":
                        case "decimal":
                            gteFilter = filterBuilder.Gte(fieldName, firstValue.GetCString());
                            lteFilter = filterBuilder.Lte(fieldName, secondValue.GetCString());
                            fieldFilter = filterBuilder.And(gteFilter, lteFilter);
                            break;
                        case "int":
                            gteFilter = filterBuilder.Gte(fieldName, firstValue.GetCInt());
                            lteFilter = filterBuilder.Lte(fieldName, secondValue.GetCInt());
                            fieldFilter = filterBuilder.And(gteFilter, lteFilter);
                            break;
                        case "date":
                            gteFilter = filterBuilder.Gte(fieldName, firstValue.GetCDate());
                            lteFilter = filterBuilder.Lte(fieldName, secondValue.GetCDate());
                            fieldFilter = filterBuilder.And(gteFilter, lteFilter);
                            break;
                        case "id":
                            gteFilter = filterBuilder.Gte(fieldName, ObjectId.Parse(firstValue.GetCString()));
                            lteFilter = filterBuilder.Lte(fieldName, ObjectId.Parse(secondValue.GetCString()));
                            fieldFilter = filterBuilder.And(gteFilter, lteFilter);
                            break;
                    }
                    break;
            }

            return fieldFilter;
        }

        /// <summary>
        /// 根据字段、MongoDbType、值设置BsonDocument
        /// </summary>
        /// <returns></returns>
        private void SetFilterBsonDocument(string fieldName, string logicType, string mongoDbType, object firstValue, object secondValue, ref BsonDocument filterBsonDocument)
        {
            switch (logicType.ToLower())
            {
                case "eq":
                    switch (mongoDbType.ToLower())
                    {
                        case "string":
                        case "decimal":
                            filterBsonDocument.Add(fieldName, new BsonDocument("$regex", new BsonRegularExpression(firstValue.GetCString(), "i")));
                            break;
                        case "int":
                            filterBsonDocument.Add(fieldName, new BsonDocument("$eq", firstValue.GetCInt()));
                            break;
                        case "date":
                            DateTime dt = firstValue.GetCDate();
                            filterBsonDocument.Add(fieldName, new BsonDocument("$eq", new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0, DateTimeKind.Utc)));
                            break;
                        case "boolean":
                            filterBsonDocument.Add(fieldName, new BsonDocument("$eq", firstValue.GetCBool()));
                            break;
                    }
                    break;
                case "ne":
                    switch (mongoDbType.ToLower())
                    {
                        case "string":
                        case "decimal":
                            filterBsonDocument.Add(fieldName, new BsonDocument("$regex", new BsonRegularExpression("^(?!" + firstValue + ")", "i")));
                            break;
                        case "int":
                            filterBsonDocument.Add(fieldName, new BsonDocument("$ne", firstValue.GetCInt()));
                            break;
                        case "date":
                            DateTime dt = firstValue.GetCDate();
                            filterBsonDocument.Add(fieldName, new BsonDocument("$ne", new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0, DateTimeKind.Utc)));
                            break;
                        case "boolean":
                            filterBsonDocument.Add(fieldName, new BsonDocument("$ne", firstValue.GetCBool()));
                            break;
                    }
                    break;
                case "lt":
                    switch (mongoDbType.ToLower())
                    {
                        case "string":
                        case "decimal":
                            filterBsonDocument.Add(fieldName, new BsonDocument("$lt", firstValue.GetCString()));
                            break;
                        case "int":
                            filterBsonDocument.Add(fieldName, new BsonDocument("$lt", firstValue.GetCInt()));
                            break;
                        case "date":
                            filterBsonDocument.Add(fieldName, new BsonDocument("$lt", firstValue.GetCDate()));
                            break;
                        case "boolean":
                            filterBsonDocument.Add(fieldName, new BsonDocument("$lt", firstValue.GetCBool()));
                            break;
                    }
                    break;
                case "lte":
                    switch (mongoDbType.ToLower())
                    {
                        case "string":
                        case "decimal":
                            filterBsonDocument.Add(fieldName, new BsonDocument("$lte", firstValue.GetCString()));
                            break;
                        case "int":
                            filterBsonDocument.Add(fieldName, new BsonDocument("$lte", firstValue.GetCInt()));
                            break;
                        case "date":
                            filterBsonDocument.Add(fieldName, new BsonDocument("$lte", firstValue.GetCDate()));
                            break;
                        case "boolean":
                            filterBsonDocument.Add(fieldName, new BsonDocument("$lte", firstValue.GetCBool()));
                            break;
                    }
                    break;
                case "gt":
                    switch (mongoDbType.ToLower())
                    {
                        case "string":
                        case "decimal":
                            filterBsonDocument.Add(fieldName, new BsonDocument("$gt", firstValue.GetCString()));
                            break;
                        case "int":
                            filterBsonDocument.Add(fieldName, new BsonDocument("$gt", firstValue.GetCInt()));
                            break;
                        case "date":
                            filterBsonDocument.Add(fieldName, new BsonDocument("$gt", firstValue.GetCDate()));
                            break;
                        case "boolean":
                            filterBsonDocument.Add(fieldName, new BsonDocument("$gt", firstValue.GetCBool()));
                            break;
                    }
                    break;
                case "gte":
                    switch (mongoDbType.ToLower())
                    {
                        case "string":
                        case "decimal":
                            filterBsonDocument.Add(fieldName, new BsonDocument("$gte", firstValue.GetCString()));
                            break;
                        case "int":
                            filterBsonDocument.Add(fieldName, new BsonDocument("$gte", firstValue.GetCInt()));
                            break;
                        case "date":
                            filterBsonDocument.Add(fieldName, new BsonDocument("$gte", firstValue.GetCDate()));
                            break;
                        case "boolean":
                            filterBsonDocument.Add(fieldName, new BsonDocument("$gte", firstValue.GetCBool()));
                            break;
                    }
                    break;
                case "like":
                    switch (mongoDbType.ToLower())
                    {
                        case "string":
                        case "decimal":
                            filterBsonDocument.Add(fieldName, new BsonDocument("$regex", new BsonRegularExpression(".*" + firstValue.GetCString() + ".*", "i")));
                            break;
                    }
                    break;
                case "in":
                    BsonArray bsonArray = BsonSerializer.Deserialize<BsonArray>(JsonHelper.ObjectToJson(firstValue));
                    filterBsonDocument.Add(fieldName, new BsonDocument("$in", bsonArray));
                    break;
                case "between":
                    FilterDefinition<BsonDocument> gteFilter = null;
                    FilterDefinition<BsonDocument> lteFilter = null;
                    switch (mongoDbType.ToLower())
                    {
                        case "string":
                        case "decimal":
                            filterBsonDocument.Add(fieldName, new BsonDocument("$gte", firstValue.GetCString()));
                            filterBsonDocument.Add(fieldName, new BsonDocument("$lte", secondValue.GetCString()));
                            break;
                        case "int":
                            filterBsonDocument.Add(fieldName, new BsonDocument("$gte", firstValue.GetCInt()));
                            filterBsonDocument.Add(fieldName, new BsonDocument("$lte", secondValue.GetCInt()));
                            break;
                        case "date":
                            filterBsonDocument.Add(fieldName, new BsonDocument("$gte", firstValue.GetCDate()));
                            filterBsonDocument.Add(fieldName, new BsonDocument("$lte", secondValue.GetCDate()));
                            break;
                    }
                    break;
            }
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="query">查询条件</param>
        /// <param name="selectField">查询字段</param>
        /// <param name="ignoreField">忽略字段</param>
        /// <returns></returns>
        public List<BsonDocument> Query(FilterDefinition<BsonDocument> query = null, object sortObj = null, List<string> selectField = null, List<string> ignoreField = null)
        {
            var builder = Builders<BsonDocument>.Projection;
            var projection = builder.Combine();
            var sortDefinitionBuilder = Builders<BsonDocument>.Sort;
            SortDefinition<BsonDocument> sort = null;
            if (sortObj.GetIsNotEmptyOrNull())
            {
                List<SortModel> sortList = JsonHelper.JsonToObject<List<SortModel>>(JsonHelper.ObjectToJson(sortObj));
                foreach (var item in sortList)
                {
                    if (item.Ascending)
                        sort = sort == null ? sortDefinitionBuilder.Ascending(item.SortParam) : sort.Ascending(item.SortParam);
                    else
                        sort = sort == null ? sortDefinitionBuilder.Descending(item.SortParam) : sort.Descending(item.SortParam);
                }
            }
            if (selectField.GetIsNotEmptyOrNull())
            {
                foreach (var item in selectField)
                    projection = projection.Include(item);
            }
            if (ignoreField.GetIsNotEmptyOrNull())
            {
                foreach (var item in ignoreField)
                    projection = projection.Exclude(item);
            }

            List<BsonDocument> list = _collection.Find(query).Project(projection).Sort(sort).ToList();
            return list;
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="query">查询条件</param>
        /// <param name="sortList">排序</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">一页展示数量</param>
        /// <returns></returns>
        public List<BsonDocument> QueryPage(out int count, FilterDefinition<BsonDocument> query = null, object sortObj = null, int pageIndex = 1, int pageSize = 20, List<string> selectField = null, List<string> ignoreField = null)
        {
            count = 0;
            var sortDefinitionBuilder = Builders<BsonDocument>.Sort;
            SortDefinition<BsonDocument> sort = null;
            var builder = Builders<BsonDocument>.Projection;
            var projection = builder.Combine();
            if (sortObj.GetIsNotEmptyOrNull())
            {
                List<SortModel> sortList = JsonHelper.JsonToObject<List<SortModel>>(JsonHelper.ObjectToJson(sortObj));
                foreach (var item in sortList)
                {
                    if (item.Ascending)
                        sort = sort == null ? sortDefinitionBuilder.Ascending(item.SortParam) : sort.Ascending(item.SortParam);
                    else
                        sort = sort == null ? sortDefinitionBuilder.Descending(item.SortParam) : sort.Descending(item.SortParam);
                }
            }
            if (selectField.GetIsNotEmptyOrNull())
            {
                foreach (var item in selectField)
                    projection = projection.Include(item);
            }
            if (ignoreField.GetIsNotEmptyOrNull())
            {
                foreach (var item in ignoreField)
                    projection = projection.Exclude(item);
            }
            int skip = (pageIndex - 1) * pageSize;
            count = _collection.Find(query).Project(projection).Sort(sort).CountDocuments().GetCInt();
            List<BsonDocument> list = _collection.Find(query).Project(projection).Sort(sort).Skip(skip).Limit(pageSize).ToList();
            return list;
        }

        public (List<BsonDocument> list,int totalCount) QueryPageGroupBy(BsonDocument group, FilterDefinition<BsonDocument> query, int pageIndex = 1, int pageSize = 20)
        {
            int skip = (pageIndex - 1) * pageSize;
            int totalCount = _collection.Aggregate().Match(query).Group(group).ToList().Count;
            List<BsonDocument> list = _collection.Aggregate().Match(query).Group(group)
                //.Facet(
                //// 子管道1：获取分页数据
                //PipelineStageDefinition<BsonDocument, BsonDocument>.Create(
                //    new BsonDocument("$skip", skip),
                //    new BsonDocument("$limit", pageSize)
                //),
                //// 子管道2：获取总记录数
                //PipelineStageDefinition<BsonDocument, BsonDocument>.Create(
                //    new BsonDocument("$count", "totalCount")
                //))
                .Skip(skip).Limit(pageSize).ToList();
            return (list, totalCount);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="fromCollection">联查目标集合</param>
        /// <param name="localField">当前集合中的关联字段</param>
        /// <param name="foreignField">目标集合中的关联字段</param>
        /// <param name="asField">联查结果保存字段名</param>
        /// <param name="selectField">查询字段</param>
        /// <param name="ignoreField">忽略字段</param>
        /// <returns></returns>
        public object QueryMuch(string fromCollection, string localField, string foreignField, string asField, object sortObj = null, List<string> selectField = null, List<string> ignoreField = null)
        {
            var builder = Builders<BsonDocument>.Projection;
            BsonDocument project = new BsonDocument();
            BsonDocument sort = new BsonDocument();
            if (sortObj.GetIsNotEmptyOrNull())
            {
                List<SortModel> sortList = JsonHelper.JsonToObject<List<SortModel>>(JsonHelper.ObjectToJson(sortObj));
                foreach (var item in sortList)
                {
                    if (item.Ascending)
                        sort.Add(item.SortParam, 1);
                    else
                        sort.Add(item.SortParam, -1);
                }
            }
            if (selectField.GetIsNotEmptyOrNull())
            {
                foreach (var item in selectField)
                    project.Add(item, 1);
            }
            if (ignoreField.GetIsNotEmptyOrNull())
            {
                foreach (var item in ignoreField)
                    project.Add(item, 0);
            }
            var pipeline = new BsonDocument[]
            {
                // 联查
                new BsonDocument("$lookup",
                    new BsonDocument
                    {
                        { "from", fromCollection },
                        { "localField", localField }, // 当前集合中的关联字段
                        { "foreignField", foreignField }, // 目标集合中的关联字段
                        { "as", asField }
                    }
                ),
                // 忽略、显示
                new BsonDocument("$project",
                    project
                ),
                // 排序
                new BsonDocument("$sort",
                    sort
                )
            };

            List<BsonDocument> list = _collection.Aggregate<BsonDocument>(pipeline).ToList();
            return list;
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="fromCollection">联查目标集合</param>
        /// <param name="localField">当前集合中的关联字段</param>
        /// <param name="foreignField">目标集合中的关联字段</param>
        /// <param name="asField">联查结果保存字段名</param>
        /// <param name="selectField">查询字段</param>
        /// <param name="ignoreField">忽略字段</param>
        /// <returns></returns>
        public object QueryMuchPage(string fromCollection, string localField, string foreignField, string asField, object sortObj = null, int pageIndex = 1, int pageSize = 20, List<string> selectField = null, List<string> ignoreField = null)
        {
            var builder = Builders<BsonDocument>.Projection;
            Dictionary<string, int> project = new Dictionary<string, int>();
            BsonDocument sort = new BsonDocument();
            if (sortObj.GetIsNotEmptyOrNull())
            {
                List<SortModel> sortList = JsonHelper.JsonToObject<List<SortModel>>(JsonHelper.ObjectToJson(sortObj));
                foreach (var item in sortList)
                {
                    if (item.Ascending)
                        sort.Add(item.SortParam, 1);
                    else
                        sort.Add(item.SortParam, -1);
                }
            }
            if (selectField.GetIsNotEmptyOrNull())
            {
                foreach (var item in selectField)
                    project.Add(item, 1);
            }
            if (ignoreField.GetIsNotEmptyOrNull())
            {
                foreach (var item in ignoreField)
                    project.Add(item, 0);
            }
            int skip = (pageIndex - 1) * pageSize;
            var pipeline = new List<BsonDocument>
            {
                // 联查
                new BsonDocument("$lookup",
                    new BsonDocument
                    {
                        { "from", fromCollection },
                        { "localField", localField }, // 当前集合中的关联字段
                        { "foreignField", foreignField }, // 目标集合中的关联字段
                        { "as", asField }
                    }
                ),
                // 分页
                new BsonDocument("$skip",skip),
                new BsonDocument("$limit",pageSize),
                // 排序
                new BsonDocument("$sort",
                    sort
                )
            };
            // 忽略、显示
            if (project.Count > 0)
                pipeline.Add(new BsonDocument("$project", project.ToBsonDocument()));

            List<BsonDocument> list = _collection.Aggregate<BsonDocument>(pipeline).ToList();
            return list;
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="pipeline">pipeline</param>
        /// <returns></returns>
        public object QueryMuch(List<BsonDocument> pipeline)
        {
            List<BsonDocument> list = _collection.Aggregate<BsonDocument>(pipeline).ToList();
            return list.ToJson(new JsonWriterSettings { Indent = true });
        }
        public T Query<T>(Dictionary<string, object> dict)
        {
            var query = new BsonDocument(dict);
            IAsyncCursor<BsonDocument> result = _collection.FindSync(query);
            if (result.MoveNext())
            {
                if (result.Current is List<BsonDocument> list)
                {
                    BsonDocument bd = list[0];
                    bd.Remove("_id");
                    //bd.Remove(defaultColumn);

                    return (T)JsonHelper.JsonToObject(bd.ToString(), typeof(T));
                }
                else
                {
                    return default;
                }
            }
            else
            {
                return default;
            }
        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public void Add<TEntity>(TEntity entity)
        {
            _collection.InsertOne(entity.ToBsonDocument());
        }
        public async Task AddAsync<TEntity>(TEntity entity)
        {
            await _collection.InsertOneAsync(entity.ToBsonDocument());
        }
        public void AddMany<TEntity>(List<TEntity> entities)
        {
            List<BsonDocument> lb = new List<BsonDocument>();
            entities.ForEach(it =>
            {
                lb.Add(it.ToBsonDocument());
            });
            _collection.InsertMany(lb);
        }
        public async Task AddManyAsync<TEntity>(List<TEntity> entities)
        {
            List<BsonDocument> lb = new List<BsonDocument>();
            entities.ForEach(it =>
            {
                lb.Add(it.ToBsonDocument());
            });
            await _collection.InsertManyAsync(lb);
        }
        public void AddJson(string json)
        {
            BsonDocument document = BsonDocument.Parse(json);
            _collection.InsertOne(document);
        }
        public void AddJson(BsonDocument document)
        {
            _collection.InsertOne(document);
        }
        public async Task AddJsonAsync(string json)
        {
            BsonDocument document = BsonDocument.Parse(json);
            await _collection.InsertOneAsync(document);
        }
        public void AddJsonMany(string json)
        {
            DataTable dt = JsonHelper.NewJsonToDataTable(json);
            var documents = new List<BsonDocument>();
            foreach (DataRow item in dt.Rows)
            {
                var document = new BsonDocument();
                foreach (DataColumn column in dt.Columns)
                {
                    var columnName = column.ColumnName;
                    var columnValue = item[columnName];
                    document.Add(columnName, BsonValue.Create(columnValue));
                }
                documents.Add(document);
            }
            _collection.InsertMany(documents);
        }
        public async Task AddJsonManyAsync(string json)
        {
            DataTable dt = JsonHelper.NewJsonToDataTable(json);
            var documents = new List<BsonDocument>();
            foreach (DataRow item in dt.Rows)
            {
                var document = new BsonDocument();
                foreach (DataColumn column in dt.Columns)
                {
                    var columnName = column.ColumnName;
                    var columnValue = item[columnName];
                    document.Add(columnName, BsonValue.Create(columnValue));
                }
                documents.Add(document);
            }
            await _collection.InsertManyAsync(documents);
        }

        public async Task AddJsonManyAsync(List<BsonDocument> documents)
        {
            await _collection.InsertManyAsync(documents);
        }

        /// <summary>
        /// 上传单个附件
        /// </summary>
        /// <param name="file">上传文件</param>
        /// <returns>MongoDB文件ID</returns>
        public string Upload(IFormFile file)
        {
            // 上传附件
            var bucket = new GridFSBucket(_database);
            var metadata = new BsonDocument
                {
                    { "filename" , file.FileName },
                    { "contentType" , "application/octet-stream" }
                };
            using (Stream fs = file.OpenReadStream())
            {
                bucket.UploadFromStream(file.FileName, fs, new GridFSUploadOptions
                {
                    Metadata = metadata
                });
            }
            return metadata["_id"].AsObjectId.GetCString();
        }

        ///// <summary>
        ///// 通过文件ID下载
        ///// </summary>
        ///// <typeparam name="TEntity"></typeparam>
        ///// <param name="fromCollection">files联chunks,fromCollection必须指定files对应的chunks文档</param>
        ///// <param name="fileId"></param>
        ///// <returns></returns>
        //public List<FileUploadResult> DownloadByFileId<TEntity>(string fromCollection, string fileId)
        //{
        //    // 校验
        //    if (_collectionName.Split(".").Length != 2 || _collectionName.Split(".").ToList().Last().ToLower() == "files")
        //        throw new Exception("调用DownloadByFileId方法必须指定.files集合");
        //    if (fromCollection.Split(".").Length != 2 || fromCollection.Split(".").ToList().Last().ToLower() == "chunks")
        //        throw new Exception("调用DownloadByFileId方法时,fromCollection必须指定.chunks集合");
        //    if (_collectionName.Split(".").ToList().First().ToLower() != fromCollection.Split(".").ToList().First().ToLower())
        //        throw new Exception("调用DownloadByFileId方法时,必须指定相同的文档集合");

        //    var pipeline = new BsonDocument[]
        //    {
        //        // 联查
        //        new BsonDocument("$lookup",
        //            new BsonDocument
        //            {
        //                { "from", fromCollection },
        //                { "localField", "_id" }, // 当前集合中的关联字段
        //                { "foreignField", foreignField }, // 目标集合中的关联字段
        //                { "as", asField }
        //            }
        //        )
        //    };

        //    List<BsonDocument> list = _collection.Aggregate<BsonDocument>(pipeline).ToList();




        //    // 返回集合
        //    List<FileUploadResult> listFileUploadResult = new List<FileUploadResult>();
        //    var bucket = new GridFSBucket(_database);
        //    var options = new GridFSDownloadOptions
        //    {
        //        Seekable = true // 设置为 true 可以支持 Seek 操作，方便大文件下载
        //    };
        //    using (var memoryStream = new MemoryStream())
        //    {
        //        bucket.DownloadToStream(fileId, memoryStream, options);

        //        // 将内存流中的数据转换为 Base64 字符串
        //        byte[] bytes = memoryStream.ToArray();
        //        string base64String = Convert.ToBase64String(bytes);

        //        listFileUploadResult.Add(new FileUploadResult()
        //        {
        //            FileName =

        //        });
        //    }
        //    return listFileUploadResult;
        //}
        //public List<FileUploadResult> DownloadByFileId<TEntity>(List<string> listFileId)
        //{
        //    // 返回集合
        //    List<FileUploadResult> listFileUploadResult = new List<FileUploadResult>();

        //    return listFileUploadResult;
        //}
        //public List<FileUploadResult> DownloadByDataId<TEntity>(string dataId)
        //{
        //    // 返回集合
        //    List<FileUploadResult> listFileUploadResult = new List<FileUploadResult>();

        //    return listFileUploadResult;
        //}
        //public List<FileUploadResult> DownloadByDataId<TEntity>(List<string> listDataId)
        //{
        //    // 返回集合
        //    List<FileUploadResult> listFileUploadResult = new List<FileUploadResult>();

        //    return listFileUploadResult;
        //}
        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        public object UpdateOne(FilterDefinition<BsonDocument> filter, UpdateDefinition<BsonDocument> update)
        {
            return _collection.UpdateOne(filter, update);
        }
        public async Task<UpdateResult> UpdateOneAsync(FilterDefinition<BsonDocument> filter, UpdateDefinition<BsonDocument> update)
        {
            return await _collection.UpdateOneAsync(filter, update);
        }
        public object UpdateMany(FilterDefinition<BsonDocument> filter, UpdateDefinition<BsonDocument> update)
        {
            return _collection.UpdateMany(filter, update);
        }
        public async Task<object> UpdateManyAsync(FilterDefinition<BsonDocument> filter, UpdateDefinition<BsonDocument> update)
        {
            return await _collection.UpdateManyAsync(filter, update);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public object Delete(FilterDefinition<BsonDocument> filter)
        {
            return _collection.DeleteOne(filter);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public async Task<DeleteResult> DeleteAsync(FilterDefinition<BsonDocument> filter)
        {
            return await _collection.DeleteOneAsync(filter);
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<DeleteResult> DeleteManyAsync(FilterDefinition<BsonDocument> filter)
        {
            return await _collection.DeleteManyAsync(filter);
        }

        /// <summary>
        /// 根据fileId返回base64字符串
        /// </summary>
        /// <param name="fileId">fs.files集合中的_id</param>
        /// <returns>base64</returns>
        public string Download(string fileId)
        {
            var bucket = new GridFSBucket(_database, new GridFSBucketOptions
            {
                BucketName = "fs"
            });
            byte[] fileBytes = bucket.DownloadAsBytes(ObjectId.Parse(fileId));
            return Convert.ToBase64String(fileBytes);
        }
        ///<summary>
        ///
        ///</summary>
        public DataTable BsonDocumentToDataTable(List<BsonDocument> list)
        {
            DataTable dt = new DataTable();
            try
            {
                // 生成表头
                List<string> tableHeader = new List<string>();
                list.ForEach(ff => tableHeader.AddRange(ff.Names.ToList()));
                tableHeader = tableHeader.Distinct().ToList();
                foreach (var item in tableHeader)
                {
                    dt.Columns.Add(item, typeof(string));
                }
                //var tableHeader = list[0];
                //foreach (var element in tableHeader)
                //{
                //    if (element.Value is BsonDocument)
                //    {
                //        // 如果值是BsonDocument类型，则递归调用ConvertToDataTable方法并将结果添加到DataTable中
                //        var subDocument = (BsonDocument)element.Value;
                //        dt.Columns.Add(element.Name, typeof(DataTable));
                //    }
                //    else
                //    {
                //        // 否则，将BsonType转换为相应的.NET类型，并将结果添加到DataTable中
                //        BsonType bsonType = element.Value.BsonType;
                //        var columnType = GetColumnType(bsonType);
                //        dt.Columns.Add(element.Name, columnType);
                //    }
                //}
                // 插入数据
                foreach (var elements in list)
                {
                    DataRow dr = dt.NewRow();
                    foreach (var element in elements)
                    {
                        dr[element.Name] = element.Value.GetCString();
                    }
                    dt.Rows.Add(dr);
                }
                return dt;
            }
            catch (Exception ex)
            {

            }
            return dt;
        }

        private static Type GetColumnType(BsonType bsonType)
        {
            switch (bsonType)
            {
                case BsonType.String:
                    return typeof(string);
                case BsonType.Int32:
                    return typeof(int);
                case BsonType.Int64:
                    return typeof(long);
                case BsonType.Double:
                    return typeof(double);
                case BsonType.Boolean:
                    return typeof(bool);
                // 其他BsonType类型可以根据需要添加
                default:
                    return typeof(object);
            }
        }
        /// <summary>
        /// 自定义查询产生BsonDocument
        /// </summary>
        public class FilterBsonDocument
        {
            /// <summary>
            /// 文件名
            /// </summary>
            public BsonDocument Main { get; set; }

            /// <summary>
            /// 文件ID
            /// </summary>
            public BsonDocument Detail { get; set; }
        }
        ///<summary>
        ///
        ///</summary>
        public class DynamicSearchModel
        {
            /// <summary>
            /// 描述 : 查询字段列表 当字段列表为空或不填写时，默认查询全部字段
            /// 空值 : False
            /// 默认 : 
            /// </summary>
            public List<string> FieldList { get; set; }

            /// <summary>
            /// 描述 : 查询条件列表 当条件列表为空或不填时，则不设查询条件
            /// 空值 : False
            /// 默认 : 
            /// </summary>
            public List<ConditionFieldModel> ConditionList { get; set; }

        }

        /// <summary>
        /// 字段条件
        /// </summary>
        public class ConditionFieldModel
        {
            /// <summary>
            /// 描述 : 字段
            /// 空值 : False
            /// 默认 : 
            /// </summary>
            public string Field { get; set; }

            /// <summary>
            /// 描述 : 条件
            /// 空值 : False
            /// 默认 : 
            /// </summary>
            public List<ConditionModel> Conditions { get; set; }
        }

        /// <summary>
        /// 条件
        /// </summary>
        public class ConditionModel
        {
            /// <summary>
            /// 描述 : 字段类型
            /// 空值 : False
            /// 默认 : 
            /// </summary>
            public string MongoDbType { get; set; }

            /// <summary>
            /// 描述 : 条件类型 eq：等于，ne：不等于，lt：小于，lte：小于等于，gt：大于，gte：大于等于，like：模糊匹配，in：包含,between：在...之间
            /// 空值 : False
            /// 默认 : 
            /// </summary>
            public string Type { get; set; }

            /// <summary>
            /// 描述 : 该参数可为任意值，当类型为boolean时，只可以使用eq类型。当类型为in时，需传输Array对象
            /// 空值 : False
            /// 默认 : 
            /// </summary>
            public object FirstValue { get; set; }

            /// <summary>
            /// 描述 : 该参数只在类型为between时使用，为between的第二个值
            /// 空值 : False
            /// 默认 : 
            /// </summary>
            public object SecondValue { get; set; }

            /// <summary>
            /// 描述 : 该参数设置与上一个条件的连接关系，只要and或or，默认使用and
            /// 空值 : False
            /// 默认 : 
            /// </summary>
            public bool IgnoreCase { get; set; } = true;

            /// <summary>
            /// 描述 : 该参数设置与上一个条件的连接关系，只要and或or，默认使用and
            /// 空值 : False
            /// 默认 : 
            /// </summary>
            public string Operation { get; set; } = "and";
        }
    }

}

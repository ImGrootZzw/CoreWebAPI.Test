using SqlSugar;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace CoreWebAPI.Common.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public static class DbDynamicQueryHelper
    {
        /// <summary>
        /// 获取动态查询条件语句 
        /// </summary>
        /// <typeparam name="TEntity">带SugarColumn(ColumnName)特性的实体，建立动态查询条件字段与数据库字段的mapping关系</typeparam>
        /// <param name="searchModel">动态查询条件字段及条件</param>
        /// <returns></returns>
        public static string PgsqlDynamicWhereSqlGet<TEntity>(object searchModel)
        {
            string sqlStr = string.Empty;
            var dynamicSearchModel = JsonHelper.JsonToObject<DynamicSearchModel>(JsonHelper.ObjectToJson(searchModel));
            if (dynamicSearchModel == null || dynamicSearchModel.ConditionList == null)
                return "";
            foreach (var fieldCondition in dynamicSearchModel.ConditionList)
            {
                string conditionStr = string.Empty;

                var props = typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
                var prop = props.FirstOrDefault(p => p.Name.ToLower() == fieldCondition.Field.ToLower());
                if (prop != null)
                {
                    var pAttr = prop.GetCustomAttribute<SugarColumn>();
                    if (pAttr.GetIsNotEmptyOrNull() && pAttr.ColumnName != null)
                    {
                        //var lastOperation = string.Empty;
                        foreach (var condition in fieldCondition.Conditions)
                        {
                            if (condition.FirstValue.GetIsEmptyOrNull())
                                continue;
                            switch (condition.Type)
                            {
                                case "eq":
                                    if (prop.PropertyType.Name.ToLower() == "string" && (condition.IgnoreCase.GetIsEmptyOrNull() || condition.IgnoreCase.GetCBool()))
                                        conditionStr += $" lower({ pAttr.ColumnName }) = '{ condition.FirstValue.GetCString().ToLower() }' or ";
                                    else
                                        conditionStr += $" { pAttr.ColumnName } = '{ condition.FirstValue.GetCString() }' or ";
                                    break;
                                case "ne":
                                    if (prop.PropertyType.Name.ToLower() == "string" && (condition.IgnoreCase.GetIsEmptyOrNull() || condition.IgnoreCase.GetCBool()))
                                        conditionStr += $" lower({ pAttr.ColumnName }) != '{ condition.FirstValue.GetCString().ToLower() }' or ";
                                    else
                                        conditionStr += $" { pAttr.ColumnName } != '{ condition.FirstValue.GetCString().ToSqlFilter() }' or ";
                                    break;
                                case "lt":
                                    conditionStr += $" { pAttr.ColumnName } < '{ condition.FirstValue.GetCString() }' or ";
                                    break;
                                case "lte":
                                    conditionStr += $" { pAttr.ColumnName } <= '{ condition.FirstValue.GetCString() }' or ";
                                    break;
                                case "gt":
                                    conditionStr += $" { pAttr.ColumnName } > '{ condition.FirstValue.GetCString() }' or ";
                                    break;
                                case "gte":
                                    conditionStr += $" { pAttr.ColumnName } >= '{ condition.FirstValue.GetCString() }' or ";
                                    break;
                                case "like":
                                    if (prop.PropertyType.Name.ToLower() == "string" && (condition.IgnoreCase.GetIsEmptyOrNull() || condition.IgnoreCase.GetCBool()))
                                        conditionStr += $" lower({ pAttr.ColumnName }) like '%{ condition.FirstValue.GetCString().ToLower() }%' or ";
                                    else
                                        throw new Exception($"当前字段数据类型为{prop.PropertyType.Name},不支持模糊匹配");
                                    break;
                                case "in":
                                    if (prop.PropertyType.Name.ToLower() == "string" && (condition.IgnoreCase.GetIsEmptyOrNull() || condition.IgnoreCase.GetCBool()))
                                        conditionStr += $" lower({ pAttr.ColumnName }) in ('{ string.Join("','", JsonHelper.JsonToObject<List<string>>(JsonHelper.ObjectToJson(condition.FirstValue))).ToLower() }') or ";
                                    else
                                        conditionStr += $" { pAttr.ColumnName } in ('{ string.Join("','", JsonHelper.JsonToObject<List<string>>(JsonHelper.ObjectToJson(condition.FirstValue))) }') or ";
                                    break;
                                case "between":
                                    conditionStr += $" { pAttr.ColumnName } >= '{ condition.FirstValue.GetCString() }' and { pAttr.ColumnName } <= '{ condition.SecondValue.GetCString() }'or ";
                                    break;
                            }

                            //lastOperation = condition.Operation;
                        }

                        conditionStr = conditionStr.Substring(0, conditionStr.LastIndexOf("or"));
                    }
                }

                if (conditionStr.Length > 0)
                    sqlStr += "(" + conditionStr + ") and ";
            }

            if (sqlStr.EndsWith("and "))
                sqlStr = sqlStr[0..^4];
            return sqlStr;
        }

        /// <summary>
        /// 获取动态查询条件语句 
        /// </summary>
        /// <typeparam name="TEntity">带SugarColumn(ColumnName)特性的实体，建立动态查询条件字段与数据库字段的mapping关系</typeparam>
        /// <param name="searchModel">动态查询条件字段及条件</param>
        /// <returns></returns>
        public static (string, object) PgsqlDynamicWhereSqlParaGet<TEntity>(object searchModel)
        {
            string sqlStr = string.Empty;
            //创建匿名对象
            var paraObj = new Dictionary<string, object>();
            //var paraObj = paraObj as IDictionary<string, object>;

            var dynamicSearchModel = JsonHelper.JsonToObject<DynamicSearchModel>(JsonHelper.ObjectToJson(searchModel));
            if (dynamicSearchModel == null || dynamicSearchModel.ConditionList == null)
                return (sqlStr, paraObj);
            foreach (var fieldCondition in dynamicSearchModel.ConditionList)
            {
                string conditionStr = string.Empty;

                var props = typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
                var prop = props.FirstOrDefault(p => p.Name.ToLower() == fieldCondition.Field.ToLower());
                if (prop != null)
                {
                    var pAttr = prop.GetCustomAttribute<SugarColumn>();
                    if (pAttr.GetIsNotEmptyOrNull() && pAttr.ColumnName != null)
                    {
                        var v_i = 0;
                        foreach (var condition in fieldCondition.Conditions)
                        {
                            if (condition.FirstValue.GetIsEmptyOrNull())
                                continue;

                            //给匿名对象添加属性
                            //if (prop.PropertyType.Name.ToLower() == "datetime" || prop.PropertyType.Name.ToLower() == "datetimeoffset")
                            //    paraObj[pAttr.ColumnName + v_i] = condition.FirstValue.GetCDate();
                            //else if (prop.PropertyType.Name.ToLower() == "decimal" || prop.PropertyType.Name.ToLower() == "double")
                            //    paraObj[pAttr.ColumnName + v_i] = condition.FirstValue.GetCDecimal();
                            //else if (prop.PropertyType.Name.ToLower() == "int")
                            //    paraObj[pAttr.ColumnName + v_i] = condition.FirstValue.GetCInt();
                            //else if (prop.PropertyType.Name.ToLower() == "bool")
                            //    paraObj[pAttr.ColumnName + v_i] = condition.FirstValue.GetCBool();
                            //else
                            paraObj[pAttr.ColumnName + v_i] = condition.FirstValue;

                            switch (condition.Type.GetCString().ToLower())
                            {
                                case "eq":
                                    if (prop.PropertyType.Name.ToLower() == "string" && (condition.IgnoreCase.GetIsEmptyOrNull() || condition.IgnoreCase.GetCBool()))
                                    {
                                        conditionStr += $" lower({ pAttr.ColumnName }) = @{ pAttr.ColumnName + v_i } or ";
                                        paraObj[pAttr.ColumnName + v_i] = condition.FirstValue.GetCString().ToLower();
                                    }
                                    else
                                        conditionStr += $" { pAttr.ColumnName } = @{ pAttr.ColumnName + v_i } or ";
                                    break;
                                case "ne":
                                    if (prop.PropertyType.Name.ToLower() == "string" && (condition.IgnoreCase.GetIsEmptyOrNull() || condition.IgnoreCase.GetCBool()))
                                    {
                                        conditionStr += $" lower({ pAttr.ColumnName }) != @{ pAttr.ColumnName + v_i } or ";
                                        paraObj[pAttr.ColumnName + v_i] = condition.FirstValue.GetCString().ToLower();
                                    }
                                    else
                                        conditionStr += $" { pAttr.ColumnName } != @{ pAttr.ColumnName + v_i } or ";
                                    break;
                                case "lt":
                                    conditionStr += $" { pAttr.ColumnName } < @{ pAttr.ColumnName + v_i } or ";
                                    break;
                                case "lte":
                                    conditionStr += $" { pAttr.ColumnName } <= @{ pAttr.ColumnName + v_i } or ";
                                    break;
                                case "gt":
                                    conditionStr += $" { pAttr.ColumnName } > @{ pAttr.ColumnName + v_i } or ";
                                    break;
                                case "gte":
                                    conditionStr += $" { pAttr.ColumnName } >= @{ pAttr.ColumnName + v_i } or ";
                                    break;
                                case "like":
                                    if (prop.PropertyType.Name.ToLower() == "string" && (condition.IgnoreCase.GetIsEmptyOrNull() || condition.IgnoreCase.GetCBool()))
                                    {
                                        conditionStr += $" lower({ pAttr.ColumnName }) like @{ pAttr.ColumnName + v_i }  ESCAPE '^' or ";
                                        paraObj[pAttr.ColumnName + v_i] = condition.FirstValue.GetCString().Replace("%", "^%").Replace("_", "^_").Replace("*", "%");
                                    }
                                    else
                                        throw new Exception($"当前字段数据类型为{prop.PropertyType.Name},不支持模糊匹配");
                                    break;
                                case "in":
                                    if (prop.PropertyType.Name.ToLower() == "string" && (condition.IgnoreCase.GetIsEmptyOrNull() || condition.IgnoreCase.GetCBool()))
                                    {
                                        var inData = JsonHelper.JsonToObject<List<string>>(JsonHelper.ObjectToJson(condition.FirstValue));
                                        var inIndex = 0;
                                        var inPara = string.Empty;
                                        foreach (var data in inData)
                                        {
                                            inPara += $"@{ pAttr.ColumnName }In{ inIndex },";
                                            paraObj[$"{ pAttr.ColumnName }In" + inIndex] = data.GetCString().ToLower();
                                            inIndex++;
                                        }
                                        conditionStr += $" lower({ pAttr.ColumnName }) in ({ inPara.Trim(',') }) or ";
                                    }
                                    else
                                    {
                                        var inData = JsonHelper.JsonToObject<List<object>>(JsonHelper.ObjectToJson(condition.FirstValue));
                                        var inIndex = 0;
                                        var inPara = string.Empty;
                                        foreach (var data in inData)
                                        {
                                            inPara += $"@{ pAttr.ColumnName }In{ inIndex },";
                                            paraObj[$"{ pAttr.ColumnName }In" + inIndex] = data;
                                            inIndex++;
                                        }
                                        conditionStr += $" { pAttr.ColumnName } in ({ inPara.Trim(',') }) or ";
                                    }
                                    break;
                                case "between":
                                    conditionStr += $" { pAttr.ColumnName } >= @{ pAttr.ColumnName + v_i }From and { pAttr.ColumnName } <= @{ pAttr.ColumnName + v_i }To or ";
                                    if (prop.PropertyType.Name.ToLower() == "string" && (condition.IgnoreCase.GetIsEmptyOrNull() || condition.IgnoreCase.GetCBool()))
                                    {
                                        paraObj[pAttr.ColumnName + v_i + "From"] = condition.FirstValue.GetCString().ToLower();
                                        paraObj[pAttr.ColumnName + v_i + "To"] = condition.SecondValue.GetCString().ToLower();
                                    }
                                    else
                                    {
                                        paraObj[pAttr.ColumnName + v_i + "From"] = condition.FirstValue;
                                        paraObj[pAttr.ColumnName + v_i + "To"] = condition.SecondValue;
                                    }
                                    break;
                            }

                            v_i++;
                            //lastOperation = condition.Operation;
                        }

                        if (conditionStr.Length > 0)
                            conditionStr = conditionStr.Substring(0, conditionStr.LastIndexOf("or"));
                    }
                }

                if (conditionStr.Length > 0)
                    sqlStr += "(" + conditionStr + ") and ";
            }

            if (sqlStr.EndsWith("and "))
                sqlStr = sqlStr[0..^4];
            return (sqlStr, paraObj);
        }

        /// <summary>
        /// 获取动态查询条件语句 (表格查询)
        /// </summary>
        /// <typeparam name="TEntity">带SugarColumn(ColumnName)特性的实体，建立动态查询条件字段与数据库字段的mapping关系</typeparam>
        /// <param name="searchModel">动态查询条件字段及条件</param>
        /// <returns></returns>
        public static string PgsqlDynamicWhereSqlGetBY<TEntity>(object searchModel)
        {
            string sqlStr = string.Empty;
            string json = string.Empty;
            var dynamicSearchModel = JsonHelper.JsonToObject<DynamicSearchModel>(JsonHelper.ObjectToJson(searchModel));
            if (dynamicSearchModel == null || dynamicSearchModel.ConditionList == null)
                return "";



            //{
            //    "query": {
            //        "match": [{
            //            "name": "jk",
            //   "age": "25"

            //        },
            //  {
            //                        "realName": "zs",
            //   "realAge": "9"

            //        }]
            // },
            // "page": 5
            //}
            {
                dynamic objmatch = new ExpandoObject();

                Dictionary<string, string> matchKV = new Dictionary<string, string>();
                matchKV.Add("name", "jk");
                matchKV.Add("age", "25");

                Dictionary<string, string> matchKV2 = new Dictionary<string, string>();
                matchKV2.Add("realName", "zs");
                matchKV2.Add("realAge", "9");

                List<dynamic> matchList = new List<dynamic>();
                matchList.Add(matchKV);
                matchList.Add(matchKV2);


                objmatch.match = matchList;

                dynamic obj = new ExpandoObject();
                obj.query = objmatch;
                obj.page = 5;
            }
            

            dynamic conditionJson = new ExpandoObject();

            List<dynamic> conditionList = new List<dynamic>();



            foreach (var fieldCondition in dynamicSearchModel.ConditionList)
            {
                int _i = 0;
                string conditionStr = string.Empty;

                dynamic conditionItem = new ExpandoObject();
                conditionItem.Key = _i ==0 ? -1 : 0;
                conditionItem.Value = "";

                

                var props = typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
                var prop = props.FirstOrDefault(p => p.Name.ToLower() == fieldCondition.Field.ToLower());
                if (prop != null)
                {
                    var pAttr = prop.GetCustomAttribute<SugarColumn>();
                    if (pAttr.GetIsNotEmptyOrNull() && pAttr.ColumnName != null)
                    {

                        if (fieldCondition.Conditions.GetNotNull().Count() == 1)
                        {
                            var condition = fieldCondition.Conditions.FirstOrDefault();
                            dynamic childItem = new ExpandoObject();
                            childItem.FieldName = pAttr.ColumnName;
                            childItem.FieldValue = condition.FirstValue.GetCString();
                            childItem.ConditionalType = 0;

                            switch (condition.Type)
                            {
                                case "eq":
                                    if (prop.PropertyType.Name.ToLower() == "string" && (condition.IgnoreCase.GetIsEmptyOrNull() || condition.IgnoreCase.GetCBool()))
                                    {
                                        childItem.FieldValue = condition.FirstValue.GetCString().ToLower();
                                        childItem.ConditionalType = 0;
                                    }
                                    else
                                        childItem.ConditionalType = 0;
                                    break;
                                case "ne":
                                    if (prop.PropertyType.Name.ToLower() == "string" && (condition.IgnoreCase.GetIsEmptyOrNull() || condition.IgnoreCase.GetCBool()))
                                    {
                                        childItem.FieldValue = condition.FirstValue.GetCString().ToLower();
                                        childItem.ConditionalType = 10;
                                    }
                                    else
                                        childItem.ConditionalType = 10;
                                    break;
                                case "lt":
                                    childItem.ConditionalType = 0;
                                    break;
                                case "lte":
                                    childItem.ConditionalType = 0;
                                    break;
                                case "gt":
                                    childItem.ConditionalType = 0;
                                    break;
                                case "gte":
                                    childItem.ConditionalType = 0;
                                    break;
                                case "like":
                                    if (prop.PropertyType.Name.ToLower() == "string" && (condition.IgnoreCase.GetIsEmptyOrNull() || condition.IgnoreCase.GetCBool()))
                                    {
                                        conditionStr += $" lower({ pAttr.ColumnName }) like '%{ condition.FirstValue.GetCString().ToLower() }%' or ";

                                        childItem.ConditionalType = 0;
                                    }
                                    else
                                        throw new Exception($"当前字段数据类型为{prop.PropertyType.Name},不支持模糊匹配");
                                    break;
                                case "in":
                                    if (prop.PropertyType.Name.ToLower() == "string" && (condition.IgnoreCase.GetIsEmptyOrNull() || condition.IgnoreCase.GetCBool()))
                                        conditionStr += $" lower({ pAttr.ColumnName }) in ('{ string.Join("','", JsonHelper.JsonToObject<List<string>>(JsonHelper.ObjectToJson(condition.FirstValue))).ToLower() }') or ";
                                    else
                                        childItem.ConditionalType = 0;
                                    break;

                                case "between":
                                    conditionStr += $" { pAttr.ColumnName } >= '{ condition.FirstValue.GetCString() }' and { pAttr.ColumnName } <= '{ condition.SecondValue.GetCString() }'or ";
                                    break;
                            }
                        }
                        else
                        {
                            List<dynamic> matchList = new();

                            int _j = 0;
                            foreach (var condition in fieldCondition.Conditions.GetNotNull())
                            {
                                if (condition.FirstValue.GetIsEmptyOrNull())
                                    continue;

                                dynamic childItem = new ExpandoObject();
                                childItem.FieldName = pAttr.ColumnName;
                                childItem.FieldValue = condition.FirstValue.GetCString();
                                childItem.ConditionalType = 0;

                                switch (condition.Type)
                                {
                                    case "eq":
                                        if (prop.PropertyType.Name.ToLower() == "string" && (condition.IgnoreCase.GetIsEmptyOrNull() || condition.IgnoreCase.GetCBool()))


                                            conditionStr += $" lower({ pAttr.ColumnName }) = '{ condition.FirstValue.GetCString().ToLower() }' or ";
                                        else
                                            conditionStr += $" { pAttr.ColumnName } = '{ condition.FirstValue.GetCString() }' or ";
                                        break;
                                    case "ne":
                                        if (prop.PropertyType.Name.ToLower() == "string" && (condition.IgnoreCase.GetIsEmptyOrNull() || condition.IgnoreCase.GetCBool()))
                                            conditionStr += $" lower({ pAttr.ColumnName }) != '{ condition.FirstValue.GetCString().ToLower() }' or ";
                                        else
                                            conditionStr += $" { pAttr.ColumnName } != '{ condition.FirstValue.GetCString() }' or ";
                                        break;
                                    case "lt":
                                        conditionStr += $" { pAttr.ColumnName } < '{ condition.FirstValue.GetCString() }' or ";
                                        break;
                                    case "lte":
                                        conditionStr += $" { pAttr.ColumnName } <= '{ condition.FirstValue.GetCString() }' or ";
                                        break;
                                    case "gt":
                                        conditionStr += $" { pAttr.ColumnName } > '{ condition.FirstValue.GetCString() }' or ";
                                        break;
                                    case "gte":
                                        conditionStr += $" { pAttr.ColumnName } >= '{ condition.FirstValue.GetCString() }' or ";
                                        break;
                                    case "like":
                                        if (prop.PropertyType.Name.ToLower() == "string" && (condition.IgnoreCase.GetIsEmptyOrNull() || condition.IgnoreCase.GetCBool()))
                                            conditionStr += $" lower({ pAttr.ColumnName }) like '%{ condition.FirstValue.GetCString().ToLower() }%' or ";
                                        else
                                            throw new Exception($"当前字段数据类型为{prop.PropertyType.Name},不支持模糊匹配");
                                        break;
                                    case "in":
                                        if (prop.PropertyType.Name.ToLower() == "string" && (condition.IgnoreCase.GetIsEmptyOrNull() || condition.IgnoreCase.GetCBool()))
                                            conditionStr += $" lower({ pAttr.ColumnName }) in ('{ string.Join("','", JsonHelper.JsonToObject<List<string>>(JsonHelper.ObjectToJson(condition.FirstValue))).ToLower() }') or ";
                                        else
                                            conditionStr += $" { pAttr.ColumnName } in ('{ string.Join("','", JsonHelper.JsonToObject<List<string>>(JsonHelper.ObjectToJson(condition.FirstValue))) }') or ";
                                        break;
                                    case "between":
                                        conditionStr += $" { pAttr.ColumnName } >= '{ condition.FirstValue.GetCString() }' and { pAttr.ColumnName } <= '{ condition.SecondValue.GetCString() }'or ";
                                        break;
                                }


                                _j++;
                                //lastOperation = condition.Operation;
                            }
                        }

                        conditionStr = conditionStr.Substring(0, conditionStr.LastIndexOf("or"));
                    }
                }

                if (conditionStr.Length > 0)
                    sqlStr += "(" + conditionStr + ") and ";

                _i++;
            }

            if (sqlStr.EndsWith("and "))
                sqlStr = sqlStr[0..^4];
            return sqlStr;
        }
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

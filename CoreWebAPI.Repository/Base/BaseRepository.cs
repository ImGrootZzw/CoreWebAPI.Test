using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SqlSugar.Extensions;
using CoreWebAPI.Repository.UnitOfWork;
using System.Linq;

namespace CoreWebAPI.Repository.Base
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class, new()
    {
        private readonly IUnitOfWork _unitOfWork;
        private SqlSugarClient _dbBase;
        private bool isChangeDb = false;

        public BaseRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _dbBase = unitOfWork.GetDbClient();
        }

        public ISqlSugarClient _db
        {
            get
            {
                // 如果实体有tableDescription标识，切换库到该标识库
                if (isChangeDb == false && typeof(TEntity).GetTypeInfo().GetCustomAttributes(typeof(SugarTable), true).FirstOrDefault((x => x.GetType() == typeof(SugarTable))) is SugarTable sugarTable && !string.IsNullOrEmpty(sugarTable.TableDescription))
                {
                    var dbTEntity = _unitOfWork.GetDbClient();
                    dbTEntity.ChangeDatabase(sugarTable.TableDescription.ToLower());
                    return dbTEntity;
                }

                return _dbBase;
            }
        }
        internal ISqlSugarClient Db => _db;

        public void ChangeDB(string configId)
        {
            _dbBase.ChangeDatabase(configId.ToLower());
            isChangeDb = true;
        }

        public ISqlSugarClient GetDBByConfigid(string configId)
        {
            SqlSugarClient sqlSugarClient = _unitOfWork.GetDbClient();
            sqlSugarClient.ChangeDatabase(configId);
            return sqlSugarClient;
        }

        #region "查询"
        /// <summary>
        /// 功能描述:根据ID查询一条数据
        /// </summary>
        /// <param name="objId">id（必须指定主键特性 [SugarColumn(IsPrimaryKey=true)]），如果是联合主键，请使用Where条件</param>
        /// <returns>数据实体</returns>
        public async Task<TEntity> QueryById(object objId)
        {
            return await _db.Queryable<TEntity>().In(objId).SingleAsync();
        }

        /// <summary>
        /// 功能描述:根据ID查询一条数据
        /// </summary>
        /// <param name="objId">id（必须指定主键特性 [SugarColumn(IsPrimaryKey=true)]），如果是联合主键，请使用Where条件</param>
        /// <param name="blnUseCache">是否使用缓存</param>
        /// <returns>数据实体</returns>
        public async Task<TEntity> QueryById(object objId, bool blnUseCache = false)
        {
            return await _db.Queryable<TEntity>().WithCacheIF(blnUseCache).In(objId).SingleAsync();
        }

        /// <summary>
        /// 功能描述:根据ID查询数据
        /// </summary>
        /// <param name="lstIds">id列表（必须指定主键特性 [SugarColumn(IsPrimaryKey=true)]），如果是联合主键，请使用Where条件</param>
        /// <returns>数据实体列表</returns>
        public async Task<List<TEntity>> QueryByIDs(object[] lstIds)
        {
            return await _db.Queryable<TEntity>().In(lstIds).ToListAsync();
        }

        /// <summary>
        /// 功能描述:无实体查询，根据表名查询数据
        /// </summary>
        /// <param name="strWhere">Where条件</param>
        /// <param name="tableName">表名</param>
        /// <returns>数据实体列表</returns>
        public async Task<List<dynamic>> QueryByTableName(string strWhere, string tableName)
        {
            return await _db.Queryable<dynamic>().AS(tableName).WhereIF(!string.IsNullOrEmpty(strWhere), strWhere).ToListAsync();
        }

        /// <summary>
        /// 功能描述:查询最大值
        /// </summary>
        /// <param name="whereExpression">Where条件</param>
        /// <param name="fieldName">查询字段</param>
        /// <returns>数据实体列表</returns>
        public async Task<int> QueryMax(Expression<Func<TEntity, bool>> whereExpression, string fieldName)
        {
            return await _db.Queryable<TEntity>().WhereIF(whereExpression != null, whereExpression).MaxAsync<int>(fieldName);
        }

        /// <summary>
        /// 功能描述:查询所有数据
        /// </summary>
        /// <returns>数据列表</returns>
        public async Task<List<TEntity>> Query()
        {
            return await _db.Queryable<TEntity>().ToListAsync();
        }

        /// <summary>
        /// 功能描述:查询所有数据
        /// </summary>
        /// <returns>数据列表</returns>
        public async Task<List<TEntity>> Query(string configid)
        {
            return await GetDBByConfigid(configid).Queryable<TEntity>().ToListAsync();
        }

        /// <summary>
        /// 功能描述:查询所有数据
        /// </summary>
        /// <param name="splitFunc">分表过滤，详细可参考https://www.donet5.com/Home/Doc?typeId=1201</param>
        /// <returns>数据列表</returns>
        public async Task<List<TEntity>> QueryWithSplitTable(Func<List<SplitTableInfo>, IEnumerable<SplitTableInfo>> splitFunc)
        {
            return await _db.Queryable<TEntity>().ToListAsync();
        }

        /// <summary>
        /// 功能描述:查询数据列表
        /// </summary>
        /// <param name="strWhere">条件</param>
        /// <returns>数据列表</returns>
        public async Task<List<TEntity>> Query(string strWhere, object paramObj)
        {
            return await _db.Queryable<TEntity>().WhereIF(!string.IsNullOrEmpty(strWhere), strWhere, paramObj).ToListAsync();
        }

        /// <summary>
        /// 功能描述:查询数据列表
        /// </summary>
        /// <param name="whereExpression">whereExpression</param>
        /// <returns>数据列表</returns>
        public async Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression)
        {
            return await _db.Queryable<TEntity>().WhereIF(whereExpression != null, whereExpression).ToListAsync();
        }

        /// <summary>
        /// 功能描述:查询数据列表-分表查询
        /// </summary>
        /// <param name="whereExpression">whereExpression</param>
        /// <param name="splitFunc">分表过滤，详细可参考https://www.donet5.com/Home/Doc?typeId=1201</param>
        /// <returns>数据列表</returns>
        public async Task<List<TEntity>> QueryWithSplitTable(Expression<Func<TEntity, bool>> whereExpression, Func<List<SplitTableInfo>, IEnumerable<SplitTableInfo>> splitFunc)
        {
            return await _db.Queryable<TEntity>().WhereIF(whereExpression != null, whereExpression).SplitTable(splitFunc).ToListAsync();
        }

        /// <summary>
        /// 功能描述:查询数据列表 - 悲观锁排它模式
        /// </summary>
        /// <param name="whereExpression">whereExpression</param>
        /// <returns>数据列表</returns>
        public async Task<List<TEntity>> QueryLock(Expression<Func<TEntity, bool>> whereExpression)
        {
            return await _db.Queryable<TEntity>().TranLock(DbLockType.Error).WhereIF(whereExpression != null, whereExpression).ToListAsync();
        }

        /// <summary>
        /// 功能描述:查询一个列表
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>数据列表</returns>
        public async Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression, string strOrderByFileds)
        {
            return await _db.Queryable<TEntity>().WhereIF(whereExpression != null, whereExpression).OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds).ToListAsync();
        }

        /// <summary>
        /// 功能描述:查询一个列表
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <param name="orderByExpression"></param>
        /// <param name="isAsc"></param>
        /// <returns></returns>
        public async Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, object>> orderByExpression, bool isAsc = true)
        {
            return await _db.Queryable<TEntity>().OrderByIF(orderByExpression != null, orderByExpression, isAsc ? OrderByType.Asc : OrderByType.Desc).WhereIF(whereExpression != null, whereExpression).ToListAsync();
        }

        /// <summary>
        /// 功能描述:查询一个列表
        /// </summary>
        /// <param name="strWhere">条件</param>
        /// <param name="paramObj">查询参数</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>数据列表</returns>
        public async Task<List<TEntity>> Query(string strWhere, object paramObj, string strOrderByFileds)
        {
            return await _db.Queryable<TEntity>().WhereIF(!string.IsNullOrEmpty(strWhere), strWhere, paramObj).OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds).ToListAsync();
        }

        /// <summary>
        /// 功能描述:分页查询-分表查询
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="splitFunc">分表过滤，详细可参考https://www.donet5.com/Home/Doc?typeId=1201</param>
        /// <param name="intPageIndex">页码（下标0）</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>数据列表</returns>
        public async Task<List<TEntity>> QueryWithSplitTable(Expression<Func<TEntity, bool>> whereExpression, Func<List<SplitTableInfo>, IEnumerable<SplitTableInfo>> splitFunc, int intPageIndex, int intPageSize, string strOrderByFileds)
        {
            return await _db.Queryable<TEntity>().WhereIF(whereExpression != null, whereExpression).SplitTable(splitFunc).OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds).ToPageListAsync(intPageIndex, intPageSize == 0 ? 100 : intPageSize == 0 ? 100 : intPageSize);
        }

        /// <summary>
        /// 功能描述:分页查询
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="intPageIndex">页码（下标0）</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>数据列表</returns>
        public async Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression, int intPageIndex, int intPageSize, string strOrderByFileds)
        {
            return await _db.Queryable<TEntity>().OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds).WhereIF(whereExpression != null, whereExpression).ToPageListAsync(intPageIndex, intPageSize == 0 ? 100 : intPageSize == 0 ? 100 : intPageSize);
        }

        /// <summary>
        /// 功能描述:分页查询
        /// </summary>
        /// <param name="strWhere">条件</param>
        /// <param name="intPageIndex">页码（下标0）</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>数据列表</returns>
        public async Task<List<TEntity>> Query(string strWhere, object paramObj, int intPageIndex, int intPageSize, string strOrderByFileds)
        {
            return await _db.Queryable<TEntity>().OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds).WhereIF(!string.IsNullOrEmpty(strWhere), strWhere, paramObj).ToPageListAsync(intPageIndex, intPageSize == 0 ? 100 : intPageSize == 0 ? 100 : intPageSize);
        }

        /// <summary>
        /// 功能描述:查询一个列表
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>DataTable</returns>
        public async Task<DataTable> QueryTable(Expression<Func<TEntity, bool>> whereExpression, string strOrderByFileds)
        {
            return await  _db.Queryable<TEntity>().WhereIF(whereExpression != null, whereExpression).OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds).ToDataTableAsync();
        }

        /// <summary>
        /// 功能描述:查询一个列表
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="sortExpression">排序表达式</param>
        /// <param name="sortType">排序类型</param>
        /// <returns>DataTable</returns>
        public async Task<DataTable> QueryTable(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, object>> sortExpression = null, OrderByType sortType = OrderByType.Asc)
        {
            return await _db.Queryable<TEntity>().WhereIF(whereExpression != null, whereExpression).OrderByIF(sortExpression != null, sortExpression, sortType).ToDataTableAsync();
        }

        /// <summary>
        /// 两表联合查询
        /// </summary>
        /// <typeparam name="T">实体1</typeparam>
        /// <typeparam name="T2">实体1</typeparam>
        /// <typeparam name="TResult">返回对象</typeparam>
        /// <param name="joinExpression">关联表达式</param>
        /// <param name="selectExpression">返回表达式</param>
        /// <param name="whereExpression">查询表达式</param>
        /// <param name="strOrderByFileds">排序字段</param>
        /// <returns></returns>
        public async Task<DataTable> QueryTable<T, T2, TResult>(
            Expression<Func<T, T2, object[]>> joinExpression,
            Expression<Func<T, T2, TResult>> selectExpression,
            Expression<Func<T, T2, bool>> whereExpression,
            string strOrderByFileds = null)
        {
            return await _db.Queryable(joinExpression)
                .WhereIF(whereExpression != null, whereExpression)
                .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
                .Select(selectExpression)
                .ToDataTableAsync();
        }

        /// <summary>
        /// 三表联合查询
        /// </summary>
        /// <typeparam name="T">实体1</typeparam>
        /// <typeparam name="T2">实体2</typeparam>
        /// <typeparam name="T3">实体3</typeparam>
        /// <typeparam name="TResult">返回对象</typeparam>
        /// <param name="joinExpression">关联表达式</param>
        /// <param name="selectExpression">返回表达式</param>
        /// <param name="whereExpression">查询表达式</param>
        /// <param name="strOrderByFileds">排序字段</param>
        /// <returns></returns>
        public async Task<DataTable> QueryTable<T, T2, T3, TResult>(
            Expression<Func<T, T2, T3, object[]>> joinExpression,
            Expression<Func<T, T2, T3, TResult>> selectExpression,
            Expression<Func<T, T2, T3, bool>> whereExpression,
            string strOrderByFileds = null)
        {
            return await _db.Queryable(joinExpression)
                .WhereIF(whereExpression != null, whereExpression)
                .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
                .Select(selectExpression)
                .ToDataTableAsync();
        }

        /// <summary>
        /// 四表联合查询
        /// </summary>
        /// <typeparam name="T">实体1</typeparam>
        /// <typeparam name="T2">实体2</typeparam>
        /// <typeparam name="T3">实体3</typeparam>
        /// <typeparam name="T4">实体4</typeparam>
        /// <typeparam name="TResult">返回对象</typeparam>
        /// <param name="joinExpression">关联表达式</param>
        /// <param name="selectExpression">返回表达式</param>
        /// <param name="whereExpression">查询表达式</param>
        /// <param name="strOrderByFileds">排序字段</param>
        /// <returns></returns>
        public async Task<DataTable> QueryTable<T, T2, T3, T4, TResult>(
            Expression<Func<T, T2, T3, T4, object[]>> joinExpression,
            Expression<Func<T, T2, T3, T4, TResult>> selectExpression,
            Expression<Func<T, T2, T3, T4, bool>> whereExpression,
            string strOrderByFileds = null)
        {
            return await _db.Queryable(joinExpression)
                .WhereIF(whereExpression != null, whereExpression)
                .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
                .Select(selectExpression)
                .ToDataTableAsync();
        }

        /// <summary>
        /// 五表联合查询
        /// </summary>
        /// <typeparam name="T">实体1</typeparam>
        /// <typeparam name="T2">实体2</typeparam>
        /// <typeparam name="T3">实体3</typeparam>
        /// <typeparam name="T4">实体4</typeparam>
        /// <typeparam name="T5">实体5</typeparam>
        /// <typeparam name="TResult">返回对象</typeparam>
        /// <param name="joinExpression">关联表达式</param>
        /// <param name="selectExpression">返回表达式</param>
        /// <param name="whereExpression">查询表达式</param>
        /// <param name="strOrderByFileds">排序字段</param>
        /// <returns></returns>
        public async Task<DataTable> QueryTable<T, T2, T3, T4, T5, TResult>(
            Expression<Func<T, T2, T3, T4, T5, object[]>> joinExpression,
            Expression<Func<T, T2, T3, T4, T5, TResult>> selectExpression,
            Expression<Func<T, T2, T3, T4, T5, bool>> whereExpression,
            string strOrderByFileds = null)
        {
            return await _db.Queryable(joinExpression)
                .WhereIF(whereExpression != null, whereExpression)
                .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
                .Select(selectExpression)
                .ToDataTableAsync();
        }

        /// <summary>
        /// 六表联合查询
        /// </summary>
        /// <typeparam name="T">实体1</typeparam>
        /// <typeparam name="T2">实体2</typeparam>
        /// <typeparam name="T3">实体3</typeparam>
        /// <typeparam name="T4">实体4</typeparam>
        /// <typeparam name="T5">实体5</typeparam>
        /// <typeparam name="T6">实体6</typeparam>
        /// <typeparam name="TResult">返回对象</typeparam>
        /// <param name="joinExpression">关联表达式</param>
        /// <param name="selectExpression">返回表达式</param>
        /// <param name="whereExpression">查询表达式</param>
        /// <param name="strOrderByFileds">排序字段</param>
        /// <returns></returns>
        public async Task<DataTable> QueryTable<T, T2, T3, T4, T5, T6, TResult>(
            Expression<Func<T, T2, T3, T4, T5, T6, object[]>> joinExpression,
            Expression<Func<T, T2, T3, T4, T5, T6, TResult>> selectExpression,
            Expression<Func<T, T2, T3, T4, T5, T6, bool>> whereExpression,
            string strOrderByFileds = null)
        {
            return await _db.Queryable(joinExpression)
                .WhereIF(whereExpression != null, whereExpression)
                .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
                .Select(selectExpression)
                .ToDataTableAsync();
        }

        /// <summary>
        /// 七表联合查询
        /// </summary>
        /// <typeparam name="T">实体1</typeparam>
        /// <typeparam name="T2">实体2</typeparam>
        /// <typeparam name="T3">实体3</typeparam>
        /// <typeparam name="T4">实体4</typeparam>
        /// <typeparam name="T5">实体5</typeparam>
        /// <typeparam name="T6">实体6</typeparam>
        /// <typeparam name="T7">实体7</typeparam>
        /// <typeparam name="TResult">返回对象</typeparam>
        /// <param name="joinExpression">关联表达式</param>
        /// <param name="selectExpression">返回表达式</param>
        /// <param name="whereExpression">查询表达式</param>
        /// <param name="strOrderByFileds">排序字段</param>
        /// <returns></returns>
        public async Task<DataTable> QueryTable<T, T2, T3, T4, T5, T6, T7, TResult>(
            Expression<Func<T, T2, T3, T4, T5, T6, T7, object[]>> joinExpression,
            Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> selectExpression,
            Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> whereExpression,
            string strOrderByFileds = null)
        {
            return await _db.Queryable(joinExpression)
                .WhereIF(whereExpression != null, whereExpression)
                .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
                .Select(selectExpression)
                .ToDataTableAsync();
        }

        /// <summary>
        /// 八表联合查询
        /// </summary>
        /// <typeparam name="T">实体1</typeparam>
        /// <typeparam name="T2">实体2</typeparam>
        /// <typeparam name="T3">实体3</typeparam>
        /// <typeparam name="T4">实体4</typeparam>
        /// <typeparam name="T5">实体5</typeparam>
        /// <typeparam name="T6">实体6</typeparam>
        /// <typeparam name="T7">实体7</typeparam>
        /// <typeparam name="T8">实体8</typeparam>
        /// <typeparam name="TResult">返回对象</typeparam>
        /// <param name="joinExpression">关联表达式</param>
        /// <param name="selectExpression">返回表达式</param>
        /// <param name="whereExpression">查询表达式</param>
        /// <param name="strOrderByFileds">排序字段</param>
        /// <returns></returns>
        public async Task<DataTable> QueryTable<T, T2, T3, T4, T5, T6, T7, T8, TResult>(
            Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object[]>> joinExpression,
            Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> selectExpression,
            Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> whereExpression,
            string strOrderByFileds = null)
        {
            return await _db.Queryable(joinExpression)
                .WhereIF(whereExpression != null, whereExpression)
                .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
                .Select(selectExpression)
                .ToDataTableAsync();
        }

        /// <summary>
        /// 功能描述:查询一个列表
        /// </summary>
        /// <param name="whereStr">条件表达式</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>DataTable</returns>
        public async Task<DataTable> QueryTable(string whereStr, object paramObj, string strOrderByFileds)
        {
            return await _db.Queryable<TEntity>().WhereIF(!string.IsNullOrEmpty(whereStr), whereStr, paramObj).OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds).ToDataTableAsync();
        }

        /// <summary>
        /// 功能描述:查询一个列表 - 分页
        /// </summary>
        /// <param name="whereStr">条件表达式</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>DataTable</returns>
        public async Task<PageModel<DataTable>> QueryTablePage(string whereStr, object paramObj, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null)
        {


            RefAsync<int> totalCount = 0;
            var dt = await _db.Queryable<TEntity>()
                .WhereIF(!string.IsNullOrEmpty(whereStr), whereStr, paramObj)
                .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
                .ToDataTablePageAsync(intPageIndex, intPageSize == 0 ? 100 : intPageSize, totalCount);
            int pageCount = (Math.Ceiling(totalCount.ObjToDecimal() / intPageSize.ObjToDecimal())).ObjToInt();
            return new PageModel<DataTable>() { Count = totalCount, PageCount = pageCount, PageIndex = intPageIndex, PageSize = intPageSize, List = dt };
        }

        /// <summary>
        /// 两表联合查询
        /// </summary>
        /// <typeparam name="T">实体1</typeparam>
        /// <typeparam name="T2">实体1</typeparam>
        /// <typeparam name="TResult">返回对象</typeparam>
        /// <param name="joinExpression">关联表达式</param>
        /// <param name="selectExpression">返回表达式</param>
        /// <param name="whereExpression">查询表达式</param>
        /// <param name="strOrderByFileds">排序字段</param>
        /// <returns></returns>
        public async Task<DataTable> QueryTable<T, T2, TResult>(
            Expression<Func<T, T2, object[]>> joinExpression,
            Expression<Func<T, T2, TResult>> selectExpression,
            string whereExpression,
            object paramObj,
            string strOrderByFileds = null)
        {
            return await _db.Queryable(joinExpression)
                .WhereIF(!string.IsNullOrEmpty(whereExpression), whereExpression,paramObj)
                .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
                .Select(selectExpression)
                .ToDataTableAsync();
        }

        /// <summary>
        /// 三表联合查询
        /// </summary>
        /// <typeparam name="T">实体1</typeparam>
        /// <typeparam name="T2">实体2</typeparam>
        /// <typeparam name="T3">实体3</typeparam>
        /// <typeparam name="TResult">返回对象</typeparam>
        /// <param name="joinExpression">关联表达式</param>
        /// <param name="selectExpression">返回表达式</param>
        /// <param name="whereExpression">查询表达式</param>
        /// <param name="strOrderByFileds">排序字段</param>
        /// <returns></returns>
        public async Task<DataTable> QueryTable<T, T2, T3, TResult>(
            Expression<Func<T, T2, T3, object[]>> joinExpression,
            Expression<Func<T, T2, T3, TResult>> selectExpression,
            string whereExpression,
            object paramObj,
            string strOrderByFileds = null)
        {
            return await _db.Queryable(joinExpression)
                .WhereIF(!string.IsNullOrEmpty(whereExpression), whereExpression, paramObj)
                .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
                .Select(selectExpression)
                .ToDataTableAsync();
        }

        /// <summary>
        /// 四表联合查询
        /// </summary>
        /// <typeparam name="T">实体1</typeparam>
        /// <typeparam name="T2">实体2</typeparam>
        /// <typeparam name="T3">实体3</typeparam>
        /// <typeparam name="T4">实体4</typeparam>
        /// <typeparam name="TResult">返回对象</typeparam>
        /// <param name="joinExpression">关联表达式</param>
        /// <param name="selectExpression">返回表达式</param>
        /// <param name="whereExpression">查询表达式</param>
        /// <param name="strOrderByFileds">排序字段</param>
        /// <returns></returns>
        public async Task<DataTable> QueryTable<T, T2, T3, T4, TResult>(
            Expression<Func<T, T2, T3, T4, object[]>> joinExpression,
            Expression<Func<T, T2, T3, T4, TResult>> selectExpression,
            string whereExpression,
            object paramObj,
            string strOrderByFileds = null)
        {
            return await _db.Queryable(joinExpression)
                .WhereIF(!string.IsNullOrEmpty(whereExpression), whereExpression, paramObj)
                .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
                .Select(selectExpression)
                .ToDataTableAsync();
        }

        /// <summary>
        /// 五表联合查询
        /// </summary>
        /// <typeparam name="T">实体1</typeparam>
        /// <typeparam name="T2">实体2</typeparam>
        /// <typeparam name="T3">实体3</typeparam>
        /// <typeparam name="T4">实体4</typeparam>
        /// <typeparam name="T5">实体5</typeparam>
        /// <typeparam name="TResult">返回对象</typeparam>
        /// <param name="joinExpression">关联表达式</param>
        /// <param name="selectExpression">返回表达式</param>
        /// <param name="whereExpression">查询表达式</param>
        /// <param name="strOrderByFileds">排序字段</param>
        /// <returns></returns>
        public async Task<DataTable> QueryTable<T, T2, T3, T4, T5, TResult>(
            Expression<Func<T, T2, T3, T4, T5, object[]>> joinExpression,
            Expression<Func<T, T2, T3, T4, T5, TResult>> selectExpression,
            string whereExpression,
            object paramObj,
            string strOrderByFileds = null)
        {
            return await _db.Queryable(joinExpression)
                .WhereIF(!string.IsNullOrEmpty(whereExpression), whereExpression, paramObj)
                .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
                .Select(selectExpression)
                .ToDataTableAsync();
        }

        /// <summary>
        /// 六表联合查询
        /// </summary>
        /// <typeparam name="T">实体1</typeparam>
        /// <typeparam name="T2">实体2</typeparam>
        /// <typeparam name="T3">实体3</typeparam>
        /// <typeparam name="T4">实体4</typeparam>
        /// <typeparam name="T5">实体5</typeparam>
        /// <typeparam name="T6">实体6</typeparam>
        /// <typeparam name="TResult">返回对象</typeparam>
        /// <param name="joinExpression">关联表达式</param>
        /// <param name="selectExpression">返回表达式</param>
        /// <param name="whereExpression">查询表达式</param>
        /// <param name="strOrderByFileds">排序字段</param>
        /// <returns></returns>
        public async Task<DataTable> QueryTable<T, T2, T3, T4, T5, T6, TResult>(
            Expression<Func<T, T2, T3, T4, T5, T6, object[]>> joinExpression,
            Expression<Func<T, T2, T3, T4, T5, T6, TResult>> selectExpression,
            string whereExpression,
            object paramObj,
            string strOrderByFileds = null)
        {
            return await _db.Queryable(joinExpression)
                .WhereIF(!string.IsNullOrEmpty(whereExpression), whereExpression, paramObj)
                .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
                .Select(selectExpression)
                .ToDataTableAsync();
        }

        /// <summary>
        /// 七表联合查询
        /// </summary>
        /// <typeparam name="T">实体1</typeparam>
        /// <typeparam name="T2">实体2</typeparam>
        /// <typeparam name="T3">实体3</typeparam>
        /// <typeparam name="T4">实体4</typeparam>
        /// <typeparam name="T5">实体5</typeparam>
        /// <typeparam name="T6">实体6</typeparam>
        /// <typeparam name="T7">实体7</typeparam>
        /// <typeparam name="TResult">返回对象</typeparam>
        /// <param name="joinExpression">关联表达式</param>
        /// <param name="selectExpression">返回表达式</param>
        /// <param name="whereExpression">查询表达式</param>
        /// <param name="strOrderByFileds">排序字段</param>
        /// <returns></returns>
        public async Task<DataTable> QueryTable<T, T2, T3, T4, T5, T6, T7, TResult>(
            Expression<Func<T, T2, T3, T4, T5, T6, T7, object[]>> joinExpression,
            Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> selectExpression,
            string whereExpression,
            object paramObj,
            string strOrderByFileds = null)
        {
            return await _db.Queryable(joinExpression)
                .WhereIF(!string.IsNullOrEmpty(whereExpression), whereExpression, paramObj)
                .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
                .Select(selectExpression)
                .ToDataTableAsync();
        }

        /// <summary>
        /// 八表联合查询
        /// </summary>
        /// <typeparam name="T">实体1</typeparam>
        /// <typeparam name="T2">实体2</typeparam>
        /// <typeparam name="T3">实体3</typeparam>
        /// <typeparam name="T4">实体4</typeparam>
        /// <typeparam name="T5">实体5</typeparam>
        /// <typeparam name="T6">实体6</typeparam>
        /// <typeparam name="T7">实体7</typeparam>
        /// <typeparam name="T8">实体8</typeparam>
        /// <typeparam name="TResult">返回对象</typeparam>
        /// <param name="joinExpression">关联表达式</param>
        /// <param name="selectExpression">返回表达式</param>
        /// <param name="whereExpression">查询表达式</param>
        /// <param name="strOrderByFileds">排序字段</param>
        /// <returns></returns>
        public async Task<DataTable> QueryTable<T, T2, T3, T4, T5, T6, T7, T8, TResult>(
            Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object[]>> joinExpression,
            Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> selectExpression,
            string whereExpression,
            object paramObj,
            string strOrderByFileds = null)
        {
            return await _db.Queryable(joinExpression)
                .WhereIF(!string.IsNullOrEmpty(whereExpression), whereExpression, paramObj)
                .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
                .Select(selectExpression)
                .ToDataTableAsync();
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="intPageIndex">页码（下标0）</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>数据列表</returns>
        public async Task<PageModel<TEntity>> QueryPage(Expression<Func<TEntity, bool>> whereExpression, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null)
        {
            RefAsync<int> totalCount = 0;
            var list = await _db.Queryable<TEntity>()
                .WhereIF(whereExpression != null, whereExpression)
                .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
                .ToPageListAsync(intPageIndex, intPageSize == 0 ? 100 : intPageSize, totalCount);

            int pageCount = (Math.Ceiling(totalCount.ObjToDecimal() / intPageSize.ObjToDecimal())).ObjToInt();
            return new PageModel<TEntity>() { PageIndex = intPageIndex, PageSize = intPageSize, PageCount = pageCount, List = list, Count = totalCount };
        }

        /// <summary>
        /// 分页查询 - 自动分页
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="intPageIndex">页码（下标0）</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>数据列表</returns>
        public async Task<PageModel<TEntity>> QueryPageWithSplitTable(Expression<Func<TEntity, bool>> whereExpression, Func<List<SplitTableInfo>, IEnumerable<SplitTableInfo>> splitFunc, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null)
        {
            RefAsync<int> totalCount = 0;
            var list = await _db.Queryable<TEntity>()
                .WhereIF(whereExpression != null, whereExpression)
                .SplitTable(splitFunc)
                .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
                .ToPageListAsync(intPageIndex, intPageSize == 0 ? 100 : intPageSize, totalCount);

            int pageCount = (Math.Ceiling(totalCount.ObjToDecimal() / intPageSize.ObjToDecimal())).ObjToInt();
            return new PageModel<TEntity>() { PageIndex = intPageIndex, PageSize = intPageSize, PageCount = pageCount, List = list, Count = totalCount };
        }

        /// <summary>
        /// 分页查询 - 二级缓存
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="intPageIndex">页码（下标0）</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>数据列表</returns>
        public async Task<PageModel<TEntity>> QueryPageWithCache(Expression<Func<TEntity, bool>> whereExpression, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null)
        {
            RefAsync<int> totalCount = 0;
            var list = await _db.Queryable<TEntity>()
                .WhereIF(whereExpression != null, whereExpression)
                .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
                .WithCache()
                .ToPageListAsync(intPageIndex, intPageSize == 0 ? 100 : intPageSize, totalCount);

            int pageCount = (Math.Ceiling(totalCount.ObjToDecimal() / intPageSize.ObjToDecimal())).ObjToInt();
            return new PageModel<TEntity>() { PageIndex = intPageIndex, PageSize = intPageSize, PageCount = pageCount, List = list, Count = totalCount };
        }

        /// <summary>
        /// 两表联合查询-分页-分组
        /// </summary>
        /// <typeparam name="T">实体</typeparam>
        /// <typeparam name="TResult">返回对象</typeparam>
        /// <param name="selectExpression">返回表达式</param>
        /// <param name="whereExpression">查询表达式</param>
        /// <param name="groupExpression">聚合表达式</param>
        /// <param name="intPageIndex">页码</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="strOrderByFileds">排序字段</param>
        /// <returns></returns>
        public async Task<PageModel<TResult>> QueryPage<T, TResult>(Expression<Func<T, TResult>> selectExpression, Expression<Func<T, object>> groupExpression, Expression<Func<T, bool>> whereExpression, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null)
        {

            RefAsync<int> totalCount = 0;
            var list = await _db.Queryable<T>()
                .WhereIF(whereExpression != null, whereExpression)
                .GroupBy(groupExpression)
             .Select(selectExpression)
             .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
             .ToPageListAsync(intPageIndex, intPageSize == 0 ? 100 : intPageSize, totalCount);

            int pageCount = (Math.Ceiling(totalCount.ObjToDecimal() / intPageSize.ObjToDecimal())).ObjToInt();
            return new PageModel<TResult>() { Count = totalCount, PageCount = pageCount, PageIndex = intPageIndex, PageSize = intPageSize, List = list };
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="whereStr">条件表达式</param>
        /// <param name="intPageIndex">页码（下标0）</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>数据列表</returns>
        public async Task<PageModel<TEntity>> QueryPage(string whereStr, object paramObj, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null)
        {
            RefAsync<int> totalCount = 0;
            var list = await _db.Queryable<TEntity>()
                .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
                .WhereIF(!string.IsNullOrEmpty(whereStr), whereStr, paramObj)
                .ToPageListAsync(intPageIndex, intPageSize == 0 ? 100 : intPageSize, totalCount);

            int pageCount = (Math.Ceiling(totalCount.ObjToDecimal() / intPageSize.ObjToDecimal())).ObjToInt();
            return new PageModel<TEntity>() { PageIndex = intPageIndex, PageSize = intPageSize, PageCount = pageCount, List = list, Count = totalCount };
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="intPageIndex">页码（下标0）</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="sortExpression">排序方法</param>
        /// <param name="sortType">排序类型:OrderByType.Asc，OrderByType.Desc</param>
        /// <returns>数据列表</returns>
        public async Task<PageModel<TEntity>> QueryPage(Expression<Func<TEntity, bool>> whereExpression, int intPageIndex = 1, int intPageSize = 20, Expression<Func<TEntity, object>> sortExpression = null, OrderByType sortType = OrderByType.Asc)
        {
            RefAsync<int> totalCount = 0;
            var list = await _db.Queryable<TEntity>()
                .OrderByIF(sortExpression != null, sortExpression, sortType)
                .WhereIF(whereExpression != null, whereExpression)
                .ToPageListAsync(intPageIndex, intPageSize == 0 ? 100 : intPageSize, totalCount);

            int pageCount = (Math.Ceiling(totalCount.ObjToDecimal() / intPageSize.ObjToDecimal())).ObjToInt();
            return new PageModel<TEntity>() { PageIndex = intPageIndex, PageSize = intPageSize, PageCount = pageCount, List = list, Count = totalCount };
        }

        /// <summary> 
        ///查询-两表联合查询
        /// </summary> 
        /// <typeparam name="T">实体1</typeparam>
        /// <typeparam name="T2">实体2</typeparam>
        /// <typeparam name="TResult">返回对象</typeparam>
        /// <param name="joinExpression">关联表达式 (t1, t2) => new object[] {JoinType.Left, t1.id == t2.id}</param> 
        /// <param name="selectExpression">返回表达式 (t1, t2) => new tr{ Id = t1.id, Id1 = t2.name}</param>
        /// <param name="whereLambda">查询表达式 (t1, t2, tr) => (t1.name == "")</param> 
        /// <returns>数据列表</returns>
        public async Task<List<TResult>> QueryMuch<T, T2, TResult>(
            Expression<Func<T, T2, object[]>> joinExpression,
            Expression<Func<T, T2, TResult>> selectExpression,
            Expression<Func<T, T2, bool>> whereLambda = null) where T : class, new()
        {
            if (whereLambda == null)
            {
                return await _db.Queryable(joinExpression).Select(selectExpression).ToListAsync();
            }
            return await _db.Queryable(joinExpression).Where(whereLambda).Select(selectExpression).ToListAsync();
        }

        /// <summary> 
        ///查询-三表联合查询
        /// </summary> 
        /// <typeparam name="T">实体1</typeparam>
        /// <typeparam name="T2">实体2</typeparam>
        /// <typeparam name="T3">实体3</typeparam>
        /// <typeparam name="TResult">返回对象</typeparam>
        /// <param name="joinExpression">关联表达式 (t1, t2, t3) => new object[] {JoinType.Left, t1.id == t2.id, JoinType.Left, t1.id == t3.id}</param> 
        /// <param name="selectExpression">返回表达式 (t1, t2, t3, tr) => new tr{ id1 = t1.id, id2 = t2.id, id3 = t3.id }</param>
        /// <param name="whereLambda">查询表达式 (t1, t2, t3, tr) => (t1.name == "")</param> 
        /// <returns>数据列表</returns>
        public async Task<List<TResult>> QueryMuch<T, T2, T3, TResult>(
            Expression<Func<T, T2, T3, object[]>> joinExpression,
            Expression<Func<T, T2, T3, TResult>> selectExpression,
            Expression<Func<T, T2, T3, bool>> whereLambda = null) where T : class, new()
        {
            if (whereLambda == null)
            {
                return await _db.Queryable(joinExpression).Select(selectExpression).ToListAsync();
            }
            return await _db.Queryable(joinExpression).Where(whereLambda).Select(selectExpression).ToListAsync();
        }

        /// <summary> 
        ///查询-四表联合查询
        /// </summary> 
        /// <typeparam name="T">实体1</typeparam>
        /// <typeparam name="T2">实体2</typeparam>
        /// <typeparam name="T3">实体3</typeparam>
        /// <typeparam name="T4">实体4</typeparam>
        /// <typeparam name="TResult">返回对象</typeparam>
        /// <param name="joinExpression">关联表达式 (t1, t2, t3, t4) => new object[] {JoinType.Left, t1.id == t2.id, JoinType.Left, t1.id == t3.id}</param> 
        /// <param name="selectExpression">返回表达式 (t1, t2, t3, t4, tr) => new tr{ id1 = t1.id, id2 = t2.id, id3 = t3.id }</param>
        /// <param name="whereLambda">查询表达式 (t1, t2, t3, t4, tr) => (t1.name == "")</param> 
        /// <returns>数据列表</returns>
        public async Task<List<TResult>> QueryMuch<T, T2, T3, T4, TResult>(
            Expression<Func<T, T2, T3, T4, object[]>> joinExpression,
            Expression<Func<T, T2, T3, T4, TResult>> selectExpression,
            Expression<Func<T, T2, T3, T4, bool>> whereLambda = null) where T : class, new()
        {
            if (whereLambda == null)
            {
                return await _db.Queryable(joinExpression).Select(selectExpression).ToListAsync();
            }
            return await _db.Queryable(joinExpression).Where(whereLambda).Select(selectExpression).ToListAsync();
        }

        /// <summary> 
        ///查询-五表联合查询
        /// </summary> 
        /// <typeparam name="T">实体1</typeparam>
        /// <typeparam name="T2">实体2</typeparam>
        /// <typeparam name="T3">实体3</typeparam>
        /// <typeparam name="T4">实体4</typeparam>
        /// <typeparam name="T5">实体5</typeparam>
        /// <typeparam name="TResult">返回对象</typeparam>
        /// <param name="joinExpression">关联表达式 (t1, t2, t3, t4, t5) => new object[] {JoinType.Left, t1.id == t2.id, JoinType.Left, t1.id == t3.id}</param> 
        /// <param name="selectExpression">返回表达式 (t1, t2, t3, t4, t5, tr) => new tr{ id1 = t1.id, id2 = t2.id, id3 = t3.id }</param>
        /// <param name="whereLambda">查询表达式 (t1, t2, t3, t4, t5, tr) => (t1.name == "")</param> 
        /// <returns>数据列表</returns>
        public async Task<List<TResult>> QueryMuch<T, T2, T3, T4, T5, TResult>(
            Expression<Func<T, T2, T3, T4, T5, object[]>> joinExpression,
            Expression<Func<T, T2, T3, T4, T5, TResult>> selectExpression,
            Expression<Func<T, T2, T3, T4, T5, bool>> whereLambda = null) where T : class, new()
        {
            if (whereLambda == null)
            {
                return await _db.Queryable(joinExpression).Select(selectExpression).ToListAsync();
            }
            return await _db.Queryable(joinExpression).Where(whereLambda).Select(selectExpression).ToListAsync();
        }

        /// <summary> 
        ///查询-六表联合查询
        /// </summary> 
        /// <typeparam name="T">实体1</typeparam>
        /// <typeparam name="T2">实体2</typeparam>
        /// <typeparam name="T3">实体3</typeparam>
        /// <typeparam name="T4">实体4</typeparam>
        /// <typeparam name="T5">实体5</typeparam>
        /// <typeparam name="T6">实体6</typeparam>
        /// <typeparam name="TResult">返回对象</typeparam>
        /// <param name="joinExpression">关联表达式 (t1, t2, t3, t4, t5, t6) => new object[] {JoinType.Left, t1.id == t2.id, JoinType.Left, t1.id == t3.id}</param> 
        /// <param name="selectExpression">返回表达式 (t1, t2, t3, t4, t5,, t6 tr) => new tr{ id1 = t1.id, id2 = t2.id, id3 = t3.id }</param>
        /// <param name="whereLambda">查询表达式 (t1, t2, t3, t4, t5,, t6 tr) => (t1.name == "")</param> 
        /// <returns>数据列表</returns>
        public async Task<List<TResult>> QueryMuch<T, T2, T3, T4, T5, T6, TResult>(
            Expression<Func<T, T2, T3, T4, T5, T6, object[]>> joinExpression,
            Expression<Func<T, T2, T3, T4, T5, T6, TResult>> selectExpression,
            Expression<Func<T, T2, T3, T4, T5, T6, bool>> whereLambda = null) where T : class, new()
        {
            if (whereLambda == null)
            {
                return await _db.Queryable(joinExpression).Select(selectExpression).ToListAsync();
            }
            return await _db.Queryable(joinExpression).Where(whereLambda).Select(selectExpression).ToListAsync();
        }

        /// <summary> 
        ///查询-七表联合查询
        /// </summary> 
        /// <typeparam name="T">实体1</typeparam>
        /// <typeparam name="T2">实体2</typeparam>
        /// <typeparam name="T3">实体3</typeparam>
        /// <typeparam name="T4">实体4</typeparam>
        /// <typeparam name="T5">实体5</typeparam>
        /// <typeparam name="T6">实体6</typeparam>
        /// <typeparam name="T7">实体7</typeparam>
        /// <typeparam name="TResult">返回对象</typeparam>
        /// <param name="joinExpression">关联表达式 (t1, t2, t3, t4, t5, t6) => new object[] {JoinType.Left, t1.id == t2.id, JoinType.Left, t1.id == t3.id}</param> 
        /// <param name="selectExpression">返回表达式 (t1, t2, t3, t4, t5,, t6 tr) => new tr{ id1 = t1.id, id2 = t2.id, id3 = t3.id }</param>
        /// <param name="whereLambda">查询表达式 (t1, t2, t3, t4, t5,, t6 tr) => (t1.name == "")</param> 
        /// <returns>数据列表</returns>
        public async Task<List<TResult>> QueryMuch<T, T2, T3, T4, T5, T6, T7, TResult>(
            Expression<Func<T, T2, T3, T4, T5, T6, T7, object[]>> joinExpression,
            Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> selectExpression,
            Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> whereLambda = null) where T : class, new()
        {
            if (whereLambda == null)
            {
                return await _db.Queryable(joinExpression).Select(selectExpression).ToListAsync();
            }
            return await _db.Queryable(joinExpression).Where(whereLambda).Select(selectExpression).ToListAsync();
        }

        /// <summary> 
        ///查询-八表联合查询
        /// </summary> 
        /// <typeparam name="T">实体1</typeparam>
        /// <typeparam name="T2">实体2</typeparam>
        /// <typeparam name="T3">实体3</typeparam>
        /// <typeparam name="T4">实体4</typeparam>
        /// <typeparam name="T5">实体5</typeparam>
        /// <typeparam name="T6">实体6</typeparam>
        /// <typeparam name="T7">实体7</typeparam>
        /// <typeparam name="T8">实体8</typeparam>
        /// <typeparam name="TResult">返回对象</typeparam>
        /// <param name="joinExpression">关联表达式 (t1, t2, t3, t4, t5, t6) => new object[] {JoinType.Left, t1.id == t2.id, JoinType.Left, t1.id == t3.id}</param> 
        /// <param name="selectExpression">返回表达式 (t1, t2, t3, t4, t5,, t6 tr) => new tr{ id1 = t1.id, id2 = t2.id, id3 = t3.id }</param>
        /// <param name="whereLambda">查询表达式 (t1, t2, t3, t4, t5,, t6 tr) => (t1.name == "")</param> 
        /// <returns>数据列表</returns>
        public async Task<List<TResult>> QueryMuch<T, T2, T3, T4, T5, T6, T7, T8, TResult>(
            Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object[]>> joinExpression,
            Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> selectExpression,
            Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> whereLambda = null) where T : class, new()
        {
            if (whereLambda == null)
            {
                return await _db.Queryable(joinExpression).Select(selectExpression).ToListAsync();
            }
            return await _db.Queryable(joinExpression).Where(whereLambda).Select(selectExpression).ToListAsync();
        }

        /// <summary>
        /// 两表联合查询-分页
        /// </summary>
        /// <typeparam name="T">实体1</typeparam>
        /// <typeparam name="T2">实体1</typeparam>
        /// <typeparam name="TResult">返回对象</typeparam>
        /// <param name="joinExpression">关联表达式</param>
        /// <param name="selectExpression">返回表达式</param>
        /// <param name="whereExpression">查询表达式</param>
        /// <param name="intPageIndex">页码</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="strOrderByFileds">排序字段</param>
        /// <returns></returns>
        public async Task<PageModel<TResult>> QueryMuchPage<T, T2, TResult>(
            Expression<Func<T, T2, object[]>> joinExpression,
            Expression<Func<T, T2, TResult>> selectExpression,
            Expression<Func<T, T2, bool>> whereExpression,
            int intPageIndex = 1,
            int intPageSize = 20,
            string strOrderByFileds = null)
        {
            RefAsync<int> totalCount = 0;
            var list = await _db.Queryable(joinExpression)
                .WhereIF(whereExpression != null, whereExpression)
             .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
             .Select(selectExpression)
             .ToPageListAsync(intPageIndex, intPageSize == 0 ? 100 : intPageSize, totalCount);
            int pageCount = (Math.Ceiling(totalCount.ObjToDecimal() / intPageSize.ObjToDecimal())).ObjToInt();
            return new PageModel<TResult>() { Count = totalCount, PageCount = pageCount, PageIndex = intPageIndex, PageSize = intPageSize, List = list };
        }

        /// <summary>
        /// 两表联合查询-分页-分组
        /// </summary>
        /// <typeparam name="T">实体1</typeparam>
        /// <typeparam name="T2">实体1</typeparam>
        /// <typeparam name="TResult">返回对象</typeparam>
        /// <param name="joinExpression">关联表达式</param>
        /// <param name="selectExpression">返回表达式</param>
        /// <param name="whereExpression">查询表达式</param>
        /// <param name="groupExpression">聚合表达式</param>
        /// <param name="intPageIndex">页码</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="strOrderByFileds">排序字段</param>
        /// <returns></returns>
        public async Task<PageModel<TResult>> QueryMuchPage<T, T2, TResult>(
            Expression<Func<T, T2, object[]>> joinExpression,
            Expression<Func<T, T2, TResult>> selectExpression,
            Expression<Func<TResult, bool>> whereExpression,
            Expression<Func<T, object>> groupExpression,
            int intPageIndex = 1,
            int intPageSize = 20,
            string strOrderByFileds = null)
        {

            RefAsync<int> totalCount = 0;
            var list = await _db.Queryable<T, T2>(joinExpression).GroupBy(groupExpression)
             .Select(selectExpression)
             .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
             .WhereIF(whereExpression != null, whereExpression)
             .ToPageListAsync(intPageIndex, intPageSize == 0 ? 100 : intPageSize, totalCount);
            int pageCount = (Math.Ceiling(totalCount.ObjToDecimal() / intPageSize.ObjToDecimal())).ObjToInt();
            return new PageModel<TResult>() { Count = totalCount, PageCount = pageCount, PageIndex = intPageIndex, PageSize = intPageSize, List = list };
        }

        /// <summary>
        /// 三表联合查询-分页
        /// </summary>
        /// <typeparam name="T">实体1</typeparam>
        /// <typeparam name="T2">实体2</typeparam>
        /// <typeparam name="T3">实体3</typeparam>
        /// <typeparam name="TResult">返回对象</typeparam>
        /// <param name="joinExpression">关联表达式</param>
        /// <param name="selectExpression">返回表达式</param>
        /// <param name="whereExpression">查询表达式</param>
        /// <param name="intPageIndex">页码</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="strOrderByFileds">排序字段</param>
        /// <returns></returns>
        public async Task<PageModel<TResult>> QueryMuchPage<T, T2, T3, TResult>(
            Expression<Func<T, T2, T3, object[]>> joinExpression,
            Expression<Func<T, T2, T3, TResult>> selectExpression,
            Expression<Func<T, T2, T3, bool>> whereExpression,
            int intPageIndex = 1,
            int intPageSize = 20,
            string strOrderByFileds = null)
        {
            RefAsync<int> totalCount = 0;
            var list = await _db.Queryable(joinExpression)
                .WhereIF(whereExpression != null, whereExpression)
             .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
             .Select(selectExpression)
             //.WhereIF(whereExpression != null, whereExpression)
             .ToPageListAsync(intPageIndex, intPageSize == 0 ? 100 : intPageSize, totalCount);
            int pageCount = (Math.Ceiling(totalCount.ObjToDecimal() / intPageSize.ObjToDecimal())).ObjToInt();
            return new PageModel<TResult>() { Count = totalCount, PageCount = pageCount, PageIndex = intPageIndex, PageSize = intPageSize, List = list };
        }

        /// <summary>
        /// 四表联合查询-分页
        /// </summary>
        /// <typeparam name="T">实体1</typeparam>
        /// <typeparam name="T2">实体2</typeparam>
        /// <typeparam name="T3">实体3</typeparam>
        /// <typeparam name="T4">实体4</typeparam>
        /// <typeparam name="TResult">返回对象</typeparam>
        /// <param name="joinExpression">关联表达式</param>
        /// <param name="selectExpression">返回表达式</param>
        /// <param name="whereExpression">查询表达式</param>
        /// <param name="intPageIndex">页码</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="strOrderByFileds">排序字段</param>
        /// <returns></returns>
        public async Task<PageModel<TResult>> QueryMuchPage<T, T2, T3, T4, TResult>(
            Expression<Func<T, T2, T3, T4, object[]>> joinExpression,
            Expression<Func<T, T2, T3, T4, TResult>> selectExpression,
            Expression<Func<T, T2, T3, T4, bool>> whereExpression,
            int intPageIndex = 1,
            int intPageSize = 20,
            string strOrderByFileds = null)
        {
            RefAsync<int> totalCount = 0;
            var list = await _db.Queryable(joinExpression)
                .WhereIF(whereExpression != null, whereExpression)
             .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
             .Select(selectExpression)
             //.WhereIF(whereExpression != null, whereExpression)
             .ToPageListAsync(intPageIndex, intPageSize == 0 ? 100 : intPageSize, totalCount);
            int pageCount = (Math.Ceiling(totalCount.ObjToDecimal() / intPageSize.ObjToDecimal())).ObjToInt();
            return new PageModel<TResult>() { Count = totalCount, PageCount = pageCount, PageIndex = intPageIndex, PageSize = intPageSize, List = list };
        }

        /// <summary>
        /// 五表联合查询-分页
        /// </summary>
        /// <typeparam name="T">实体1</typeparam>
        /// <typeparam name="T2">实体2</typeparam>
        /// <typeparam name="T3">实体3</typeparam>
        /// <typeparam name="T4">实体4</typeparam>
        /// <typeparam name="T5">实体5</typeparam>
        /// <typeparam name="TResult">返回对象</typeparam>
        /// <param name="joinExpression">关联表达式</param>
        /// <param name="selectExpression">返回表达式</param>
        /// <param name="whereExpression">查询表达式</param>
        /// <param name="intPageIndex">页码</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="strOrderByFileds">排序字段</param>
        /// <returns></returns>
        public async Task<PageModel<TResult>> QueryMuchPage<T, T2, T3, T4, T5, TResult>(
            Expression<Func<T, T2, T3, T4, T5, object[]>> joinExpression,
            Expression<Func<T, T2, T3, T4, T5, TResult>> selectExpression,
            Expression<Func<T, T2, T3, T4, T5, bool>> whereExpression,
            int intPageIndex = 1,
            int intPageSize = 20,
            string strOrderByFileds = null)
        {
            RefAsync<int> totalCount = 0;
            var list = await _db.Queryable(joinExpression)
                .WhereIF(whereExpression != null, whereExpression)
             .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
             .Select(selectExpression)
             //.WhereIF(whereExpression != null, whereExpression)
             .ToPageListAsync(intPageIndex, intPageSize == 0 ? 100 : intPageSize, totalCount);
            int pageCount = (Math.Ceiling(totalCount.ObjToDecimal() / intPageSize.ObjToDecimal())).ObjToInt();
            return new PageModel<TResult>() { Count = totalCount, PageCount = pageCount, PageIndex = intPageIndex, PageSize = intPageSize, List = list };
        }

        /// <summary>
        /// 六表联合查询-分页
        /// </summary>
        /// <typeparam name="T">实体1</typeparam>
        /// <typeparam name="T2">实体2</typeparam>
        /// <typeparam name="T3">实体3</typeparam>
        /// <typeparam name="T4">实体4</typeparam>
        /// <typeparam name="T5">实体5</typeparam>
        /// <typeparam name="T6">实体6</typeparam>
        /// <typeparam name="TResult">返回对象</typeparam>
        /// <param name="joinExpression">关联表达式</param>
        /// <param name="selectExpression">返回表达式</param>
        /// <param name="whereExpression">查询表达式</param>
        /// <param name="intPageIndex">页码</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="strOrderByFileds">排序字段</param>
        /// <returns></returns>
        public async Task<PageModel<TResult>> QueryMuchPage<T, T2, T3, T4, T5, T6, TResult>(
            Expression<Func<T, T2, T3, T4, T5, T6, object[]>> joinExpression,
            Expression<Func<T, T2, T3, T4, T5, T6, TResult>> selectExpression,
            Expression<Func<T, T2, T3, T4, T5, T6, bool>> whereExpression,
            int intPageIndex = 1,
            int intPageSize = 20,
            string strOrderByFileds = null)
        {
            RefAsync<int> totalCount = 0;
            var list = await _db.Queryable(joinExpression)
                .WhereIF(whereExpression != null, whereExpression)
             .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
             .Select(selectExpression)
             //.WhereIF(whereExpression != null, whereExpression)
             .ToPageListAsync(intPageIndex, intPageSize == 0 ? 100 : intPageSize, totalCount);
            int pageCount = (Math.Ceiling(totalCount.ObjToDecimal() / intPageSize.ObjToDecimal())).ObjToInt();
            return new PageModel<TResult>() { Count = totalCount, PageCount = pageCount, PageIndex = intPageIndex, PageSize = intPageSize, List = list };
        }

        /// <summary>
        /// 七表联合查询-分页
        /// </summary>
        /// <typeparam name="T">实体1</typeparam>
        /// <typeparam name="T2">实体2</typeparam>
        /// <typeparam name="T3">实体3</typeparam>
        /// <typeparam name="T4">实体4</typeparam>
        /// <typeparam name="T5">实体5</typeparam>
        /// <typeparam name="T6">实体6</typeparam>
        /// <typeparam name="T7">实体7</typeparam>
        /// <typeparam name="TResult">返回对象</typeparam>
        /// <param name="joinExpression">关联表达式</param>
        /// <param name="selectExpression">返回表达式</param>
        /// <param name="whereExpression">查询表达式</param>
        /// <param name="intPageIndex">页码</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="strOrderByFileds">排序字段</param>
        /// <returns></returns>
        public async Task<PageModel<TResult>> QueryMuchPage<T, T2, T3, T4, T5, T6, T7, TResult>(
            Expression<Func<T, T2, T3, T4, T5, T6, T7, object[]>> joinExpression,
            Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> selectExpression,
            Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> whereExpression,
            int intPageIndex = 1,
            int intPageSize = 20,
            string strOrderByFileds = null)
        {
            RefAsync<int> totalCount = 0;
            var list = await _db.Queryable(joinExpression)
                .WhereIF(whereExpression != null, whereExpression)
             .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
             .Select(selectExpression)
             //.WhereIF(whereExpression != null, whereExpression)
             .ToPageListAsync(intPageIndex, intPageSize == 0 ? 100 : intPageSize, totalCount);
            int pageCount = (Math.Ceiling(totalCount.ObjToDecimal() / intPageSize.ObjToDecimal())).ObjToInt();
            return new PageModel<TResult>() { Count = totalCount, PageCount = pageCount, PageIndex = intPageIndex, PageSize = intPageSize, List = list };
        }

        /// <summary>
        /// 八表联合查询-分页
        /// </summary>
        /// <typeparam name="T">实体1</typeparam>
        /// <typeparam name="T2">实体2</typeparam>
        /// <typeparam name="T3">实体3</typeparam>
        /// <typeparam name="T4">实体4</typeparam>
        /// <typeparam name="T5">实体5</typeparam>
        /// <typeparam name="T6">实体6</typeparam>
        /// <typeparam name="T7">实体7</typeparam>
        /// <typeparam name="T8">实体8</typeparam>
        /// <typeparam name="TResult">返回对象</typeparam>
        /// <param name="joinExpression">关联表达式</param>
        /// <param name="selectExpression">返回表达式</param>
        /// <param name="whereExpression">查询表达式</param>
        /// <param name="intPageIndex">页码</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="strOrderByFileds">排序字段</param>
        /// <returns></returns>
        public async Task<PageModel<TResult>> QueryMuchPage<T, T2, T3, T4, T5, T6, T7, T8, TResult>(
            Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object[]>> joinExpression,
            Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> selectExpression,
            Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> whereExpression,
            int intPageIndex = 1,
            int intPageSize = 20,
            string strOrderByFileds = null)
        {
            RefAsync<int> totalCount = 0;
            var list = await _db.Queryable(joinExpression)
                .WhereIF(whereExpression != null, whereExpression)
             .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
             .Select(selectExpression)
             //.WhereIF(whereExpression != null, whereExpression)
             .ToPageListAsync(intPageIndex, intPageSize == 0 ? 100 : intPageSize, totalCount);
            int pageCount = (Math.Ceiling(totalCount.ObjToDecimal() / intPageSize.ObjToDecimal())).ObjToInt();
            return new PageModel<TResult>() { Count = totalCount, PageCount = pageCount, PageIndex = intPageIndex, PageSize = intPageSize, List = list };
        }

        /// <summary>
        /// 两表联合查询
        /// </summary>
        /// <typeparam name="T">实体1</typeparam>
        /// <typeparam name="T2">实体1</typeparam>
        /// <typeparam name="TResult">返回对象</typeparam>
        /// <param name="joinExpression">关联表达式</param>
        /// <param name="selectExpression">返回表达式</param>
        /// <param name="whereExpression">查询表达式</param>
        /// <returns></returns>
        public async Task<List<TResult>> QueryMuch<T, T2, TResult>(
            Expression<Func<T, T2, object[]>> joinExpression,
            Expression<Func<T, T2, TResult>> selectExpression,
            string whereExpression,
            object paramObj)
        {
            return await _db.Queryable(joinExpression)
                .WhereIF(!string.IsNullOrEmpty(whereExpression), whereExpression, paramObj)
             .Select(selectExpression)
             .ToListAsync();
        }

        /// <summary>
        /// 三表联合查询
        /// </summary>
        /// <typeparam name="T">实体1</typeparam>
        /// <typeparam name="T2">实体2</typeparam>
        /// <typeparam name="T3">实体3</typeparam>
        /// <typeparam name="TResult">返回对象</typeparam>
        /// <param name="joinExpression">关联表达式</param>
        /// <param name="selectExpression">返回表达式</param>
        /// <param name="whereExpression">查询表达式</param>
        /// <returns></returns>
        public async Task<List<TResult>> QueryMuch<T, T2, T3, TResult>(
            Expression<Func<T, T2, T3, object[]>> joinExpression,
            Expression<Func<T, T2, T3, TResult>> selectExpression,
            string whereExpression,
            object paramObj)
        {
            return await _db.Queryable(joinExpression)
                .WhereIF(!string.IsNullOrEmpty(whereExpression), whereExpression, paramObj)
             .Select(selectExpression)
             .ToListAsync();
        }

        /// <summary>
        /// 四表联合查询-分页
        /// </summary>
        /// <typeparam name="T">实体1</typeparam>
        /// <typeparam name="T2">实体2</typeparam>
        /// <typeparam name="T3">实体3</typeparam>
        /// <typeparam name="T4">实体4</typeparam>
        /// <typeparam name="TResult">返回对象</typeparam>
        /// <param name="joinExpression">关联表达式</param>
        /// <param name="selectExpression">返回表达式</param>
        /// <param name="whereExpression">查询表达式</param>
        /// <returns></returns>
        public async Task<List<TResult>> QueryMuch<T, T2, T3, T4, TResult>(
            Expression<Func<T, T2, T3, T4, object[]>> joinExpression,
            Expression<Func<T, T2, T3, T4, TResult>> selectExpression,
            string whereExpression,
            object paramObj)
        {
            return await _db.Queryable(joinExpression)
                .WhereIF(!string.IsNullOrEmpty(whereExpression), whereExpression, paramObj)
             .Select(selectExpression)
             .ToListAsync();
        }

        /// <summary>
        /// 五表联合查询
        /// </summary>
        /// <typeparam name="T">实体1</typeparam>
        /// <typeparam name="T2">实体2</typeparam>
        /// <typeparam name="T3">实体3</typeparam>
        /// <typeparam name="T4">实体4</typeparam>
        /// <typeparam name="T5">实体5</typeparam>
        /// <typeparam name="TResult">返回对象</typeparam>
        /// <param name="joinExpression">关联表达式</param>
        /// <param name="selectExpression">返回表达式</param>
        /// <param name="whereExpression">查询表达式</param>
        /// <returns></returns>
        public async Task<List<TResult>> QueryMuch<T, T2, T3, T4, T5, TResult>(
            Expression<Func<T, T2, T3, T4, T5, object[]>> joinExpression,
            Expression<Func<T, T2, T3, T4, T5, TResult>> selectExpression,
            string whereExpression,
            object paramObj)
        {
            return await _db.Queryable(joinExpression)
                .WhereIF(!string.IsNullOrEmpty(whereExpression), whereExpression, paramObj)
             .Select(selectExpression)
             .ToListAsync();
        }

        /// <summary>
        /// 六表联合查询
        /// </summary>
        /// <typeparam name="T">实体1</typeparam>
        /// <typeparam name="T2">实体2</typeparam>
        /// <typeparam name="T3">实体3</typeparam>
        /// <typeparam name="T4">实体4</typeparam>
        /// <typeparam name="T5">实体5</typeparam>
        /// <typeparam name="T6">实体6</typeparam>
        /// <typeparam name="TResult">返回对象</typeparam>
        /// <param name="joinExpression">关联表达式</param>
        /// <param name="selectExpression">返回表达式</param>
        /// <param name="whereExpression">查询表达式</param>
        /// <param name="paramObj">参数化</param>
        /// <returns></returns>
        public async Task<List<TResult>> QueryMuch<T, T2, T3, T4, T5, T6, TResult>(
            Expression<Func<T, T2, T3, T4, T5, T6, object[]>> joinExpression,
            Expression<Func<T, T2, T3, T4, T5, T6, TResult>> selectExpression,
            string whereExpression,
            object paramObj)
        {
            return await _db.Queryable(joinExpression)
                .WhereIF(!string.IsNullOrEmpty(whereExpression), whereExpression, paramObj)
             .Select(selectExpression)
             .ToListAsync();
        }

        /// <summary>
        /// 七表联合查询
        /// </summary>
        /// <typeparam name="T">实体1</typeparam>
        /// <typeparam name="T2">实体2</typeparam>
        /// <typeparam name="T3">实体3</typeparam>
        /// <typeparam name="T4">实体4</typeparam>
        /// <typeparam name="T5">实体5</typeparam>
        /// <typeparam name="T6">实体6</typeparam>
        /// <typeparam name="T7">实体7</typeparam>
        /// <typeparam name="TResult">返回对象</typeparam>
        /// <param name="joinExpression">关联表达式</param>
        /// <param name="selectExpression">返回表达式</param>
        /// <param name="whereExpression">查询表达式</param>
        /// <param name="paramObj">参数化</param>
        /// <returns></returns>
        public async Task<List<TResult>> QueryMuch<T, T2, T3, T4, T5, T6, T7, TResult>(
            Expression<Func<T, T2, T3, T4, T5, T6, T7, object[]>> joinExpression,
            Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> selectExpression,
            string whereExpression,
            object paramObj)
        {
            return await _db.Queryable(joinExpression)
                .WhereIF(!string.IsNullOrEmpty(whereExpression), whereExpression, paramObj)
             .Select(selectExpression)
             .ToListAsync();
        }

        /// <summary>
        /// 八表联合查询
        /// </summary>
        /// <typeparam name="T">实体1</typeparam>
        /// <typeparam name="T2">实体2</typeparam>
        /// <typeparam name="T3">实体3</typeparam>
        /// <typeparam name="T4">实体4</typeparam>
        /// <typeparam name="T5">实体5</typeparam>
        /// <typeparam name="T6">实体6</typeparam>
        /// <typeparam name="T7">实体7</typeparam>
        /// <typeparam name="T8">实体8</typeparam>
        /// <typeparam name="TResult">返回对象</typeparam>
        /// <param name="joinExpression">关联表达式</param>
        /// <param name="selectExpression">返回表达式</param>
        /// <param name="whereExpression">查询表达式</param>
        /// <param name="paramObj">参数化</param>
        /// <returns></returns>
        public async Task<List<TResult>> QueryMuch<T, T2, T3, T4, T5, T6, T7, T8, TResult>(
            Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object[]>> joinExpression,
            Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> selectExpression,
            string whereExpression,
            object paramObj)
        {
            return await _db.Queryable(joinExpression)
                .WhereIF(!string.IsNullOrEmpty(whereExpression), whereExpression, paramObj)
             .Select(selectExpression)
             .ToListAsync();
        }

        /// <summary>
        /// 两表联合查询-分页
        /// </summary>
        /// <typeparam name="T">实体1</typeparam>
        /// <typeparam name="T2">实体1</typeparam>
        /// <typeparam name="TResult">返回对象</typeparam>
        /// <param name="joinExpression">关联表达式</param>
        /// <param name="selectExpression">返回表达式</param>
        /// <param name="whereExpression">查询表达式</param>
        /// <param name="intPageIndex">页码</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="strOrderByFileds">排序字段</param>
        /// <returns></returns>
        public async Task<PageModel<TResult>> QueryMuchPage<T, T2, TResult>(
            Expression<Func<T, T2, object[]>> joinExpression,
            Expression<Func<T, T2, TResult>> selectExpression,
            string whereExpression,
            object paramObj,
            int intPageIndex = 1,
            int intPageSize = 20,
            string strOrderByFileds = null)
        {
            RefAsync<int> totalCount = 0;
            var list = await _db.Queryable(joinExpression)
                .WhereIF(!string.IsNullOrEmpty(whereExpression), whereExpression, paramObj)
             .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
             .Select(selectExpression)
             .ToPageListAsync(intPageIndex, intPageSize == 0 ? 100 : intPageSize, totalCount);
            int pageCount = (Math.Ceiling(totalCount.ObjToDecimal() / intPageSize.ObjToDecimal())).ObjToInt();
            return new PageModel<TResult>() { Count = totalCount, PageCount = pageCount, PageIndex = intPageIndex, PageSize = intPageSize, List = list };
        }

        /// <summary>
        /// 两表联合查询-分页-分组
        /// </summary>
        /// <typeparam name="T">实体1</typeparam>
        /// <typeparam name="T2">实体1</typeparam>
        /// <typeparam name="TResult">返回对象</typeparam>
        /// <param name="joinExpression">关联表达式</param>
        /// <param name="selectExpression">返回表达式</param>
        /// <param name="whereExpression">查询表达式</param>
        /// <param name="groupExpression">聚合表达式</param>
        /// <param name="intPageIndex">页码</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="strOrderByFileds">排序字段</param>
        /// <returns></returns>
        public async Task<PageModel<TResult>> QueryMuchPage<T, T2, TResult>(
            Expression<Func<T, T2, object[]>> joinExpression,
            Expression<Func<T, T2, TResult>> selectExpression,
            string whereExpression,
            object paramObj,
            Expression<Func<T, object>> groupExpression,
            int intPageIndex = 1,
            int intPageSize = 20,
            string strOrderByFileds = null)
        {

            RefAsync<int> totalCount = 0;
            var list = await _db.Queryable<T, T2>(joinExpression).GroupBy(groupExpression)
             .Select(selectExpression)
             .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
             .WhereIF(!string.IsNullOrEmpty(whereExpression), whereExpression, paramObj)
             .ToPageListAsync(intPageIndex, intPageSize == 0 ? 100 : intPageSize, totalCount);
            int pageCount = (Math.Ceiling(totalCount.ObjToDecimal() / intPageSize.ObjToDecimal())).ObjToInt();
            return new PageModel<TResult>() { Count = totalCount, PageCount = pageCount, PageIndex = intPageIndex, PageSize = intPageSize, List = list };
        }

        /// <summary>
        /// 三表联合查询-分页
        /// </summary>
        /// <typeparam name="T">实体1</typeparam>
        /// <typeparam name="T2">实体2</typeparam>
        /// <typeparam name="T3">实体3</typeparam>
        /// <typeparam name="TResult">返回对象</typeparam>
        /// <param name="joinExpression">关联表达式</param>
        /// <param name="selectExpression">返回表达式</param>
        /// <param name="whereExpression">查询表达式</param>
        /// <param name="intPageIndex">页码</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="strOrderByFileds">排序字段</param>
        /// <returns></returns>
        public async Task<PageModel<TResult>> QueryMuchPage<T, T2, T3, TResult>(
            Expression<Func<T, T2, T3, object[]>> joinExpression,
            Expression<Func<T, T2, T3, TResult>> selectExpression,
            string whereExpression,
            object paramObj,
            int intPageIndex = 1,
            int intPageSize = 20,
            string strOrderByFileds = null)
        {
            RefAsync<int> totalCount = 0;
            var list = await _db.Queryable(joinExpression)
                .WhereIF(!string.IsNullOrEmpty(whereExpression), whereExpression, paramObj)
             .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
             .Select(selectExpression)
             //.WhereIF(whereExpression != null, whereExpression)
             .ToPageListAsync(intPageIndex, intPageSize == 0 ? 100 : intPageSize, totalCount);
            int pageCount = (Math.Ceiling(totalCount.ObjToDecimal() / intPageSize.ObjToDecimal())).ObjToInt();
            return new PageModel<TResult>() { Count = totalCount, PageCount = pageCount, PageIndex = intPageIndex, PageSize = intPageSize, List = list };
        }

        /// <summary>
        /// 四表联合查询-分页
        /// </summary>
        /// <typeparam name="T">实体1</typeparam>
        /// <typeparam name="T2">实体2</typeparam>
        /// <typeparam name="T3">实体3</typeparam>
        /// <typeparam name="T4">实体4</typeparam>
        /// <typeparam name="TResult">返回对象</typeparam>
        /// <param name="joinExpression">关联表达式</param>
        /// <param name="selectExpression">返回表达式</param>
        /// <param name="whereExpression">查询表达式</param>
        /// <param name="intPageIndex">页码</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="strOrderByFileds">排序字段</param>
        /// <returns></returns>
        public async Task<PageModel<TResult>> QueryMuchPage<T, T2, T3, T4, TResult>(
            Expression<Func<T, T2, T3, T4, object[]>> joinExpression,
            Expression<Func<T, T2, T3, T4, TResult>> selectExpression,
            string whereExpression,
            object paramObj,
            int intPageIndex = 1,
            int intPageSize = 20,
            string strOrderByFileds = null)
        {
            RefAsync<int> totalCount = 0;
            var list = await _db.Queryable(joinExpression)
                .WhereIF(!string.IsNullOrEmpty(whereExpression), whereExpression, paramObj)
             .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
             .Select(selectExpression)
             //.WhereIF(whereExpression != null, whereExpression)
             .ToPageListAsync(intPageIndex, intPageSize == 0 ? 100 : intPageSize, totalCount);
            int pageCount = (Math.Ceiling(totalCount.ObjToDecimal() / intPageSize.ObjToDecimal())).ObjToInt();
            return new PageModel<TResult>() { Count = totalCount, PageCount = pageCount, PageIndex = intPageIndex, PageSize = intPageSize, List = list };
        }

        /// <summary>
        /// 五表联合查询-分页
        /// </summary>
        /// <typeparam name="T">实体1</typeparam>
        /// <typeparam name="T2">实体2</typeparam>
        /// <typeparam name="T3">实体3</typeparam>
        /// <typeparam name="T4">实体4</typeparam>
        /// <typeparam name="T5">实体5</typeparam>
        /// <typeparam name="TResult">返回对象</typeparam>
        /// <param name="joinExpression">关联表达式</param>
        /// <param name="selectExpression">返回表达式</param>
        /// <param name="whereExpression">查询表达式</param>
        /// <param name="intPageIndex">页码</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="strOrderByFileds">排序字段</param>
        /// <returns></returns>
        public async Task<PageModel<TResult>> QueryMuchPage<T, T2, T3, T4, T5, TResult>(
            Expression<Func<T, T2, T3, T4, T5, object[]>> joinExpression,
            Expression<Func<T, T2, T3, T4, T5, TResult>> selectExpression,
            string whereExpression,
            object paramObj,
            int intPageIndex = 1,
            int intPageSize = 20,
            string strOrderByFileds = null)
        {
            RefAsync<int> totalCount = 0;
            var list = await _db.Queryable(joinExpression)
                .WhereIF(!string.IsNullOrEmpty(whereExpression), whereExpression, paramObj)
             .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
             .Select(selectExpression)
             //.WhereIF(whereExpression != null, whereExpression)
             .ToPageListAsync(intPageIndex, intPageSize == 0 ? 100 : intPageSize, totalCount);
            int pageCount = (Math.Ceiling(totalCount.ObjToDecimal() / intPageSize.ObjToDecimal())).ObjToInt();
            return new PageModel<TResult>() { Count = totalCount, PageCount = pageCount, PageIndex = intPageIndex, PageSize = intPageSize, List = list };
        }

        /// <summary>
        /// 六表联合查询-分页
        /// </summary>
        /// <typeparam name="T">实体1</typeparam>
        /// <typeparam name="T2">实体2</typeparam>
        /// <typeparam name="T3">实体3</typeparam>
        /// <typeparam name="T4">实体4</typeparam>
        /// <typeparam name="T5">实体5</typeparam>
        /// <typeparam name="T6">实体6</typeparam>
        /// <typeparam name="TResult">返回对象</typeparam>
        /// <param name="joinExpression">关联表达式</param>
        /// <param name="selectExpression">返回表达式</param>
        /// <param name="whereExpression">查询表达式</param>
        /// <param name="paramObj">参数化查询</param>
        /// <param name="intPageIndex">页码</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="strOrderByFileds">排序字段</param>
        /// <returns></returns>
        public async Task<PageModel<TResult>> QueryMuchPage<T, T2, T3, T4, T5, T6, TResult>(
            Expression<Func<T, T2, T3, T4, T5, T6, object[]>> joinExpression,
            Expression<Func<T, T2, T3, T4, T5, T6, TResult>> selectExpression,
            string whereExpression,
            object paramObj,
            int intPageIndex = 1,
            int intPageSize = 20,
            string strOrderByFileds = null)
        {
            RefAsync<int> totalCount = 0;
            var list = await _db.Queryable(joinExpression)
                .WhereIF(!string.IsNullOrEmpty(whereExpression), whereExpression, paramObj)
             .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
             .Select(selectExpression)
             //.WhereIF(whereExpression != null, whereExpression)
             .ToPageListAsync(intPageIndex, intPageSize == 0 ? 100 : intPageSize, totalCount);
            int pageCount = (Math.Ceiling(totalCount.ObjToDecimal() / intPageSize.ObjToDecimal())).ObjToInt();
            return new PageModel<TResult>() { Count = totalCount, PageCount = pageCount, PageIndex = intPageIndex, PageSize = intPageSize, List = list };
        }

        /// <summary>
        /// 七表联合查询-分页
        /// </summary>
        /// <typeparam name="T">实体1</typeparam>
        /// <typeparam name="T2">实体2</typeparam>
        /// <typeparam name="T3">实体3</typeparam>
        /// <typeparam name="T4">实体4</typeparam>
        /// <typeparam name="T5">实体5</typeparam>
        /// <typeparam name="T6">实体6</typeparam>
        /// <typeparam name="T7">实体7</typeparam>
        /// <typeparam name="TResult">返回对象</typeparam>
        /// <param name="joinExpression">关联表达式</param>
        /// <param name="selectExpression">返回表达式</param>
        /// <param name="whereExpression">查询表达式</param>
        /// <param name="paramObj">参数化查询</param>
        /// <param name="intPageIndex">页码</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="strOrderByFileds">排序字段</param>
        /// <returns></returns>
        public async Task<PageModel<TResult>> QueryMuchPage<T, T2, T3, T4, T5, T6, T7, TResult>(
            Expression<Func<T, T2, T3, T4, T5, T6, T7, object[]>> joinExpression,
            Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> selectExpression,
            string whereExpression,
            object paramObj,
            int intPageIndex = 1,
            int intPageSize = 20,
            string strOrderByFileds = null)
        {
            RefAsync<int> totalCount = 0;
            var list = await _db.Queryable(joinExpression)
                .WhereIF(!string.IsNullOrEmpty(whereExpression), whereExpression, paramObj)
             .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
             .Select(selectExpression)
             //.WhereIF(whereExpression != null, whereExpression)
             .ToPageListAsync(intPageIndex, intPageSize == 0 ? 100 : intPageSize, totalCount);
            int pageCount = (Math.Ceiling(totalCount.ObjToDecimal() / intPageSize.ObjToDecimal())).ObjToInt();
            return new PageModel<TResult>() { Count = totalCount, PageCount = pageCount, PageIndex = intPageIndex, PageSize = intPageSize, List = list };
        }

        /// <summary>
        /// 八表联合查询-分页
        /// </summary>
        /// <typeparam name="T">实体1</typeparam>
        /// <typeparam name="T2">实体2</typeparam>
        /// <typeparam name="T3">实体3</typeparam>
        /// <typeparam name="T4">实体4</typeparam>
        /// <typeparam name="T5">实体5</typeparam>
        /// <typeparam name="T6">实体6</typeparam>
        /// <typeparam name="T7">实体7</typeparam>
        /// <typeparam name="T8">实体8</typeparam>
        /// <typeparam name="TResult">返回对象</typeparam>
        /// <param name="joinExpression">关联表达式</param>
        /// <param name="selectExpression">返回表达式</param>
        /// <param name="whereExpression">查询表达式</param>
        /// <param name="paramObj">参数化查询</param>
        /// <param name="intPageIndex">页码</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="strOrderByFileds">排序字段</param>
        /// <returns></returns>
        public async Task<PageModel<TResult>> QueryMuchPage<T, T2, T3, T4, T5, T6, T7, T8, TResult>(
            Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object[]>> joinExpression,
            Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> selectExpression,
            string whereExpression,
            object paramObj,
            int intPageIndex = 1,
            int intPageSize = 20,
            string strOrderByFileds = null)
        {
            RefAsync<int> totalCount = 0;
            var list = await _db.Queryable(joinExpression)
                .WhereIF(!string.IsNullOrEmpty(whereExpression), whereExpression, paramObj)
             .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
             .Select(selectExpression)
             //.WhereIF(whereExpression != null, whereExpression)
             .ToPageListAsync(intPageIndex, intPageSize == 0 ? 100 : intPageSize, totalCount);
            int pageCount = (Math.Ceiling(totalCount.ObjToDecimal() / intPageSize.ObjToDecimal())).ObjToInt();
            return new PageModel<TResult>() { Count = totalCount, PageCount = pageCount, PageIndex = intPageIndex, PageSize = intPageSize, List = list };
        }

        /// <summary>
        /// 两表联合查询-分页
        /// </summary>
        /// <typeparam name="T">实体1</typeparam>
        /// <typeparam name="T2">实体1</typeparam>
        /// <typeparam name="TResult">返回对象</typeparam>
        /// <param name="joinExpression">关联表达式</param>
        /// <param name="selectExpression">返回表达式</param>
        /// <param name="whereExpression">查询表达式</param>
        /// <param name="intPageIndex">页码</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="strOrderByFileds">排序字段</param>
        /// <returns></returns>
        public async Task<PageModel<TResult>> QueryMuchPage<T, T2, TResult>(Expression<Func<T, T2, object[]>> joinExpression, Expression<Func<T, T2, TResult>> selectExpression, Expression<Func<TResult, bool>> whereExpression, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null)
        {
            RefAsync<int> totalCount = 0;
            var list = await _db.Queryable(joinExpression)
                .Select(selectExpression)
                .MergeTable()// 将上面的操作结果变成一个新表MergeTable
                .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
                .WhereIF(whereExpression != null, whereExpression)//对表MergeTable进行条件筛选
                .ToPageListAsync(intPageIndex, intPageSize == 0 ? 100 : intPageSize, totalCount);

            int pageCount = (Math.Ceiling(totalCount.ObjToDecimal() / intPageSize.ObjToDecimal())).ObjToInt();
            return new PageModel<TResult>() { Count = totalCount, PageCount = pageCount, PageIndex = intPageIndex, PageSize = intPageSize, List = list };
        }

        #region 基于结果查询
        /// <summary>
        /// 两表联合查询-分页
        /// </summary>
        /// <typeparam name="T">实体1</typeparam>
        /// <typeparam name="T2">实体1</typeparam>
        /// <typeparam name="TResult">返回对象</typeparam>
        /// <param name="joinExpression">关联表达式</param>
        /// <param name="selectExpression">返回表达式</param>
        /// <param name="whereExpression">查询表达式</param>
        /// <param name="intPageIndex">页码</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="strOrderByFileds">排序字段</param>
        /// <returns></returns>
        public async Task<PageModel<TResult>> QueryMuchPageByResult<T, T2, TResult>(
            Expression<Func<T, T2, object[]>> joinExpression,
            Expression<Func<T, T2, TResult>> selectExpression,
            string whereExpression,
            object paramObj,
            int intPageIndex = 1,
            int intPageSize = 20,
            string strOrderByFileds = null)
        {
            RefAsync<int> totalCount = 0;
            var list = await _db.Queryable(joinExpression)
                .Select(selectExpression)
                .MergeTable()// 将上面的操作结果变成一个新表MergeTable
                .WhereIF(!string.IsNullOrEmpty(whereExpression), whereExpression, paramObj)
                .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
                .ToPageListAsync(intPageIndex, intPageSize == 0 ? 100 : intPageSize, totalCount);
            int pageCount = (Math.Ceiling(totalCount.ObjToDecimal() / intPageSize.ObjToDecimal())).ObjToInt();
            return new PageModel<TResult>() { Count = totalCount, PageCount = pageCount, PageIndex = intPageIndex, PageSize = intPageSize, List = list };
        }

        /// <summary>
        /// 三表联合查询-分页
        /// </summary>
        /// <typeparam name="T">实体1</typeparam>
        /// <typeparam name="T2">实体2</typeparam>
        /// <typeparam name="T3">实体3</typeparam>
        /// <typeparam name="TResult">返回对象</typeparam>
        /// <param name="joinExpression">关联表达式</param>
        /// <param name="selectExpression">返回表达式</param>
        /// <param name="whereExpression">查询表达式</param>
        /// <param name="intPageIndex">页码</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="strOrderByFileds">排序字段</param>
        /// <returns></returns>
        public async Task<PageModel<TResult>> QueryMuchPageByResult<T, T2, T3, TResult>(
            Expression<Func<T, T2, T3, object[]>> joinExpression,
            Expression<Func<T, T2, T3, TResult>> selectExpression,
            string whereExpression,
            object paramObj,
            int intPageIndex = 1,
            int intPageSize = 20,
            string strOrderByFileds = null)
        {
            RefAsync<int> totalCount = 0;
            var list = await _db.Queryable(joinExpression)
                .Select(selectExpression)
                .MergeTable()// 将上面的操作结果变成一个新表MergeTable
                .WhereIF(!string.IsNullOrEmpty(whereExpression), whereExpression, paramObj)
                .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
                .ToPageListAsync(intPageIndex, intPageSize == 0 ? 100 : intPageSize, totalCount);
            int pageCount = (Math.Ceiling(totalCount.ObjToDecimal() / intPageSize.ObjToDecimal())).ObjToInt();
            return new PageModel<TResult>() { Count = totalCount, PageCount = pageCount, PageIndex = intPageIndex, PageSize = intPageSize, List = list };
        }

        /// <summary>
        /// 四表联合查询-分页
        /// </summary>
        /// <typeparam name="T">实体1</typeparam>
        /// <typeparam name="T2">实体2</typeparam>
        /// <typeparam name="T3">实体3</typeparam>
        /// <typeparam name="T4">实体4</typeparam>
        /// <typeparam name="TResult">返回对象</typeparam>
        /// <param name="joinExpression">关联表达式</param>
        /// <param name="selectExpression">返回表达式</param>
        /// <param name="whereExpression">查询表达式</param>
        /// <param name="intPageIndex">页码</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="strOrderByFileds">排序字段</param>
        /// <returns></returns>
        public async Task<PageModel<TResult>> QueryMuchPageByResult<T, T2, T3, T4, TResult>(
            Expression<Func<T, T2, T3, T4, object[]>> joinExpression,
            Expression<Func<T, T2, T3, T4, TResult>> selectExpression,
            string whereExpression,
            object paramObj,
            int intPageIndex = 1,
            int intPageSize = 20,
            string strOrderByFileds = null)
        {
            RefAsync<int> totalCount = 0;
            var list = await _db.Queryable(joinExpression)
                .Select(selectExpression)
                .MergeTable()// 将上面的操作结果变成一个新表MergeTable
                .WhereIF(!string.IsNullOrEmpty(whereExpression), whereExpression, paramObj)
                .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
                .ToPageListAsync(intPageIndex, intPageSize == 0 ? 100 : intPageSize, totalCount);
            int pageCount = (Math.Ceiling(totalCount.ObjToDecimal() / intPageSize.ObjToDecimal())).ObjToInt();
            return new PageModel<TResult>() { Count = totalCount, PageCount = pageCount, PageIndex = intPageIndex, PageSize = intPageSize, List = list };
        }

        /// <summary>
        /// 五表联合查询-分页
        /// </summary>
        /// <typeparam name="T">实体1</typeparam>
        /// <typeparam name="T2">实体2</typeparam>
        /// <typeparam name="T3">实体3</typeparam>
        /// <typeparam name="T4">实体4</typeparam>
        /// <typeparam name="T5">实体5</typeparam>
        /// <typeparam name="TResult">返回对象</typeparam>
        /// <param name="joinExpression">关联表达式</param>
        /// <param name="selectExpression">返回表达式</param>
        /// <param name="whereExpression">查询表达式</param>
        /// <param name="intPageIndex">页码</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="strOrderByFileds">排序字段</param>
        /// <returns></returns>
        public async Task<PageModel<TResult>> QueryMuchPageByResult<T, T2, T3, T4, T5, TResult>(
            Expression<Func<T, T2, T3, T4, T5, object[]>> joinExpression,
            Expression<Func<T, T2, T3, T4, T5, TResult>> selectExpression,
            string whereExpression,
            object paramObj,
            int intPageIndex = 1,
            int intPageSize = 20,
            string strOrderByFileds = null)
        {
            RefAsync<int> totalCount = 0;
            var list = await _db.Queryable(joinExpression)
                .Select(selectExpression)
                .MergeTable()// 将上面的操作结果变成一个新表MergeTable
                .WhereIF(!string.IsNullOrEmpty(whereExpression), whereExpression, paramObj)
                .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
                .ToPageListAsync(intPageIndex, intPageSize == 0 ? 100 : intPageSize, totalCount);
            int pageCount = (Math.Ceiling(totalCount.ObjToDecimal() / intPageSize.ObjToDecimal())).ObjToInt();
            return new PageModel<TResult>() { Count = totalCount, PageCount = pageCount, PageIndex = intPageIndex, PageSize = intPageSize, List = list };
        }

        /// <summary>
        /// 六表联合查询-分页
        /// </summary>
        /// <typeparam name="T">实体1</typeparam>
        /// <typeparam name="T2">实体2</typeparam>
        /// <typeparam name="T3">实体3</typeparam>
        /// <typeparam name="T4">实体4</typeparam>
        /// <typeparam name="T5">实体5</typeparam>
        /// <typeparam name="T6">实体6</typeparam>
        /// <typeparam name="TResult">返回对象</typeparam>
        /// <param name="joinExpression">关联表达式</param>
        /// <param name="selectExpression">返回表达式</param>
        /// <param name="whereExpression">查询表达式</param>
        /// <param name="paramObj">查询参数</param>
        /// <param name="intPageIndex">页码</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="strOrderByFileds">排序字段</param>
        /// <returns></returns>
        public async Task<PageModel<TResult>> QueryMuchPageByResult<T, T2, T3, T4, T5, T6, TResult>(
            Expression<Func<T, T2, T3, T4, T5, T6, object[]>> joinExpression,
            Expression<Func<T, T2, T3, T4, T5, T6, TResult>> selectExpression,
            string whereExpression,
            object paramObj,
            int intPageIndex = 1,
            int intPageSize = 20,
            string strOrderByFileds = null)
        {
            RefAsync<int> totalCount = 0;
            var list = await _db.Queryable(joinExpression)
                .Select(selectExpression)
                .MergeTable()// 将上面的操作结果变成一个新表MergeTable
                .WhereIF(!string.IsNullOrEmpty(whereExpression), whereExpression, paramObj)
                .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
                .ToPageListAsync(intPageIndex, intPageSize == 0 ? 100 : intPageSize, totalCount);
            int pageCount = (Math.Ceiling(totalCount.ObjToDecimal() / intPageSize.ObjToDecimal())).ObjToInt();
            return new PageModel<TResult>() { Count = totalCount, PageCount = pageCount, PageIndex = intPageIndex, PageSize = intPageSize, List = list };
        }

        /// <summary>
        /// 七表联合查询-分页
        /// </summary>
        /// <typeparam name="T">实体1</typeparam>
        /// <typeparam name="T2">实体2</typeparam>
        /// <typeparam name="T3">实体3</typeparam>
        /// <typeparam name="T4">实体4</typeparam>
        /// <typeparam name="T5">实体5</typeparam>
        /// <typeparam name="T6">实体6</typeparam>
        /// <typeparam name="T7">实体7</typeparam>
        /// <typeparam name="TResult">返回对象</typeparam>
        /// <param name="joinExpression">关联表达式</param>
        /// <param name="selectExpression">返回表达式</param>
        /// <param name="whereExpression">查询表达式</param>
        /// <param name="paramObj">查询参数</param>
        /// <param name="intPageIndex">页码</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="strOrderByFileds">排序字段</param>
        /// <returns></returns>
        public async Task<PageModel<TResult>> QueryMuchPageByResult<T, T2, T3, T4, T5, T6, T7, TResult>(
            Expression<Func<T, T2, T3, T4, T5, T6, T7, object[]>> joinExpression,
            Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> selectExpression,
            string whereExpression,
            object paramObj,
            int intPageIndex = 1,
            int intPageSize = 20,
            string strOrderByFileds = null)
        {
            RefAsync<int> totalCount = 0;
            var list = await _db.Queryable(joinExpression)
                .Select(selectExpression)
                .MergeTable()// 将上面的操作结果变成一个新表MergeTable
                .WhereIF(!string.IsNullOrEmpty(whereExpression), whereExpression, paramObj)
                .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
                .ToPageListAsync(intPageIndex, intPageSize == 0 ? 100 : intPageSize, totalCount);
            int pageCount = (Math.Ceiling(totalCount.ObjToDecimal() / intPageSize.ObjToDecimal())).ObjToInt();
            return new PageModel<TResult>() { Count = totalCount, PageCount = pageCount, PageIndex = intPageIndex, PageSize = intPageSize, List = list };
        }

        /// <summary>
        /// 八表联合查询-分页
        /// </summary>
        /// <typeparam name="T">实体1</typeparam>
        /// <typeparam name="T2">实体2</typeparam>
        /// <typeparam name="T3">实体3</typeparam>
        /// <typeparam name="T4">实体4</typeparam>
        /// <typeparam name="T5">实体5</typeparam>
        /// <typeparam name="T6">实体6</typeparam>
        /// <typeparam name="T7">实体7</typeparam>
        /// <typeparam name="T8">实体8</typeparam>
        /// <typeparam name="TResult">返回对象</typeparam>
        /// <param name="joinExpression">关联表达式</param>
        /// <param name="selectExpression">返回表达式</param>
        /// <param name="whereExpression">查询表达式</param>
        /// <param name="paramObj">查询参数</param>
        /// <param name="intPageIndex">页码</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="strOrderByFileds">排序字段</param>
        /// <returns></returns>
        public async Task<PageModel<TResult>> QueryMuchPageByResult<T, T2, T3, T4, T5, T6, T7, T8, TResult>(
            Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object[]>> joinExpression,
            Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> selectExpression,
            string whereExpression,
            object paramObj,
            int intPageIndex = 1,
            int intPageSize = 20,
            string strOrderByFileds = null)
        {
            RefAsync<int> totalCount = 0;
            var list = await _db.Queryable(joinExpression)
                .Select(selectExpression)
                .MergeTable()// 将上面的操作结果变成一个新表MergeTable
                .WhereIF(!string.IsNullOrEmpty(whereExpression), whereExpression, paramObj)
                .OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds)
                .ToPageListAsync(intPageIndex, intPageSize == 0 ? 100 : intPageSize, totalCount);
            int pageCount = (Math.Ceiling(totalCount.ObjToDecimal() / intPageSize.ObjToDecimal())).ObjToInt();
            return new PageModel<TResult>() { Count = totalCount, PageCount = pageCount, PageIndex = intPageIndex, PageSize = intPageSize, List = list };
        }
        #endregion

        /// <summary>
        /// 功能描述:查询前N条数据
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="intTop">前N条</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>数据列表</returns>
        public async Task<List<TEntity>> QueryTop(Expression<Func<TEntity, bool>> whereExpression, int intTop, string strOrderByFileds)
        {
            return await _db.Queryable<TEntity>().OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds).WhereIF(whereExpression != null, whereExpression).Take(intTop).ToListAsync();
        }

        /// <summary>
        /// 功能描述:查询前N条数据
        /// </summary>
        /// <param name="strWhere">条件</param>
        /// <param name="intTop">前N条</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>数据列表</returns>
        public async Task<List<TEntity>> QueryTop(string strWhere, int intTop, string strOrderByFileds)
        {
            return await _db.Queryable<TEntity>().OrderByIF(!string.IsNullOrEmpty(strOrderByFileds), strOrderByFileds).WhereIF(!string.IsNullOrEmpty(strWhere), strWhere).Take(intTop).ToListAsync();
        }

        /// <summary>
        /// 根据sql语句查询
        /// </summary>
        /// <param name="sql">完整的sql语句</param>
        /// <param name="parameters">参数</param>
        /// <returns>泛型集合</returns>
        public async Task<List<TEntity>> QuerySql(string sql, List<SugarParameter> parameters = null)
        {
            return await _db.Ado.SqlQueryAsync<TEntity>(sql, parameters);
        }

        /// <summary>
        /// 根据sql语句查询
        /// </summary>
        /// <param name="sql">完整的sql语句</param>
        /// <param name="parameters">参数</param>
        /// <returns>DataTable</returns>
        public async Task<DataTable> QuerySqlTable(string sql, List<SugarParameter> parameters = null)
        {
            return await _db.Ado.GetDataTableAsync(sql, parameters);
        }

        #endregion

        #region "新增"
        /// <summary>
        /// 写入实体数据
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public async Task<int> Add(TEntity entity)
        {
            return await _db.Insertable(entity).EnableDiffLogEvent().ExecuteCommandAsync();
        }

        /// <summary>
        /// 写入实体数据-自动分表
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public async Task<int> AddWithSplitTable(TEntity entity)
        {
            return await _db.Insertable(entity).SplitTable().ExecuteCommandAsync();
        }

        /// <summary>
        /// 写入实体数据
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>返回插入后的实体</returns>
        public async Task<TEntity> AddGetRestult(TEntity entity)
        {
            return await _db.Insertable(entity).ExecuteReturnEntityAsync();
        }

        /// <summary>
        /// 写入数据-无实体
        /// </summary>
        /// <param name="entity">数据字典</param>
        /// <param name="tableName">表名</param>
        /// <returns>tableName</returns>
        public async Task<int> Add(Dictionary<string, object> entity, string tableName)
        {
            return await _db.Insertable(entity).AS(tableName).ExecuteCommandAsync();
        }

        /// <summary>
        /// 批量写入数据-无实体
        /// </summary>
        /// <param name="entity">数据字典</param>
        /// <param name="tableName">表名</param>
        /// <returns>tableName</returns>
        public async Task<int> Add(List<Dictionary<string, object>> entitys, string tableName)
        {
            return await _db.Insertable(entitys).AS(tableName).ExecuteCommandAsync();
        }

        /// <summary>
        /// 写入实体数据
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="insertColumns">指定插入列</param>
        /// <param name="ingoreColumns">指定忽略插入列</param>
        /// <returns>返回影响行数</returns>
        public async Task<int> Add(TEntity entity, Expression<Func<TEntity, object>> insertColumns = null, Expression<Func<TEntity, object>> ingoreColumns = null)
        {
            var insert = _db.Insertable(entity);
            if (insertColumns != null)
            {
                insert = insert.InsertColumns(insertColumns);
            }
            if (insertColumns != null)
            {
                insert = insert.IgnoreColumns(ingoreColumns);
            }
            return await insert.ExecuteReturnIdentityAsync();
        }

        /// <summary>
        /// 批量插入实体，500条以下速度最快，兼容所有类型和emoji，500以上就开始慢了
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <returns>影响行数</returns>
        public async Task<int> Add(List<TEntity> entities)
        {
            return await _db.Insertable(entities.ToArray()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 批量插入实体，500条以下速度最快，兼容所有类型和emoji，500以上就开始慢了 - 自动分表
        /// </summary>
        /// <param name="entities">实体</param>
        /// <returns></returns>
        public async Task<int> AddWithSplitTable(List<TEntity> entities)
        {
            return await _db.Insertable(entities.ToArray()).SplitTable().ExecuteCommandAsync();
        }

        /// <summary>
        /// 大批量插入实体，1000条以上性能无敌手
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <returns>影响行数</returns>
        public async Task<int> AddMuch(List<TEntity> entities)
        {
            return await _db.Fastest<TEntity>().BulkCopyAsync(entities); ;
        }

        /// <summary>
        /// 大批量插入实体，1000条以上性能无敌手 - 自动分表
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <returns>影响行数</returns>
        public async Task<int> AddMuchWithSplitTable(List<TEntity> entities)
        {
            return await _db.Fastest<TEntity>().SplitTable().BulkCopyAsync(entities); ;
        }

        #endregion

        #region "修改"
        /// <summary>
        /// 更新实体数据
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public async Task<bool> Update(TEntity entity)
        {
            return await _db.Updateable(entity).EnableDiffLogEvent().ExecuteCommandHasChangeAsync();
        }

        /// <summary>
        /// 更新实体数据 - 自动分表
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public async Task<int> UpdateWithSplitTable(TEntity entity)
        {
            return await _db.Updateable(entity).SplitTable().ExecuteCommandAsync();
        }

        /// <summary>
        /// 更新实体数据（校验行版本）
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public async Task<int> UpdateCheckVersion(TEntity entity)
        {
            return await _db.Updateable(entity).ExecuteCommandWithOptLockAsync(true);
        }

        /// <summary>
        /// 更新数据-无实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="tableName">表名</param>
        /// <param name="updateColumns">更新列</param>
        /// <param name="ignoreColumns">忽略列</param>
        /// <param name="whereColumns">条件列</param>
        /// <returns></returns>
        public async Task<bool> Update(TEntity entity, string tableName, List<string> updateColumns = null, List<string> ignoreColumns = null, string[] whereColumns = null)
        {
            IUpdateable<TEntity> up = _db.Updateable(entity).AS(tableName);
            if (ignoreColumns != null && ignoreColumns.Count > 0)
            {
                up = up.IgnoreColumns(ignoreColumns.ToArray());
            }
            if (updateColumns != null && updateColumns.Count > 0)
            {
                up = up.UpdateColumns(updateColumns.ToArray());
            }
            if (whereColumns != null)
            {
                up = up.WhereColumns(whereColumns);
            }
            return await up.ExecuteCommandHasChangeAsync();
        }

        /// <summary>
        /// 增量更新数据库数据
        /// </summary>
        /// <param name="incrementalExpression">增量更新表达式 it => it.Age == it.Age + 10 && it.Name = it.Name + "test" </param>
        /// <param name="whereExpression">条件表达式</param>
        /// <returns></returns>
        public async Task<bool> UpdateIncremental(Expression<Func<TEntity, bool>> incrementalExpression, Expression<Func<TEntity, bool>> whereExpression)
        {
            return await _db.Updateable<TEntity>().SetColumnsIF(incrementalExpression != null, incrementalExpression).Where(whereExpression).ExecuteCommandHasChangeAsync();
        }

        /// <summary>
        /// 增量更新数据库数据
        /// </summary>
        /// <param name="incrementalExpression">增量更新表达式 it => new TEntity { Age == it.Age + 10 , Name = it.Name + "test"} </param>
        /// <param name="whereExpression">条件表达式</param>
        /// <returns></returns>
        public async Task<bool> UpdateIncremental(Expression<Func<TEntity, TEntity>> incrementalExpression, Expression<Func<TEntity, bool>> whereExpression)
        {
            return await _db.Updateable<TEntity>().SetColumnsIF(incrementalExpression != null, incrementalExpression).Where(whereExpression).ExecuteCommandHasChangeAsync();
        }

        /// <summary>
        /// 更新实体数据
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="strWhere">条件字符串</param>
        /// <returns></returns>
        public async Task<bool> Update(TEntity entity, string strWhere)
        {
            return await _db.Updateable(entity).Where(strWhere).ExecuteCommandHasChangeAsync();
        }

        /// <summary>
        /// 更新实体数据
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="updateColumns">更新列</param>
        /// <param name="ignoreColumns">忽略列</param>
        /// <param name="strWhere">条件语句</param>
        /// <returns></returns>
        public async Task<bool> Update(TEntity entity, List<string> updateColumns = null, List<string> ignoreColumns = null, string strWhere = "")
        {
            IUpdateable<TEntity> up = _db.Updateable(entity);
            if (updateColumns != null && updateColumns.Count > 0)
            {
                up = up.UpdateColumns(updateColumns.ToArray());
            }
            if (ignoreColumns != null && ignoreColumns.Count > 0)
            {
                up = up.IgnoreColumns(ignoreColumns.ToArray());
            }
            if (!string.IsNullOrEmpty(strWhere))
            {
                up = up.Where(strWhere);
            }
            return await up.ExecuteCommandHasChangeAsync();
        }

        /// <summary> 
        /// 更新实体数据(部分更新)
        /// </summary> 
        /// <param name="entity">实体</param> 
        /// <param name="updateExpression"> o => new {TEntity.name}</param> 
        /// <param name="ignoreExpression"> o => new {TEntity.password}</param> 
        /// <returns></returns>
        public async Task<bool> Update(TEntity entity, Expression<Func<TEntity, object>> updateExpression, Expression<Func<TEntity, object>> ignoreExpression)
        {
            IUpdateable<TEntity> up = _db.Updateable(entity);
            if (updateExpression != null)
            {
                up = up.UpdateColumns(updateExpression);
            }
            if (ignoreExpression != null)
            {
                up = up.IgnoreColumns(ignoreExpression);
            }
            return await up.ExecuteCommandHasChangeAsync();
        }

        /// <summary>
        /// 更新实体数据
        /// </summary>
        /// <param name="entitys">实体集合</param>
        /// <returns></returns>
        public async Task<bool> Update(List<TEntity> entitys)
        {
            return await _db.Updateable(entitys).EnableDiffLogEvent().ExecuteCommandHasChangeAsync();
        }

        /// <summary>
        /// 更新实体数据 - 自动分表
        /// </summary>
        /// <param name="entitys">实体集合</param>
        /// <returns></returns>
        public async Task<int> UpdateWithSplitTable(List<TEntity> entitys)
        {
            return await _db.Updateable(entitys).SplitTable().ExecuteCommandAsync();
        }

        /// <summary>
        /// 更新实体数据
        /// </summary>
        /// <param name="entitys">实体集合</param>
        /// <param name="strWhere">条件字符串</param>
        /// <returns></returns>
        public async Task<bool> Update(List<TEntity> entitys, string strWhere)
        {
            return await _db.Updateable(entitys).Where(strWhere).ExecuteCommandHasChangeAsync();
        }

        /// <summary>
        /// 更新实体数据
        /// </summary>
        /// <param name="entitys">实体</param>
        /// <param name="updateColumns">更新列</param>
        /// <param name="ignoreColumns">忽略列</param>
        /// <param name="strWhere">条件语句</param>
        /// <returns></returns>
        public async Task<bool> Update(List<TEntity> entitys, List<string> updateColumns = null, List<string> ignoreColumns = null, string strWhere = "")
        {
            IUpdateable<TEntity> up = _db.Updateable(entitys);
            if (updateColumns != null && updateColumns.Count > 0)
            {
                up = up.UpdateColumns(updateColumns.ToArray());
            }
            if (ignoreColumns != null && ignoreColumns.Count > 0)
            {
                up = up.IgnoreColumns(ignoreColumns.ToArray());
            }
            if (!string.IsNullOrEmpty(strWhere))
            {
                up = up.Where(strWhere);
            }
            return await up.ExecuteCommandHasChangeAsync();
        }

        /// <summary> 
        /// 更新实体数据(部分更新)
        /// </summary> 
        /// <param name="entitys">实体</param> 
        /// <param name="updateExpression"> o => new {TEntity.name}</param> 
        /// <param name="ignoreExpression"> o => new {TEntity.password}</param> 
        /// <returns></returns>
        public async Task<bool> Update(List<TEntity> entitys, Expression<Func<TEntity, object>> updateExpression, Expression<Func<TEntity, object>> ignoreExpression)
        {
            IUpdateable<TEntity> up = _db.Updateable(entitys);
            if (updateExpression != null)
            {
                up = up.UpdateColumns(updateExpression);
            }
            if (ignoreExpression != null)
            {
                up = up.IgnoreColumns(ignoreExpression);
            }
            return await up.ExecuteCommandHasChangeAsync();
        }

        /// <summary>
        /// 大批量更新实体，1000条以上性能无敌手
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <returns>影响行数</returns>
        public async Task<int> UpdateMuch(List<TEntity> entities)
        {
            return await _db.Fastest<TEntity>().BulkUpdateAsync(entities);
        }

        /// <summary>
        /// 大批量更新实体，1000条以上性能无敌手 - 自动分表
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <returns>影响行数</returns>
        public async Task<int> UpdateMuchWithSplitTable(List<TEntity> entities)
        {
            return await _db.Fastest<TEntity>().SplitTable().BulkUpdateAsync(entities);
        }
        #endregion

        #region "删除"
        /// <summary>
        /// 删除指定ID的数据
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        public async Task<bool> DeleteById(object id)
        {
            return await _db.Deleteable<TEntity>(id).ExecuteCommandHasChangeAsync();
        }
        /// <summary>
        /// 删除指定ID集合的数据(批量删除)
        /// </summary>
        /// <param name="ids">主键ID集合</param>
        /// <returns></returns>
        public async Task<bool> DeleteByIds(object[] ids)
        {
            return await _db.Deleteable<TEntity>().In(ids).ExecuteCommandHasChangeAsync();
        }

        /// <summary>
        /// 根据实体删除一条数据
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public async Task<bool> Delete(TEntity entity)
        {
            return await _db.Deleteable(entity).EnableDiffLogEvent().ExecuteCommandHasChangeAsync();
        }

        /// <summary>
        /// 根据实体删除一条数据 - 自动分表
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public async Task<int> DeleteWithSplitTable(TEntity entity)
        {
            return await _db.Deleteable(entity).SplitTable().ExecuteCommandAsync();
        }

        /// <summary>
        /// 根据实体集合删除多条数据(批量删除)
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <returns></returns>
        public async Task<bool> Delete(List<TEntity> entities)
        {
            return await _db.Deleteable(entities).EnableDiffLogEvent().ExecuteCommandHasChangeAsync();
        }

        /// <summary>
        /// 根据实体集合删除多条数据(批量删除) - 自动分表
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <returns></returns>
        public async Task<int> DeleteWithSplitTable(List<TEntity> entities)
        {
            return await _db.Deleteable(entities).SplitTable().ExecuteCommandAsync();
        }

        /// <summary>
        /// 根据表达式删除数据
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <returns></returns>
        public async Task<bool> Delete(Expression<Func<TEntity, bool>> whereExpression)
        {
            return await _db.Deleteable<TEntity>().Where(whereExpression).ExecuteCommandHasChangeAsync();
        }
        #endregion

        #region 数据新增/修改
        /// <summary>
        /// 调用存储过程
        /// </summary>
        /// <param name="storedProcedureName"></param>
        /// <param name="parameters"></param>
        /// <returns>List</returns>
        public async Task<object> InportInsertOrUpdate(List<TEntity> entities, Expression<Func<TEntity, object>> columns)
        {
            //List<StorageableMessage<TEntity>> errorList = new List<StorageableMessage<TEntity>>();
            //_db.Utilities.PageEach(entities, 1000, pageList =>
            //{
            //    var x = Db.Storageable(pageList)
            //        .SplitUpdate(it => it.Any())// 默认根据主键
            //        .SplitInsert(it => true)// 其余插入
            //        .WhereColumns(columns)//如果不是主键可以这样实现（多字段it=>new{it.x1,it.x2}）
            //        .ToStorage();//将数据进行分组

            //    x.BulkCopy();
            //    x.BulkUpdate(); //5.0.4.6
            //    errorList.AddRange(x.ErrorList);

            //});
            //return errorList;

            var x = Db.Storageable(entities)
                   .SplitUpdate(it => it.Any())// 默认根据主键
                   .SplitInsert(it => true)// 其余插入
                   .WhereColumns(columns)//如果不是主键可以这样实现（多字段it=>new{it.x1,it.x2}）
                   .ToStorage();//将数据进行分组

            await x.BulkCopyAsync();
            await x.BulkUpdateAsync(); //5.0.4.6
            return x.ErrorList;
        }
        #endregion

        #region 调用存储过程
        /// <summary>
        /// 调用存储过程
        /// </summary>
        /// <param name="storedProcedureName"></param>
        /// <param name="parameters"></param>
        /// <returns>List</returns>
        public async Task<List<TEntity>> UseStoredProcedureGetList(string storedProcedureName,  params SugarParameter[] parameters)
        {
            return await _db.Ado.UseStoredProcedure().SqlQueryAsync<TEntity>(storedProcedureName, parameters);
        }

        /// <summary>
        /// 调用存储过程
        /// </summary>
        /// <param name="storedProcedureName"></param>
        /// <param name="parameters"></param>
        /// <returns>DataTable</returns>
        public async Task<DataTable> UseStoredProcedureGetDataTable(string storedProcedureName, params SugarParameter[] parameters)
        {
            return await _db.Ado.UseStoredProcedure().GetDataTableAsync(storedProcedureName, parameters);
        }

        /// <summary>
        /// 调用存储过程
        /// </summary>
        /// <param name="storedProcedureName"></param>
        /// <param name="parameters"></param>
        /// <returns>string</returns>
        public async Task<string> UseStoredProcedureGetString(string storedProcedureName, params SugarParameter[] parameters)
        {
            return await _db.Ado.UseStoredProcedure().GetStringAsync(storedProcedureName, parameters);
        }
        #endregion

        /// <summary>
        /// 新增、修改、删除
        /// </summary>
        /// <param name="sql">完整的sql语句</param>
        /// <param name="parameters">参数</param>
        /// <returns>影响行数</returns>
        public async Task<int> CommandBySql(string sql, List<SugarParameter> parameters = null)
        {
            return await _db.Ado.ExecuteCommandAsync(sql, parameters);
        }
    }

}

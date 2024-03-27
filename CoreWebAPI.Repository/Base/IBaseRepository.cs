using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CoreWebAPI.Repository.Base
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        ISqlSugarClient _db { get;}

        /// <summary>
        /// 手动切换数据库
        /// </summary>
        /// <param name="configId">ConnId</param>
        void ChangeDB(string configId);

        #region 查询
        Task<TEntity> QueryById(object objId);
        Task<TEntity> QueryById(object objId, bool blnUseCache = false);
        Task<List<TEntity>> QueryByIDs(object[] lstIds);

        /// <summary>
        /// 功能描述:无实体查询，根据表名查询数据
        /// </summary>
        /// <param name="strWhere">Where条件</param>
        /// <param name="tableName">表名</param>
        /// <returns>数据实体列表</returns>
        Task<List<dynamic>> QueryByTableName(string strWhere, string tableName);

        /// <summary>
        /// 功能描述:查询最大值
        /// </summary>
        /// <param name="whereExpression">Where条件</param>
        /// <param name="fieldName">查询字段</param>
        /// <returns>数据实体列表</returns>
        Task<int> QueryMax(Expression<Func<TEntity, bool>> whereExpression, string fieldName);

        Task<List<TEntity>> Query();
        Task<List<TEntity>> Query(string configid);
        Task<List<TEntity>> Query(string strWhere, object paramObj);

        /// <summary>
        /// 功能描述:查询数据列表
        /// </summary>
        /// <param name="whereExpression">whereExpression</param>
        /// <returns>数据列表</returns>
        Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression);

        /// <summary>
        /// 功能描述:查询数据列表
        /// </summary>
        /// <param name="whereExpression">whereExpression</param>
        /// <param name="splitFunc">分表过滤，详细可参考https://www.donet5.com/Home/Doc?typeId=1201</param>
        /// <returns>数据列表</returns>
        Task<List<TEntity>> QueryWithSplitTable(Expression<Func<TEntity, bool>> whereExpression, Func<List<SplitTableInfo>, IEnumerable<SplitTableInfo>> splitFunc);

        /// <summary>
        /// 功能描述:查询数据列表 - 悲观锁排它模式
        /// </summary>
        /// <param name="whereExpression">whereExpression</param>
        /// <returns>数据列表</returns>
        Task<List<TEntity>> QueryLock(Expression<Func<TEntity, bool>> whereExpression);

        /// <summary>
        /// 功能描述:查询一个列表
        /// </summary>
        /// <param name="strWhere">条件</param>
        /// <param name="paramObj">查询参数</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>数据列表</returns>
        Task<List<TEntity>> Query(string strWhere, object paramObj, string strOrderByFileds);
        Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression, string strOrderByFileds);
        Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, object>> orderByExpression, bool isAsc = true);
        Task<List<TEntity>> Query(string strWhere, object paramObj, int intPageIndex, int intPageSize, string strOrderByFileds);

        /// <summary>
        /// 功能描述:分页查询
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="intPageIndex">页码（下标0）</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>数据列表</returns>
        Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression, int intPageIndex, int intPageSize, string strOrderByFileds);

        /// <summary>
        /// 功能描述:分页查询-分表查询
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="splitFunc">分表过滤，详细可参考https://www.donet5.com/Home/Doc?typeId=1201</param>
        /// <param name="intPageIndex">页码（下标0）</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>数据列表</returns>
        Task<List<TEntity>> QueryWithSplitTable(Expression<Func<TEntity, bool>> whereExpression, Func<List<SplitTableInfo>, IEnumerable<SplitTableInfo>> splitFunc, int intPageIndex, int intPageSize, string strOrderByFileds);

        Task<DataTable> QueryTable(Expression<Func<TEntity, bool>> whereExpression, string strOrderByFileds);
        Task<DataTable> QueryTable(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, object>> sortExpression = null, OrderByType sortType = OrderByType.Asc);
        Task<DataTable> QueryTable<T, T2, TResult>(Expression<Func<T, T2, object[]>> joinExpression, Expression<Func<T, T2, TResult>> selectExpression, Expression<Func<T, T2, bool>> whereExpression, string strOrderByFileds = null);
        Task<DataTable> QueryTable<T, T2, T3, TResult>(Expression<Func<T, T2, T3, object[]>> joinExpression, Expression<Func<T, T2, T3, TResult>> selectExpression, Expression<Func<T, T2, T3, bool>> whereExpression, string strOrderByFileds = null);
        Task<DataTable> QueryTable<T, T2, T3, T4, TResult>(Expression<Func<T, T2, T3, T4, object[]>> joinExpression, Expression<Func<T, T2, T3, T4, TResult>> selectExpression, Expression<Func<T, T2, T3, T4, bool>> whereExpression, string strOrderByFileds = null);
        Task<DataTable> QueryTable<T, T2, T3, T4, T5, TResult>(Expression<Func<T, T2, T3, T4, T5, object[]>> joinExpression, Expression<Func<T, T2, T3, T4, T5, TResult>> selectExpression, Expression<Func<T, T2, T3, T4, T5, bool>> whereExpression, string strOrderByFileds = null);
        Task<DataTable> QueryTable<T, T2, T3, T4, T5, T6, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, object[]>> joinExpression, Expression<Func<T, T2, T3, T4, T5, T6, TResult>> selectExpression, Expression<Func<T, T2, T3, T4, T5, T6, bool>> whereExpression, string strOrderByFileds = null);
        Task<DataTable> QueryTable<T, T2, T3, T4, T5, T6, T7, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, object[]>> joinExpression, Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> selectExpression, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> whereExpression, string strOrderByFileds = null);
        Task<DataTable> QueryTable<T, T2, T3, T4, T5, T6, T7, T8, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object[]>> joinExpression, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> selectExpression, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> whereExpression, string strOrderByFileds = null);
        //Task<DataTable> QueryTable<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object[]>> joinExpression, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> selectExpression, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> whereExpression, string strOrderByFileds = null);
        //Task<DataTable> QueryTable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object[]>> joinExpression, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> selectExpression, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> whereExpression, string strOrderByFileds = null);


        Task<DataTable> QueryTable(string whereStr, object paramObj, string strOrderByFileds);
        Task<PageModel<DataTable>> QueryTablePage(string whereStr, object paramObj, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null);

        Task<DataTable> QueryTable<T, T2, TResult>(Expression<Func<T, T2, object[]>> joinExpression, Expression<Func<T, T2, TResult>> selectExpression, string whereExpression, object obj, string strOrderByFileds = null);
        Task<DataTable> QueryTable<T, T2, T3, TResult>(Expression<Func<T, T2, T3, object[]>> joinExpression, Expression<Func<T, T2, T3, TResult>> selectExpression, string whereExpression, object obj, string strOrderByFileds = null);
        Task<DataTable> QueryTable<T, T2, T3, T4, TResult>(Expression<Func<T, T2, T3, T4, object[]>> joinExpression, Expression<Func<T, T2, T3, T4, TResult>> selectExpression, string whereExpression, object obj, string strOrderByFileds = null);
        Task<DataTable> QueryTable<T, T2, T3, T4, T5, TResult>(Expression<Func<T, T2, T3, T4, T5, object[]>> joinExpression, Expression<Func<T, T2, T3, T4, T5, TResult>> selectExpression, string whereExpression, object obj, string strOrderByFileds = null);
        Task<DataTable> QueryTable<T, T2, T3, T4, T5, T6, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, object[]>> joinExpression, Expression<Func<T, T2, T3, T4, T5, T6, TResult>> selectExpression, string whereExpression, object obj, string strOrderByFileds = null);
        Task<DataTable> QueryTable<T, T2, T3, T4, T5, T6, T7, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, object[]>> joinExpression, Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> selectExpression, string whereExpression, object obj, string strOrderByFileds = null);
        Task<DataTable> QueryTable<T, T2, T3, T4, T5, T6, T7, T8, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object[]>> joinExpression, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> selectExpression, string whereExpression, object obj, string strOrderByFileds = null);
        //Task<DataTable> QueryTable<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object[]>> joinExpression, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> selectExpression, string whereExpression, object obj, string strOrderByFileds = null);
        //Task<DataTable> QueryTable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object[]>> joinExpression, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> selectExpression, string whereExpression, object obj, string strOrderByFileds = null);

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="intPageIndex">页码（下标0）</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>数据列表</returns>
        Task<PageModel<TEntity>> QueryPage(Expression<Func<TEntity, bool>> whereExpression, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null);

        /// <summary>
        /// 分页查询 - 自动分页
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="splitFunc">分页表达式</param>
        /// <param name="intPageIndex">页码（下标0）</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>数据列表</returns>
        Task<PageModel<TEntity>> QueryPageWithSplitTable(Expression<Func<TEntity, bool>> whereExpression, Func<List<SplitTableInfo>, IEnumerable<SplitTableInfo>> splitFunc, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null);

        /// <summary>
        /// 分页查询 - 二级缓存
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="intPageIndex">页码（下标0）</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>数据列表</returns>
        Task<PageModel<TEntity>> QueryPageWithCache(Expression<Func<TEntity, bool>> whereExpression, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null);


        Task<PageModel<TResult>> QueryPage<T, TResult>(Expression<Func<T, TResult>> selectExpression, Expression<Func<T, object>> groupExpression, Expression<Func<T, bool>> havingExpression, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null);
        Task<PageModel<TEntity>> QueryPage(string whereStr, object obj, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null);
        Task<PageModel<TEntity>> QueryPage(Expression<Func<TEntity, bool>> whereExpression, int intPageIndex = 1, int intPageSize = 20, Expression<Func<TEntity, object>> sortExpression = null, OrderByType sortType = OrderByType.Asc);

        Task<List<TResult>> QueryMuch<T, T2, TResult>(Expression<Func<T, T2, object[]>> joinExpression, Expression<Func<T, T2, TResult>> selectExpression, Expression<Func<T, T2, bool>> whereLambda = null) where T : class, new();
        Task<List<TResult>> QueryMuch<T, T2, T3, TResult>(Expression<Func<T, T2, T3, object[]>> joinExpression, Expression<Func<T, T2, T3, TResult>> selectExpression, Expression<Func<T, T2, T3, bool>> whereLambda = null) where T : class, new();
        Task<List<TResult>> QueryMuch<T, T2, T3, T4, TResult>(Expression<Func<T, T2, T3, T4, object[]>> joinExpression, Expression<Func<T, T2, T3, T4, TResult>> selectExpression, Expression<Func<T, T2, T3, T4, bool>> whereLambda = null) where T : class, new();
        Task<List<TResult>> QueryMuch<T, T2, T3, T4, T5, TResult>(Expression<Func<T, T2, T3, T4, T5, object[]>> joinExpression, Expression<Func<T, T2, T3, T4, T5, TResult>> selectExpression, Expression<Func<T, T2, T3, T4, T5, bool>> whereLambda = null) where T : class, new();
        Task<List<TResult>> QueryMuch<T, T2, T3, T4, T5, T6, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, object[]>> joinExpression, Expression<Func<T, T2, T3, T4, T5, T6, TResult>> selectExpression, Expression<Func<T, T2, T3, T4, T5, T6, bool>> whereLambda = null) where T : class, new();
        Task<List<TResult>> QueryMuch<T, T2, T3, T4, T5, T6, T7, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, object[]>> joinExpression, Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> selectExpression, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> whereLambda = null) where T : class, new();
        Task<List<TResult>> QueryMuch<T, T2, T3, T4, T5, T6, T7, T8, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object[]>> joinExpression, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> selectExpression, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> whereLambda = null) where T : class, new();

        Task<PageModel<TResult>> QueryMuchPage<T, T2, TResult>(Expression<Func<T, T2, object[]>> joinExpression, Expression<Func<T, T2, TResult>> selectExpression, Expression<Func<T, T2, bool>> whereExpression, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null);
        Task<PageModel<TResult>> QueryMuchPage<T, T2, TResult>(Expression<Func<T, T2, object[]>> joinExpression, Expression<Func<T, T2, TResult>> selectExpression, Expression<Func<TResult, bool>> whereExpression, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null);
        Task<PageModel<TResult>> QueryMuchPage<T, T2, TResult>(Expression<Func<T, T2, object[]>> joinExpression, Expression<Func<T, T2, TResult>> selectExpression, Expression<Func<TResult, bool>> whereExpression, Expression<Func<T, object>> groupExpression, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null);
        Task<PageModel<TResult>> QueryMuchPage<T, T2, T3, TResult>(Expression<Func<T, T2, T3, object[]>> joinExpression, Expression<Func<T, T2, T3, TResult>> selectExpression, Expression<Func<T, T2, T3, bool>> whereExpression, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null);
        Task<PageModel<TResult>> QueryMuchPage<T, T2, T3, T4, TResult>(Expression<Func<T, T2, T3, T4, object[]>> joinExpression, Expression<Func<T, T2, T3, T4, TResult>> selectExpression, Expression<Func<T, T2, T3, T4, bool>> whereExpression, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null);
        Task<PageModel<TResult>> QueryMuchPage<T, T2, T3, T4, T5, TResult>(Expression<Func<T, T2, T3, T4, T5, object[]>> joinExpression, Expression<Func<T, T2, T3, T4, T5, TResult>> selectExpression, Expression<Func<T, T2, T3, T4, T5, bool>> whereExpression, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null);
        Task<PageModel<TResult>> QueryMuchPage<T, T2, T3, T4, T5, T6, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, object[]>> joinExpression, Expression<Func<T, T2, T3, T4, T5, T6, TResult>> selectExpression, Expression<Func<T, T2, T3, T4, T5, T6, bool>> whereExpression, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null);
        Task<PageModel<TResult>> QueryMuchPage<T, T2, T3, T4, T5, T6, T7, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, object[]>> joinExpression, Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> selectExpression, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> whereExpression, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null);
        Task<PageModel<TResult>> QueryMuchPage<T, T2, T3, T4, T5, T6, T7, T8, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object[]>> joinExpression, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> selectExpression, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> whereExpression, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null);
        //Task<PageModel<TResult>> QueryMuchPage<T, T2, T3, T4, T5, T6, T7, T8, T9,  TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object[]>> joinExpression, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> selectExpression, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> whereExpression, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null);
        //Task<PageModel<TResult>> QueryMuchPage<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object[]>> joinExpression, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> selectExpression, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> whereExpression, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null);


        Task<List<TResult>> QueryMuch<T, T2, TResult>(Expression<Func<T, T2, object[]>> joinExpression, Expression<Func<T, T2, TResult>> selectExpression, string whereExpression, object obj);
        Task<List<TResult>> QueryMuch<T, T2, T3, TResult>(Expression<Func<T, T2, T3, object[]>> joinExpression, Expression<Func<T, T2, T3, TResult>> selectExpression, string whereExpression, object obj);
        Task<List<TResult>> QueryMuch<T, T2, T3, T4, TResult>(Expression<Func<T, T2, T3, T4, object[]>> joinExpression, Expression<Func<T, T2, T3, T4, TResult>> selectExpression, string whereExpression, object obj);
        Task<List<TResult>> QueryMuch<T, T2, T3, T4, T5, TResult>(Expression<Func<T, T2, T3, T4, T5, object[]>> joinExpression, Expression<Func<T, T2, T3, T4, T5, TResult>> selectExpression, string whereExpression, object obj);
        Task<List<TResult>> QueryMuch<T, T2, T3, T4, T5, T6, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, object[]>> joinExpression, Expression<Func<T, T2, T3, T4, T5, T6, TResult>> selectExpression, string whereExpression, object obj);
        Task<List<TResult>> QueryMuch<T, T2, T3, T4, T5, T6, T7, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, object[]>> joinExpression, Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> selectExpression, string whereExpression, object obj);
        Task<List<TResult>> QueryMuch<T, T2, T3, T4, T5, T6, T7, T8, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object[]>> joinExpression, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> selectExpression, string whereExpression, object obj);
        //Task<List<TResult>> QueryMuch<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object[]>> joinExpression, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> selectExpression, string whereExpression, object obj);
        //Task<List<TResult>> QueryMuch<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object[]>> joinExpression, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> selectExpression, string whereExpression, object obj);

        Task<PageModel<TResult>> QueryMuchPage<T, T2, TResult>(Expression<Func<T, T2, object[]>> joinExpression, Expression<Func<T, T2, TResult>> selectExpression, string whereExpression, object obj, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null);
        Task<PageModel<TResult>> QueryMuchPage<T, T2, TResult>(Expression<Func<T, T2, object[]>> joinExpression, Expression<Func<T, T2, TResult>> selectExpression, string whereExpression, object obj, Expression<Func<T, object>> groupExpression, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null);
        Task<PageModel<TResult>> QueryMuchPage<T, T2, T3, TResult>(Expression<Func<T, T2, T3, object[]>> joinExpression, Expression<Func<T, T2, T3, TResult>> selectExpression, string whereExpression, object obj, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null);
        Task<PageModel<TResult>> QueryMuchPage<T, T2, T3, T4, TResult>(Expression<Func<T, T2, T3, T4, object[]>> joinExpression, Expression<Func<T, T2, T3, T4, TResult>> selectExpression, string whereExpression, object obj, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null);
        Task<PageModel<TResult>> QueryMuchPage<T, T2, T3, T4, T5, TResult>(Expression<Func<T, T2, T3, T4, T5, object[]>> joinExpression, Expression<Func<T, T2, T3, T4, T5, TResult>> selectExpression, string whereExpression, object obj, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null);
        Task<PageModel<TResult>> QueryMuchPage<T, T2, T3, T4, T5, T6, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, object[]>> joinExpression, Expression<Func<T, T2, T3, T4, T5, T6, TResult>> selectExpression, string whereExpression, object obj, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null);
        Task<PageModel<TResult>> QueryMuchPage<T, T2, T3, T4, T5, T6, T7, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, object[]>> joinExpression, Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> selectExpression, string whereExpression, object obj, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null);
        Task<PageModel<TResult>> QueryMuchPage<T, T2, T3, T4, T5, T6, T7, T8, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object[]>> joinExpression, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> selectExpression, string whereExpression, object obj, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null);
        //Task<PageModel<TResult>> QueryMuchPage<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object[]>> joinExpression, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> selectExpression, string whereExpression, object obj, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null);
        //Task<PageModel<TResult>> QueryMuchPage<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object[]>> joinExpression, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> selectExpression, string whereExpression, object obj, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null);

        Task<PageModel<TResult>> QueryMuchPageByResult<T, T2, TResult>(Expression<Func<T, T2, object[]>> joinExpression, Expression<Func<T, T2, TResult>> selectExpression, string whereExpression, object obj, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null);
        Task<PageModel<TResult>> QueryMuchPageByResult<T, T2, T3, TResult>(Expression<Func<T, T2, T3, object[]>> joinExpression, Expression<Func<T, T2, T3, TResult>> selectExpression, string whereExpression, object obj, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null);
        Task<PageModel<TResult>> QueryMuchPageByResult<T, T2, T3, T4, TResult>(Expression<Func<T, T2, T3, T4, object[]>> joinExpression, Expression<Func<T, T2, T3, T4, TResult>> selectExpression, string whereExpression, object obj, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null);
        Task<PageModel<TResult>> QueryMuchPageByResult<T, T2, T3, T4, T5, TResult>(Expression<Func<T, T2, T3, T4, T5, object[]>> joinExpression, Expression<Func<T, T2, T3, T4, T5, TResult>> selectExpression, string whereExpression, object obj, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null);
        Task<PageModel<TResult>> QueryMuchPageByResult<T, T2, T3, T4, T5, T6, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, object[]>> joinExpression, Expression<Func<T, T2, T3, T4, T5, T6, TResult>> selectExpression, string whereExpression, object obj, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null);
        Task<PageModel<TResult>> QueryMuchPageByResult<T, T2, T3, T4, T5, T6, T7, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, object[]>> joinExpression, Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> selectExpression, string whereExpression, object obj, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null);
        Task<PageModel<TResult>> QueryMuchPageByResult<T, T2, T3, T4, T5, T6, T7, T8, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object[]>> joinExpression, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> selectExpression, string whereExpression, object obj, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null);
        //Task<PageModel<TResult>> QueryMuchPageByResult<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object[]>> joinExpression, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> selectExpression, string whereExpression, object obj, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null);
        //Task<PageModel<TResult>> QueryMuchPageByResult<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object[]>> joinExpression, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> selectExpression, string whereExpression, object obj, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null);


        Task<List<TEntity>> QueryTop(string strWhere, int intTop, string strOrderByFileds);
        Task<List<TEntity>> QueryTop(Expression<Func<TEntity, bool>> whereExpression, int intTop, string strOrderByFileds);

        Task<List<TEntity>> QuerySql(string sql, List<SugarParameter> parameters = null);
        Task<DataTable> QuerySqlTable(string sql, List<SugarParameter> parameters = null);
        #endregion

        #region 新增
        /// <summary>
        /// 写入实体数据
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        Task<int> Add(TEntity entity);

        /// <summary>
        /// 写入实体数据-自动分表
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        Task<int> AddWithSplitTable(TEntity entity);

        /// <summary>
        /// 写入实体数据
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>返回插入后的实体</returns>
        Task<TEntity> AddGetRestult(TEntity entity);

        /// <summary>
        /// 写入数据-无实体
        /// </summary>
        /// <param name="entity">数据字典</param>
        /// <param name="tableName">表名</param>
        /// <returns>tableName</returns>
        Task<int> Add(Dictionary<string, object> entity, string tableName);

        /// <summary>
        /// 批量写入数据-无实体
        /// </summary>
        /// <param name="entity">数据字典</param>
        /// <param name="tableName">表名</param>
        /// <returns>tableName</returns>
        Task<int> Add(List<Dictionary<string, object>> entitys, string tableName);

        /// <summary>
        /// 写入实体数据
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="insertColumns">指定插入列</param>
        /// <param name="ingoreColumns">指定忽略插入列</param>
        Task<int> Add(TEntity entity, Expression<Func<TEntity, object>> insertColumns = null, Expression<Func<TEntity, object>> ingoreColumns = null);

        /// <summary>
        /// 批量插入实体，500条以下速度最快，兼容所有类型和emoji，500以上就开始慢了
        /// </summary>
        /// <param name="entities">实体</param>
        /// <returns></returns>
        Task<int> Add(List<TEntity> entities);

        /// <summary>
        /// 批量插入实体，500条以下速度最快，兼容所有类型和emoji，500以上就开始慢了 - 自动分表
        /// </summary>
        /// <param name="entities">实体</param>
        /// <returns></returns>
        Task<int> AddWithSplitTable(List<TEntity> entities);

        /// <summary>
        /// 大批量插入实体，1000条以上性能无敌手
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <returns>影响行数</returns>
        Task<int> AddMuch(List<TEntity> entities);

        /// <summary>
        /// 大批量插入实体，1000条以上性能无敌手 - 自动分表
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <returns>影响行数</returns>
        Task<int> AddMuchWithSplitTable(List<TEntity> entities);
        #endregion

        #region 修改
        /// <summary>
        /// 更新实体数据
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        Task<bool> Update(TEntity entity);

        /// <summary>
        /// 更新实体数据 - 自动分表
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        Task<int> UpdateWithSplitTable(TEntity entity);

        /// <summary>
        /// 更新实体数据（校验行版本）
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        Task<int> UpdateCheckVersion(TEntity entity);

        /// <summary>
        /// 更新数据-无实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="tableName">表名</param>
        /// <param name="updateColumns">更新列</param>
        /// <param name="ignoreColumns">忽略列</param>
        /// <param name="whereColumns">条件列</param>
        /// <returns></returns>
        Task<bool> Update(TEntity entity, string tableName, List<string> updateColumns = null, List<string> ignoreColumns = null, string[] whereColumns = null);

        /// <summary>
        /// 增量更新数据库数据
        /// </summary>
        /// <param name="incrementalExpression">增量更新表达式 it => it.Age == it.Age + 10 && it.Name = it.Name + "test" </param>
        /// <param name="whereExpression">条件表达式</param>
        /// <returns></returns>
        Task<bool> UpdateIncremental(Expression<Func<TEntity, bool>> incrementalExpression, Expression<Func<TEntity, bool>> whereExpression);

        /// <summary>
        /// 增量更新数据库数据
        /// </summary>
        /// <param name="incrementalExpression">增量更新表达式 it => new TEntity { Age == it.Age + 10 , Name = it.Name + "test"} </param>
        /// <param name="whereExpression">条件表达式</param>
        /// <returns></returns>
        Task<bool> UpdateIncremental(Expression<Func<TEntity, TEntity>> incrementalExpression, Expression<Func<TEntity, bool>> whereExpression);

        Task<bool> Update(TEntity entity, string strWhere);
        Task<bool> Update(TEntity entity, List<string> updateColumns = null, List<string> ignoreColumns = null, string strWhere = "");
        Task<bool> Update(TEntity entity, Expression<Func<TEntity, object>> updateExpression, Expression<Func<TEntity, object>> ignoreExpression);

        /// <summary>
        /// 更新实体数据
        /// </summary>
        /// <param name="entitys">实体集合</param>
        /// <returns></returns>
        Task<bool> Update(List<TEntity> entitys);

        /// <summary>
        /// 更新实体数据 - 自动分表
        /// </summary>
        /// <param name="entitys">实体集合</param>
        /// <returns></returns>
        Task<int> UpdateWithSplitTable(List<TEntity> entitys);

        Task<bool> Update(List<TEntity> entitys, string strWhere);
        Task<bool> Update(List<TEntity> entity, List<string> updateColumns = null, List<string> ignoreColumns = null, string strWhere = "");
        Task<bool> Update(List<TEntity> entity, Expression<Func<TEntity, object>> updateExpression, Expression<Func<TEntity, object>> ignoreExpression);

        /// <summary>
        /// 大批量更新实体，1000条以上性能无敌手
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <returns>影响行数</returns>
        Task<int> UpdateMuch(List<TEntity> entities);

        /// <summary>
        /// 大批量更新实体，1000条以上性能无敌手 - 自动分表
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <returns>影响行数</returns>
        Task<int> UpdateMuchWithSplitTable(List<TEntity> entities);
        #endregion

        #region 删除
        Task<bool> DeleteById(object id);
        Task<bool> DeleteByIds(object[] ids);

        /// <summary>
        /// 根据实体删除一条数据
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        Task<bool> Delete(TEntity entity);

        /// <summary>
        /// 根据实体删除一条数据 - 自动分表
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        Task<int> DeleteWithSplitTable(TEntity entity);

        /// <summary>
        /// 根据实体集合删除多条数据(批量删除)
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <returns></returns>
        Task<bool> Delete(List<TEntity> entities);

        /// <summary>
        /// 根据实体集合删除多条数据(批量删除) - 自动分表
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <returns></returns>
        Task<int> DeleteWithSplitTable(List<TEntity> entities);

        Task<bool> Delete(Expression<Func<TEntity, bool>> whereExpression);
        #endregion


        #region 数据新增/修改
        /// <summary>
        /// 调用存储过程
        /// </summary>
        /// <returns>List</returns>
        Task<object> InportInsertOrUpdate(List<TEntity> entities, Expression<Func<TEntity, object>> columns);
        #endregion

        #region 存储过程
        /// <summary>
        /// 调用存储过程
        /// </summary>
        /// <param name="storedProcedureName"></param>
        /// <param name="parameters"></param>
        /// <returns>List</returns>
        Task<List<TEntity>> UseStoredProcedureGetList(string storedProcedureName, params SugarParameter[] parameters);

        /// <summary>
        /// 调用存储过程
        /// </summary>
        /// <param name="storedProcedureName"></param>
        /// <param name="parameters"></param>
        /// <returns>DataTable</returns>
        Task<DataTable> UseStoredProcedureGetDataTable(string storedProcedureName, params SugarParameter[] parameters);

        /// <summary>
        /// 调用存储过程
        /// </summary>
        /// <param name="storedProcedureName"></param>
        /// <param name="parameters"></param>
        /// <returns>string</returns>
        Task<string> UseStoredProcedureGetString(string storedProcedureName, params SugarParameter[] parameters);
        #endregion

        Task<int> CommandBySql(string sql, List<SugarParameter> parameters = null);

    }

}

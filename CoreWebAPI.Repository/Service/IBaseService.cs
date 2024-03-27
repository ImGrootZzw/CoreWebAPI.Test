using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CoreWebAPI.Repository.IService
{
    public interface IBaseService<TEntity> where TEntity : class
    {


        #region 查询
        Task<TEntity> QueryById(object objId);
        Task<TEntity> QueryById(object objId, bool blnUseCache = false);
        Task<List<TEntity>> QueryByIDs(object[] lstIds);

        Task<List<TEntity>> Query();
        Task<List<TEntity>> Query(string strWhere);
        Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression);
        Task<List<TEntity>> Query(string strWhere, string strOrderByFileds);
        Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression, string strOrderByFileds);
        Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, object>> orderByExpression, bool isAsc = true);
        Task<List<TEntity>> Query(string strWhere, int intPageIndex, int intPageSize, string strOrderByFileds);
        Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression, int intPageIndex, int intPageSize, string strOrderByFileds);

        Task<DataTable> QueryTable(Expression<Func<TEntity, bool>> whereExpression, string strOrderByFileds);
        Task<DataTable> QueryTable(string whereStr, string strOrderByFileds);
        Task<DataTable> QueryTable(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, object>> sortExpression = null, OrderByType sortType = OrderByType.Asc);
        Task<DataTable> QueryTable<T, T2, TResult>(Expression<Func<T, T2, object[]>> joinExpression, Expression<Func<T, T2, TResult>> selectExpression, Expression<Func<T, T2, bool>> whereExpression, string strOrderByFileds = null);
        Task<DataTable> QueryTable<T, T2, T3, TResult>(Expression<Func<T, T2, T3, object[]>> joinExpression, Expression<Func<T, T2, T3, TResult>> selectExpression, Expression<Func<T, T2, T3, bool>> whereExpression, string strOrderByFileds = null);
        Task<DataTable> QueryTable<T, T2, T3, T4, TResult>(Expression<Func<T, T2, T3, T4, object[]>> joinExpression, Expression<Func<T, T2, T3, T4, TResult>> selectExpression, Expression<Func<T, T2, T3, T4, bool>> whereExpression, string strOrderByFileds = null);
        Task<DataTable> QueryTable<T, T2, T3, T4, T5, TResult>(Expression<Func<T, T2, T3, T4, T5, object[]>> joinExpression, Expression<Func<T, T2, T3, T4, T5, TResult>> selectExpression, Expression<Func<T, T2, T3, T4, T5, bool>> whereExpression, string strOrderByFileds = null);

        Task<PageModel<TEntity>> QueryPage(Expression<Func<TEntity, bool>> whereExpression, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null);
        Task<PageModel<TEntity>> QueryPage(string whereStr, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null);
        Task<PageModel<TEntity>> QueryPage(Expression<Func<TEntity, bool>> whereExpression, int intPageIndex = 1, int intPageSize = 20, Expression<Func<TEntity, object>> sortExpression = null, OrderByType sortType = OrderByType.Asc);

        Task<List<TResult>> QueryMuch<T, T2, TResult>(Expression<Func<T, T2, object[]>> joinExpression, Expression<Func<T, T2, TResult>> selectExpression, Expression<Func<T, T2, bool>> whereLambda = null) where T : class, new();
        Task<List<TResult>> QueryMuch<T, T2, T3, TResult>(Expression<Func<T, T2, T3, object[]>> joinExpression, Expression<Func<T, T2, T3, TResult>> selectExpression, Expression<Func<T, T2, T3, bool>> whereLambda = null) where T : class, new();
        Task<List<TResult>> QueryMuch<T, T2, T3, T4, TResult>(Expression<Func<T, T2, T3, T4, object[]>> joinExpression, Expression<Func<T, T2, T3, T4, TResult>> selectExpression, Expression<Func<T, T2, T3, T4, bool>> whereLambda = null) where T : class, new();
        Task<List<TResult>> QueryMuch<T, T2, T3, T4, T5, TResult>(Expression<Func<T, T2, T3, T4, T5, object[]>> joinExpression, Expression<Func<T, T2, T3, T4, T5, TResult>> selectExpression, Expression<Func<T, T2, T3, T4, T5, bool>> whereLambda = null) where T : class, new();

        Task<PageModel<TResult>> QueryMuchPage<T, T2, TResult>(Expression<Func<T, T2, object[]>> joinExpression, Expression<Func<T, T2, TResult>> selectExpression, Expression<Func<T, T2, bool>> whereExpression, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null);
        Task<PageModel<TResult>> QueryMuchPage<T, T2, TResult>(Expression<Func<T, T2, object[]>> joinExpression, Expression<Func<T, T2, TResult>> selectExpression, Expression<Func<TResult, bool>> whereExpression, Expression<Func<T, object>> groupExpression, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null);
        Task<PageModel<TResult>> QueryMuchPage<T, T2, T3, TResult>(Expression<Func<T, T2, T3, object[]>> joinExpression, Expression<Func<T, T2, T3, TResult>> selectExpression, Expression<Func<T, T2, T3, bool>> whereExpression, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null);
        Task<PageModel<TResult>> QueryMuchPage<T, T2, T3, T4, TResult>(Expression<Func<T, T2, T3, T4, object[]>> joinExpression, Expression<Func<T, T2, T3, T4, TResult>> selectExpression, Expression<Func<T, T2, T3, T4, bool>> whereExpression, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null);
        Task<PageModel<TResult>> QueryMuchPage<T, T2, T3, T4, T5, TResult>(Expression<Func<T, T2, T3, T4, T5, object[]>> joinExpression, Expression<Func<T, T2, T3, T4, T5, TResult>> selectExpression, Expression<Func<T, T2, T3, T4, T5, bool>> whereExpression, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null);

        Task<List<TEntity>> QuerySql(string sql, List<SugarParameter> parameters = null);
        Task<DataTable> QuerySqlTable(string sql, List<SugarParameter> parameters = null);
        #endregion

        #region 新增
        Task<int> Add(TEntity model);
        Task<TEntity> AddGetRestult(TEntity model);
        Task<int> Add(TEntity entity, Expression<Func<TEntity, object>> insertColumns = null, Expression<Func<TEntity, object>> ingoreColumns = null);
        Task<int> Add(List<TEntity> listEntity);
        Task<int> AddMuch(List<TEntity> listEntity);
        #endregion

        #region 更新
        Task<bool> Update(TEntity model);
        Task<bool> Update(TEntity entity, string strWhere);
        Task<bool> Update(TEntity entity, List<string> updateColumns = null, List<string> ignoreColumns = null, string strWhere = "");
        Task<bool> Update(TEntity entity, Expression<Func<TEntity, object>> updateExpression, Expression<Func<TEntity, object>> ignoreExpression);

        /// <summary>
        /// 大批量更新实体，1000条以上性能无敌手
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <returns>影响行数</returns>
        Task<int> UpdateMuch(List<TEntity> entities);
        #endregion

        #region 删除
        Task<bool> DeleteById(object id);
        Task<bool> DeleteByIds(object[] ids);
        Task<bool> Delete(TEntity model);
        Task<bool> Delete(List<TEntity> entity);
        Task<bool> Delete(Expression<Func<TEntity, bool>> whereExpression);
        #endregion

        Task<int> CommandBySql(string sql, List<SugarParameter> parameters = null);
    }
}

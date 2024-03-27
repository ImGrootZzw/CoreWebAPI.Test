using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CoreWebAPI.Repository.Base;
using CoreWebAPI.Repository.IService;

namespace CoreWebAPI.Repository.Service
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class BaseService<TEntity> : IBaseService<TEntity> where TEntity : class, new()
    {

        public IBaseRepository<TEntity> BaseDal;//通过在子类的构造函数中注入，这里是基类，不用构造函数

        #region 查询
        /// <summary>
        /// 功能描述:根据ID查询一条数据
        /// </summary>
        /// <param name="objId">id（必须指定主键特性 [SugarColumn(IsPrimaryKey=true)]），如果是联合主键，请使用Where条件</param>
        /// <returns>数据实体</returns>
        public async Task<TEntity> QueryById(object objId)
        {
            return await BaseDal.QueryById(objId);
        }
        /// <summary>
        /// 功能描述:根据ID查询一条数据
        /// </summary>
        /// <param name="objId">id（必须指定主键特性 [SugarColumn(IsPrimaryKey=true)]），如果是联合主键，请使用Where条件</param>
        /// <param name="blnUseCache">是否使用缓存</param>
        /// <returns>数据实体</returns>
        public async Task<TEntity> QueryById(object objId, bool blnUseCache = false)
        {
            return await BaseDal.QueryById(objId, blnUseCache);
        }

        /// <summary>
        /// 功能描述:根据ID查询数据
        /// </summary>
        /// <param name="lstIds">id列表（必须指定主键特性 [SugarColumn(IsPrimaryKey=true)]），如果是联合主键，请使用Where条件</param>
        /// <returns>数据实体列表</returns>
        public async Task<List<TEntity>> QueryByIDs(object[] lstIds)
        {
            return await BaseDal.QueryByIDs(lstIds);
        }

        /// <summary>
        /// 功能描述:查询所有数据
        /// </summary>
        /// <returns>数据列表</returns>
        public async Task<List<TEntity>> Query()
        {
            return await BaseDal.Query();
        }

        /// <summary>
        /// 功能描述:查询数据列表
        /// </summary>
        /// <param name="strWhere">条件</param>
        /// <returns>数据列表</returns>
        public async Task<List<TEntity>> Query(string strWhere)
        {
            return await BaseDal.Query(strWhere, new Dictionary<string, object>());
        }

        /// <summary>
        /// 功能描述:查询数据列表
        /// </summary>
        /// <param name="whereExpression">whereExpression</param>
        /// <returns>数据列表</returns>
        public async Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression)
        {
            return await BaseDal.Query(whereExpression);
        }
        /// <summary>
        /// 功能描述:查询一个列表
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="orderByExpression">排序字段</param>
        /// <param name="isAsc">排序字段</param>
        /// <returns>数据列表</returns>
        public async Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, object>> orderByExpression, bool isAsc = true)
        {
            return await BaseDal.Query(whereExpression, orderByExpression, isAsc);
        }

        public async Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression, string strOrderByFileds)
        {
            return await BaseDal.Query(whereExpression, strOrderByFileds);
        }

        /// <summary>
        /// 功能描述:查询一个列表
        /// </summary>
        /// <param name="strWhere">条件</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>数据列表</returns>
        public async Task<List<TEntity>> Query(string strWhere, string strOrderByFileds)
        {
            return await BaseDal.Query(strWhere, strOrderByFileds);
        }
        /// <summary>
        /// 功能描述:查询前N条数据
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="intTop">前N条</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>数据列表</returns>
        public async Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression, int intTop, string strOrderByFileds)
        {
            return await BaseDal.QueryTop(whereExpression, intTop, strOrderByFileds);
        }

        /// <summary>
        /// 功能描述:查询前N条数据
        /// </summary>
        /// <param name="strWhere">条件</param>
        /// <param name="intTop">前N条</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>数据列表</returns>
        public async Task<List<TEntity>> Query(
            string strWhere,
            int intTop,
            string strOrderByFileds)
        {
            return await BaseDal.QueryTop(strWhere, intTop, strOrderByFileds);
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
            return await BaseDal.Query(
              whereExpression,
              intPageIndex,
              intPageSize,
              strOrderByFileds);
        }

        /// <summary>
        /// 功能描述:分页查询
        /// </summary>
        /// <param name="strWhere">条件</param>
        /// <param name="intPageIndex">页码（下标0）</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>数据列表</returns>
        public async Task<List<TEntity>> Query(string strWhere, int intPageIndex, int intPageSize, string strOrderByFileds)
        {
            return await BaseDal.Query(
            strWhere, new Dictionary<string, object>(),
            intPageIndex,
            intPageSize,
            strOrderByFileds);
        }

        /// <summary>
        /// 根据sql语句查询
        /// </summary>
        /// <param name="strSql">完整的sql语句</param>
        /// <param name="parameters">参数</param>
        /// <returns>DataTable</returns>
        public async Task<DataTable> QueryTable(string strSql, List<SugarParameter> parameters = null)
        {
            return await BaseDal.QuerySqlTable(strSql, parameters);

        }

        /// <summary>
        /// 功能描述:查询一个列表
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>DataTable</returns>
        public async Task<DataTable> QueryTable(Expression<Func<TEntity, bool>> whereExpression, string strOrderByFileds)
        {
            return await BaseDal.QueryTable(whereExpression, strOrderByFileds);
        }

        /// <summary>
        /// 功能描述:查询一个列表
        /// </summary>
        /// <param name="whereStr">条件表达式</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>DataTable</returns>
        public async Task<DataTable> QueryTable(string whereStr, string strOrderByFileds)
        {
            return await BaseDal.QueryTable(whereStr, new Dictionary<string, object>(), strOrderByFileds);
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
            return await BaseDal.QueryTable(whereExpression, sortExpression, sortType);
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
            return await BaseDal.QueryTable(joinExpression, selectExpression, whereExpression, strOrderByFileds);
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
            return await BaseDal.QueryTable(joinExpression, selectExpression, whereExpression, strOrderByFileds);
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
            return await BaseDal.QueryTable(joinExpression, selectExpression, whereExpression, strOrderByFileds);
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
            return await BaseDal.QueryTable(joinExpression, selectExpression, whereExpression, strOrderByFileds);
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
            return await BaseDal.QueryPage(whereExpression, intPageIndex, intPageSize, strOrderByFileds);
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="whereStr">条件表达式</param>
        /// <param name="intPageIndex">页码（下标0）</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="strOrderByFileds">排序字段，如name asc,age desc</param>
        /// <returns>数据列表</returns>
        public async Task<PageModel<TEntity>> QueryPage(string whereStr, int intPageIndex = 1, int intPageSize = 20, string strOrderByFileds = null)
        {
            return await BaseDal.QueryPage(whereStr, new Dictionary<string, object>(), intPageIndex, intPageSize, strOrderByFileds);
        }
        public async Task<PageModel<TEntity>> QueryPage(Expression<Func<TEntity, bool>> whereExpression, int intPageIndex = 1, int intPageSize = 20, Expression<Func<TEntity, object>> sortExpression = null, OrderByType sortType = OrderByType.Asc)
        {
            return await BaseDal.QueryPage(whereExpression, intPageIndex, intPageSize, sortExpression, sortType);
        }

        public async Task<List<TResult>> QueryMuch<T, T2, TResult>(Expression<Func<T, T2, object[]>> joinExpression, Expression<Func<T, T2, TResult>> selectExpression, Expression<Func<T, T2, bool>> whereLambda = null) where T : class, new()
        {
            return await BaseDal.QueryMuch(joinExpression, selectExpression, whereLambda);
        }
        public async Task<List<TResult>> QueryMuch<T, T2, T3, TResult>(Expression<Func<T, T2, T3, object[]>> joinExpression, Expression<Func<T, T2, T3, TResult>> selectExpression, Expression<Func<T, T2, T3, bool>> whereLambda = null) where T : class, new()
        {
            return await BaseDal.QueryMuch(joinExpression, selectExpression, whereLambda);
        }
        public async Task<List<TResult>> QueryMuch<T, T2, T3, T4, TResult>(Expression<Func<T, T2, T3, T4, object[]>> joinExpression, Expression<Func<T, T2, T3, T4, TResult>> selectExpression, Expression<Func<T, T2, T3, T4, bool>> whereLambda = null) where T : class, new()
        {
            return await BaseDal.QueryMuch(joinExpression, selectExpression, whereLambda);
        }
        public async Task<List<TResult>> QueryMuch<T, T2, T3, T4, T5, TResult>(Expression<Func<T, T2, T3, T4, T5, object[]>> joinExpression, Expression<Func<T, T2, T3, T4, T5, TResult>> selectExpression, Expression<Func<T, T2, T3, T4, T5, bool>> whereLambda = null) where T : class, new()
        {
            return await BaseDal.QueryMuch(joinExpression, selectExpression, whereLambda);
        }
        public async Task<PageModel<TResult>> QueryMuchPage<T, T2, TResult>(
            Expression<Func<T, T2, object[]>> joinExpression,
            Expression<Func<T, T2, TResult>> selectExpression,
            Expression<Func<T, T2, bool>> whereExpression,
            int intPageIndex = 1,
            int intPageSize = 20,
            string strOrderByFileds = null)
        {
            return await BaseDal.QueryMuchPage(joinExpression, selectExpression, whereExpression, intPageIndex, intPageSize, strOrderByFileds);
        }
        public async Task<PageModel<TResult>> QueryMuchPage<T, T2, TResult>(
            Expression<Func<T, T2, object[]>> joinExpression,
            Expression<Func<T, T2, TResult>> selectExpression,
            Expression<Func<TResult, bool>> whereExpression,
            Expression<Func<T, object>> groupExpression,
            int intPageIndex = 1,
            int intPageSize = 20,
            string strOrderByFileds = null)
        {
            return await BaseDal.QueryMuchPage(joinExpression, selectExpression, whereExpression, groupExpression, intPageIndex, intPageSize, strOrderByFileds);
        }
        public async Task<PageModel<TResult>> QueryMuchPage<T, T2, T3, TResult>(
            Expression<Func<T, T2, T3, object[]>> joinExpression,
            Expression<Func<T, T2, T3, TResult>> selectExpression,
            Expression<Func<T, T2, T3, bool>> whereExpression,
            int intPageIndex = 1,
            int intPageSize = 20,
            string strOrderByFileds = null)
        {
            return await BaseDal.QueryMuchPage(joinExpression, selectExpression, whereExpression, intPageIndex, intPageSize, strOrderByFileds);
        }
        public async Task<PageModel<TResult>> QueryMuchPage<T, T2, T3, T4, TResult>(
            Expression<Func<T, T2, T3, T4, object[]>> joinExpression,
            Expression<Func<T, T2, T3, T4, TResult>> selectExpression,
            Expression<Func<T, T2, T3, T4, bool>> whereExpression,
            int intPageIndex = 1,
            int intPageSize = 20,
            string strOrderByFileds = null)
        {
            return await BaseDal.QueryMuchPage(joinExpression, selectExpression, whereExpression, intPageIndex, intPageSize, strOrderByFileds);
        }
        public async Task<PageModel<TResult>> QueryMuchPage<T, T2, T3, T4, T5, TResult>(
            Expression<Func<T, T2, T3, T4, T5, object[]>> joinExpression,
            Expression<Func<T, T2, T3, T4, T5, TResult>> selectExpression,
            Expression<Func<T, T2, T3, T4, T5, bool>> whereExpression,
            int intPageIndex = 1,
            int intPageSize = 20,
            string strOrderByFileds = null)
        {
            return await BaseDal.QueryMuchPage(joinExpression, selectExpression, whereExpression, intPageIndex, intPageSize, strOrderByFileds);
        }

        /// <summary>
        /// 根据sql语句查询
        /// </summary>
        /// <param name="strSql">完整的sql语句</param>
        /// <param name="parameters">参数</param>
        /// <returns>泛型集合</returns>
        public async Task<List<TEntity>> QuerySql(string strSql, List<SugarParameter> parameters = null)
        {
            return await BaseDal.QuerySql(strSql, parameters);

        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="strSql">完整的sql语句</param>
        /// <param name="parameters">参数</param>
        /// <returns>结果集</returns>
        public async Task<DataTable> QuerySqlTable(string strSql, List<SugarParameter> parameters = null)
        {
            return await BaseDal.QuerySqlTable(strSql, parameters);
        }
        #endregion

        #region 新增
        /// <summary>
        /// 写入实体数据
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public async Task<int> Add(TEntity entity)
        {
            return await BaseDal.Add(entity);
        }
        /// <summary>
        /// 写入实体数据
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>插入实体</returns>
        public async Task<TEntity> AddGetRestult(TEntity entity)
        {
            return await BaseDal.AddGetRestult(entity);
        }

        /// <summary>
        /// 写入实体数据
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="insertColumns">指定插入列</param>
        /// <param name="ingoreColumns">指定忽略插入列</param>
        /// <returns></returns>
        public async Task<int> Add(TEntity entity, Expression<Func<TEntity, object>> insertColumns = null, Expression<Func<TEntity, object>> ingoreColumns = null)
        {
            return await BaseDal.Add(entity, insertColumns, ingoreColumns);
        }

        /// <summary>
        /// 批量插入实体，500条以下速度最快，兼容所有类型和emoji，500以上就开始慢了
        /// </summary>
        /// <param name="listEntity">实体集合</param>
        /// <returns>影响行数</returns>
        public async Task<int> Add(List<TEntity> listEntity)
        {
            return await BaseDal.Add(listEntity);
        }

        /// <summary>
        /// 大批量插入实体，1000条以上性能无敌手
        /// </summary>
        /// <param name="listEntity">实体集合</param>
        /// <returns>影响行数</returns>
        public async Task<int> AddMuch(List<TEntity> listEntity)
        {
            return await BaseDal.AddMuch(listEntity);
        }


        #endregion

        #region 更新
        /// <summary>
        /// 更新实体数据
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public async Task<bool> Update(TEntity entity)
        {
            return await BaseDal.Update(entity);
        }
        /// <summary>
        /// 更新实体数据
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="strWhere">条件字符串</param>
        /// <returns></returns>
        public async Task<bool> Update(TEntity entity, string strWhere)
        {
            return await BaseDal.Update(entity, strWhere);
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
            return await BaseDal.Update(entity, updateColumns, ignoreColumns, strWhere);
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
            return await BaseDal.Update(entity, updateExpression, ignoreExpression);
        }

        /// <summary>
        /// 大批量更新实体，1000条以上性能无敌手
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <returns>影响行数</returns>
        public async Task<int> UpdateMuch(List<TEntity> entities)
        {
            return await BaseDal.UpdateMuch(entities);
        }
        #endregion

        #region 删除
        /// <summary>
        /// 删除指定ID的数据
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        public async Task<bool> DeleteById(object id)
        {
            return await BaseDal.DeleteById(id);
        }
        /// <summary>
        /// 删除指定ID集合的数据(批量删除)
        /// </summary>
        /// <param name="ids">主键ID集合</param>
        /// <returns></returns>
        public async Task<bool> DeleteByIds(object[] ids)
        {
            return await BaseDal.DeleteByIds(ids);
        }

        /// <summary>
        /// 根据实体删除一条数据
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public async Task<bool> Delete(TEntity entity)
        {
            return await BaseDal.Delete(entity);
        }
        /// <summary>
        /// 根据实体删除多条数据
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Delete(List<TEntity> entity)
        {
            return await BaseDal.Delete(entity);
        }

        /// <summary>
        /// 根据表达式删除数据
        /// </summary>
        /// <param name="whereExpression">条件表达式</param>
        /// <returns></returns>
        public async Task<bool> Delete(Expression<Func<TEntity, bool>> whereExpression)
        {
            return await BaseDal.Delete(whereExpression);
        }
        #endregion

        #region "原生sql操作"
        /// <summary>
        /// 新增、修改、删除
        /// </summary>
        /// <param name="strSql">完整的sql语句</param>
        /// <param name="parameters">参数</param>
        /// <returns>影响行数</returns>
        public async Task<int> CommandBySql(string strSql, List<SugarParameter> parameters = null)
        {
            return await BaseDal.CommandBySql(strSql, parameters);
        }

        #endregion
    }
}

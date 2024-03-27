using SqlSugar;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CoreWebAPI.Common.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public static class ClassMapSqlsugar
    {
        /// <summary>
        /// 获取模型属性名
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="field">数据库字段名</param>
        /// <returns></returns>
        public static string GetDBColumnName<TEntity>(string field)
        {
            var rt = "";
            if (field.GetIsNotEmptyOrNull())
            {
                var prop = typeof(TEntity).GetProperty(field.GetCString(), BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
                if (prop.GetIsNotEmptyOrNull())
                    rt = prop.GetCustomAttribute<SugarColumn>().GetIsNotEmptyOrNull() ? prop.GetCustomAttribute<SugarColumn>().ColumnName.GetCString() : "";
            }
            return rt;
        }

        /// <summary>
        /// 获取数据库排序字段
        /// </summary>
        /// <typeparam name="TEntity">数据库模型</typeparam>
        /// <param name="sortObj">排序字段List</param>
        /// <returns></returns>
        public static string GetDBSort<TEntity>(object sortObj)
        {
            var rt = "";
            try
            {
                List<SortModel> sortList = (List<SortModel>)JsonHelper.JsonToObject(JsonHelper.ObjectToJson(sortObj), typeof(List<SortModel>));
                if (sortList != null && sortList.Count > 0)
                {
                    foreach (SortModel sort in sortList)
                    {
                        var prop = typeof(TEntity).GetProperty(sort.SortParam.GetCString(), BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
                        if (prop.GetIsNotEmptyOrNull() && prop.GetCustomAttribute<SugarColumn>().GetIsNotEmptyOrNull())
                        {
                            rt += prop.GetCustomAttribute<SugarColumn>().ColumnName.GetCString() + (sort.Ascending ? " asc," : " desc,");
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            if (rt == "")
            {
                var props = typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
                foreach (var p in props)
                {
                    var pAttr = p.GetCustomAttribute<SugarColumn>();
                    if (pAttr.GetIsNotEmptyOrNull() && pAttr.IndexGroupNameList != null)
                    {
                        if (Array.Exists(pAttr.IndexGroupNameList, str => str.Contains("search_index")))
                            rt += pAttr.ColumnName.GetCString() + ",";
                    }
                }
            }
            if (rt == "")
            {
                var props = typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
                foreach (var p in props)
                {
                    var pAttr = p.GetCustomAttribute<SugarColumn>();
                    if (pAttr.GetIsNotEmptyOrNull() && pAttr.IndexGroupNameList != null)
                    {
                        if (Array.Exists(pAttr.IndexGroupNameList, str => str.Contains("uk_index")) || Array.Exists(pAttr.IndexGroupNameList, str => str.Contains("unique_index")))
                            rt += pAttr.ColumnName.GetCString() + ",";
                    }
                }
            }
            return rt.Trim(',');
        }
    }

    /// <summary>
    /// 排序
    /// </summary>
    public class SortModel
    {
        /// <summary>
        /// 描述 : 排序字段
        /// 空值 : False
        /// 默认 : 
        /// </summary>
        public string SortParam { get; set; }

        /// <summary>
        /// 描述 : 正序
        /// 空值 : False
        /// 默认 : 
        /// </summary>
        public bool Ascending { get; set; }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace CoreWebAPI.Common.Helper
{
    /// <summary>
    /// 排序类型
    /// </summary>
    public enum SortType
    {
        /// <summary>
        /// 正序
        /// </summary>
        ASC = 0,

        /// <summary>
        /// 倒序
        /// </summary>
        DESC = 1
    }

    public static class ListHelper
    {
        /// <summary>
        /// 校验列表是否为空，是返回空列表，否返回当前列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetNotNull<T>(this IEnumerable<T> list)
        {
            return list ?? new List<T>(0); 
        }

        /// <summary>
        /// List转DataTable
        /// </summary>
        /// <returns></returns>
        public static DataTable ListToDataTable<T>(IEnumerable<T> collection)
        {
            var props = typeof(T).GetProperties();
            var dt = new DataTable();
            dt.Columns.AddRange(props.Select(p => new DataColumn(p.Name, p.PropertyType)).ToArray());
            if (collection.Count() > 0)
            {
                for (int i = 0; i < collection.Count(); i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in props)
                    {
                        object obj = pi.GetValue(collection.ElementAt(i), null);
                        tempList.Add(obj);
                    }
                    object[] array = tempList.ToArray();
                    dt.LoadDataRow(array, true);
                }
            }
            return dt;
        }

        /// <summary>
        /// DataTable转成List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> ToDataList<T>(DataTable dt)
        {
            var list = new List<T>();
            var plist = new List<PropertyInfo>(typeof(T).GetProperties());
            foreach (DataRow item in dt.Rows)
            {
                T s = Activator.CreateInstance<T>();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    PropertyInfo info = plist.Find(p => p.Name == dt.Columns[i].ColumnName);
                    if (info != null)
                    {
                        try
                        {
                            if (!Convert.IsDBNull(item[i]))
                            {
                                object v = null;
                                if (info.PropertyType.ToString().Contains("System.Nullable"))
                                {
                                    v = Convert.ChangeType(item[i], Nullable.GetUnderlyingType(info.PropertyType));
                                }
                                else
                                {
                                    v = Convert.ChangeType(item[i], info.PropertyType);
                                }
                                info.SetValue(s, v, null);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("字段[" + info.Name + "]转换出错," + ex.Message);
                        }
                    }
                }
                list.Add(s);
            }
            return list;
        }

        /// <summary>
        /// List转Dictionary
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, object>[] ListToDictionary<T>(IEnumerable<T> collection)
        {
            var props = typeof(T).GetProperties();
            if (collection.Any())
            {
                var da = new Dictionary<string, object>[collection.Count()];
                for (int i = 0; i < collection.Count(); i++)
                {
                    foreach (PropertyInfo pi in props)
                    {
                        object obj = pi.GetValue(collection.ElementAt(i), null);
                        da[i].Add(pi.Name, obj);
                    }
                }
                return da;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// List分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="thisList">数据源</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="sortObj">排序字段:[{sortParam: "fieldName", ascending: true}]</param>
        /// <returns></returns>
        public static List<T> GetPage<T>(this List<T> thisList, int pageIndex = 1, int pageSize = 20, object sortObj = null)
        {
            int _PageIndex = pageIndex == 0 ? 1 : pageIndex;
            int _PageSize = pageSize == 0 ? 20 : pageSize;
            int _PageConut = (int)Math.Ceiling(Convert.ToDecimal(thisList.Count) / _PageSize);

            if (_PageConut >= _PageIndex)
            {
                if (sortObj != null)
                {
                    List<SortModel> sortList = JsonHelper.JsonToObject<List<SortModel>>(JsonHelper.ObjectToJson(sortObj));
                    if (sortList != null && sortList.Count > 0)
                    {
                        var listResult = JsonHelper.JsonToObject<List<T>>(JsonHelper.ObjectToJson(thisList));
                        foreach (SortModel sort in sortList)
                        {
                            var prop = typeof(T).GetProperty(sort.SortParam.GetCString(), BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
                            object keySelector(T enty) => prop.GetValue(enty, null);
                            listResult = sort.Ascending ? listResult.OrderBy(keySelector).ToList() : listResult.OrderByDescending(keySelector).ToList();
                        }
                        return listResult.Skip((_PageIndex - 1) * _PageSize).Take(_PageSize).ToList();
                    }
                }

                return thisList.Skip((_PageIndex - 1) * _PageSize).Take(_PageSize).ToList();
            }
            else
            {
                if (sortObj != null)
                {
                    List<SortModel> sortList = JsonHelper.JsonToObject<List<SortModel>>(JsonHelper.ObjectToJson(sortObj));
                    if (sortList != null && sortList.Count > 0)
                    {
                        var listResult = JsonHelper.JsonToObject<List<T>>(JsonHelper.ObjectToJson(thisList));
                        foreach (SortModel sort in sortList)
                        {
                            var prop = typeof(T).GetProperty(sort.SortParam.GetCString(), BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
                            object keySelector(T enty) => prop.GetValue(enty, null);
                            listResult = sort.Ascending ? listResult.OrderBy(keySelector).ToList() : listResult.OrderByDescending(keySelector).ToList();
                        }
                        return listResult;
                    }
                }

                return thisList;
            }
        }

        /// <summary>
        /// List分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="thisList">数据源</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="sortObj">排序字段:[{sortParam: "fieldName", ascending: true}]</param>
        /// <returns></returns>
        public static object GetPageModel<T>(this List<T> thisList, int pageIndex = 1, int pageSize = 20, object sortObj = null)
        {
            int _PageIndex = pageIndex == 0 ? 1 : pageIndex;
            int _PageSize = pageSize == 0 ? 20 : pageSize;
            int _PageConut = (int)Math.Ceiling(Convert.ToDecimal(thisList.Count) / _PageSize);

            if (_PageConut >= _PageIndex)
            {
                if (sortObj != null)
                {
                    List<SortModel> sortList = JsonHelper.JsonToObject<List<SortModel>>(JsonHelper.ObjectToJson(sortObj));
                    if (sortList != null && sortList.Count > 0)
                    {
                        var listResult = JsonHelper.JsonToObject<List<T>>(JsonHelper.ObjectToJson(thisList));
                        foreach (SortModel sort in sortList)
                        {
                            var prop = typeof(T).GetProperty(sort.SortParam.GetCString(), BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
                            object keySelector(T enty) => prop.GetValue(enty, null);
                            listResult = sort.Ascending ? listResult.OrderBy(keySelector).ToList() : listResult.OrderByDescending(keySelector).ToList();
                        }
                        return listResult.Skip((_PageIndex - 1) * _PageSize).Take(_PageSize).ToList();
                    }
                }

                return thisList.Skip((_PageIndex - 1) * _PageSize).Take(_PageSize).ToList();
            }
            else
            {
                if (sortObj != null)
                {
                    List<SortModel> sortList = JsonHelper.JsonToObject<List<SortModel>>(JsonHelper.ObjectToJson(sortObj));
                    if (sortList != null && sortList.Count > 0)
                    {
                        var listResult = JsonHelper.JsonToObject<List<T>>(JsonHelper.ObjectToJson(thisList));
                        foreach (SortModel sort in sortList)
                        {
                            var prop = typeof(T).GetProperty(sort.SortParam.GetCString(), BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
                            object keySelector(T enty) => prop.GetValue(enty, null);
                            listResult = sort.Ascending ? listResult.OrderBy(keySelector).ToList() : listResult.OrderByDescending(keySelector).ToList();
                        }
                        return listResult;
                    }
                }

                return thisList;
            }
        }

        /// <summary>
        /// List获取分页数据、列筛选数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="thisList">数据源</param>
        /// <param name="selectFileds">列筛选字段</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="sortObj">排序字段:[{sortParam: "fieldName", ascending: true}]</param>
        /// <returns></returns>
        public static object GetPageWithSelect<T>(this List<T> thisList, List<string> selectFileds = null, int pageIndex = 1, int pageSize = 20, object sortObj = null)
        {
            if (thisList == null)
                return thisList;

            int _PageIndex = pageIndex == 0 ? 1 : pageIndex;
            int _PageSize = pageSize == 0 ? 20 : pageSize;
            int _PageConut = (int)Math.Ceiling(Convert.ToDecimal(thisList.Count) / _PageSize);
            PageModel<T> data = new PageModel<T>()
            {
                PageIndex = _PageIndex,
                PageSize = _PageSize,
                PageCount = _PageConut
            };
            List<SelectDataModel> selectData = new List<SelectDataModel>();

            #region 获取分页数据
            var listResult = JsonHelper.JsonToObject<List<T>>(JsonHelper.ObjectToJson(thisList));
            if (_PageConut >= _PageIndex)
            {
                if (sortObj != null)
                {
                    List<SortModel> sortList = JsonHelper.JsonToObject<List<SortModel>>(JsonHelper.ObjectToJson(sortObj));
                    if (sortList != null && sortList.Count > 0)
                    {
                        foreach (SortModel sort in sortList)
                        {
                            var prop = typeof(T).GetProperty(sort.SortParam.GetCString(), BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
                            object keySelector(T enty) => prop.GetValue(enty, null);
                            listResult = sort.Ascending ? listResult.OrderBy(keySelector).ToList() : listResult.OrderByDescending(keySelector).ToList();
                        }
                        data.List = listResult.Skip((_PageIndex - 1) * _PageSize).Take(_PageSize).ToList();
                    }
                }

                data.List = listResult.Skip((_PageIndex - 1) * _PageSize).Take(_PageSize).ToList();
            }
            else
            {
                if (sortObj != null)
                {
                    List<SortModel> sortList = JsonHelper.JsonToObject<List<SortModel>>(JsonHelper.ObjectToJson(sortObj));
                    if (sortList != null && sortList.Count > 0)
                    {
                        foreach (SortModel sort in sortList)
                        {
                            var prop = typeof(T).GetProperty(sort.SortParam.GetCString(), BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
                            object keySelector(T enty) => prop.GetValue(enty, null);
                            listResult = sort.Ascending ? listResult.OrderBy(keySelector).ToList() : listResult.OrderByDescending(keySelector).ToList();
                        }
                        data.List = listResult;
                    }
                }

                data.List = listResult;
            }
            data.Count = thisList.Count;
            #endregion

            #region 获取列筛选数据
            foreach(var field in selectFileds.GetNotNull())
            {
                var prop = typeof(T).GetProperty(field.GetCString(), BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
                var fieldDataList = new List<object>();
                if (prop.PropertyType.Name.ToLower() == "string")
                {
                    string keySelector(T enty) => prop.GetValue(enty, null).GetCString();
                    fieldDataList = JsonHelper.JsonToObject<List<object>>(JsonHelper.ObjectToJson(thisList.GroupBy(keySelector, StringComparer.InvariantCultureIgnoreCase).Select(g => g.Key).ToList()));
                }
                else
                {
                    object keySelector(T enty) => prop.GetValue(enty, null);
                    fieldDataList = thisList.GroupBy(keySelector).Select(g => g.Key).ToList();
                }
                    
                selectData.Add(new SelectDataModel
                {
                    Name = field,
                    List = fieldDataList
                });
            }
            #endregion

            return new ListPageModel<T> { Data = data, SelectData = selectData };
        }

        /// <summary>
        /// List获取列分页数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="thisList">数据源</param>
        /// <param name="selectFileds">列筛选字段</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页大小</param>
        /// <returns></returns>
        public static object GetSelectPage<T>(this List<T> thisList, List<string> selectFileds = null, int pageIndex = 1, int pageSize = 20)
        {
            if (thisList == null)
                return thisList;

            int _PageIndex = pageIndex == 0 ? 1 : pageIndex;
            int _PageSize = pageSize == 0 ? 20 : pageSize;

            List<SelectDataPageModel> selectData = new List<SelectDataPageModel>();
            foreach (var field in selectFileds.GetNotNull())
            {
                #region 获取列筛选数据
                var fieldDataList = new List<object>();
                var prop = typeof(T).GetProperty(field.GetCString(), BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
                if (prop is not null)
                    if (prop.PropertyType.Name.ToLower() == "string")
                    {
                        string keySelector(T enty) => prop.GetValue(enty, null).GetCString();
                        fieldDataList = JsonHelper.JsonToObject<List<object>>(JsonHelper.ObjectToJson(thisList.GroupBy(keySelector, StringComparer.InvariantCultureIgnoreCase).Select(g => g.Key).ToList()));
                    }
                    else
                    {
                        object keySelector(T enty) => prop.GetValue(enty, null);
                        fieldDataList = thisList.GroupBy(keySelector).Select(g => g.Key).ToList();
                    }
                #endregion

                #region 获取分页
                int _PageConut = (int)Math.Ceiling(Convert.ToDecimal(fieldDataList.Count) / _PageSize);
                PageModel<object> data = new PageModel<object>()
                {
                    PageIndex = _PageIndex,
                    PageSize = _PageSize,
                    PageCount = _PageConut
                };
                //var listResult = JsonHelper.JsonToObject<List<object>>(JsonHelper.ObjectToJson(fieldDataList));
                if (_PageConut >= _PageIndex)
                {
                    data.List = fieldDataList.Skip((_PageIndex - 1) * _PageSize).Take(_PageSize).ToList();
                }
                else
                {
                    data.List = fieldDataList;
                }
                data.Count = fieldDataList.Count;
                #endregion

                selectData.Add(new SelectDataPageModel
                {
                    Name = field,
                    Data = data
                });
            }

            return selectData;
        }
    }

    partial class ListPageModel<T>
    {
        public PageModel<T> Data { get; set; }

        public List<SelectDataModel> SelectData { get; set; }
    }

    partial class SelectDataModel
    {
        public string Name { get; set; }

        public List<object> List { get; set; }
    }

    partial class SelectDataPageModel
    {
        public string Name { get; set; }

        public PageModel<object> Data { get; set; }
    }



    /// <summary>
    /// 通用分页信息类
    /// </summary>
    partial class PageModel<T>
    {
        /// <summary>
        /// 返回数据
        /// </summary>
        public object List { get; set; }
        /// <summary>
        /// 数据总数
        /// </summary>
        public int Count { get; set; } = 0;
        /// <summary>
        /// 当前页标
        /// </summary>
        public int PageIndex { get; set; } = 1;
        /// <summary>
        /// 每页大小
        /// </summary>
        public int PageSize { set; get; }
        /// <summary>
        /// 总页数
        /// </summary>
        public int PageCount { get; set; } = 100;

    }
}

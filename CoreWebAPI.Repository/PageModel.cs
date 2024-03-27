using System.Collections.Generic;

namespace CoreWebAPI.Repository
{
    /// <summary>
    /// 通用分页信息类
    /// </summary>
    public class PageModel<T>
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

    /// <summary>
    /// 通用分页信息类
    /// </summary>
    public class PageModelReturn<T>
    {
        /// <summary>
        /// 返回数据
        /// </summary>
        public List<T> List { get; set; }
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

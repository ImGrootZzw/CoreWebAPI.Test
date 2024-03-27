namespace CoreWebAPINuGet.Test.Models.ViewModels
{
    /// <summary>
    /// 表格头信息类
    /// </summary>
    public class TableHeaderModel
    {
        /// <summary>
        /// 描述 : 标签
        /// 空值 : False
        /// 默认 : 
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// 描述 : 字段
        /// 空值 : False
        /// 默认 : 
        /// </summary>
        public string Property { get; set; }

        /// <summary>
        /// 描述 : 是否显示
        /// 空值 : False
        /// 默认 : 
        /// </summary>
        public bool IsShow { get; set; }

        /// <summary>
        /// 列宽
        /// </summary>
        public string Width { set; get; }

        /// <summary>
        /// 字段类型
        /// </summary>
        public string Type { set; get; }

    }
}

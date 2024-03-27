namespace CoreWebAPINuGet.Test.Models
{
    /// <summary>
    /// 通用返回信息类
    /// </summary>
    public class ResultModel
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public string ResultCode { get; set; } = "100000";
        /// <summary>
        /// 返回信息
        /// </summary>
        public string ResultMsg { get; set; } = "执行成功";
        /// <summary>
        /// 返回数据集合
        /// </summary>
        public object Data { get; set; }
        /// <summary>
        /// 方法名
        /// </summary>
        public string MethodName { get; set; }
    }
    /// <summary>
    /// 通用返回信息类
    /// </summary>
    public class ResultModelReturn<T>
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public string ResultCode { get; set; } = "100000";
        /// <summary>
        /// 返回信息
        /// </summary>
        public string ResultMsg { get; set; } = "执行成功";
        /// <summary>
        /// 返回数据集合
        /// </summary>
        public T Data { get; set; }
        /// <summary>
        /// 方法名
        /// </summary>
        public string MethodName { get; set; }
    }
}

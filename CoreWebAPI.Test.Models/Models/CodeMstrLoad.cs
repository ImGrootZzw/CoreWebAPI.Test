namespace CoreWebAPI.Test.Models.Models
{
    /// <summary>
    ///
    /// </summary>
    public class CodeMstrLoad : CodeMstr
    {
        /// <summary>
        /// 执行结果
        /// </summary>
        public bool Canpass { get; set; } = true;

        /// <summary>
        /// 错误信息
        /// </summary>
        public string Errormessage { get; set; } = "导入成功";

    }
}
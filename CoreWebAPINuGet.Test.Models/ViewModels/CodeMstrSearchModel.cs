using System;
using System.Linq;
using System.Text;
using SqlSugar;


namespace CoreWebAPINuGet.Test.Models.ViewModels
{
    ///<summary>
    ///
    ///</summary>
    public class CodeMstrSearchModel: BasicSearchModel
    {
        /// <summary>
        /// 描述 : 名称 
        /// 空值 : False
        /// 默认 : 
        /// </summary>        
        public string CodeName { get; set; }

        /// <summary>
        /// 描述 : 值 
        /// 空值 : False
        /// 默认 : 
        /// </summary>        
        public string CodeValue { get; set; }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlSugar;


namespace CoreWebAPI.Test.Models.Models
{
    ///<summary>
    ///
    ///</summary>
    [Tenant("mydb_pgsql")] //必须小写
    [SugarTable("code_mstr", tableDescription: "MYDB_PGSQL")]
    public class CodeMstr
    {
        /// <summary>
        /// 描述 : ID 
        /// 空值 : False
        /// 默认 : 
        /// </summary>        
        [SugarColumn(IsPrimaryKey = true, IsNullable = false, ColumnName = "code_id")]
        public string CodeId { get; set; }

        /// <summary>
        /// 描述 : 公司ID 
        /// 空值 : False
        /// 默认 : 
        /// </summary>        
        [SugarColumn(IsNullable = false, ColumnName = "code_corp_id")]
        public string CodeCorpId { get; set; }

        /// <summary>
        /// 描述 : 域ID 
        /// 空值 : False
        /// 默认 : 
        /// </summary>        
        [SugarColumn(IsNullable = false, ColumnName = "code_domain_id")]
        public string CodeDomainId { get; set; }

        /// <summary>
        /// 描述 : 名称 
        /// 空值 : False
        /// 默认 : 
        /// </summary>        
        [SugarColumn(IsNullable = false, ColumnName = "code_name", IndexGroupNameList = new string[] { "index_search" })]
        public string CodeName { get; set; }

        /// <summary>
        /// 描述 : 值 
        /// 空值 : False
        /// 默认 : 
        /// </summary>        
        [SugarColumn(IsNullable = false, ColumnName = "code_value", IndexGroupNameList = new string[] { "index_search" })]
        public string CodeValue { get; set; }

        /// <summary>
        /// 描述 : 描述 
        /// 空值 : True
        /// 默认 : 
        /// </summary>        
        [SugarColumn(ColumnName = "code_desc")]
        public string CodeDesc { get; set; }

        /// <summary>
        /// 描述 : 激活 
        /// 空值 : True
        /// 默认 : 
        /// </summary>        
        [SugarColumn(ColumnName = "code_active")]
        public bool? CodeActive { get; set; }

        /// <summary>
        /// 描述 : 创建日期 
        /// 空值 : True
        /// 默认 : 
        /// </summary>        
        [SugarColumn(ColumnName = "code_crt_datetime", IsOnlyIgnoreUpdate = true)]
        public DateTime? CodeCrtDatetime { get; set; }

        /// <summary>
        /// 描述 : 程序名 
        /// 空值 : True
        /// 默认 : 
        /// </summary>        
        [SugarColumn(ColumnName = "code_crt_prog", IsOnlyIgnoreUpdate = true)]
        public string CodeCrtProg { get; set; }

        /// <summary>
        /// 描述 : 创建用户 
        /// 空值 : True
        /// 默认 : 
        /// </summary>        
        [SugarColumn(ColumnName = "code_crt_user", IsOnlyIgnoreUpdate = true)]
        public string CodeCrtUser { get; set; }

        /// <summary>
        /// 描述 : 修改日期 
        /// 空值 : True
        /// 默认 : 
        /// </summary>        
        [SugarColumn(ColumnName = "code_mod_datetime", IsOnlyIgnoreInsert = true)]
        public DateTime? CodeModDatetime { get; set; }

        /// <summary>
        /// 描述 : 修改程序名 
        /// 空值 : True
        /// 默认 : 
        /// </summary>        
        [SugarColumn(ColumnName = "code_mod_prog", IsOnlyIgnoreInsert = true)]
        public string CodeModProg { get; set; }

        /// <summary>
        /// 描述 : 修改用户 
        /// 空值 : True
        /// 默认 : 
        /// </summary>        
        [SugarColumn(ColumnName = "code_mod_user", IsOnlyIgnoreInsert = true)]
        public string CodeModUser { get; set; }

        ///// <summary>
        ///// 
        ///// </summary>
        //[SugarColumn(IsIgnore = true)]
        //public string CodeFirstId { get { return CodeName + CodeModUser; } }

        /// <summary>
        /// 
        /// </summary>
        //[Navigate(NavigateType.Dynamic, null)]
        [Navigate(NavigateType.OneToMany, nameof(CodeMstrSalve.CodeSalveName), nameof(CodeName))]
        public List<CodeMstrSalve> CodeSalve { get; set; }
    }
}
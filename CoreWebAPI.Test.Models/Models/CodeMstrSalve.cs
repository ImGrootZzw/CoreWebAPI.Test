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
    //[Tenant("mydb_mysql")]
    //[SugarTable("code_mstr1", tableDescription: "mydb_mysql")]
    [Tenant("mydb_pgsqlnew")]
    [SugarTable("codesalve_mstr", tableDescription: "mydb_pgsqlnew")]
    public class CodeMstrSalve
    {
        /// <summary>
        /// 描述 : ID 
        /// 空值 : False
        /// 默认 : 
        /// </summary>        
        [SugarColumn(IsPrimaryKey = true, IsNullable = false, ColumnName = "codesalve_id")]
        public string CodeSalveId { get; set; }

        /// <summary>
        /// 描述 : 公司ID 
        /// 空值 : False
        /// 默认 : 
        /// </summary>        
        [SugarColumn(IsNullable = false, ColumnName = "codesalve_corp_id")]
        public string CodeSalveCorpId { get; set; }

        /// <summary>
        /// 描述 : 域ID 
        /// 空值 : False
        /// 默认 : 
        /// </summary>        
        [SugarColumn(IsNullable = false, ColumnName = "codesalve_domain_id")]
        public string CodeSalveDomainId { get; set; }

        /// <summary>
        /// 描述 : 名称 
        /// 空值 : False
        /// 默认 : 
        /// </summary>        
        [SugarColumn(IsNullable = false, ColumnName = "codesalve_name", IndexGroupNameList = new string[] { "index_search" })]
        public string CodeSalveName { get; set; }

        /// <summary>
        /// 描述 : 值 
        /// 空值 : False
        /// 默认 : 
        /// </summary>        
        [SugarColumn(IsNullable = false, ColumnName = "codesalve_value", IndexGroupNameList = new string[] { "index_search" })]
        public string CodeSalveValue { get; set; }

        /// <summary>
        /// 描述 : 描述 
        /// 空值 : True
        /// 默认 : 
        /// </summary>        
        [SugarColumn(ColumnName = "codesalve_desc")]
        public string CodeSalveDesc { get; set; }

        /// <summary>
        /// 描述 : 激活 
        /// 空值 : True
        /// 默认 : 
        /// </summary>        
        [SugarColumn(ColumnName = "codesalve_active")]
        public bool? CodeSalveActive { get; set; }

        /// <summary>
        /// 描述 : 创建日期 
        /// 空值 : True
        /// 默认 : 
        /// </summary>        
        [SugarColumn(ColumnName = "codesalve_crt_datetime", IsOnlyIgnoreUpdate = true)]
        public DateTime? CodeSalveCrtDatetime { get; set; }

        /// <summary>
        /// 描述 : 程序名 
        /// 空值 : True
        /// 默认 : 
        /// </summary>        
        [SugarColumn(ColumnName = "codesalve_crt_prog", IsOnlyIgnoreUpdate = true)]
        public string CodeSalveCrtProg { get; set; }

        /// <summary>
        /// 描述 : 创建用户 
        /// 空值 : True
        /// 默认 : 
        /// </summary>        
        [SugarColumn(ColumnName = "codesalve_crt_user", IsOnlyIgnoreUpdate = true)]
        public string CodeSalveCrtUser { get; set; }

        /// <summary>
        /// 描述 : 修改日期 
        /// 空值 : True
        /// 默认 : 
        /// </summary>        
        [SugarColumn(ColumnName = "codesalve_mod_datetime", IsOnlyIgnoreInsert = true)]
        public DateTime? CodeSalveModDatetime { get; set; }

        /// <summary>
        /// 描述 : 修改程序名 
        /// 空值 : True
        /// 默认 : 
        /// </summary>        
        [SugarColumn(ColumnName = "codesalve_mod_prog", IsOnlyIgnoreInsert = true)]
        public string CodeSalveModProg { get; set; }

        /// <summary>
        /// 描述 : 修改用户 
        /// 空值 : True
        /// 默认 : 
        /// </summary>        
        [SugarColumn(ColumnName = "codesalve_mod_user", IsOnlyIgnoreInsert = true)]
        public string CodeSalveModUser { get; set; }

        ///// <summary>
        ///// 
        ///// </summary>
        //[SugarColumn(IsIgnore = true)]
        //public string CodeForeignKey { get { return CodeSalveName + CodeSalveValue; } }
    }
}
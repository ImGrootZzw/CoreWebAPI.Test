using CoreWebAPI.Common.Helper;
using CoreWebAPI.Common.LogHelper;
using CoreWebAPI.Common.Redis;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreWebAPI.Test.WebAPI.Controllers.Master
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DemoController : ControllerBase
    {
        /// <summary>
        /// 跨库联查
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Query(int pageIndex)
        {
            try
            {
                ICacheService myCache = new SqlSugarRedisCache();

                var db = new SqlSugarScope(new List<ConnectionConfig>()
                        {
                            new ConnectionConfig(){
                                ConfigId = "mydb_pgsql",
                                DbType = DbType.PostgreSQL,
                                ConnectionString = "PORT=5432;DATABASE=postgres;HOST=localhost;USER ID=postgres;PASSWORD=5899363*",
                                IsAutoCloseConnection = true,
                                ConfigureExternalServices = new ConfigureExternalServices()
                                {
                                    DataInfoCacheService = Appsettings.App("Startup", "SqlSugar", "RedisEnabled").GetCBool() ? myCache : null,
                                },
                            },
                            new ConnectionConfig(){
                                ConfigId = "mydb_pgsqlnew",
                                DbType = DbType.PostgreSQL,
                                ConnectionString = "PORT=5432;DATABASE=tesla;HOST=localhost;USER ID=postgres;PASSWORD=5899363*",
                                IsAutoCloseConnection = true
                            }
                        });

                db.Aop.OnLogExecuting = (sql, p) =>
                {

                };

                RefAsync<int> totalCount = 0;
                RefAsync<int> pageCount = 0;
                db.InitMappingInfo<CodeMstrSalve>();
                var data = await db.Queryable<CodeMstr>().Includes(x => x.CodeSalve
                    //.Where(z => z.CodeSalveValue == "value1000")
                    .ToList()
                )
                .OrderBy(x => x.CodeId)
                .Where(x => x.CodeDomainId == "1" && x.CodeSalve.Any(z => SqlFunc.GreaterThan(z.CodeSalveName, "name10000")))
                .WithCache()
                .ToPageListAsync(pageIndex, 100, totalCount, pageCount);
                return Ok(data);
            }
            catch (Exception ex)
            {
                SerilogHelper.WriteErrorLog("11", "22", ex);
                return Ok(ex);
            }
        }
    }

    ///<summary>
    /// 主表
    ///</summary>
    [Tenant("mydb_pgsql")]
    [SugarTable("code_mstr")]
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

    ///<summary>
    /// 子表
    ///</summary>
    [Tenant("mydb_pgsql")]
    [SugarTable("foreign_table_code")]
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

    }
}

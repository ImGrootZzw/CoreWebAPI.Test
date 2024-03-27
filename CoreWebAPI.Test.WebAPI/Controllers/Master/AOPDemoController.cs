using CoreWebAPI.Common.Helper;
using CoreWebAPI.Common.Redis;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreWebAPI.Test.WebAPI.Controllers.Master.AOPDemo
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AOPDemoController : ControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AOPDemo()
        {
            try
            {
                var UninTimeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                var UninTimeStamp1 = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                List<OutMenu> outMenus = GetOutMenus(new List<Menu>(), "X");

                List<OutMenu> GetOutMenus(List<Menu> menus, string code)
                {
                    List<OutMenu> outMenus = new List<OutMenu>();
                    OutMenu outMenu = new OutMenu();
                    var CList = menus.FindAll(m => m.menu_nbr.ToString() == code);
                    //遍历数据  
                    foreach (Menu item in CList)
                    {
                        outMenu = new OutMenu();
                        //依据需求针对性判断  
                        outMenu.MenuId = item.menu_nbr != "X" ? item.menu_nbr + "." + item.menu_select : item.menu_select;
                        outMenu.MenuName = item.menu_label;
                        //递归  
                        outMenu.OutMenus = GetOutMenus(menus, outMenu.MenuId);
                        outMenus.Add(outMenu);
                    }
                    return outMenus;
                }

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
                            }
                        });

                db.Aop.OnDiffLogEvent = (DiffLogModel) =>
                {
                    if(DiffLogModel.DiffType == DiffType.insert)
                    {
                        Console.WriteLine("-----------Insert After----------");
                        foreach (var data in DiffLogModel.AfterData)
                        {
                            foreach(var col in data.Columns)
                            {
                                Console.WriteLine(col.ColumnName + "-" + col.ColumnDescription);
                            }
                        }
                    }
                    else if (DiffLogModel.DiffType == DiffType.update)
                    {
                        Console.WriteLine("-----------Update After----------");
                        foreach (var data in DiffLogModel.AfterData)
                        {
                            foreach (var col in data.Columns)
                            {
                                Console.WriteLine(col.ColumnName + "-" + col.ColumnDescription);
                            }

                        }
                    }
                };

                var codeNew = new CodeMstr()
                {
                    CodeId = Guid.NewGuid().ToString(),
                    CodeName = "test",
                    CodeValue = "test"
                };
                var insertRt = await db.Insertable(codeNew).EnableDiffLogEvent().ExecuteCommandAsync();

                codeNew.CodeValue = "demo";
                var updateRt = await db.Updateable(codeNew).EnableDiffLogEvent().ExecuteCommandAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return Ok(ex);
            }
        }
    }

    /// <summary>  
    /// 菜单输出表  
    /// </summary>  
    public class OutMenu
    {
        public string MenuId { get; set; }
        public string MenuName { get; set; }
        public List<OutMenu> OutMenus { get; set; }
    }

    /// <summary>  
    /// 菜单表  
    /// </summary>  
    public class Menu
    {
        //父级菜单编号  
        public string menu_nbr { get; set; }
        //当前菜单序号  
        public string menu_select { get; set; }
        //菜单标签  
        public string menu_label { get; set; }
    }

    ///<summary>
    /// 主表
    ///</summary>
    [SugarTable("code_mstr")]
    public class CodeMstr
    {
        /// <summary>
        /// 描述 : ID 
        /// </summary>        
        [SugarColumn(IsPrimaryKey = true, IsNullable = false, ColumnName = "code_id")]
        public string CodeId { get; set; }

        /// <summary>
        /// 描述 : 公司ID 
        /// </summary>        
        [SugarColumn(IsNullable = false, ColumnName = "code_corp_id")]
        public string CodeCorpId { get; set; }

        /// <summary>
        /// 描述 : 域ID 
        /// </summary>        
        [SugarColumn(IsNullable = false, ColumnName = "code_domain_id")]
        public string CodeDomainId { get; set; }

        /// <summary>
        /// 描述 : 名称 
        /// </summary>        
        [SugarColumn(IsNullable = false, ColumnName = "code_name", IndexGroupNameList = new string[] { "index_search" })]
        public string CodeName { get; set; }

        /// <summary>
        /// 描述 : 值 
        /// </summary>        
        [SugarColumn(IsNullable = false, ColumnName = "code_value", IndexGroupNameList = new string[] { "index_search" })]
        public string CodeValue { get; set; }

        /// <summary>
        /// 描述 : 描述 
        /// </summary>        
        [SugarColumn(ColumnName = "code_desc")]
        public string CodeDesc { get; set; }

        /// <summary>
        /// 描述 : 激活 
        /// </summary>        
        [SugarColumn(ColumnName = "code_active")]
        public bool? CodeActive { get; set; }

        /// <summary>
        /// 描述 : 创建日期 
        /// </summary>        
        [SugarColumn(ColumnName = "code_crt_datetime", IsOnlyIgnoreUpdate = true)]
        public DateTime? CodeCrtDatetime { get; set; }

        /// <summary>
        /// 描述 : 程序名 
        /// </summary>        
        [SugarColumn(ColumnName = "code_crt_prog", IsOnlyIgnoreUpdate = true)]
        public string CodeCrtProg { get; set; }

        /// <summary>
        /// 描述 : 创建用户 
        /// </summary>        
        [SugarColumn(ColumnName = "code_crt_user", IsOnlyIgnoreUpdate = true)]
        public string CodeCrtUser { get; set; }

        /// <summary>
        /// 描述 : 修改日期 
        /// </summary>        
        [SugarColumn(ColumnName = "code_mod_datetime", IsOnlyIgnoreInsert = true)]
        public DateTime? CodeModDatetime { get; set; }

        /// <summary>
        /// 描述 : 修改程序名 
        /// </summary>        
        [SugarColumn(ColumnName = "code_mod_prog", IsOnlyIgnoreInsert = true)]
        public string CodeModProg { get; set; }

        /// <summary>
        /// 描述 : 修改用户 
        /// </summary>        
        [SugarColumn(ColumnName = "code_mod_user", IsOnlyIgnoreInsert = true)]
        public string CodeModUser { get; set; }

    }

}

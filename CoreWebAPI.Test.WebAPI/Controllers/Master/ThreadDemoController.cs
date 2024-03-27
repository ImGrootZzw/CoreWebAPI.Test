using CoreWebAPI.Common.Helper;
using CoreWebAPI.Common.LogHelper;
using CoreWebAPI.Common.Redis;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.POIFS.Crypt.Dsig;
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
    public class ThreadDemoController : ControllerBase
    {
        private readonly ISqlSugarClient _sqlSugarClient;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlSugarClient"></param>
        public ThreadDemoController(ISqlSugarClient sqlSugarClient)
        {
            _sqlSugarClient = sqlSugarClient;
        }
        /// <summary>
        /// 跨库联查
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Query(int pageIndex)
        {
            try
            {
                var db = new SqlSugarScope(new List<ConnectionConfig>()
                        {
                            new ConnectionConfig(){
                                ConfigId = "mydb_pgsql",
                                DbType = DbType.PostgreSQL,
                                ConnectionString = "PORT=5432;DATABASE=postgres;HOST=localhost;USER ID=postgres;PASSWORD=5899363*",
                                IsAutoCloseConnection = true
                            }
                        });
                var db1 = new SqlSugarScope(new List<ConnectionConfig>()
                        {
                            new ConnectionConfig(){
                                ConfigId = "mydb_pgsql",
                                DbType = DbType.PostgreSQL,
                                ConnectionString = "PORT=5432;DATABASE=postgres;HOST=localhost;USER ID=postgres;PASSWORD=5899363*",
                                IsAutoCloseConnection = true
                            }
                        });
                _sqlSugarClient.Open();
                Task<CodeMstr> taskGetCode = _sqlSugarClient.Queryable<CodeMstr>().FirstAsync();
                Task<List<CodeMstr>> taskGetCode1 = _sqlSugarClient.Queryable<CodeMstr>().ToListAsync();
                await Task.WhenAll(taskGetCode, taskGetCode1);
                _sqlSugarClient.Close();

                var data = taskGetCode.Result;
                var data2 = taskGetCode1.Result;
                return Ok(data);
            }
            catch (Exception ex)
            {
                SerilogHelper.WriteErrorLog("11", "22", ex);
                return Ok(ex);
            }
        }
    }
}

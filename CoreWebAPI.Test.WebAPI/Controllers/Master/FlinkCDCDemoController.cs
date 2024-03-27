using CoreWebAPI.Common.Helper;
using CoreWebAPI.Common.Redis;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Flink.DotNet.Client;
using Flink.DotNet.Cdc.Sources;
using Flink.DotNet.Cdc.TypeConverters;
using Flink.DotNet.Cdc.TypeConverters.Row;
using Flink.DotNet.Cdc.TypeConverters.RowList;
using Flink.DotNet.Cdc.Types;
using Flink.DotNet.Cdc.Values.Row;
using Flink.DotNet.Cdc.Values.RowList;
using Flink.DotNet.Cdc.Values.Time;
using Flink.DotNet.Cdc.Values.Value;
using Flink.DotNet.Examples.Utils;
using NPOI.Util.Collections;

namespace CoreWebAPI.Test.WebAPI.Controllers.Master
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FlinkCDCDemoController : ControllerBase
    {
        /// <summary>
        /// 跨库联查
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Query(int pageIndex)
        {
            try
            {
                var jobName = "cdc-example";
                var parallelism = 1;
                var environment = FlinkEnvironment.CreateRemoteEnvironment("localhost", 8081);

                // 配置 CDC 连接信息
                var properties = new Properties();
                properties.Set("connector.properties.connection.url", "jdbc:mysql://localhost:3306/testdb");
                properties.Set("connector.properties.connection.user", "root");
                properties.Set("connector.properties.connection.password", "123456");

                // 创建 CDC source
                var source = new MySqlCdcRowListSource()
                    .SetDatabaseList("testdb") // 监控的数据库名
                    .SetTableList("test_table") // 监控的表名
                    .SetProperties(properties)
                    .Build();

                // 处理 CDC 数据流
                var result = source
                    .Filter(r => r.RowKind == RowKind.Insert) // 只处理插入操作
                    .Map(r =>
                    {
                        var name = r.GetString(0);
                        var age = r.GetInt(1);
                        return Tuple.Create(name, age);
                    });

                // 输出结果
                result.Print();

                // 提交 Flink 任务
                environment.Submit(jobName, parallelism, result);

                Console.WriteLine("Job submitted!");

                return Ok(data);
            }
            catch (Exception ex)
            {
                return Ok(ex);
            }
        }
    }

}

using CoreWebAPI.Common.Helper;
using CoreWebAPI.Common.HttpContext;
using CoreWebAPI.Common.HttpContextUser;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;
using NpgsqlTypes;
using NPOI.HPSF;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Sinks.PostgreSQL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Security.Policy;
using System.Threading.Tasks;

namespace CoreWebAPI.Common.LogHelper
{
    /// <summary>
    /// 日志输出类
    /// </summary>
    public class SerilogHelper
    {
        /// <summary>
        /// 记录日常日志
        /// </summary>
        /// <param name="filename">日志文件名，前缀为</param>
        /// <param name="methodName">方法名</param>
        /// <param name="dataParas">内容参数</param>
        public static void WriteLog(string filename, string methodName, params object[] dataParas)
        {
            try
            {
                Parallel.For(0, 1, e =>
                {
                    if (!Appsettings.App("Serilog", "Info", "Enabled").GetCBool())
                        return;
                    // 过滤用户
                    if (!Appsettings.App("Serilog", "Info", "AllowAllUsers").GetCBool()
                        && !Appsettings.App<string>("Serilog", "Info", "AllowUsers").Contains(StaticHttpContext.User)
                        && Appsettings.App<string>("Serilog", "Info", "IgnoreUsers").Contains(StaticHttpContext.User))
                    {
                        return;
                    }
                    // 过滤接口
                    if (!Appsettings.App("Serilog", "Info", "AllowAllApis").GetCBool()
                        && !Appsettings.App<string>("Serilog", "Info", "AllowAPIs").Contains(StaticHttpContext.API)
                        && Appsettings.App<string>("Serilog", "Info", "IgnoreApis").Contains(StaticHttpContext.API))
                    {
                        return;
                    }

                    string filePath = Appsettings.App("Serilog", "Info", "Path").GetCString() == "" ? AppContext.BaseDirectory + "/log" : Appsettings.App("Serilog", "Info", "Path").GetCString();
                    if (filename.ToLower() != "recordaccesslogs")
                        filename = $"{StaticHttpContext.API.Trim('/').Replace("/", "_")}-{StaticHttpContext.User}-{filename}";

                    // mongodb
                    var mongoUrl = new MongoUrl("mongodb://10.124.0.116:27017/servicelog");
                    var mongoClient = new MongoClient(mongoUrl);
                    var mongoDatabase = mongoClient.GetDatabase("servicelog");

                    // postgresql
                    var connectionString = "User ID=postgres;Password=qadesun;Host=10.124.0.109;Port=5432;Database=logs;";

                    var columnOptions1 = new ColumnOptions();
                    var columnOptions = new Dictionary<string, ColumnWriterBase>()
                    {
                        {
                            "message",
                            new RenderedMessageColumnWriter()
                        },
                        {
                            "message_template",
                            new MessageTemplateColumnWriter()
                        },
                        {
                            "level",
                            new LevelColumnWriter()
                        },
                        {
                            "timestamp",
                            new TimestampColumnWriter()
                        },
                        {
                            "exception",
                            new ExceptionColumnWriter()
                        },
                        {
                            "log_event",
                            new LogEventSerializedColumnWriter()
                        },
                        {
                            "User",
                            new RenderedMessageColumnWriter()
                        },
                        {
                            "API",
                            new RenderedMessageColumnWriter()
                        }
                    };

                    Log.Logger = new LoggerConfiguration()
                        .MinimumLevel.Debug()
                        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                        .Enrich.FromLogContext()
                        .Enrich.WithProperty("User", StaticHttpContext.User)
                        .Enrich.WithProperty("API", StaticHttpContext.API)
                        .WriteTo.Async(a => a.File(Path.Combine(filePath, DateTime.Now.ToString("yyyyMMdd"), filename + ".log"), rollingInterval: RollingInterval.Infinite, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} || {Level} || {SourceContext:l} || {Message} || {Exception} ||end {NewLine}"))
                    //.WriteTo.MongoDB("mongodb://10.124.0.116:27107/servicelog",
                    //   collectionName: "Logs", Serilog.Events.LogEventLevel.Debug)
                    //.WriteTo.MongoDB(mongoDatabase, collectionName: "Logs")
                    //.WriteTo.PostgreSQL(connectionString, tableName: "Logs", needAutoCreateTable: true, columnOptions: columnOptions)
                    .CreateLogger();


                    string logContent = methodName;
                    foreach (string para in dataParas)
                    {
                        logContent += " --- " + para;
                    }

                    Log.Information(logContent);
                    Log.CloseAndFlush();
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        /// <summary>
        /// 记录异常日志
        /// </summary>
        /// <param name="filename">日志文件名</param>
        /// <param name="methodName">方法名</param>
        /// <param name="ex">异常错误</param>
        public static void WriteErrorLog(string filename, string methodName, Exception ex)
        {
            try
            {
                if (!Appsettings.App("Serilog", "Error", "Enabled").GetCBool())
                    return;
                // 过滤用户
                if (!Appsettings.App("Serilog", "Error", "AllowAllUsers").GetCBool()
                    && !Appsettings.App<string>("Serilog", "Error", "AllowUsers").Contains(StaticHttpContext.User)
                    && Appsettings.App<string>("Serilog", "Error", "IgnoreUsers").Contains(StaticHttpContext.User))
                {
                    return;
                }
                // 过滤接口
                if (!Appsettings.App("Serilog", "Error", "AllowAllApis").GetCBool()
                    && !Appsettings.App<string>("Serilog", "Error", "AllowAPIs").Contains(StaticHttpContext.API)
                    && Appsettings.App<string>("Serilog", "Error", "IgnoreApis").Contains(StaticHttpContext.API))
                {
                    return;
                }

                string filePath = Appsettings.App("Serilog", "Error", "Path").GetCString() == "" ? AppContext.BaseDirectory + "/log" : Appsettings.App("Serilog", "Error", "Path").GetCString();
                if (filename.ToLower() != "recordaccesslogs")
                    filename = $"{StaticHttpContext.API.Trim('/').Replace("/", "_")}-{StaticHttpContext.User}-{filename}";

                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                    .WriteTo.Async(a => a.File(Path.Combine(filePath, DateTime.Now.ToString("yyyyMMdd"), filename + ".log"), rollingInterval: RollingInterval.Infinite, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} || {Level} || {SourceContext:l} || {Message} || {Exception} ||end {NewLine}"))
                    .CreateLogger();

                Log.Error(ex, methodName + " --- " + ex.Message);
                Log.CloseAndFlush();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
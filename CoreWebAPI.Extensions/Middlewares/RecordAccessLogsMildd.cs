using CoreWebAPI.Common.HttpContextUser;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CoreWebAPI.Common.Helper;
using CoreWebAPI.Common.LogHelper;
using System.Web;
using Microsoft.Extensions.Logging;
using System.Threading;
using CoreWebAPI.Common.HttpContext;
using System.Reflection;
using System.Net.Http;
using CoreWebAPI.Common.DB.MongoDB;
using LogDashboard.Models;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;

namespace CoreWebAPI.Middlewares
{
    /// <summary>
    /// 中间件
    /// 记录用户方访问数据
    /// </summary>
    public class RecordAccessLogsMildd
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly RequestDelegate _next;
        private readonly IHttpContextCore _httpContextCore;
        private readonly ILogger<RecordAccessLogsMildd> _logger;
        private Stopwatch _stopwatch;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        public RecordAccessLogsMildd(RequestDelegate next, IHttpContextCore httpContextCore, ILogger<RecordAccessLogsMildd> logger)
        {
            _next = next;
            _httpContextCore = httpContextCore;
            _logger = logger;
            _stopwatch = new Stopwatch();
        }

        public async Task InvokeAsync(Microsoft.AspNetCore.Http.HttpContext context)
        {
            if (Appsettings.App("Middleware", "RecordAccessLogs", "Enabled").GetCBool())
            {
                var api = context.Request.Path.GetCString().TrimEnd('/').ToLower();
                var allowAllApis = Appsettings.App("Middleware", "RecordAccessLogs", "AllowAllApis").GetCBool();
                var allowApis = Appsettings.App<string>("Middleware", "RecordAccessLogs", "AllowApis");
                var ignoreApis = Appsettings.App<string>("Middleware", "RecordAccessLogs", "IgnoreApis");

                // 过滤，只有接口
                if (api.Contains("api") && (allowAllApis || (allowApis.Contains(api) && !ignoreApis.Contains(api))))
                {
                    _stopwatch.Restart();

                    HttpRequest request = context.Request;
                    var userAccessModel = new UserAccessModel
                    {
                        IP = _httpContextCore.IP,
                        User = _httpContextCore.User,
                        API = api,
                        RequestQuery = context.Request.QueryString.Value.GetCString(),
                        RequestHeader = JsonHelper.ObjectToJson(request.Headers),
                        BeginTime = DateTime.Now
                    };

                    // 获取请求body内容
                    if (request.Method.ToLower().Equals("post") || request.Method.ToLower().Equals("put") || request.Method.ToLower().Equals("delete"))
                    {
                        // 启用倒带功能，就可以让 Request.Body 可以再次读取
                        request.EnableBuffering();

                        Stream stream = request.Body;
                        byte[] buffer = new byte[request.ContentLength.Value];
                        await stream.ReadAsync(buffer, 0, buffer.Length);
                        userAccessModel.RequestData = Encoding.UTF8.GetString(buffer);

                        request.Body.Position = 0;
                    }
                    else if (request.Method.ToLower().Equals("get"))
                    {
                        userAccessModel.RequestData = HttpUtility.UrlDecode(request.QueryString.GetCString(), Encoding.UTF8);
                    }
                    //// 获取请求Body内容
                    //using (Stream stream = request.Body)
                    //{
                    //    // 启用倒带功能，就可以让 Request.Body 可以再次读取
                    //    request.EnableBuffering();

                    //    //Stream stream = request.Body;
                    //    if(request.ContentLength.GetCInt() > 0)
                    //    {
                    //        byte[] buffer = new byte[request.ContentLength.Value];
                    //        await stream.ReadAsync(buffer, 0, buffer.Length);
                    //        userAccessModel.RequestData = Encoding.UTF8.GetString(buffer);

                    //        request.Body.Position = 0;
                    //    }
                    //}
                    //else if (request.Method.ToLower().Equals("get"))
                    //{
                    //    userAccessModel.RequestData = HttpUtility.UrlDecode(request.QueryString.getCString(), Encoding.UTF8);
                    //}

                    // 获取响应Body内容
                    var originalBodyStream = context.Response.Body;
                    using (var responseBody = new MemoryStream())
                    {
                        context.Response.Body = responseBody;
                        try
                        {

                            await _next(context);
                        }
                        catch (Exception ex)
                        {
                            context.Response.StatusCode = 500;
                            _logger.LogError(ex.Message, ex);
                            SerilogHelper.WriteErrorLog("RecordAccessLogs", "RecordInfo", ex);
                        }
                        var responseBodyData = await GetResponse(context.Response);
                        userAccessModel.ResponseStatus = context.Response.StatusCode.GetCString();
                        userAccessModel.ResponseData = responseBodyData;
                        userAccessModel.ResponseHeader = JsonHelper.ObjectToJson(context.Response.Headers);

                        await responseBody.CopyToAsync(originalBodyStream);
                    }

                    // 响应完成记录时间和存入日志
                    context.Response.OnCompleted(() =>
                    {
                        _stopwatch.Stop();

                        userAccessModel.EndTime = DateTime.Now;
                        userAccessModel.OPTime = _stopwatch.ElapsedMilliseconds + "ms";

                        // 自定义log输出
                        var recordInfo = JsonConvert.SerializeObject(userAccessModel);
                        Parallel.For(0, 1, e =>
                        {
                            //_logger.LogInformation("RecordInfo", recordInfo);
                            SerilogHelper.WriteLog("RecordAccessLogs", "RecordInfo", JsonHelper.JsonFormat(recordInfo));
                        });

                        // 发送接口日志到消息队列
                        if (Appsettings.App<string>("Startup", "RequestLog", "RabbitMQ", "Enabled").GetCBool())
                        {
                            Task task3 = Task.Run(() =>
                            {
                                try
                                {
                                    var requestLogModel = new RequestLogModel
                                    {
                                        ip = userAccessModel.IP,
                                        module = Appsettings.App("ModuleName"),
                                        service = Appsettings.App("ServiceName"),
                                        userId = userAccessModel.User,
                                        userName = userAccessModel.User,
                                        requestUrl = userAccessModel.API,
                                        requestParam = userAccessModel.RequestQuery,
                                        requestType = request.Method,
                                        requestHeader = JsonHelper.ObjectToJson(request.Headers),
                                        requestBody = JsonHelper.ObjectToJson(request.Body),
                                        costTime = userAccessModel.OPTime,
                                        startDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                        createDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                                    };

                                    var hostNames = Appsettings.App<string>("Startup", "RabbitMQ", "HostNames");
                                    var userName = Appsettings.App("Startup", "RabbitMQ", "UserName");
                                    var password = Appsettings.App("Startup", "RabbitMQ", "Password");
                                    var port = Appsettings.App("Startup", "RabbitMQ", "Port").GetCInt();
                                    var virtualHost = Appsettings.App("Startup", "RabbitMQ", "VirtualHost");
                                    var exchangeName = Appsettings.App("Startup", "RabbitMQ", "ExchangeName");
                                    var queueName = Appsettings.App("Startup", "RabbitMQ", "QueueName");
                                    var routeKey = Appsettings.App("Startup", "RabbitMQ", "RouteKey");
                                    RabbitHelper rabbitHelper = new RabbitHelper(hostNames, userName, password, port, virtualHost, exchangeName, queueName, routeKey);
                                    rabbitHelper.SendMsg(JsonHelper.ObjectToJson(requestLogModel));
                                }
                                catch (Exception ex)
                                {
                                    SerilogHelper.WriteErrorLog("RecordAccessLogs", "发送接口请求日志到消息队列", ex);
                                }
                            });
                        }

                        // 发送接口日志到PgSql
                        if (Appsettings.App("Middleware", "RecordAccessLogs", "WriteToPgSql", "Enabled").GetCBool())
                        {
                            Task task3 = Task.Run(() =>
                            {
                                try
                                {
                                    var db = new SqlSugarScope(new List<ConnectionConfig>()
                                         {
                                             new ConnectionConfig(){
                                                ConfigId = "WriteLogToPgsql",
                                                DbType = DbType.PostgreSQL,
                                                ConnectionString =Appsettings.App("Middleware", "RecordAccessLogs", "WriteToPgsql", "Connection").GetCString(),
                                                IsAutoCloseConnection = true,
                                             }
                                         });
                                    try
                                    {
                                        var domain = context.Request.Headers["x-domain"].GetCString();
                                        var token = context.Request.Headers["x-token"].GetCString();

                                        var bl = new BlockLogsMstr();
                                        bl.Id = Guid.NewGuid().ToString();
                                        bl.BlockLogsSollutionName = Assembly.GetExecutingAssembly().FullName;
                                        bl.BlockLogsTimeStamp = DateTime.Now.ToString();
                                        bl.Domain = domain;
                                        bl.BlockLogsUser = userAccessModel.User;
                                        bl.BlockLogsSession = token;
                                        bl.BlockLogsIp = userAccessModel.IP;//请求ip;
                                        bl.BlockLogsMenu = userAccessModel.API;
                                        bl.BlockLogsApiName = userAccessModel.API;
                                        bl.BlockLogsCallStartTime = userAccessModel.BeginTime;
                                        bl.BlockLogsCallApiTime = userAccessModel.BeginTime;
                                        bl.BlockLogsApiTotalTime = userAccessModel.OPTime;
                                        bl.BlockLogsUiOutTime = DateTime.Now;
                                        bl.BlockLogsMsgInsize = userAccessModel.RequestData.Length.ToString();
                                        bl.BlockLogsMsgOutSize = userAccessModel.RequestData.Length.ToString();
                                        bl.BlockLogsMsg = userAccessModel.RequestData;
                                        bl.CrtUser = userAccessModel.User;
                                        bl.CrtDatetime = DateTime.Now;
                                        bl.CrtProg = userAccessModel.API;

                                        var res = db.Insertable(bl).ExecuteCommandAsync().Result;
                                    }
                                    catch (Exception ex)
                                    {
                                        SerilogHelper.WriteErrorLog("RecordAccessLogs", "发送接口日志到PgSql", ex);
                                    }
                                    finally
                                    {
                                        db.Close();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    SerilogHelper.WriteErrorLog("RecordAccessLogs", "发送接口日志到PgSql", ex);
                                }
                            });
                        }

                        // 发送接口日志到MongoDB
                        if (Appsettings.App("Middleware", "RecordAccessLogs", "WriteToMongoDB", "Enabled").GetCBool())
                        {
                            Task task3 = Task.Run(() =>
                            {
                                try
                                {
                                    var userUid = context.Request.Headers["x-user"].GetCString();
                                    var token = context.Request.Headers["x-token"].GetCString();
                                    var corp = context.Request.Headers["x-corp"].GetCString();
                                    var domain = context.Request.Headers["x-domain"].GetCString();

                                    string conn = Appsettings.App("Middleware", "RecordAccessLogs", "WriteToMongoDB", "Connection").GetCString();
                                    string dbName = Appsettings.App("Middleware", "RecordAccessLogs", "WriteToMongoDB", "DataBase").GetCString();
                                    string collectionName = Appsettings.App("Middleware", "RecordAccessLogs", "WriteToMongoDB", "CollectionName").GetCString();
                                    MongoDBHelper dbHelper = new MongoDBHelper(conn, dbName, collectionName);
                                    try
                                    {
                                        dbHelper.Add(new 
                                        {
                                            Id = Guid.NewGuid().ToString(),
                                            SolutionName = Assembly.GetExecutingAssembly().FullName,
                                            Domain = domain,
                                            Timestamp = DateTime.Now.ToString(),
                                            User = userAccessModel.User,
                                            Session = context.Request.Headers["x-token"].GetCString(),
                                            Ip = userAccessModel.IP,
                                            Menu = userAccessModel.API,
                                            ApiName = userAccessModel.API,
                                            CallStarttime = userAccessModel.BeginTime,
                                            CallApitime = userAccessModel.BeginTime,
                                            ApiTotaltime = userAccessModel.OPTime,
                                            UiOuttime = userAccessModel.EndTime,
                                            MsgOutSize = userAccessModel.RequestData.Length,
                                            MsgInSize = userAccessModel.RequestData.Length,
                                            Msg = userAccessModel.RequestData,
                                            crtDatetime = DateTime.Now,
                                            CrtProg = userAccessModel.API,
                                            CrtUser = userAccessModel.User
                                        });
                                    }
                                    catch (Exception ex)
                                    {
                                        SerilogHelper.WriteErrorLog("RecordAccessLogs", "发送接口日志到MongoDB", ex);
                                    }
                                    finally
                                    {
                                        dbHelper.Dispose();
                                    }
                                    
                                }
                                catch (Exception ex)
                                {
                                    SerilogHelper.WriteErrorLog("RecordAccessLogs", "发送接口日志到MongoDB", ex);
                                }
                            });
                        }

                        return Task.CompletedTask;
                    });
                }
                else
                {
                    await _next(context);
                }
            }
            else
            {
                await _next(context);
            }
        }


        /// <summary>
        /// 获取响应内容
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static async Task<string> GetResponse(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var text = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);
            return text;
        }

        /// <summary>
        /// 获取请求IP
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetClientIP(Microsoft.AspNetCore.Http.HttpContext context)
        {
            var ip = context.Request.Headers["X-Forwarded-For"].GetCString();
            if (string.IsNullOrEmpty(ip))
            {
                ip = context.Connection.RemoteIpAddress.GetCString();
            }
            return ip;
        }

        ///<summary>
        ///操作记录日志 
        ///</summary>
        [SugarTable("blocklogs_mstr", tableDescription: "dam")]
        private class BlockLogsMstr
        {
            /// <summary>
            /// 描述 : ID
            /// 允许空值 : False
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "id", IsNullable = false, IsPrimaryKey = true)]
            public string Id { get; set; }

            /// <summary>
            /// 描述 : 项目
            /// 允许空值 : True
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "blocklogs_solutionname")]
            public string BlockLogsSollutionName { get; set; }

            /// <summary>
            /// 描述 : 时间戳
            /// 允许空值 : False
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "blocklogs_timestamp", IsNullable = false, IsPrimaryKey = true)]
            public string BlockLogsTimeStamp { get; set; }

            /// <summary>
            /// 描述 : 用户
            /// 允许空值 : True
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "blocklogs_user", IndexGroupNameList = new string[] { "uk_blocklogs_mstr" })]
            public string BlockLogsUser { get; set; }

            /// <summary>
            /// 描述 : session
            /// 允许空值 : True
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "blocklogs_session", IndexGroupNameList = new string[] { "uk_blocklogs_mstr" })]
            public string BlockLogsSession { get; set; }

            /// <summary>
            /// 描述 : ip
            /// 允许空值 : True
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "blocklogs_ip", IndexGroupNameList = new string[] { "uk_blocklogs_mstr" })]
            public string BlockLogsIp { get; set; }

            /// <summary>
            /// 描述 : 菜单功能
            /// 允许空值 : True
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "blocklogs_menu", IndexGroupNameList = new string[] { "uk_blocklogs_mstr" })]
            public string BlockLogsMenu { get; set; }

            /// <summary>
            /// 描述 : 接口名
            /// 允许空值 : True
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "blocklogs_api_name", IndexGroupNameList = new string[] { "uk_blocklogs_mstr" })]
            public string BlockLogsApiName { get; set; }

            /// <summary>
            /// 描述 : ID
            /// 允许空值 : 调用开始时间
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "blocklogs_call_starttime", IndexGroupNameList = new string[] { "uk_blocklogs_mstr" })]
            public DateTime? BlockLogsCallStartTime { get; set; }

            /// <summary>
            /// 描述 : 调到接口时间
            /// 允许空值 : True
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "blocklogs_call_apitime")]
            public DateTime? BlockLogsCallApiTime { get; set; }

            /// <summary>
            /// 描述 : 接口耗时时间
            /// 允许空值 : True
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "blocklogs_api_totaltime")]
            public string BlockLogsApiTotalTime { get; set; }

            /// <summary>
            /// 描述 : 界面输出时间
            /// 允许空值 : True
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "blocklogs_ui_outtime")]
            public DateTime? BlockLogsUiOutTime { get; set; }

            /// <summary>
            /// 描述 : 报文输出大小
            /// 允许空值 : True
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "blocklogs_msg_outsize")]
            public string BlockLogsMsgOutSize { get; set; }

            /// <summary>
            /// 描述 : 报文输入大小
            /// 允许空值 : True
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "blocklogs_msg_insize")]
            public string BlockLogsMsgInsize { get; set; }

            /// <summary>
            /// 描述 : ID
            /// 允许空值 : 日志信息
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "blocklogs_msg")]
            public string BlockLogsMsg { get; set; }

            /// <summary>
            /// 描述 : 创建时间
            /// 允许空值 : True
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "blocklogs_crt_datetime")]
            public DateTime? CrtDatetime { get; set; }

            /// <summary>
            /// 描述 : 程序名
            /// 允许空值 : True
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "blocklogs_crt_prog")]
            public string CrtProg { get; set; }

            /// <summary>
            /// 描述 : 创建用户
            /// 允许空值 : True
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "blocklogs_crt_user")]
            public string CrtUser { get; set; }

            /// <summary>
            /// 描述 : 修改日期
            /// 允许空值 : True
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "blocklogs_mod_datetime")]
            public DateTime? ModDatetime { get; set; }

            /// <summary>
            /// 描述 : 修改程序名
            /// 允许空值 : True
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "blocklogs_mod_prog")]
            public string ModProg { get; set; }


            /// <summary>
            /// 描述 : 修改用户
            /// 允许空值 : True
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "blocklogs_mod_user")]
            public string ModUser { get; set; }

            /// <summary>
            /// 描述 : 域
            /// 允许空值 : True
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "blocklogs_domain", IndexGroupNameList = new string[] { "uk_blocklogs_mstr" })]
            public string Domain { get; set; }


        }
    }

    public class UserAccessModel
    {
        public string IP { get; set; }
        public string User { get; set; }
        public string API { get; set; }
        public string RequestQuery { get; set; }
        public string RequestHeader { get; set; }
        public string RequestData { get; set; }
        public string ResponseStatus { get; set; }
        public string ResponseHeader { get; set; }
        public string ResponseData { get; set; }
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }
        public string OPTime { get; set; }

    }


    [Serializable()]
    partial class RequestLogModel
    {
        /// <summary>
        /// IP地址
        /// </summary>
        public string ip { get; set; }
        /// <summary>
        /// 请求地址
        /// </summary>
        public string requestUrl { get; set; }
        /// <summary>
        /// 请求头
        /// </summary>
        public string requestHeader { get; set; }
        /// <summary>
        /// 请求参数JSON
        /// </summary>
        public string requestParam { get; set; }
        /// <summary>
        /// 请求参数JSON
        /// </summary>
        public string requestBody { get; set; }
        /// <summary>
        /// 请求类型 GET/POST/PUT/DELETE
        /// </summary>
        public string requestType { get; set; }
        /// <summary>
        /// 导入文件信息(导入接口才有)
        /// </summary>
        public string importData { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string exception { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public string startDate { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public string endDate { get; set; }
        /// <summary>
        /// 花费时间
        /// </summary>
        public string costTime { get; set; }
        /// <summary>
        /// 日志状态
        /// 0:正常
        /// 1：有错误未确认
        /// 2:有错误已确认
        /// </summary>
        public int status { get; set; } = 0;
        /// <summary>
        /// <summary>
        /// 信息
        /// </summary>
        public string message { get; set; }
        /// 创建时间
        /// </summary>
        public string createDatetime { get; set; }
        /// <summary>
        /// 模块名
        /// </summary>
        public string module { get; set; }
        /// <summary>
        /// 服务名
        /// </summary>
        public string service { get; set; }
        /// <summary>
        /// 登录用户ID
        /// </summary>
        public string userId { get; set; }
        /// <summary>
        /// 登录用户名
        /// </summary>
        public string userName { get; set; }
        /// <summary>
        /// 日志类型
        ///
        /// </summary>
        public int logType { get; set; }
        /// <summary>
        /// 接口名称
        /// </summary>
        public string interfaceName { get; set; }

    }

}


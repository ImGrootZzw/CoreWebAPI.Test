using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CoreWebAPI.Common.Helper;
using CoreWebAPI.Common.LogHelper;

namespace CoreWebAPI.Middlewares
{
    /// <summary>
    /// 中间件
    /// 记录请求和响应数据
    /// </summary>
    public class RequRespLogMildd
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly RequestDelegate _next;
        private readonly ILogger<RequRespLogMildd> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        public RequRespLogMildd(RequestDelegate next, ILogger<RequRespLogMildd> logger)
        {
            _next = next;
            _logger = logger;
        }



        public async Task InvokeAsync(HttpContext context)
        {
            if (Appsettings.App("Middleware", "RequestResponseLog", "Enabled").GetCBool())
            {
                // 过滤，只有接口
                if (context.Request.Path.Value.Contains("api"))
                {
                    context.Request.EnableBuffering();
                    Stream originalBody = context.Response.Body;

                    try
                    {
                        // 存储请求数据
                        await RequestDataLog(context.Request);

                        using (var ms = new MemoryStream())
                        {
                            context.Response.Body = ms;

                            await _next(context);

                            // 存储响应数据
                            await ResponseDataLog(context.Response, ms);
                            
                            ms.Position = 0;
                            await ms.CopyToAsync(originalBody);
                        }
                    }
                    catch (Exception ex)
                    {
                        // 记录异常                        
                        _logger.LogError(ex.Message + "" + ex.InnerException);
                    }
                    finally
                    {
                        context.Response.Body = originalBody;
                    }
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

        private async Task RequestDataLog(HttpRequest request)
        {
            var sr = new StreamReader(request.Body);

            var content = $" QueryUrl:{request.Path + request.QueryString}\r\n BodyData:{await sr.ReadToEndAsync()}";

            if (!string.IsNullOrEmpty(content))
            {
                Parallel.For(0, 1, e =>
                {
                    //LogLock.OutSql2Log("RequestResponseLog", new string[] { "Request Data:", content });
                    SerilogServer.WriteLog("RecordAccessLogs", "RequestDataLog", "Request Data:", content);
                    _logger.LogInformation(content);
                });

                request.Body.Position = 0;
            }
        }

        private async Task ResponseDataLog(HttpResponse response, MemoryStream ms)
        {
            ms.Position = 0;
            var ResponseBody = await new StreamReader(ms).ReadToEndAsync();

            // 去除 Html
            var reg = "<[^>]+>";
            var isHtml = Regex.IsMatch(ResponseBody, reg);

            var content = $" ResponseCode:{response.StatusCode}\r\n BodyData:{ResponseBody}";

            if (!string.IsNullOrEmpty(ResponseBody))
            {
                Parallel.For(0, 1, e =>
                {
                    //LogLock.OutSql2Log("RequestResponseLog", new string[] { "Response Data:", ResponseBody });
                    SerilogServer.WriteLog("RecordAccessLogs", "ResponseDataLog", "Response Data:", content);
                    _logger.LogInformation(content);
                });
            }
        }
    }
}


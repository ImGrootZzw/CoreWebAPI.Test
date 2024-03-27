using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using CoreWebAPI.Common.Helper;

namespace CoreWebAPI.Extensions.Middlewares
{
    /// <summary>
    /// Options预检请求
    /// </summary>
    public class OptionsRequestMildd
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly RequestDelegate _next;
        private readonly ILogger<OptionsRequestMildd> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        public OptionsRequestMildd(RequestDelegate next, ILogger<OptionsRequestMildd> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Method.ToUpper() == "OPTIONS")
            {
                // 如果跨域设置不是允许所有地址
                if (!Appsettings.App(new string[] { "Startup", "Cors", "EnableAllIPs" }).GetCBool())
                {
                    //如果请求地址不在允许跨域地址中，返回403
                    if (Array.IndexOf(Appsettings.App(new string[] { "Startup", "Cors", "IPs" }).GetCString().Split(','), context.Request.Headers["Origin"].GetCString()) == -1)
                    {
                        context.Response.StatusCode = 403;
                        return;
                    }
                }

                context.Response.StatusCode = 200;
                context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
                context.Response.Headers.Add("Access-Control-Allow-Origin", context.Request.Headers["Origin"]);
                context.Response.Headers.Add("Access-Control-Allow-Methods", "POST,OPTIONS,GET,PUT,DELETE");
                context.Response.Headers.Add("Access-Control-Allow-Headers", context.Request.Headers["Access-Control-Request-Headers"]);
                return;
            }

            await _next.Invoke(context);
        }
    }
}

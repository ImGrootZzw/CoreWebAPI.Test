using Microsoft.AspNetCore.Http;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Headers;
using CoreWebAPI.Common.Helper;

namespace CoreWebAPI.Middlewares
{
    /// <summary>
    /// 中间件
    /// 记录用户方访问数据
    /// </summary>
    public class SwaggerAuthMilddleware
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly RequestDelegate _next;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        public SwaggerAuthMilddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/index.html") && Appsettings.App(new string[] { "Startup", "Swagger", "Auth", "Enabled" }).GetCBool())
            {
                string authHeader = context.Request.Headers["Authorization"];
                if (authHeader != null && authHeader.StartsWith("Basic "))
                {
                    // Get the credentials from request header
                    var header = AuthenticationHeaderValue.Parse(authHeader);
                    var inBytes = Convert.FromBase64String(header.Parameter);
                    var credentials = Encoding.UTF8.GetString(inBytes).Split(':');
                    var username = credentials[0];
                    var password = credentials[1];
                    // validate credentials
                    if (username.Equals(Appsettings.App(new string[] { "Startup", "Swagger", "Auth", "UserName" }).GetCString()) 
                        && password.Equals(Appsettings.App(new string[] { "Startup", "Swagger", "Auth", "Passwrod" }).GetCString()))
                    {
                        await _next.Invoke(context).ConfigureAwait(false);
                        return;
                    }
                }
                context.Response.Headers["WWW-Authenticate"] = "Basic";
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
            else
            {
                await _next.Invoke(context).ConfigureAwait(false);
            }
        }
    }

}


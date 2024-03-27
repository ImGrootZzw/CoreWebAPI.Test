using CoreWebAPI.Common.HttpContext;
using CoreWebAPI.Common.HttpContextUser;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CoreWebAPI.Extensions
{
    /// <summary>
    /// HttpContext 相关服务
    /// </summary>
    public static class HttpContextSetup
    {
        public static void AddHttpContextSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IHttpContextCore, HttpContextCore>();
            services.AddScoped<IHttpContext, Common.HttpContext.HttpContext>();
            services.AddScoped<IUser, AspNetUser>();
        }
    }
}

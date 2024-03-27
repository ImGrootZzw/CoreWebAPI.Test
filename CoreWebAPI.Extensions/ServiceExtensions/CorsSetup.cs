using Microsoft.Extensions.DependencyInjection;
using System;
using CoreWebAPI.Common.Helper;

namespace CoreWebAPI.Extensions
{
    /// <summary>
    /// Cors 启动服务
    /// </summary>
    public static class CorsSetup
    {
        public static void AddCorsSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddCors(c =>
            {
                if (Appsettings.App(new string[] { "Startup", "Cors", "EnableAllIPs" }).GetCBool())
                {
                    //允许任意跨域请求
                    c.AddPolicy(Appsettings.App(new string[] { "Startup", "Cors", "PolicyName" }),
                        policy =>
                        {
                            policy
                            .SetIsOriginAllowed((host) => true)
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                        });
                }
                else
                {
                    c.AddPolicy(Appsettings.App(new string[] { "Startup", "Cors", "PolicyName" }),

                        policy =>
                        {
                            policy
                            .WithOrigins(Appsettings.App<string>(new string[] { "Startup", "Cors", "AllowIPs" }).ToArray())
                            .AllowAnyHeader()//Ensures that the policy allows any header.
                            .AllowAnyMethod()
                            .AllowCredentials();
                        });
                }

            });
        }
    }
}

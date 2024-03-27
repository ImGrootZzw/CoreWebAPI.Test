using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using CoreWebAPI.Common.Helper;
using CoreWebAPI.Common.Redis;

namespace CoreWebAPI.Extensions
{
    /// <summary>
    /// Redis缓存 启动服务
    /// </summary>
    public static class RedisCacheSetup
    {
        public static void AddRedisCacheSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddTransient<IRedisBasketRepository, RedisBasketRepository>();

            // 配置启动Redis服务，虽然可能影响项目启动速度，但是不能在运行的时候报错，所以是合理的
            if (Appsettings.App(new string[] { "Startup", "Redis", "Enabled" }).GetCBool())
            {
                services.AddSingleton<ConnectionMultiplexer>(sp =>
                {
                    //获取连接字符串
                    string redisConfiguration = Appsettings.App(new string[] { "Startup", "Redis", "ConnectionString" });
                    var configuration = ConfigurationOptions.Parse(redisConfiguration, true);
                    configuration.ResolveDns = true;
                    return ConnectionMultiplexer.Connect(configuration);
                });
            }
        }
    }
}

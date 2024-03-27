using Microsoft.Extensions.DependencyInjection;
using System;
using CoreWebAPI.Common.Helper;
using System.Collections.Generic;
using CSRedis;

namespace CoreWebAPI.Extensions
{
    /// <summary>
    /// Sentinel
    /// </summary>
    public static class SentinelSetup
    {
        public static void AddSentinelSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            if (Appsettings.App("Startup", "CsRedisConfig", "Enable").GetCBool())
            {
                string connectionString = Appsettings.App("Startup", "CsRedisConfig", "SentinelConnection");
                List<string> sentinels = Appsettings.App<string>("Startup", "CsRedisConfig", "Sentinel");

                CSRedisClient csredis = new CSRedisClient(connectionString, sentinels.ToArray());

                RedisHelper.Initialization(csredis);//初始化
            }
            
        }
    }

}

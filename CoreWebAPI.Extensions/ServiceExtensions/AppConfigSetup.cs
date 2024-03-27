using CoreWebAPI.Common;
using CoreWebAPI.Common.Helper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text;

namespace CoreWebAPI.Extensions
{
    /// <summary>
    /// 项目 启动服务
    /// </summary>
    public static class AppConfigSetup
    {
        public static void AddAppConfigSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            if (true || Appsettings.App(new string[] { "Startup", "AppConfigAlert", "Enabled" }).GetCBool())
            {
                //if (env.IsDevelopment())
                //{
                //.NET Core 在默认情况下是没有注册EncodeProvider
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                //}

                Console.WriteLine("************ Blog.Core Config Set *****************");

                //ConsoleHelper.WriteSuccessLine("Current environment: " + Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));

                // 授权策略方案
                if (Permissions.IsUseIds4)
                {
                    //ConsoleHelper.WriteSuccessLine($"Current authorization scheme: " + (Permissions.IsUseIds4 ? "Ids4" : "JWT"));
                }
                else
                {
                    Console.WriteLine($"Current authorization scheme: " + (Permissions.IsUseIds4 ? "Ids4" : "JWT"));
                }

                // Redis缓存AOP
                if (!Appsettings.App(new string[] { "AppSettings", "RedisCachingAOP", "Enabled" }).GetCBool())
                {
                    Console.WriteLine($"Redis Caching AOP: False");
                }
                else
                {
                    //ConsoleHelper.WriteSuccessLine($"Redis Caching AOP: True");
                }

                // 内存缓存AOP
                if (!Appsettings.App(new string[] { "AppSettings", "MemoryCachingAOP", "Enabled" }).GetCBool())
                {
                    Console.WriteLine($"Memory Caching AOP: False");
                }
                else
                {
                    //ConsoleHelper.WriteSuccessLine($"Memory Caching AOP: True");
                }

                // 服务日志AOP
                if (!Appsettings.App(new string[] { "AppSettings", "LogAOP", "Enabled" }).GetCBool())
                {
                    Console.WriteLine($"Service Log AOP: False");
                }
                else
                {
                    //ConsoleHelper.WriteSuccessLine($"Service Log AOP: True");
                }

                // 事务AOP
                if (!Appsettings.App(new string[] { "AppSettings", "TranAOP", "Enabled" }).GetCBool())
                {
                    Console.WriteLine($"Transaction AOP: False");
                }
                else
                {
                    //ConsoleHelper.WriteSuccessLine($"Transaction AOP: True");
                }

                // 数据库Sql执行AOP
                if (!Appsettings.App(new string[] { "AppSettings", "SqlAOP", "Enabled" }).GetCBool())
                {
                    Console.WriteLine($"DB Sql AOP: False");
                }
                else
                {
                    //ConsoleHelper.WriteSuccessLine($"DB Sql AOP: True");
                }

                // SingnalR发送数据
                if (!Appsettings.App(new string[] { "Middleware", "SignalR", "Enabled" }).GetCBool())
                {
                    Console.WriteLine($"SignalR send data: False");
                }
                else
                {
                    //ConsoleHelper.WriteSuccessLine($"SignalR send data: True");
                }

                // IP限流
                if (!Appsettings.App("Middleware", "IpRateLimit", "Enabled").GetCBool())
                {
                    Console.WriteLine($"IpRateLimiting: False");
                }
                else
                {
                    //ConsoleHelper.WriteSuccessLine($"IpRateLimiting: True");
                }

                // redis消息队列
                if (!Appsettings.App("Startup", "RedisMq", "Enabled").GetCBool())
                {
                    Console.WriteLine($"Redis MQ: False");
                }
                else
                {
                    //ConsoleHelper.WriteSuccessLine($"Redis MQ: True");
                }

                // RabbitMQ 消息队列
                if (!Appsettings.App("RabbitMQ", "Enabled").GetCBool())
                {
                    Console.WriteLine($"RabbitMQ: False");
                }
                else
                {
                    //ConsoleHelper.WriteSuccessLine($"RabbitMQ: True");
                }

                // EventBus 事件总线
                if (!Appsettings.App("EventBus", "Enabled").GetCBool())
                {
                    Console.WriteLine($"EventBus: False");
                }
                else
                {
                    //ConsoleHelper.WriteSuccessLine($"EventBus: True");
                }

                // 多库
                if (!Appsettings.App(new string[] { "MutiDBEnabled" }).GetCBool())
                {
                    Console.WriteLine($"Is multi-DataBase: False");
                }
                else
                {
                    //ConsoleHelper.WriteSuccessLine($"Is multi-DataBase: True");
                }

                // 读写分离
                if (!Appsettings.App(new string[] { "CQRSEnabled" }).GetCBool())
                {
                    Console.WriteLine($"Is CQRS: False");
                }
                else
                {
                    //ConsoleHelper.WriteSuccessLine($"Is CQRS: True");
                }

                Console.WriteLine();
            }

        }
    }
}

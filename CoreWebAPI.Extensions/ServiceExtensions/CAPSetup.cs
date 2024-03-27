using Microsoft.Extensions.DependencyInjection;
using System;
using CoreWebAPI.Common.Helper;

namespace CoreWebAPI.Extensions
{
    /// <summary>
    /// CAP分布式事务
    /// </summary>
    public static class CAPSetup
    {
        public static void AddCAPSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            if (Appsettings.App("Startup", "CAP", "Enable").GetCBool())
            {
                //注意: 注入的服务需要在 `services.AddCap()` 之前
                //services.AddTransient<ISubscriberService, SubscriberService>();

                services.AddCap(x =>
                {
                    ////如果你使用的 EF 进行数据操作，你需要添加如下配置：
                    //x.UseEntityFramework<AppDbContext>();  //可选项，你不需要再次配置 x.UseSqlServer 了

                    ////如果你使用的ADO.NET，根据数据库选择进行配置：
                    //x.UseSqlServer("数据库连接字符串");
                    //x.UseMySql("数据库连接字符串");
                    //x.UsePostgreSql(options =>
                    //{
                    //    options.ConnectionString = Appsettings.App(new string[] { "CAP", "PostgreSQL" });
                    //    //options.Schema = "public";
                    //});

                    ////如果你使用的 MongoDB，你可以添加如下配置：
                    //x.UseMongoDB("ConnectionStrings");  //注意，仅支持MongoDB 4.0+集群

                    //CAP支持 RabbitMQ、Kafka、AzureServiceBus、AmazonSQS 等作为MQ，根据使用选择配置：
                    x.UseRabbitMQ(rb => {
                        rb.HostName =string.Join(",", Appsettings.App<string>(new string[] { "Startup", "RabbitMQ", "HostName" }));
                        rb.UserName = Appsettings.App(new string[] { "Startup", "RabbitMQ", "UserName" });
                        rb.Password = Appsettings.App(new string[] { "Startup", "RabbitMQ", "Password" });
                        rb.Port = Appsettings.App(new string[] { "Startup", "RabbitMQ", "Port" }).GetCInt();
                        rb.VirtualHost = Appsettings.App(new string[] { "Startup", "RabbitMQ", "VirtualHost" });
                    });
                    //x.UseKafka("ConnectionStrings");
                    //x.UseAzureServiceBus("ConnectionStrings");
                    //x.UseAmazonSQS();

                    //失败后的重试次数，默认50次；在FailedRetryInterval默认60秒的情况下，即默认重试50*60秒(50分钟)之后放弃失败重试
                    x.FailedRetryCount = 10;

                    //失败后的重拾间隔，默认60秒
                    x.FailedRetryInterval = 60;

                    //设置成功信息的删除时间默认24*3600秒
                    //x.SucceedMessageExpiredAfter = 60 * 60;

                    // 注册 Dashboard
                    x.UseDashboard();

                    //// 注册节点到 Consul
                    //x.UseDiscovery(d =>
                    //{
                    //    d.DiscoveryServerHostName = "localhost";
                    //    d.DiscoveryServerPort = 8500;
                    //    d.CurrentNodeHostName = "localhost";
                    //    d.CurrentNodePort = 5800;
                    //    d.NodeId = 1;
                    //    d.NodeName = "CAP No.1 Node";
                    //});
                });
            }
        }
    }

}

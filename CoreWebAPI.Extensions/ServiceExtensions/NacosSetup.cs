using Microsoft.Extensions.DependencyInjection;
using System;
using CoreWebAPI.Common.Helper;
using Microsoft.Extensions.Configuration;
using Nacos;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Nacos.V2.DependencyInjection;
using Nacos.AspNetCore.V2;
using Nacos.V2;

namespace CoreWebAPI.Extensions
{
    /// <summary>
    /// 注册Nacos
    /// </summary>
    public static class NacosSetup
    {
        public static void AddNacosSetup(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            if (Appsettings.App("Startup", "Nacos", "Enabled").GetCBool())
            {
                // 注册服务
                services.AddNacosAspNet(configuration, "Startup:nacos");


                services.AddNacosV2Naming(x =>
                {
                    x.ServerAddresses = Appsettings.App<string>("Startup", "Nacos", "ServerAddresses");
                    x.EndPoint = Appsettings.App("Startup", "Nacos", "EndPoint");
                    x.Namespace = Appsettings.App("Startup", "Nacos", "Namespace");


                    /*x.UserName = "nacos";
                   x.Password = "nacos";*/

                    // swich to use http or rpc
                    x.NamingUseRpc = Appsettings.App("Startup", "Nacos", "NamingUseRpc").GetCBool();
                });

                // 注册配置
                services.AddNacosV2Config(x =>
                {
                    x.ServerAddresses = Appsettings.App<string>("Startup", "Nacos", "ServerAddresses");
                    x.DefaultTimeOut = Appsettings.App("Startup", "Nacos", "DefaultTimeOut" ).GetCInt();
                    x.Namespace = Appsettings.App("Startup", "Nacos", "Namespace");

                    // swich to use http or rpc
                    x.ConfigUseRpc = Appsettings.App("Startup", "Nacos", "ConfigUseRpc").GetCBool();
                });

                // 添加监听，从Nacos读取配置成功后，会写入配置信息到本地缓存中，后面访问的话会优先去读缓存的内容。
                services.AddHostedService<NacosListener>();

            }
        }
    }

    // 监听Nacos
    partial class NacosListener : BackgroundService
    {
        private readonly INacosConfigService _configClient;

        public NacosListener(INacosConfigService configClient)
        {
            _configClient = configClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var _listener = new NacosConfigListener();

            // Add listener
            await _configClient.AddListener(Appsettings.App("Startup", "Nacos", "DataId"),
                 Appsettings.App("Startup", "Nacos", "Group"),
                _listener
            );
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            var _listener = new NacosConfigListener();

            // Remove listener
            await _configClient.RemoveListener(Appsettings.App("Startup", "Nacos", "DataId"),
                Appsettings.App("Startup", "Nacos", "Group"),
               _listener
            );

            await base.StopAsync(cancellationToken);
        }
    }

    partial class NacosConfigListener : IListener
    {
        public void ReceiveConfigInfo(string configInfo)
        {
            Appsettings.Update(configInfo);
        }
    }
}

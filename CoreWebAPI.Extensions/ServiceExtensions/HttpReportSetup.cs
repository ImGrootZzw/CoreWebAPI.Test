using Microsoft.Extensions.DependencyInjection;
using System;
using CoreWebAPI.Common.Helper;
using System.Data;

namespace CoreWebAPI.Extensions
{
    /// <summary>
    /// 微服务统计，分析，图表，监控一体化
    /// </summary>
    public static class HttpReportSetup
    {
        public static void AddHttpReportSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            if (Appsettings.App("Startup", "CAP", "Enable").GetCBool())
            {
                // 添加HttpReports中间件
                services.AddHttpReports().AddHttpTransport();

            }
        }
    }

}

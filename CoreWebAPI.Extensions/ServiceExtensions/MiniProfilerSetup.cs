using Microsoft.Extensions.DependencyInjection;
using StackExchange.Profiling;
using System;

namespace WebAPI.Extensions
{
    /// <summary>
    /// MiniProfiler 启动服务
    /// </summary>
    public static class MiniProfilerSetup
    {
        public static void AddMiniProfilerSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddMiniProfiler(options =>
            {
                // 设定弹出窗口的位置是左下角
                options.PopupRenderPosition = RenderPosition.BottomLeft;
                // 设定在弹出的明细窗口里会显式Time With Children这列
                options.PopupShowTimeWithChildren = true;
            });

            // 3.x使用MiniProfiler，必须要注册MemoryCache服务
            // services.AddMiniProfiler(options =>
            // {
            //     options.RouteBasePath = "/profiler";
            //     //(options.Storage as MemoryCacheStorage).CacheDuration = TimeSpan.FromMinutes(10);
            //     options.PopupRenderPosition = StackExchange.Profiling.RenderPosition.Left;
            //     options.PopupShowTimeWithChildren = true;

            //     // 可以增加权限
            //     //options.ResultsAuthorize = request => request.HttpContext.User.IsInRole("Admin");
            //     //options.UserIdProvider = request => request.HttpContext.User.Identity.Name;
            // }
            //);
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using CoreWebAPI.Common.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System.Linq;
using System.Reflection;
using CoreWebAPI.Filters;
using Microsoft.AspNetCore.Mvc.Controllers;
using CoreWebAPI.Common.LogHelper;
using System.Text;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;

namespace CoreWebAPI.Extensions
{
    /// <summary>
    /// 公共服务配置
    /// </summary>
    public static class CommonServiceSetup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public static void AddCommonServiceSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            //.NET Core 在默认情况下是没有注册EncodeProvider，需要手工注入
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            //忽略循环引用（使接口正常返回DataTable）
            services.AddControllers().AddNewtonsoftJson(option =>
                option.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            );

            int bodySize = Appsettings.App(new string[] { "Startup", "MaxRequestBodySize" }).GetCInt();
            if (bodySize > 0)
            {
                //上传文件大小限制-Kestrel设置
                services.Configure<KestrelServerOptions>(options =>
                {
                    // Defaults to 30,000,000 bytes, which is approximately 28.6MB.
                    // Set the limit to 256 MB
                    options.Limits.MaxRequestBodySize = bodySize;
                });
                //上传文件大小限制-IIS设置
                services.Configure<IISServerOptions>(options =>
                {
                    options.MaxRequestBodySize = bodySize;
                });

                //上传文件大小限制-Form设置
                services.Configure<FormOptions>(options =>
                {
                    //options.ValueLengthLimit = bodySize;
                    options.MultipartBodyLengthLimit = bodySize;
                });
            }
            

        }

    }

}

using Autofac;
using CoreWebAPI.Common.Helper;
using CoreWebAPI.Extensions;
using CoreWebAPI.Middlewares;
using CoreWebAPI.Repository.Base;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CoreWebAPINuGet.Test_WebAPI
{
    /// <summary>
    /// 启动类
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 注册配置
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// 配置
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            //.NET Core 在默认情况下是没有注册EncodeProvider
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            //忽略循环引用（使接口正常返回DataTable）
            object p = services.AddControllers().AddNewtonsoftJson(option =>
                option.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            );

            // 配置类
            services.AddSingleton(new Appsettings(Configuration));

            // SqlSugar ORM框架
            services.AddSqlsugarSetup();

            // HttpContext服务
            services.AddHttpContextSetup();

            // 跨域配置
            services.AddCorsSetup();

            // Redis缓存
            services.AddRedisCacheSetup();

            // 授权+认证 jwt
            services.AddAuthorizationSetup();
            services.AddAuthentication_JWTSetup();

            // Swagger接口文档
            services.AddSwaggerSetup();

            services.AddControllers();
        }

        /// <summary>
        /// 注意在Program.CreateHostBuilder，注册Autofac服务工厂
        /// </summary>
        /// <param name="builder"></param>
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new AutofacModuleRegister(Appsettings.App<string>("Startup", "Autofac", "Dlls")));
            //注册仓储
            builder.RegisterGeneric(typeof(BaseRepository<>)).As(typeof(IBaseRepository<>)).InstancePerDependency();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // 记录请求与返回数据(必须放到外层，不然如果遇到异常，会报错，因为不能返回流)
            app.UseRecordAccessLogsMildd();

            //启用中间件Swagger
            app.UseSwaggerMildd();

            // 拦截Options请求（不执行接口逻辑，节约请求时间）
            app.UseOptionsReuestMildd();

            // CORS跨域
            app.UseCors(Appsettings.App(new string[] { "Startup", "Cors", "PolicyName" }));


            //通过url访问文件
            app.UseStaticFiles(new StaticFileOptions()//自定义自己的文件路径
            {
                RequestPath = new PathString(""),//对外的访问路径
                FileProvider = new PhysicalFileProvider(AppContext.BaseDirectory)//指定实际物理路径
            });

            app.UseRouting();

            // 先开启认证
            app.UseAuthentication();
            // 然后是授权中间件
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

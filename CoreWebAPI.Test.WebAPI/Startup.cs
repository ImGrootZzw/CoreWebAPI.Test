using Autofac;
using CoreWebAPI.Common.Helper;
using CoreWebAPI.Extensions;
using CoreWebAPI.Extensions.ServiceExtensions;
using CoreWebAPI.Extensions.ServiceExtensions.Kafka;
using CoreWebAPI.Extensions.ServiceExtensions.Kafka.Consumer;
using CoreWebAPI.Middlewares;
using CoreWebAPI.Repository.Base;
using CoreWebAPI.Test.WebAPI.Listener;
using CoreWebAPI.Test.WebAPI.Socket;
using GZY.Quartz.MUI.Extensions;
//using GZY.Quartz.MUI.Extensions;
//using Jack.RemoteLog;
using LogDashboard;
using LogDashboard.Authorization.Filters;
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

namespace CoreWebAPI.Test_WebAPI
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
        private IServiceCollection _services;

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            _services = services;

            // 配置类，读取Appsetting配置，必须放在其他服务注入之前
            services.AddSingleton(new Appsettings(Configuration));

            // 注册服务及配置中心，开启时必须写在appsetting注入之后，其他服务注入之前
            //services.AddNacosSetup(Configuration);

            // 公共服务配置
            services.AddCommonServiceSetup();

            // 日志面板启用认证
            services.AddLogDashboard(opt => opt.AddAuthorizationFilter(new LogDashboardBasicAuthFilter("admin", "123456")));

            // SqlSugar ORM框架
            services.AddSqlsugarSetup();

            // HttpContext服务
            services.AddHttpContextSetup();

            // 跨域配置
            services.AddCorsSetup();

            // Redis缓存
            services.AddRedisCacheSetup();


            #region Kafka

            //services.AddKafkaConsumerSetup(options =>
            //{
            //    options.BootstrapServers = Appsettings.App<string>("Startup", "Kafka", "BootstrapServers").ToArray();
            //    options.EnableAutoCommit = Appsettings.App("Startup", "Kafka", "EnableAutoCommit").GetCBool();
            //    options.GroupId = Appsettings.App("Startup", "Kafka", "GroupId");
            //    options.Subscribers = KafkaSubscriber.From(Appsettings.App<string>("Startup", "Kafka", "Subscribers").ToArray());

            //}).AddListener<KafkaConsumerListener>();//实现IKafkaConsumerListener接口完成消费逻辑

            #endregion


            services.AddWebSocketManager();

            // 授权+认证 jwt
            services.AddAuthorizationSetup();
            services.AddAuthentication_JWTSetup();

            // Swagger接口文档
            services.AddSwaggerSetup();

            services.AddControllers();

            //services.AddLogging(builder =>
            //{
            //    builder.AddConfiguration(Configuration.GetSection("Logging"));
            //    builder.AddConsole();
            //    builder.UseJackRemoteLogger(Configuration, new Options
            //    {
            //        UserName = "admin",
            //        Password = "adminadmin"
            //    });
            //});

            services.AddQuartzUI();
            services.AddQuartzClassJobs(); //添加本地调度任务访问
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

            app.UseStaticHttpContext();

            //启用中间件Swagger
            app.UseSwaggerMildd();

            // 拦截Options请求（不执行接口逻辑，节约请求时间）
            app.UseOptionsReuestMildd();

            // CORS跨域
            app.UseCors(Appsettings.App(new string[] { "Startup", "Cors", "PolicyName" }));


            //app.UseQuartzUIBasicAuthorized(); //添加本地调度任务访问

            //自定义Url
            app.UseLogDashboard("/LogDashboard");

            //通过url访问文件
            app.UseStaticFiles(new StaticFileOptions()//自定义自己的文件路径
            {
                RequestPath = new PathString(""),//对外的访问路径
                FileProvider = new PhysicalFileProvider(AppContext.BaseDirectory)//指定实际物理路径
            });

            app.UseWebSockets();

            app.UseAllServicesMildd(_services);

            app.UseQuartz(); //添加这行代码，浏览器中导航到 /QuartzUI 
            app.UseRouting();

            // 先开启认证
            app.UseAuthentication();
            // 然后是授权中间件
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.MapWebSocketManager("/ws", app.ApplicationServices.GetService<BusMessageHandler>());
        }
    }
}

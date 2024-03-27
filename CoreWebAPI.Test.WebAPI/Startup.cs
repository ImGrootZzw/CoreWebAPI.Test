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
    /// ������
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// ע������
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// ����
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

            // �����࣬��ȡAppsetting���ã����������������ע��֮ǰ
            services.AddSingleton(new Appsettings(Configuration));

            // ע������������ģ�����ʱ����д��appsettingע��֮����������ע��֮ǰ
            //services.AddNacosSetup(Configuration);

            // ������������
            services.AddCommonServiceSetup();

            // ��־���������֤
            services.AddLogDashboard(opt => opt.AddAuthorizationFilter(new LogDashboardBasicAuthFilter("admin", "123456")));

            // SqlSugar ORM���
            services.AddSqlsugarSetup();

            // HttpContext����
            services.AddHttpContextSetup();

            // ��������
            services.AddCorsSetup();

            // Redis����
            services.AddRedisCacheSetup();


            #region Kafka

            //services.AddKafkaConsumerSetup(options =>
            //{
            //    options.BootstrapServers = Appsettings.App<string>("Startup", "Kafka", "BootstrapServers").ToArray();
            //    options.EnableAutoCommit = Appsettings.App("Startup", "Kafka", "EnableAutoCommit").GetCBool();
            //    options.GroupId = Appsettings.App("Startup", "Kafka", "GroupId");
            //    options.Subscribers = KafkaSubscriber.From(Appsettings.App<string>("Startup", "Kafka", "Subscribers").ToArray());

            //}).AddListener<KafkaConsumerListener>();//ʵ��IKafkaConsumerListener�ӿ���������߼�

            #endregion


            services.AddWebSocketManager();

            // ��Ȩ+��֤ jwt
            services.AddAuthorizationSetup();
            services.AddAuthentication_JWTSetup();

            // Swagger�ӿ��ĵ�
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
            services.AddQuartzClassJobs(); //��ӱ��ص����������
        }

        /// <summary>
        /// ע����Program.CreateHostBuilder��ע��Autofac���񹤳�
        /// </summary>
        /// <param name="builder"></param>
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new AutofacModuleRegister(Appsettings.App<string>("Startup", "Autofac", "Dlls")));
            //ע��ִ�
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

            // ��¼�����뷵������(����ŵ���㣬��Ȼ��������쳣���ᱨ����Ϊ���ܷ�����)
            app.UseRecordAccessLogsMildd();

            app.UseStaticHttpContext();

            //�����м��Swagger
            app.UseSwaggerMildd();

            // ����Options���󣨲�ִ�нӿ��߼�����Լ����ʱ�䣩
            app.UseOptionsReuestMildd();

            // CORS����
            app.UseCors(Appsettings.App(new string[] { "Startup", "Cors", "PolicyName" }));


            //app.UseQuartzUIBasicAuthorized(); //��ӱ��ص����������

            //�Զ���Url
            app.UseLogDashboard("/LogDashboard");

            //ͨ��url�����ļ�
            app.UseStaticFiles(new StaticFileOptions()//�Զ����Լ����ļ�·��
            {
                RequestPath = new PathString(""),//����ķ���·��
                FileProvider = new PhysicalFileProvider(AppContext.BaseDirectory)//ָ��ʵ������·��
            });

            app.UseWebSockets();

            app.UseAllServicesMildd(_services);

            app.UseQuartz(); //������д��룬������е����� /QuartzUI 
            app.UseRouting();

            // �ȿ�����֤
            app.UseAuthentication();
            // Ȼ������Ȩ�м��
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.MapWebSocketManager("/ws", app.ApplicationServices.GetService<BusMessageHandler>());
        }
    }
}

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

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            //.NET Core ��Ĭ���������û��ע��EncodeProvider
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            //����ѭ�����ã�ʹ�ӿ���������DataTable��
            object p = services.AddControllers().AddNewtonsoftJson(option =>
                option.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            );

            // ������
            services.AddSingleton(new Appsettings(Configuration));

            // SqlSugar ORM���
            services.AddSqlsugarSetup();

            // HttpContext����
            services.AddHttpContextSetup();

            // ��������
            services.AddCorsSetup();

            // Redis����
            services.AddRedisCacheSetup();

            // ��Ȩ+��֤ jwt
            services.AddAuthorizationSetup();
            services.AddAuthentication_JWTSetup();

            // Swagger�ӿ��ĵ�
            services.AddSwaggerSetup();

            services.AddControllers();
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

            //�����м��Swagger
            app.UseSwaggerMildd();

            // ����Options���󣨲�ִ�нӿ��߼�����Լ����ʱ�䣩
            app.UseOptionsReuestMildd();

            // CORS����
            app.UseCors(Appsettings.App(new string[] { "Startup", "Cors", "PolicyName" }));


            //ͨ��url�����ļ�
            app.UseStaticFiles(new StaticFileOptions()//�Զ����Լ����ļ�·��
            {
                RequestPath = new PathString(""),//����ķ���·��
                FileProvider = new PhysicalFileProvider(AppContext.BaseDirectory)//ָ��ʵ������·��
            });

            app.UseRouting();

            // �ȿ�����֤
            app.UseAuthentication();
            // Ȼ������Ȩ�м��
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

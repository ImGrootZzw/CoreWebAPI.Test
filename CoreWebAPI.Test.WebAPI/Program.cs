using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog.Events;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreWebAPI.Common.HttpContext;
using FreeScheduler;

namespace CoreWebAPI.Test_WebAPI
{
    public class Program
    {

        class MyTaskHandler : FreeScheduler.TaskHandlers.FreeSqlHandler
        {
            public MyTaskHandler(IFreeSql fsql) : base(fsql) { }

            public override void OnExecuting(Scheduler scheduler, TaskInfo task)
            {
                Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss.fff")}] {task.Topic} {task.Body} ��ִ�У���ʣ {scheduler.QuantityTask} ��ѭ������");
            }
        }

        public static void Main(string[] args)
        {
            //var fsql = new FreeSql.FreeSqlBuilder()
            //    .UseConnectionString(FreeSql.DataType.PostgreSQL, "PORT=5432;DATABASE=xmax_protal;HOST=192.9.200.222;USER ID=postgres;PASSWORD=erpesun")
            //    .UseAutoSyncStructure(true)
            //    .UseNoneCommandParameter(true)
            //    //.UseMonitorCommand(cmd => Console.WriteLine($"=========sql: {cmd.CommandText}\r\n"))
            //    .Build();

            //Scheduler scheduler = new Scheduler(new MyTaskHandler(fsql));
            ////ѭ������ִ��10�Σ�ÿ�μ��10��
            //scheduler.AddTask(topic: "test001", body: "data1", round: 10, seconds: 10);

            var configuration = new ConfigurationBuilder().SetBasePath(Environment.CurrentDirectory)
                                          .AddJsonFile("appsettings.json")
                                          .Build();

            //Log.Logger = new LoggerConfiguration()
            //    .MinimumLevel.Debug()
            //    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            //    .Enrich.FromLogContext()
            //            .Enrich.WithProperty("User", StaticHttpContext.User)
            //            .Enrich.WithProperty("API", StaticHttpContext.API)
            //    .WriteTo.File($"{AppContext.BaseDirectory}Log/.log", rollingInterval: RollingInterval.Day, outputTemplate: "{Timestamp:HH:mm} || {Level} || {SourceContext:l} || {Message} || {Exception} ||end {NewLine}")
            //    .CreateLogger();

            //��ʼ��Ĭ������Builder
            Host.CreateDefaultBuilder(args)
             .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            //.ConfigureAppConfiguration((context, builder) =>
            //{
            //    var c = builder.Build();

            //    // �������ļ���ȡNacos�������
            //    // Ĭ�ϻ�ʹ��JSON����������������Nacos Server������
            //    builder.AddNacosV2Configuration(c.GetSection("Startup:nacos"));
            //    // Ҳ���԰���ʹ��ini��yaml�Ľ�����
            //    // builder.AddNacosV2Configuration(c.GetSection("NacosConfig"), Nacos.IniParser.IniConfigurationStringParser.Instance);
            //    // builder.AddNacosV2Configuration(c.GetSection("NacosConfig"), Nacos.YamlParser.YamlConfigurationStringParser.Instance);
            //})
            //.UseNacosConfig(section: "NacosConfig", parser: null ,logAction: null)
            //.ConfigureAppConfiguration((context, builder) => 
            //{
            //    var c = builder.Build();

            //    // read configuration from config files
            //    // it will use default json parser to parse the configuration store in nacos server.
            //    builder.AddNacosV2Configuration(c.GetSection("NacosConfig"));
            //    // you also can specify ini or yaml parser as well.
            //    // builder.AddNacosV2Configuration(c.GetSection("NacosConfig"), Nacos.IniParser.IniConfigurationStringParser.Instance);
            //    // builder.AddNacosV2Configuration(c.GetSection("NacosConfig"), Nacos.YamlParser.YamlConfigurationStringParser.Instance);
            //})
             .ConfigureWebHostDefaults(webBuilder =>
             {
                 webBuilder
                 //.UseConfiguration(configuration)
                 .UseStartup<Startup>()
                 .ConfigureLogging((hostingContext, builder) =>
                 {
                     //���˵�ϵͳĬ�ϵ�һЩ��־
                     builder.AddFilter("System", LogLevel.Error);
                     builder.AddFilter("Microsoft", LogLevel.Error);

                     //builder.AddFilter("WebAPI.AuthHelper.ApiResponseHandler", LogLevel.Error);

                     ////�������ļ�
                     //var path = Path.Combine(Directory.GetCurrentDirectory(), "Log4net.config");
                     //builder.AddLog4Net(path);
                 });
                 //.UseKestrel(options => {
                 //    options.Limits.MaxRequestBodySize = null;
                 //})
                 //.UseIISIntegration();
             })
             // ���ɳ��� web Ӧ�ó���� Microsoft.AspNetCore.Hosting.IWebHost��Build��WebHostBuilder���յ�Ŀ�ģ�������һ�������WebHost����������������
             .Build()
            // ���� web Ӧ�ó�����ֹ�����߳�, ֱ�������رա�
            // �������� ���쳣���鿴 Log �ļ����µ��쳣��־ ��������  
             .Run();
            
        }
    }
}

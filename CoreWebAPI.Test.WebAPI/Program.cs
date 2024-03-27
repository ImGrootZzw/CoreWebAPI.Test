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
                Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss.fff")}] {task.Topic} {task.Body} 被执行，还剩 {scheduler.QuantityTask} 个循环任务");
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
            ////循环任务，执行10次，每次间隔10秒
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

            //初始化默认主机Builder
            Host.CreateDefaultBuilder(args)
             .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            //.ConfigureAppConfiguration((context, builder) =>
            //{
            //    var c = builder.Build();

            //    // 从配置文件读取Nacos相关配置
            //    // 默认会使用JSON解析器来解析存在Nacos Server的配置
            //    builder.AddNacosV2Configuration(c.GetSection("Startup:nacos"));
            //    // 也可以按需使用ini或yaml的解析器
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
                     //过滤掉系统默认的一些日志
                     builder.AddFilter("System", LogLevel.Error);
                     builder.AddFilter("Microsoft", LogLevel.Error);

                     //builder.AddFilter("WebAPI.AuthHelper.ApiResponseHandler", LogLevel.Error);

                     ////可配置文件
                     //var path = Path.Combine(Directory.GetCurrentDirectory(), "Log4net.config");
                     //builder.AddLog4Net(path);
                 });
                 //.UseKestrel(options => {
                 //    options.Limits.MaxRequestBodySize = null;
                 //})
                 //.UseIISIntegration();
             })
             // 生成承载 web 应用程序的 Microsoft.AspNetCore.Hosting.IWebHost。Build是WebHostBuilder最终的目的，将返回一个构造的WebHost，最终生成宿主。
             .Build()
            // 运行 web 应用程序并阻止调用线程, 直到主机关闭。
            // ※※※※ 有异常，查看 Log 文件夹下的异常日志 ※※※※  
             .Run();
            
        }
    }
}

using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreWebAPINuGet.Test_WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder().SetBasePath(Environment.CurrentDirectory)
                                          .AddJsonFile("appsettings.json")
                                          .Build();
            //��ʼ��Ĭ������Builder
            Host.CreateDefaultBuilder(args)
             .UseServiceProviderFactory(new AutofacServiceProviderFactory())
             .ConfigureWebHostDefaults(webBuilder =>
             {
                 webBuilder
                 .UseConfiguration(configuration)
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
             })
             // ���ɳ��� web Ӧ�ó���� Microsoft.AspNetCore.Hosting.IWebHost��Build��WebHostBuilder���յ�Ŀ�ģ�������һ�������WebHost����������������
             .Build()
            // ���� web Ӧ�ó�����ֹ�����߳�, ֱ�������رա�
            // �������� ���쳣���鿴 Log �ļ����µ��쳣��־ ��������  
             .Run();
            
        }
    }
}

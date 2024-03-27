using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CoreWebAPI.Common.LogHelper;

namespace CoreWebAPI.Common.Helper
{
    /// <summary>
    /// appsettings.json操作类
    /// </summary>
    public class Appsettings
    {
        static IConfiguration Configuration { get; set; }

        public Appsettings(string contentPath)
        {
            string path = Path.Combine("appsettings.json");

            //如果你把配置文件 是 根据环境变量来分开了，可以这样写
            //path = $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json";

            Configuration = new ConfigurationBuilder()
               .SetBasePath(contentPath)
               .Add(new JsonConfigurationSource { Path = path, Optional = false, ReloadOnChange = true })//这样的话，可以直接读目录里的json文件，而不是 bin 文件夹下的，所以不用修改复制属性
               .Build();

        }

        public Appsettings(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// 读取Nacos配置输出到本地，并注册到全局配置
        /// </summary>
        /// <param name="json">配置json字符串</param>
        /// <returns></returns>
        public static void Update(string json)
        {
            try
            {
                string path = "NacosAppsettings.json";
                using (FileStream fs = new FileStream(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path), FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    StreamWriter sw = new StreamWriter(fs);
                    fs.SetLength(0);
                    sw.Write(json);
                    sw.Close();
                }

                Configuration = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .Add(new JsonConfigurationSource { Path = path, Optional = false, ReloadOnChange = true })
                    .Build();

                //Configuration = new ConfigurationBuilder().Add(new MyConfigSource(json)).Build();
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 封装要操作的字符
        /// </summary>
        /// <param name="sections">节点配置</param>
        /// <returns></returns>
        public static string App(params string[] sections)
        {
            if (sections.Any())
            {
                //if (Configuration.GetSection("Startup:nacos").Exists())
                //{
                //    var s = Configuration.GetSection("Startup:nacos").Value;
                //    var s1 = Configuration.GetSection("Startup:nacos").GetValue(string.Join(":", sections),"");
                //    return Configuration[string.Join(":", sections)] ?? "";
                //}
                //else
                //{
                    return Configuration[string.Join(":", sections)] ?? "";
                //}
            }
            return "";
        }

        /// <summary>
        /// 递归获取配置信息数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sections"></param>
        /// <returns></returns>
        public static List<T> App<T>(params string[] sections)
        {
            List<T> list = new List<T>();
            Configuration.Bind(string.Join(":", sections), list);
            return list;
        }

    }
}

using Autofac;
using Autofac.Extras.DynamicProxy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace CoreWebAPI.Extensions
{
    public class AutofacModuleRegister : Autofac.Module
    {
        private readonly List<string> _dlls;

        public AutofacModuleRegister(List<string> dlls)
        {
            _dlls = dlls;
        }
        protected override void Load(ContainerBuilder builder)
        {
            var basePath = AppContext.BaseDirectory;

            // AOP 开关，如果想要打开指定的功能，只需要在 appsettigns.json 对应对应 true 就行。
            var cacheType = new List<Type>();
            //if (Appsettings.App(new string[] { "AppSettings", "RedisCachingAOP", "Enabled" }).getCBool())
            //{
            //    builder.RegisterType<RedisCacheAOP>();
            //    cacheType.Add(typeof(RedisCacheAOP));
            //}
            //if (Appsettings.App(new string[] { "AppSettings", "MemoryCachingAOP", "Enabled" }).getCBool())
            //{
            //    builder.RegisterType<CacheAOP>();
            //    cacheType.Add(typeof(CacheAOP));
            //}
            //if (Appsettings.App(new string[] { "AppSettings", "LogAOP", "Enabled" }).getCBool())
            //{
            //    builder.RegisterType<LogAOP>();
            //    cacheType.Add(typeof(LogAOP));
            //}

            #region 带有接口层的服务注入
            foreach (var dll in _dlls)
            {
                var dllFile = Path.Combine(basePath, dll);
                if (!File.Exists(dllFile))
                {
                    var msg = dll + "丢失，检查appsettings.json中Autofac配置；如果项目的bin目录下没有该文件，检查是否有引用项目";
                    throw new Exception(msg);
                }

                // 获取 dll 程序集服务，并注册
                var assemblysServices = Assembly.LoadFrom(dllFile);
                builder.RegisterAssemblyTypes(assemblysServices)
                          .AsImplementedInterfaces()//自动以其实现的所有接口类型暴露
                          .InstancePerDependency()//瞬时单例
                          .EnableInterfaceInterceptors()//引用Autofac.Extras.DynamicProxy;
                          .InterceptedBy(cacheType.ToArray());//允许将拦截器服务的列表分配给注册。
            }

            #endregion

            #region 没有接口层的服务层注入
            //因为没有接口层，所以不能实现解耦，只能用 Load 方法。
            //注意如果使用没有接口的服务，并想对其使用 AOP 拦截，就必须设置为虚方法
            //var assemblysServicesNoInterfaces = Assembly.Load("WebAPI.Services");
            //builder.RegisterAssemblyTypes(assemblysServicesNoInterfaces);

            #endregion

            #region 没有接口的单独类，启用class代理拦截

            //只能注入该类中的虚方法，且必须是public
            //builder.RegisterAssemblyTypes(Assembly.GetAssembly(typeof(Love)))
            //    .EnableClassInterceptors()
            //    .InterceptedBy(cacheType.ToArray());
            #endregion

            #region 单独注册一个含有接口的类，启用interface代理拦截
            //不用虚方法
            //builder.RegisterType<AopService>().As<IAopService>()
            //   .AsImplementedInterfaces()
            //   .EnableInterfaceInterceptors()
            //   .InterceptedBy(typeof(BlogCacheAOP));
            #endregion

        }
    }
}

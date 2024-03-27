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

namespace CoreWebAPI.Extensions
{
    /// <summary>
    /// Swagger 启动服务
    /// </summary>
    public static class SwaggerSetup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public static void AddSwaggerSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            var basePath = AppContext.BaseDirectory;
            var ApiName = Appsettings.App(new string[] { "Startup", "Swagger", "ApiName" }).GetCString();
            var XmlName = Appsettings.App(new string[] { "Startup", "Swagger", "XmlName" }).GetCString();

            //注册Swagger生成器，定义一个Swagger 文档
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = $"{ApiName} 接口文档",
                    Description = $"{ApiName} RESTful API",
                    Contact = new OpenApiContact()
                    {
                        Name = "XXX项目",
                        Email = "@qq.com",
                        //Url =  "https://"
                    }
                });

                //#region Swagger接口模块分组配置
                //var ApiVsersions = Appsettings.App<SwaggerGroupConfig>(new string[] { "Startup", "Swagger", "Versions" });
                //ApiVsersions.ForEach(group =>
                //{
                //    swoption.SwaggerDoc(group.GroupName, new OpenApiInfo { Title = group.Title, Version = group.Version });   //分组显示
                //    options.SwaggerDoc("v1", new OpenApiInfo
                //    {
                //        Version = "v1",
                //        Title = $"{ApiName} 接口文档",
                //        Description = $"{ApiName} RESTful API",
                //        Contact = new OpenApiContact()
                //        {
                //            Name = "XXX项目",
                //            Email = "@qq.com",
                //            //Url =  "https://"
                //        }
                //    });
                //});
                //public class SwaggerGroupConfig
                //{
                //    public string GroupName { get; set; }
                //    public string Title { get; set; }
                //    public string Version { get; set; }
                //}
                //#endregion

                //options.DocumentFilter<SwaggerDocumentFilter>();

                //options.OperationFilter<SwaggerOperationFilter>();

                //#region 自定义Tags
                //options.TagActionsBy(apiDesc =>
                //{
                //    if (apiDesc.GroupName != null)
                //    {
                //        return new[] { apiDesc.GroupName };
                //    }

                //    var controllerActionDescriptor = apiDesc.ActionDescriptor as ControllerActionDescriptor;
                //    if (controllerActionDescriptor.ControllerName == "PiproduceMstr")
                //    {
                //        var s = "123";
                //        return new[] { "Goods Produce" };
                //    }
                //    if (controllerActionDescriptor != null)
                //    {
                //        return new[] { controllerActionDescriptor.ControllerName };
                //    }

                //    throw new InvalidOperationException("Unable to determine tag for endpoint.");
                //});
                //#endregion

                ////设置要展示的接口
                //options.DocInclusionPredicate((docName, apiDes) =>
                //{
                //    if (!apiDes.TryGetMethodInfo(out MethodInfo method))
                //        return false;
                //    /*使用ApiExplorerSettingsAttribute里面的GroupName进行特性标识
                //     * DeclaringType只能获取controller上的特性
                //     * 我们这里是想以action的特性为主
                //     * */
                //    var version = method.DeclaringType.GetCustomAttributes(true).OfType<ApiExplorerSettingsAttribute>().Select(m => m.GroupName);
                //    if (docName == "v1" && !version.Any())
                //        return true;
                //    //这里获取action的特性
                //    var actionVersion = method.GetCustomAttributes(true).OfType<ApiExplorerSettingsAttribute>().Select(m => m.GroupName);
                //    if (actionVersion.Any())
                //        return actionVersion.Any(v => v == docName);
                //    return version.Any(v => v == docName);
                //});

                // 为 Swagger 设置xml文档注释路径
                options.OrderActionsBy(o => o.RelativePath);

                // 添加控制器层注释，true表示显示控制器注释
                var xmlFile = $"{ XmlName }";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath, true);

                // 添加xml注释
                var xmls = Appsettings.App<string>("Startup", "Swagger", "Xmls");
                if (xmls != null)
                {
                    foreach (string xml in xmls)
                    {
                        var xmlFilePath = Path.Combine(AppContext.BaseDirectory, xml);
                        if (File.Exists(xmlFilePath))
                            options.IncludeXmlComments(xmlFilePath, true);
                    }
                }

                #region 添加header信息
                options.AddSecurityDefinition("x-token", new OpenApiSecurityScheme()
                {
                    Description = "在下框中输入请求头中添加Jwt授权Token：Bearer Token",
                    Name = "x-token",
                    In = ParameterLocation.Header,//jwt默认的参数名称
                    Type = SecuritySchemeType.ApiKey,//jwt默认存放Authorization信息的位置(请求头中)
                    BearerFormat = "JWT",
                    Scheme = "Token"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id = "x-token"
                            }
                        },
                        new string[] { }
                    }
                });

                options.AddSecurityDefinition("x-user", new OpenApiSecurityScheme()
                {
                    Description = "用户ID",
                    Name = "x-user",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement() { { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "x-user" } }, new string[] { } } });

                options.AddSecurityDefinition("x-corp", new OpenApiSecurityScheme()
                {
                    Description = "公司ID",
                    Name = "x-corp",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement() { { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "x-corp" } }, new string[] { } } });

                options.AddSecurityDefinition("x-domain", new OpenApiSecurityScheme()
                {
                    Description = "域ID",
                    Name = "x-domain",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement() { { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "x-domain" } }, new string[] { } } });

                options.AddSecurityDefinition("x-language", new OpenApiSecurityScheme()
                {
                    Description = "语言ID",
                    Name = "x-language",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement() { { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "x-language" } }, new string[] { } } });


                #endregion

                options.CustomSchemaIds(o => o.FullName);

            });


            //services.AddApiVersioning(option =>
            //{
            //    // 可选，为true时API返回支持的版本信息
            //    option.ReportApiVersions = true;
            //    // 请求中未指定版本时默认为1.0
            //    option.DefaultApiVersion = new ApiVersion(1, 0);
            //    //版本号以什么形式，什么字段传递
            //    option.ApiVersionReader = ApiVersionReader.Combine(new HeaderApiVersionReader("api-version"));

            //    // 在不提供版本号时，默认为1.0  如果不添加此配置，不提供版本号时会报错"message": "An API version is required, but was not specified."
            //    //option.AssumeDefaultVersionWhenUnspecified = true;
            //    //默认以当前最高版本进行访问
            //    //option.ApiVersionSelector = new CurrentImplementationApiVersionSelector(option);
            //}).AddVersionedApiExplorer(opt =>
            //{
            //    //以通知swagger替换控制器路由中的版本并配置api版本
            //    opt.SubstituteApiVersionInUrl = true;
            //    // 版本名的格式：v+版本号
            //    opt.GroupNameFormat = "'v'VVV";
            //    //是否提供API版本服务
            //    opt.AssumeDefaultVersionWhenUnspecified = true;
            //});

            //services.AddSwaggerGen();
            //services.AddOptions<SwaggerGenOptions>()
            //        .Configure<IApiVersionDescriptionProvider>((options, service) =>
            //        {
            //            options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            //            // 添加文档信息
            //            foreach (var item in service.ApiVersionDescriptions)
            //            {
            //                options.SwaggerDoc(item.GroupName, CreateInfoForApiVersion(item));
            //            }
            //            //给swagger添加过滤器
            //            //options.OperationFilter<SwaggerParameterFilter>();
            //            // 加载XML注释
            //            var xmlFile = $"{ XmlName }.xml";
            //            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile), true);
            //        });

        }

        static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
        {
            var info = new OpenApiInfo()
            {
                //标题
                Title = $".NET Core API for 测试项目 {description.ApiVersion}",
                //当前版本
                Version = description.ApiVersion.ToString(),
                //文档说明
                Description = "api项目 当前环境-",
                ////联系方式
                //Contact = new OpenApiContact() { Name = "标题", Email = "", Url = null },
                //TermsOfService = new Uri(""),
                ////许可证
                //License = new OpenApiLicense() { Name = "文档", Url = new Uri("") }
            };
            //当有弃用标记时的提示信息
            if (description.IsDeprecated)
            {
                info.Description += " - 此版本已放弃兼容";
            }
            return info;
        }
    }

    /// <summary>
    /// 自定义版本
    /// </summary>
    public class CustomApiVersion
    {
        /// <summary>
        /// Api接口版本 自定义
        /// </summary>
        public enum ApiVersions
        {
            /// <summary>
            /// V1 版本
            /// </summary>
            V1 = 1,
            /// <summary>
            /// V2 版本
            /// </summary>
            V2 = 2,
        }
    }

}

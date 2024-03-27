using CoreWebAPI.Common;
//using log4net;
using Microsoft.AspNetCore.Builder;
using System;
using System.IO;
using System.Linq;
using static CoreWebAPI.Extensions.CustomApiVersion;
using CoreWebAPI.Common.Helper;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace CoreWebAPI.Middlewares
{
    /// <summary>
    /// Swagger中间件
    /// </summary>
    public static class SwaggerMildd
    {
        //private static readonly ILog log = LogManager.GetLogger(typeof(SwaggerMildd));
        public static void UseSwaggerMildd(this IApplicationBuilder app)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));

            //验证swagger权限
            app.UseMiddleware<SwaggerAuthMilddleware>();

            //启用中间件服务生成Swagger
            app.UseSwagger();
            //指定Swagger JSON终结点
            app.UseSwaggerUI(c =>
            {
                //根据版本名称倒序 遍历展示
                var ApiName = Appsettings.App(new string[] { "Startup", "Swagger", "ApiName" });
                var ApiVsersions = Appsettings.App<string>(new string[] { "Startup", "Swagger", "Versions" });
                ApiVsersions.ForEach(version =>
                {
                    c.SwaggerEndpoint($"/swagger/{version}/swagger.json", $"{ApiName} {version}");
                });

                //c.SwaggerEndpoint($"https://petstore.swagger.io/v2/swagger.json", $"{ApiName} pet");

                //// 将swagger首页，设置成我们自定义的页面，记得这个字符串的写法：{项目名.index.html}
                //if (streamHtml.Invoke() == null)
                //{
                //    var msg = "index.html的属性，必须设置为嵌入的资源";
                //    //log.Error(msg);
                //    throw new Exception(msg);
                //}
                //c.IndexStream = streamHtml;

                //if (Permissions.IsUseIds4)
                //{
                //    c.OAuthClientId("blogadminjs"); 
                //}

                //设置根节点访问路径，设置为空，表示直接在根域名（localhost:8001）访问该文件,注意localhost:8001/swagger是访问不到的，去launchSettings.json把launchUrl去掉，如果你想换一个路径，直接写名字即可，比如直接写c.RoutePrefix = "doc";
                c.RoutePrefix = "";
            });

            ////添加Swagger中间件，主要用于拦截swagger.json请求，从而可以获取返回所需的接口架构信息
            //app.UseSwagger(opt =>
            //{
            //    //路由模板，默认值是/swagger/{documentName}/swagger.json，这个属性很重要！而且这个属性中必须包含{documentName}参数。
            //    //opt.RouteTemplate= "/swagger/{documentName}/swagger.json";
            //    // 表示按Swagger2.0格式序列化生成swagger.json，这个不推荐使用，尽可能的使用新版本的就可以了
            //    //opt.SerializeAsV2
            //});
            ////添加SwaggerUI中间件，主要用于拦截swagger / index.html页面请求，返回页面给前端
            //app.UseSwaggerUI(options =>
            //{
            //    // 为每个版本创建一个JSON
            //    foreach (var description in provider.ApiVersionDescriptions)
            //    {
            //        //这个属性是往SwaggerUI页面head标签中添加我们自己的代码，比如引入一些样式文件，或者执行自己的一些脚本代码
            //        //options.HeadContent += $"<script type='text/javascript'>alert('欢迎来到SwaggerUI页面')</script>";

            //        //展示默认头部显示的下拉版本信息
            //        //options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
            //        //自由指定头部显示的下拉版本内容
            //        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", "coreApi" + description.ApiVersion);

            //        //如果是为空 访问路径就为 根域名/index.html,注意localhost:8001/swagger是访问不到的
            //        //options.RoutePrefix = string.Empty;
            //        // 如果你想换一个路径，直接写名字即可，比如直接写c.RoutePrefix = "swagger"; 则访问路径为 根域名/swagger/index.html
            //        options.RoutePrefix = "swagger";
            //    }
            //});
        }
    }
}

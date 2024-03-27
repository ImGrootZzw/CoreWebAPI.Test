using CoreWebAPI.Common.Helper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CoreWebAPI.Common.Redis;
using CoreWebAPI.Extensions.Authorizations.Helpers;
using CoreWebAPI.Common.HttpRestSharp;
using CoreWebAPI.Common.LogHelper;

namespace CoreWebAPI.Authorizations.Policys
{
    /// <summary>
    /// 权限授权处理器
    /// </summary>
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        /// <summary>
        /// 验证方案提供对象
        /// </summary>
        public IAuthenticationSchemeProvider Schemes { get; set; }
        private readonly IHttpContextAccessor _accessor;
        private readonly IRedisBasketRepository _cache;

        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="schemes"></param>
        /// <param name="accessor"></param>
        /// <param name="cache"></param>
        public PermissionHandler(IAuthenticationSchemeProvider schemes, IHttpContextAccessor accessor, IRedisBasketRepository cache)
        {
            _accessor = accessor;
            _cache = cache;
            Schemes = schemes;
        }

        /// <summary>
        /// 重写异步处理程序
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requirement"></param>
        /// <returns></returns>
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            try
            {
                var httpContext = _accessor.HttpContext;

                if (httpContext != null)
                {
                    var questUrl = httpContext.Request.Path.Value.ToLower();

                    //判断是否开启认证
                    var isAuthentication = Appsettings.App(new string[] { "Authentication", "Enabled" }).GetCBool();
                    if (isAuthentication == false)
                    {
                        context.Succeed(requirement);
                        return;
                    }

                    //检测是否包含'Authorization'请求头
                    if (httpContext.Request.Headers.ContainsKey("x-token"))
                    {
                        var userUid = httpContext.Request.Headers["x-user"].GetCString();
                        var token = httpContext.Request.Headers["x-token"].GetCString();
                        var corp = httpContext.Request.Headers["x-corp"].GetCString();
                        var domain = httpContext.Request.Headers["x-domain"].GetCString();

                        //是否远程验证（除平台项目外，都需要到平台统一认证中心）
                        var isRemote = Appsettings.App(new string[] { "Authentication", "RemoteAuthentication" }).GetCBool();
                        if (isRemote)
                        {
                            var remoteAuthenticationUrl = Appsettings.App(new string[] { "Authentication", "RemoteUrl" }).GetCString();
                            ResultModel rm = new ResultModel();
                            switch (Appsettings.App(new string[] { "Authentication", "Type" }).GetCString().ToLower())
                            {
                                case "progress":
                                    HttpRequestHelper httpHelper = new HttpRequestHelper
                                    {
                                        Uri = remoteAuthenticationUrl,
                                        Type = HttpType.POST,
                                        BodyType = BodyType.Json,
                                        Body = new Dictionary<string, object>
                                        {
                                            { "corp", corp},
                                            { "domain", domain},
                                            { "userid", userUid},
                                            { "sessionid", token}
                                        }
                                    };
                                    rm = (ResultModel)JsonHelper.JsonToObject(httpHelper.Request(), typeof(ResultModel));
                                    break;
                                default:
                                    rm = HttpHelper.GetApi<ResultModel>(remoteAuthenticationUrl, "", $"userUid={userUid}&token={token}");
                                    break;
                            }

                            SerilogHelper.WriteLog("PermissionHandler", "HandleRequirementAsync", "rm", JsonHelper.ObjectToJson(rm));
                            if (rm.Code != 10000)
                            {
                                context.Fail();
                                return;
                            }
                            else
                            {
                                context.Succeed(requirement);
                                return;
                            }
                        }
                        else
                        {
                            TokenModelJwt tmj = JwtHelper.SerializeJwt(token);
                            var cacheToken = await _cache.GetValue(tmj.Uid.GetCString());
                            if (cacheToken.GetCString().Trim('\"') != token)
                            {
                                context.Fail();
                                return;
                            }
                            else
                            {
                                if (tmj.Expiration < DateTime.Now)
                                {
                                    var cacheRefreshToken = await _cache.GetValue(tmj.Uid.GetCString() + "_Expiration");
                                    if (cacheRefreshToken.GetCString().Trim('\"').GetCDate() < DateTime.Now)
                                    {
                                        context.Fail();
                                        return;
                                    }
                                    else
                                    {
                                        httpContext.Response.Headers["X-RefreshToken"] = JwtHelper.IssueJwt(tmj);
                                    }
                                }

                                await _cache.Set(tmj.Uid.GetCString() + "_Expiration", DateTime.Now.AddSeconds(7200).ToString(), TimeSpan.FromMinutes(120));
                                context.Succeed(requirement);
                                return;
                            };
                        }
                    }
                    else
                    {
                        context.Fail();
                        return;
                    }
                }
                else
                {
                    context.Fail();
                    return;
                }
            }
            catch (Exception ex)
            {
                SerilogHelper.WriteErrorLog("PermissionHandler", "HandleRequirementAsync", ex);
                context.Fail();
                return;
            }
        }
    }

    /// <summary>
    /// 通用返回信息类
    /// </summary>
    public class ResultModel
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public int Code { get; set; } = 10000;
        /// <summary>
        /// 返回信息
        /// </summary>
        public string Msg { get; set; } = "执行成功";
        /// <summary>
        /// 返回数据集合
        /// </summary>
        public object Data { get; set; }
        /// <summary>
        /// 方法名
        /// </summary>
        public string MethodName { get; set; }
    }
}

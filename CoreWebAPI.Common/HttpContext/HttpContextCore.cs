using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using CoreWebAPI.Common.Helper;

namespace CoreWebAPI.Common.HttpContext
{
    public class HttpContextCore : IHttpContextCore
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly ILogger<HttpContextCore> _logger;

        public HttpContextCore(IHttpContextAccessor accessor, ILogger<HttpContextCore> logger)
        {
            _accessor = accessor;
            _logger = logger;
        }

        public string IP => GetIP();
        private string GetIP()
        {
            if (_accessor is null)
                return "IHttpContextAccessor未注册，请在startup中添加app.UseStaticHttpContext();";
            if (_accessor.HttpContext is null || _accessor.HttpContext.Request is null || _accessor.HttpContext.Request.Headers is null)
                return "";
            if (_accessor.HttpContext.Request.Headers.ContainsKey("X-Real-IP"))
            {
                return _accessor.HttpContext.Request.Headers["X-Real-IP"].GetCString();
            }
            else if (_accessor.HttpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                return _accessor.HttpContext.Request.Headers["X-Forwarded-For"].GetCString();
            }
            var ip = _accessor.HttpContext.Request.Headers["X-Forwarded-For"].GetCString();
            if (string.IsNullOrEmpty(ip))
            {
                ip = _accessor.HttpContext.Connection.RemoteIpAddress.GetCString();
            }
            return ip;
        }

        public string API => GetAPI();
        private string GetAPI()
        {
            if (_accessor is null)
                return "IHttpContextAccessor未注册，请在startup中添加app.UseStaticHttpContext();";
            if (_accessor.HttpContext is null || _accessor.HttpContext.Request is null)
                return "";
            return _accessor.HttpContext.Request.Path.GetCString().TrimEnd('/').ToLower();
        }

        public string User => GetUser();
        private string GetUser()
        {
            if (_accessor is null)
                return "IHttpContextAccessor未注册，请在startup中添加app.UseStaticHttpContext();";
            if (_accessor.HttpContext is null || _accessor.HttpContext.Request is null || _accessor.HttpContext.Request.Headers is null)
                return "";
            return _accessor.HttpContext.Request.Headers["x-user"].GetCString();
        }
    }
}

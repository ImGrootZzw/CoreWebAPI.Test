using CoreWebAPI.Common.Helper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreWebAPI.Common.HttpContext
{
    public static class StaticHttpContext
    {
        private static IHttpContextAccessor _accessor;

        public static Microsoft.AspNetCore.Http.HttpContext Current => _accessor.HttpContext;

        public static void Configure(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public static string IP => GetIP();
        private static string GetIP()
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

        public static string API => GetAPI();
        private static string GetAPI()
        {
            if (_accessor is null)
                return "IHttpContextAccessor未注册，请在startup中添加app.UseStaticHttpContext();";
            if (_accessor.HttpContext is null || _accessor.HttpContext.Request is null)
                return "";
            return _accessor.HttpContext.Request.Path.GetCString().TrimEnd('/').ToLower();
        }

        public static string User => GetUser();
        private static string GetUser()
        {
            if (_accessor is null)
                return "IHttpContextAccessor未注册，请在startup中添加app.UseStaticHttpContext();";
            if (_accessor.HttpContext is null || _accessor.HttpContext.Request is null || _accessor.HttpContext.Request.Headers is null)
                return "";
            return _accessor.HttpContext.Request.Headers["x-user"].GetCString();
        }
    }
}

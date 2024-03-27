
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Security.Claims;

namespace CoreWebAPI.Common.HttpContext
{
    public interface IHttpContextCore
    {
        /// <summary>
        /// IP
        /// </summary>
        string IP { get; }

        /// <summary>
        /// API Url
        /// </summary>
        string API { get; }

        /// <summary>
        /// User，需在Header头中添加x-user传值
        /// </summary>
        string User { get; }
    }
}

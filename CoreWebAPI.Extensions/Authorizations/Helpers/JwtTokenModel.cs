using System;

namespace CoreWebAPI.Extensions.Authorizations.Helpers
{
    /// <summary>
    /// 令牌
    /// </summary>
    public class TokenModelJwt
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Uid { get; set; }
        /// <summary>
        /// 角色
        /// </summary>
        public string Role { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime Expiration { get; set; }
        /// <summary>
        /// 永久
        /// </summary>
        public bool IsPermanent { get; set; }

    }
}

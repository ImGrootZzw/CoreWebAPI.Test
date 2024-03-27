using System;
using System.Threading.Tasks;
using CoreWebAPI.Common.Helper;
using CoreWebAPI.Common.Redis;
using CoreWebAPI.Extensions.Authorizations.Helpers;

namespace CoreWebAPI.Extensions.Authorizations.Policys
{
    public class JwtTokenHandler
    {

        // 重写异步处理程序
        public async Task<bool> CheckToken(string token, IRedisBasketRepository _cache)
        {
            try
            {
                TokenModelJwt tmj = JwtHelper.SerializeJwt(token);

                var cacheToken = await _cache.GetValue(tmj.Uid);
                if (cacheToken.GetCString().Trim('\"') != token)
                {
                    return false;
                }
                else
                {
                    if (tmj.Expiration < DateTime.Now)
                    {
                        var cacheRefreshToken = await _cache.GetValue(tmj.Uid + "_Expiration");
                        if (cacheRefreshToken.GetCString().Trim('\"').GetCDate() < DateTime.Now)
                        {
                            return false;
                        }
                        else
                        {
                            //httpContext.Response.Headers["X-RefreshToken"] = JwtHelper.IssueJwt(tmj);
                        }

                        //TokenModelJwt tmjRT = JwtHelper.SerializeJwt(cacheRefreshToken);
                        //if (tmjRT.Expiration < DateTime.Now)
                        //    context.Fail();
                    }

                    await _cache.Set(tmj.Uid + "_Expiration", DateTime.Now.AddSeconds(7200).ToString(), TimeSpan.FromMinutes(120));

                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            return true;
        }
    }
}

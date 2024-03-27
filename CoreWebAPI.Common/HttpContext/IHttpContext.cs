using System.Collections.Generic;
using System.Security.Claims;

namespace CoreWebAPI.Common.HttpContext
{
    public interface IHttpContext
    {
        string Name { get; }
        int ID { get; }
        bool IsAuthenticated();
        IEnumerable<Claim> GetClaimsIdentity();
        List<string> GetClaimValueByType(string ClaimType);

        string GetToken();
        List<string> GetUserInfoFromToken(string ClaimType);
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreWebAPINuGet.Test.WebAPI.Controllers.Basic
{
    /// <summary>
    /// Basic
    /// </summary>
    [Route("api/[controller]/[action]")]
    [Authorize("Permission")]
    [ApiController]
    public class BasicController : ControllerBase
    {
        

    }
}

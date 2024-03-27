using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreWebAPI.Test.WebAPI.Controllers.Basic
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

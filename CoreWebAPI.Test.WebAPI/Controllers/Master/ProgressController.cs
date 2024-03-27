using CoreWebAPI.Common.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CoreWebAPI.Test.WebAPI.Controllers.Master
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProgressController : ControllerBase
    {

        /// <summary>
        /// Test
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetFile()
        {
            


            return Ok("");
        }

    }
    
}

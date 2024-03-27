using CoreWebAPI.Common.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

namespace CoreWebAPI.Test.WebAPI.Controllers.Master
{
    [Route("api/[controller]")]
    [ApiController]
    public class NacosController : ControllerBase
    {
        //private readonly IConfiguration _configuration;
        //private readonly AppSettings _settings;
        //private readonly AppSettings _sSettings;
        //private readonly AppSettings _mSettings;

        private readonly Nacos.V2.INacosNamingService _svc;

        public NacosController(Nacos.V2.INacosNamingService svc)
        {
            _svc = svc;
        }

        [HttpGet("test")]
        public async Task<IActionResult> Test()
        {
            // need to know the service name.
            var instance = await _svc.SelectOneHealthyInstance("App2", "DEFAULT_GROUP");
            var host = $"{instance.Ip}:{instance.Port}";

            var baseUrl = instance.Metadata.TryGetValue("secure", out _)
                ? $"https://{host}"
                : $"http://{host}";

            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                return Ok("empty");
            }

            var url = $"{baseUrl}/api/values";

            using (HttpClient client = new HttpClient())
            {
                var result = await client.GetAsync(url);
                return Ok(await result.Content.ReadAsStringAsync());
            }
        }
    }
}

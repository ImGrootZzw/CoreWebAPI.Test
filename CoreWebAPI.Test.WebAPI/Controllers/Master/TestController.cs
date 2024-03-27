using CoreWebAPI.Common.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreWebAPI.Test.WebAPI.Controllers.Master
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestController : ControllerBase
    {

        /// <summary>
        /// Test
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetFile()
        {
            HttpRequestHelper httpRequest = new HttpRequestHelper()
            {
                Uri= "http://localhost:801/123.pdf",
                Type = HttpType.GET
            };
            var rt = httpRequest.Request();

            return Ok(rt);
        }

        /// <summary>
        /// Test
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> TestJson(string json)
        {
            var test = Appsettings.App("Startup", "MaxRequestBodySize");

            var listModel = new List<Scfconfirmget_out_model>()
            {
                new Scfconfirmget_out_model()
                {
                    scfconfirmgetout_domain = "12",
                    Scfconfirmgetout_payamt = false
                }
            };
            var str = JsonHelper.ObjectToJson(listModel);
            string jsonstr = "[{\"scfconfirmgetout_domain\": \"ACS\", \"scfconfirmgetout_entity\": \"ACS\", \"scfconfirmgetout_vend\": \"000002\", \"scfconfirmgetout_vdname\": \"AUTOLIV ASP PROMONTORY FACILITY\", \"scfconfirmgetout_vdbkacct\": \"\", \"scfconfirmgetout_batchid\": \"173663\", \"scfconfirmgetout_voref\": \"071001\", \"scfconfirmgetout_votype\": \"\", \"scfconfirmgetout_vostat\": \"\", \"scfconfirmgetout_invnbr\": \"\", \"scfconfirmgetout_curr\": \"usd\", \"scfconfirmgetout_apbank\": \"Z9\", \"scfconfirmgetout_invdate\": \"2019-07-10T00:00:00\", \"scfconfirmgetout_effdate\": \"2019-07-10T00:00:00\", \"scfconfirmgetout_duedate\": \"2019-10-29T00:00:00\", \"scfconfirmgetout_voamt\": 1.0, \"scfconfirmgetout_openamt\": 1.0, \"scfconfirmgetout_holdamt\": 0.0, \"scfconfirmgetout_prepay\": false, \"scfconfirmgetout_payamt\": 1.0, \"scfconfirmgetout_matdate\": \"0001-01-01T00:00:00\"} ]";
            var outmodel = JsonHelper.JsonToObject(jsonstr, typeof(List<Scfconfirmget_out_model>));
            return Ok("123");
        }

        /// <summary>
        /// Test
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> TestFile(IFormFile file)
        {


            var listModel = new List<Scfconfirmget_out_model>()
            {
                new Scfconfirmget_out_model()
                {
                    scfconfirmgetout_domain = "12",
                    Scfconfirmgetout_payamt = false
                }
            };
            var str = JsonHelper.ObjectToJson(listModel);
            string jsonstr = "[{\"scfconfirmgetout_domain\": \"ACS\", \"scfconfirmgetout_entity\": \"ACS\", \"scfconfirmgetout_vend\": \"000002\", \"scfconfirmgetout_vdname\": \"AUTOLIV ASP PROMONTORY FACILITY\", \"scfconfirmgetout_vdbkacct\": \"\", \"scfconfirmgetout_batchid\": \"173663\", \"scfconfirmgetout_voref\": \"071001\", \"scfconfirmgetout_votype\": \"\", \"scfconfirmgetout_vostat\": \"\", \"scfconfirmgetout_invnbr\": \"\", \"scfconfirmgetout_curr\": \"usd\", \"scfconfirmgetout_apbank\": \"Z9\", \"scfconfirmgetout_invdate\": \"2019-07-10T00:00:00\", \"scfconfirmgetout_effdate\": \"2019-07-10T00:00:00\", \"scfconfirmgetout_duedate\": \"2019-10-29T00:00:00\", \"scfconfirmgetout_voamt\": 1.0, \"scfconfirmgetout_openamt\": 1.0, \"scfconfirmgetout_holdamt\": 0.0, \"scfconfirmgetout_prepay\": false, \"scfconfirmgetout_payamt\": 1.0, \"scfconfirmgetout_matdate\": \"0001-01-01T00:00:00\"} ]";
            var outmodel = JsonHelper.JsonToObject(jsonstr, typeof(List<Scfconfirmget_out_model>));
            return Ok(JsonHelper.JsonToObject<List<Scfconfirmget_out_model>>(jsonstr));
        }
    }

    public class Scfconfirmget_out_model
    {
        public string scfconfirmgetout_domain { get; set; }
        public string scfconfirmgetout_entity { get; set; }
        public string scfconfirmgetout_vend { get; set; }
        public string scfconfirmgetout_vdname { get; set; }
        public string scfconfirmgetout_vdbkacct { get; set; }
        public string scfconfirmgetout_batchid { get; set; }
        public string scfconfirmgetout_voref { get; set; }
        public string scfconfirmgetout_votype { get; set; }
        public string scfconfirmgetout_vostat { get; set; }
        public string scfconfirmgetout_invnbr { get; set; }
        public string scfconfirmgetout_curr { get; set; }
        public string scfconfirmgetout_apbank { get; set; }
        public DateTime scfconfirmgetout_invdate { get; set; }
        public DateTime scfconfirmgetout_effdate { get; set; }
        public DateTime scfconfirmgetout_duedate { get; set; }
        public decimal scfconfirmgetout_voamt { get; set; }
        public decimal scfconfirmgetout_openamt { get; set; }
        public decimal scfconfirmgetout_holdamt { get; set; }
        public decimal scfconfirmgetout_prepay { get; set; }
        public bool Scfconfirmgetout_payamt { get; set; }
        //public DateTime scfconfirmgetout_Newmatdate { get; set; }
        public DateTime scfconfirmgetout_matdate { get; set; }
    }
}

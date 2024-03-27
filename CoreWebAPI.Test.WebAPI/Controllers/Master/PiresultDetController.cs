using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CoreWebAPI.Common.Helper;
using CoreWebAPI.Repository;
using CoreWebAPI.Test.IServices.Master;
using CoreWebAPI.Test.Models;
using CoreWebAPI.Test.Models.Models;
using CoreWebAPI.Test.Models.ViewModels;
using CoreWebAPI.Test.WebAPI.Controllers.Basic;

namespace CoreWebAPI.Test.WebAPI.Controllers.Master
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    public class PiresultDetController : BasicController
    {
        private readonly ILogger<PiresultDetController> _logger;
        private readonly IHttpContextAccessor _accessor;
        private readonly IPiresultDetService _piresultdetService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="piresultdetService"></param>
        /// <param name="accessor"></param>
        /// <param name="logger"></param>
        public PiresultDetController(IHttpContextAccessor accessor, ILogger<PiresultDetController> logger, IPiresultDetService piresultdetService)
        {
            _accessor = accessor;
            _logger = logger;
            _piresultdetService = piresultdetService;
        }


        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="piresultdetSearchModel"></param>
        [ProducesResponseType(typeof(ResultModelReturn<PageModelReturn<PiresultDet>>), 200)]
        [HttpPost]
        public async Task<IActionResult> Get(PiresultDetSearchModel piresultdetSearchModel)
        {
            return Ok(await _piresultdetService.Get(piresultdetSearchModel));
        }

        ///// <summary>
        ///// 查询
        ///// </summary>
        ///// <param name="piresultdetSearchModel"></param>
        //[ProducesResponseType(typeof(ResultModelReturn<PageModelReturn<PiresultDet>>), 200)]
        //[HttpPost]
        //public async Task<IActionResult> GetMain(PiresultmainMstrSearchModel piresultdetSearchModel)
        //{
        //    return Ok(await _piresultdetService.GetMain(piresultdetSearchModel));
        //}

        /// <summary>
        /// 查询明细
        /// </summary>
        /// <param name="piresultdetSearchModel"></param>
        [ProducesResponseType(typeof(ResultModelReturn<PageModelReturn<PiresultDetViewModel>>), 200)]
        [HttpPost]
        public async Task<IActionResult> GetDetail(PiresultDetSearchModel piresultdetSearchModel)
        {
            return Ok(await _piresultdetService.GetDetail(piresultdetSearchModel));
        }

        /// <summary>
        /// 查询Bom明细
        /// </summary>
        /// <param name="piresultdetSearchModel"></param>
        [ProducesResponseType(typeof(ResultModelReturn<PageModelReturn<PiresultDetViewModel>>), 200)]
        [HttpPost]
        public async Task<IActionResult> GetBomDetail(PiresultDetSearchModel piresultdetSearchModel)
        {
            return Ok(await _piresultdetService.GetBomDetail(piresultdetSearchModel));
        }

        /// <summary>
        /// 查询NG检测明细
        /// </summary>
        /// <param name="piresultdetSearchModel"></param>
        [ProducesResponseType(typeof(ResultModelReturn<PageModelReturn<PiresultDetViewModel>>), 200)]
        [HttpPost]
        public async Task<IActionResult> GetNGDetail(PiresultDetSearchModel piresultdetSearchModel)
        {
            return Ok(await _piresultdetService.GetNGDetail(piresultdetSearchModel));
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="piresultdet"></param>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PiresultDet piresultdet)
        {
            return Ok(await _piresultdetService.Post(piresultdet));
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="piresultdet"></param>
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] PiresultDet piresultdet)
        {
            return Ok(await _piresultdetService.Put(piresultdet));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="listPiresultDet"></param>
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] List<PiresultDet> listPiresultDet)
        {
            return Ok(await _piresultdetService.Delete(listPiresultDet));
        }

        /// <summary>
        /// 下载模板
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> DownloadTemplate()
        {
            ResultModel rm = await _piresultdetService.DownloadTemplate();
            if (rm.ResultCode != "100000")
            {
                return Ok(rm);
            }
            byte[] fileBytes = System.IO.File.ReadAllBytes(rm.Data.GetCString());
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "piresultdetLoad.xlsx");
        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="file"></param>
        [HttpPost]
        public async Task<IActionResult> Import(IFormFile file)
        {
            return Ok(await _piresultdetService.Import(file));
        }

        /// <summary>
        /// 列表导入
        /// </summary>
        /// <param name="listPiresultDet"></param>
        [HttpPost]
        public async Task<IActionResult> ImportList([FromBody] List<PiresultDet> listPiresultDet)
        {
            return Ok(await _piresultdetService.ImportList(listPiresultDet));
        }

        /// <summary>
        /// 输出
        /// </summary>
        /// <param name="piresultdetSearchModel"></param>
        [HttpPost]
        public async Task<IActionResult> Export(PiresultDetSearchModel piresultdetSearchModel)
        {
            ResultModel rm = await _piresultdetService.Export(piresultdetSearchModel);
            if (rm.ResultCode != "100000")
            {
                return Ok(rm);
            }
            byte[] fileBytes = System.IO.File.ReadAllBytes(rm.Data.GetCString());
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "piresultdetExport.xlsx");
        }

    }
}

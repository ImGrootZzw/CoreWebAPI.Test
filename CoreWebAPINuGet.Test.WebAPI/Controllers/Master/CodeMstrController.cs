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
using CoreWebAPINuGet.Test.IServices.Master;
using CoreWebAPINuGet.Test.Models;
using CoreWebAPINuGet.Test.Models.Models;
using CoreWebAPINuGet.Test.Models.ViewModels;
using CoreWebAPINuGet.Test.WebAPI.Controllers.Basic;

namespace CoreWebAPINuGet.Test.WebAPI.Controllers.Master
{
    /// <summary>
    /// 通用代码维护
    /// </summary>
    [ApiController]
    public class CodeMstrController : BasicController
    {
        private readonly ILogger<CodeMstrController> _logger;
        private readonly IHttpContextAccessor _accessor;
        private readonly ICodeMstrService _codemstrService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="codemstrService"></param>
        /// <param name="accessor"></param>
        /// <param name="logger"></param>
        public CodeMstrController(IHttpContextAccessor accessor, ILogger<CodeMstrController> logger, ICodeMstrService codemstrService)
        {
            _accessor = accessor;
            _logger = logger;
            _codemstrService = codemstrService;
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="searchModel">查询参数</param>
        [ProducesResponseType(typeof(ResultModelReturn<PageModelReturn<CodeMstr>>), 200)]
        [HttpPost]
        public async Task<IActionResult> Get([FromBody] BasicSearchModel searchModel)
        {
            return Ok(await _codemstrService.Get(searchModel));
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="codemstr"></param>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CodeMstr codemstr)
        {
            return Ok(await _codemstrService.Post(codemstr));
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="codemstr"></param>
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] CodeMstr codemstr)
        {
            return Ok(await _codemstrService.Put(codemstr));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="listCodeMstr"></param>
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] List<CodeMstr> listCodeMstr)
        {
            return Ok(await _codemstrService.Delete(listCodeMstr));
        }

        /// <summary>
        /// 下载模板
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> DownloadTemplate()
        {
            ResultModel rm = await _codemstrService.DownloadTemplate();
            if (rm.ResultCode != "100000")
            {
                return Ok(rm);
            }
            byte[] fileBytes = System.IO.File.ReadAllBytes(rm.Data.GetCString());
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "codemstrLoad.xlsx");
        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="file"></param>
        [HttpPost]
        public async Task<IActionResult> Import(IFormFile file)
        {
            return Ok(await _codemstrService.Import(file));
        }

        /// <summary>
        /// 输出
        /// </summary>
        /// <param name="searchModel">查询参数</param>
        [HttpPost]
        public async Task<IActionResult> Export([FromBody] BasicSearchModel searchModel)
        {
            ResultModel rm = await _codemstrService.Export(searchModel);
            if (rm.ResultCode != "100000")
            {
                return Ok(rm);
            }
            byte[] fileBytes = System.IO.File.ReadAllBytes(rm.Data.GetCString());
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "codemstrExport.xlsx");
        }

    }
}

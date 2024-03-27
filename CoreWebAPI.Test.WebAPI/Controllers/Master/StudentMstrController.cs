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
    public class StudentMstrController : BasicController
    {
        private readonly ILogger<StudentMstrController> _logger;
        private readonly IHttpContextAccessor _accessor;
        private readonly IStudentMstrService _studentmstrService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="studentmstrService"></param>
        /// <param name="accessor"></param>
        /// <param name="logger"></param>
        public StudentMstrController(IHttpContextAccessor accessor, ILogger<StudentMstrController> logger, IStudentMstrService studentmstrService)
        {
            _accessor = accessor;
            _logger = logger;
            _studentmstrService = studentmstrService;
        }

        /// <summary>
        /// 查询所有字段
        /// </summary>
        /// <param name="searchModel"></param>
        [ProducesResponseType(typeof(ResultModelReturn<PageModelReturn<StudentMstr>>), 200)]
        [HttpPost]
        public async Task<IActionResult> GetAll(StudentMstrSearchModel searchModel)
        {
            return Ok(await _studentmstrService.Get(searchModel));
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="searchModel"></param>
        [ProducesResponseType(typeof(ResultModelReturn<PageModelReturn<StudentMstr>>), 200)]
        [HttpPost]
        public async Task<IActionResult> Get(BasicSearchModel searchModel)
        {
            return Ok(await _studentmstrService.Get(searchModel));
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="studentmstr"></param>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] StudentMstr studentmstr)
        {
            return Ok(await _studentmstrService.Post(studentmstr));
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="studentmstr"></param>
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] StudentMstr studentmstr)
        {
            return Ok(await _studentmstrService.Put(studentmstr));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="listStudentMstr"></param>
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] List<StudentMstr> listStudentMstr)
        {
            return Ok(await _studentmstrService.Delete(listStudentMstr));
        }

        /// <summary>
        /// 下载模板
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> DownloadTemplate()
        {
            ResultModel rm = await _studentmstrService.DownloadTemplate();
            if (rm.ResultCode != "100000")
            {
                return Ok(rm);
            }
            byte[] fileBytes = System.IO.File.ReadAllBytes(rm.Data.GetCString());
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "studentmstrLoad.xlsx");
        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="file"></param>
        [HttpPost]
        public async Task<IActionResult> Import(IFormFile file)
        {
            return Ok(await _studentmstrService.Import(file));
        }

        /// <summary>
        /// 列表导入
        /// </summary>
        /// <param name="listStudentMstr"></param>
        [HttpPost]
        public async Task<IActionResult> ImportList([FromBody] List<StudentMstr> listStudentMstr)
        {
            return Ok(await _studentmstrService.ImportList(listStudentMstr));
        }

        /// <summary>
        /// 输出
        /// </summary>
        /// <param name="searchModel"></param>
        [HttpPost]
        public async Task<IActionResult> Export(BasicSearchModel searchModel)
        {
            ResultModel rm = await _studentmstrService.Export(searchModel);
            if (rm.ResultCode != "100000")
            {
                return Ok(rm);
            }
            byte[] fileBytes = System.IO.File.ReadAllBytes(rm.Data.GetCString());
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "studentmstrExport.xlsx");
        }

    }
}

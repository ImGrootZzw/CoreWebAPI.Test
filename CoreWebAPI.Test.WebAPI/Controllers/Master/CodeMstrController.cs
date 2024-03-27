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
using System.Reflection;
using System.Runtime.InteropServices;

namespace CoreWebAPI.Test.WebAPI.Controllers.Master
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
        [AllowAnonymous]
        public async Task<IActionResult> Get([FromBody] BasicSearchModel searchModel)
        {
            ExcelHelper excel = new ExcelHelper("");
            //通过反射获得类注解
            MethodInfo info = typeof(ExcelHelper).GetMethod("DataTableToExcel");
            var ss = info.GetCustomAttributesData();

            // 获取类的XML注释
            System.Type type = typeof(ExcelHelper);
            string classDocumentation = GetXmlDocumentation(type);
            Console.WriteLine($"Class Documentation: {classDocumentation}");

            // 获取方法的XML注释
            MethodInfo methodInfo = type.GetMethod("MyMethod");
            string methodDocumentation = GetXmlDocumentation(methodInfo);
            Console.WriteLine($"Method Documentation: {methodDocumentation}");

            return Ok(await _codemstrService.Get(searchModel));
        }

        static string GetXmlDocumentation(MemberInfo memberInfo)
        {
            // 获取特性信息
            var customAttributes = memberInfo.GetCustomAttributesData();

            // 查找XML文档注释特性
            var xmlDocumentationAttribute = customAttributes.FirstOrDefault(attr => attr.AttributeType.Name == "DocumentationAttribute");

            // 获取注释信息
            string documentation = xmlDocumentationAttribute?.ConstructorArguments.FirstOrDefault().Value?.ToString();

            return documentation ?? "No documentation available";
        }

        [DllImport("MyLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern string test();

        ///// <summary>
        ///// 新增
        ///// </summary>
        ///// <param name="codemstr"></param>
        //[HttpPost]
        //public async Task<IActionResult> Post([FromBody] CodeMstr codemstr)
        //{
        //    return Ok(await _codemstrService.Post(codemstr));
        //}

        ///// <summary>
        ///// 编辑
        ///// </summary>
        ///// <param name="codemstr"></param>
        //[HttpPut]
        //public async Task<IActionResult> Put([FromBody] CodeMstr codemstr)
        //{
        //    return Ok(await _codemstrService.Put(codemstr));
        //}

        ///// <summary>
        ///// 删除
        ///// </summary>
        ///// <param name="listCodeMstr"></param>
        //[HttpDelete]
        //public async Task<IActionResult> Delete([FromBody] List<CodeMstr> listCodeMstr)
        //{
        //    return Ok(await _codemstrService.Delete(listCodeMstr));
        //}

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
        public async Task<IActionResult> Import(List<IFormFile> file)
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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Http;
using CoreWebAPI.Test.Models;
using CoreWebAPI.Test.Models.Models;
using CoreWebAPI.Test.Models.ViewModels;
using CoreWebAPI.Repository.IService;

namespace CoreWebAPI.Test.IServices.Master
{
    /// <summary>
    /// ÕâÊÇ×¢½â
    /// </summary>
    public interface ICodeMstrService : IBaseService<CodeMstr>
    {

        #region StandardInterface
        Task<ResultModel> Get(BasicSearchModel searchModel);
        Task<ResultModel> Post(CodeMstr codeMstr);
        Task<ResultModel> Put(CodeMstr codeMstr);
        new Task<ResultModel> Delete(List<CodeMstr> listCodeMstr);
        Task<ResultModel> DownloadTemplate();
        Task<ResultModel> Import(List<IFormFile> file);
        Task<ResultModel> Export(BasicSearchModel searchModel);
        #endregion

        #region CustomInterface
        #endregion

    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Http;
using CoreWebAPINuGet.Test.Models;
using CoreWebAPINuGet.Test.Models.Models;
using CoreWebAPINuGet.Test.Models.ViewModels;
using CoreWebAPI.Repository.IService;

namespace CoreWebAPINuGet.Test.IServices.Master
{
    public interface ICodeMstrService : IBaseService<CodeMstr>
    {

        #region StandardInterface
        Task<ResultModel> Get(BasicSearchModel searchModel);
        Task<ResultModel> Post(CodeMstr codeMstr);
        Task<ResultModel> Put(CodeMstr codeMstr);
        new Task<ResultModel> Delete(List<CodeMstr> listCodeMstr);
        Task<ResultModel> DownloadTemplate();
        Task<ResultModel> Import(IFormFile file);
        Task<ResultModel> Export(BasicSearchModel searchModel);
        #endregion

        #region CustomInterface
        #endregion

    }
}

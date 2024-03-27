using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Http;
using CoreWebAPI.Repository.IService;
using CoreWebAPI.Test.Models;
using CoreWebAPI.Test.Models.Models;
using CoreWebAPI.Test.Models.ViewModels;

namespace CoreWebAPI.Test.IServices.Master
{
    public interface IStudentMstrService : IBaseService<StudentMstr>
    {

        #region StandardInterface
        Task<ResultModel> Get(StudentMstrSearchModel studentMstrSearchModel);
        Task<ResultModel> Get(BasicSearchModel searchModel);
        Task<ResultModel> Post(StudentMstr studentMstr);
        Task<ResultModel> Put(StudentMstr studentMstr);
        new Task<ResultModel> Delete(List<StudentMstr> listStudentMstr);
        Task<ResultModel> DownloadTemplate();
        Task<ResultModel> Import(IFormFile file);
        Task<ResultModel> ImportList(List<StudentMstr> listStudentMstr);
        Task<ResultModel> Export(BasicSearchModel searchModel);
        #endregion

        #region CustomInterface
        #endregion

    }
}

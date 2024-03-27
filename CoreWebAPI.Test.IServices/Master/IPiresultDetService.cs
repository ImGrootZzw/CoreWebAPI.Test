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
    public interface IPiresultDetService : IBaseService<PiresultDet>
    {

        #region StandardInterface
        Task<ResultModel> Get(PiresultDetSearchModel piresultDetSearchModel);
        Task<ResultModel> Post(PiresultDet piresultDet);
        Task<ResultModel> Put(PiresultDet piresultDet);
        new Task<ResultModel> Delete(List<PiresultDet> listPiresultDet);
        Task<ResultModel> DownloadTemplate();
        Task<ResultModel> Import(IFormFile file);
        Task<ResultModel> ImportList(List<PiresultDet> listPiresultDet);
        Task<ResultModel> Export(PiresultDetSearchModel piresultDetSearchModel);
        #endregion

        #region CustomInterface
        //Task<ResultModel> GetMain(PiresultmainMstrSearchModel piresultmainMstrSearchModel);
        Task<ResultModel> GetDetail(PiresultDetSearchModel piresultDetSearchModel);
        Task<ResultModel> GetBomDetail(PiresultDetSearchModel piresultDetSearchModel);
        Task<ResultModel> GetNGDetail(PiresultDetSearchModel piresultDetSearchModel);
        #endregion

    }
}

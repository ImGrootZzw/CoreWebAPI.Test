using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using CoreWebAPI.Common.Helper;
using CoreWebAPI.Repository.Base;
using CoreWebAPI.Repository.Service;
using CoreWebAPI.Repository.UnitOfWork;
using CoreWebAPI.Test.Models;
using CoreWebAPI.Test.Models.Models;
using CoreWebAPI.Test.Models.ViewModels;
using CoreWebAPI.Test.IServices.Master;
using SqlSugar;
using CoreWebAPI.Common.LogHelper;
using CoreWebAPI.Repository;

namespace CoreWebAPI.Test.Services.Master
{
    public class PiresultDetService : BaseService<PiresultDet>, IPiresultDetService
    {
        private readonly IBaseRepository<PiresultDet> _dal;
        //private readonly IBaseRepository<PimachineMstr> _dalMachine;
        //private readonly IBaseRepository<PiopMstr> _dalOp;
        //private readonly IBaseRepository<PiptMstr> _dalPart;
        //private readonly IBaseRepository<PistdMstr> _dalStd;
        //private readonly IBaseRepository<PibomMstr> _dalBom;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _accessor;
        private readonly string user;
        private readonly string corp;
        private readonly string domain;
        private readonly string language;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dal"></param>
        /// <param name="unitOfWork"></param>
        /// <param name="accessor"></param>
        public PiresultDetService(IBaseRepository<PiresultDet> dal, IUnitOfWork unitOfWork, IHttpContextAccessor accessor)
        {
            base.BaseDal = dal;
            this._dal = dal;
            _unitOfWork = unitOfWork;
            _accessor = accessor;

            user = _accessor.HttpContext.Request.Headers["x-user"].GetCString();
            corp = _accessor.HttpContext.Request.Headers["x-corp"].GetCString();
            domain = _accessor.HttpContext.Request.Headers["x-domain"].GetCString();
            language = _accessor.HttpContext.Request.Headers["x-language"].GetCString();
        }

        #region StandardInterface
        /// <summary>
        /// 查询
        /// </summary>
        public async Task<ResultModel> Get(PiresultDetSearchModel piresultDetSearchModel)
        {
            ResultModel rm = new ResultModel();
            try
            {
                this.Query();

                Expression<Func<PiresultDet, bool>> whereExpression = we => 
                    we.PiresultCorpId.ToLower() == corp.ToLower()
                    && we.PiresultDomainId.ToLower() == domain.ToLower()
                    && (piresultDetSearchModel.PiresultProdline.GetIsEmptyOrNull() || we.PiresultProdline.ToLower().Contains(piresultDetSearchModel.PiresultProdline.ToLower()))
                    && (piresultDetSearchModel.PiresultDept.GetIsEmptyOrNull() || we.PiresultDept.ToLower().Contains(piresultDetSearchModel.PiresultDept.ToLower()))
                    && (piresultDetSearchModel.PiresultMachine.GetIsEmptyOrNull() || we.PiresultMachine.ToLower().Contains(piresultDetSearchModel.PiresultMachine.ToLower()))
                    && (piresultDetSearchModel.PiresultOp.GetIsEmptyOrNull() || we.PiresultOp.ToLower().Contains(piresultDetSearchModel.PiresultOp.ToLower()))
                    && (piresultDetSearchModel.PiresultPart.GetIsEmptyOrNull() || we.PiresultPart.ToLower().Contains(piresultDetSearchModel.PiresultPart.ToLower()))
                    && (piresultDetSearchModel.PiresultQrcode.GetIsEmptyOrNull() || we.PiresultQrcode.ToLower().Contains(piresultDetSearchModel.PiresultQrcode.ToLower()))
                    && (piresultDetSearchModel.PiresultLot.GetIsEmptyOrNull() || we.PiresultLot.ToLower().Contains(piresultDetSearchModel.PiresultLot.ToLower()))
                    && (piresultDetSearchModel.PiresultDatatype.GetIsEmptyOrNull() || we.PiresultDatatype.ToLower().Contains(piresultDetSearchModel.PiresultDatatype.ToLower()))
                    && (piresultDetSearchModel.PiresultDatetimeFrom.GetIsEmptyOrNull() || we.PiresultDatetime >= piresultDetSearchModel.PiresultDatetimeFrom.GetCDate())
                    && (piresultDetSearchModel.PiresultDatetimeTo.GetIsEmptyOrNull() || we.PiresultDatetime <= piresultDetSearchModel.PiresultDatetimeTo.GetCDate())
                    && (piresultDetSearchModel.PiresultResult.GetIsEmptyOrNull() || we.PiresultResult.ToLower().Contains(piresultDetSearchModel.PiresultResult.ToLower()))
                    && (piresultDetSearchModel.PiresultType.GetIsEmptyOrNull() || we.PiresultType.ToLower().Contains(piresultDetSearchModel.PiresultType.ToLower()))
                    && (piresultDetSearchModel.PiresultReleased.GetIsEmptyOrNull() || we.PiresultReleased == piresultDetSearchModel.PiresultReleased.GetCBool())
                ;
                var pageSort = ClassMapSqlsugar.GetDBSort<PiresultDet>(piresultDetSearchModel.SortList);
                var data = await _dal.QueryPage(whereExpression, piresultDetSearchModel.PageIndex, piresultDetSearchModel.PageSize, pageSort);

                rm.Data = data;
            }
            catch (Exception ex)
            {
                rm.ResultCode = "1000001";
                rm.ResultMsg = ex.Message;
            }
            return rm;
        }

        /// <summary>
        /// 新增
        /// </summary>
        public async Task<ResultModel> Post(PiresultDet piresultDet)
        {
            ResultModel rm = new ResultModel();
            try
            {
                #region "校验"
                Expression<Func<PiresultDet, bool>> whereExpression = we => 
                    we.PiresultCorpId.ToLower() == corp.ToLower()
                    && we.PiresultDomainId.ToLower() == domain.ToLower()
                    && we.PiresultProdline == piresultDet.PiresultProdline
                    && we.PiresultDept == piresultDet.PiresultDept
                    && we.PiresultMachine == piresultDet.PiresultMachine
                    && we.PiresultOp == piresultDet.PiresultOp
                    && we.PiresultPart == piresultDet.PiresultPart
                    && we.PiresultQrcode == piresultDet.PiresultQrcode
                    && we.PiresultLot == piresultDet.PiresultLot
                    && we.PiresultDatatype == piresultDet.PiresultDatatype
                    && we.PiresultDatetime == piresultDet.PiresultDatetime
                    && we.PiresultType == piresultDet.PiresultType
                ;
                var data = await _dal.Query(whereExpression);
                if(data.Count > 0)
                {
                    rm.ResultCode = "1000001";
                    rm.ResultMsg = "该记录已存在".GetMessage(1001023, language);
                    return rm;
                }

                ////校验设备是否存在
                //Expression<Func<PimachineMstr, bool>> whereExpressionMachine = we =>
                //    we.PimachineCorpId.ToLower() == corp.ToLower()
                //    && we.PimachineDomainId.ToLower() == domain.ToLower()
                //    && we.PimachineMachine.ToLower() == piresultDet.PiresultMachine.ToLower()
                //;
                //var dataMachine = await _dalMachine.Query(whereExpressionMachine);
                //if (dataMachine.Count == 0)
                //{
                //    rm.ResultCode = "1000001";
                //    rm.ResultMsg = "该设备不存在".GetMessage(1001023, language);
                //    return rm;
                //}

                ////校验工序是否存在
                //Expression<Func<PiopMstr, bool>> whereExpressionOp = we =>
                //    we.PiopCorpId.ToLower() == corp.ToLower()
                //    && we.PiopDomainId.ToLower() == domain.ToLower()
                //    && we.PiopOp.ToLower() == piresultDet.PiresultOp.ToLower()
                //;
                //var dataOp = await _dalOp.Query(whereExpressionOp);
                //if (dataOp.Count == 0)
                //{
                //    rm.ResultCode = "1000001";
                //    rm.ResultMsg = "该工序不存在".GetMessage(1001023, language);
                //    return rm;
                //}

                ////校验零件是否存在
                //Expression<Func<PiptMstr, bool>> whereExpressionPart = we =>
                //    we.PiptCorpId.ToLower() == corp.ToLower()
                //    && we.PiptDomainId.ToLower() == domain.ToLower()
                //    && we.PiptPart.ToLower() == piresultDet.PiresultPart.ToLower()
                //;
                //var dataPart = await _dalPart.Query(whereExpressionPart);
                //if (dataPart.Count == 0)
                //{
                //    rm.ResultCode = "1000001";
                //    rm.ResultMsg = "该零件不存在".GetMessage(1001023, language);
                //    return rm;
                //}

                ////校验检测项是否存在
                //Expression<Func<PistdMstr, bool>> whereExpressionStd = we =>
                //    we.PistdCorpId.ToLower() == corp.ToLower()
                //    && we.PistdDomainId.ToLower() == domain.ToLower()
                //    && we.PistdDatatype.ToLower() == piresultDet.PiresultDatatype.ToLower()
                //;
                //var dataStd = await _dalStd.Query(whereExpressionStd);
                //if (dataStd.Count == 0)
                //{
                //    rm.ResultCode = "1000001";
                //    rm.ResultMsg = "该检测项不存在".GetMessage(1001023, language);
                //    return rm;
                //}
                #endregion

                piresultDet.Id = Guid.NewGuid().ToString();
                piresultDet.PiresultCorpId = corp;
                piresultDet.PiresultDomainId = domain;
                piresultDet.PiresultCrtDatetime = DateTime.Now;
                piresultDet.PiresultCrtUser = user;
                piresultDet.PiresultCrtProg = "PiresultDetService-Post";
                piresultDet.PiresultReleased = false;
                var rtAdd = await _dal.Add(piresultDet);
                if (rtAdd != 1)
                {
                    rm.ResultCode = "1000001";
                    rm.ResultMsg = "插入失败".GetMessage(1001009, language);
                    return rm;
                }
            }
            catch (Exception ex)
            {
                rm.ResultCode = "1000001";
                rm.ResultMsg = ex.Message;
            }
            return rm;
        }

        /// <summary>
        /// 编辑
        /// </summary>
        public async Task<ResultModel> Put(PiresultDet piresultDet)
        {
            ResultModel rm = new ResultModel();
            try
            {
                #region "校验"
                Expression<Func<PiresultDet, bool>> whereExpression = we => 
                    we.PiresultCorpId.ToLower() == corp.ToLower()
                    && we.PiresultDomainId.ToLower() == domain.ToLower()
                    && we.PiresultProdline == piresultDet.PiresultProdline
                    && we.PiresultDept == piresultDet.PiresultDept
                    && we.PiresultMachine == piresultDet.PiresultMachine
                    && we.PiresultOp == piresultDet.PiresultOp
                    && we.PiresultPart == piresultDet.PiresultPart
                    && we.PiresultQrcode == piresultDet.PiresultQrcode
                    && we.PiresultLot == piresultDet.PiresultLot
                    && we.PiresultDatatype == piresultDet.PiresultDatatype
                    && we.PiresultDatetime == piresultDet.PiresultDatetime.GetCDate()
                ;
                var data = await _dal.Query(whereExpression);
                if(data.Count == 0)
                {
                    rm.ResultCode = "1000001";
                    rm.ResultMsg = "该记录不存在".GetMessage(1001024, language);
                    return rm;
                }
                #endregion

                piresultDet.Id = data.FirstOrDefault().Id;
                piresultDet.PiresultModDatetime = DateTime.Now;
                piresultDet.PiresultModUser = user;
                piresultDet.PiresultModProg = "PiresultDetService-Put";
                var rtUpdate = await _dal.Update(piresultDet);
                if (!rtUpdate)
                {
                    rm.ResultCode = "1000001";
                    rm.ResultMsg = "更新失败".GetMessage(1001010, language);
                    return rm;
                }
            }
            catch (Exception ex)
            {
                rm.ResultCode = "1000001";
                rm.ResultMsg = ex.Message;
            }
            return rm;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="listPiresultDet"></param>
        public new async Task<ResultModel> Delete(List<PiresultDet> listPiresultDet)
        {
            ResultModel rm = new ResultModel();
            try
            {
                #region "校验"
                foreach (var piresultDet in listPiresultDet)  
                {
                    Expression<Func<PiresultDet, bool>> whereExpression = we => 
                    we.PiresultCorpId.ToLower() == corp.ToLower()
                    && we.PiresultDomainId.ToLower() == domain.ToLower()
                    && we.PiresultProdline == piresultDet.PiresultProdline
                    && we.PiresultDept == piresultDet.PiresultDept
                    && we.PiresultMachine == piresultDet.PiresultMachine
                    && we.PiresultOp == piresultDet.PiresultOp
                    && we.PiresultPart == piresultDet.PiresultPart
                    && we.PiresultQrcode == piresultDet.PiresultQrcode
                    && we.PiresultLot == piresultDet.PiresultLot
                    && we.PiresultDatatype == piresultDet.PiresultDatatype
                    && we.PiresultDatetime == piresultDet.PiresultDatetime.GetCDate()
                    ;
                    var data = await _dal.Query(whereExpression);
                    if(data.Count == 0)
                    {
                        rm.ResultCode = "1000001";
                        rm.ResultMsg = "该记录不存在".GetMessage(1001024, language);
                        return rm;
                    }
                }
                #endregion

                var rtDelete = await _dal.Delete(listPiresultDet);
                if (!rtDelete)
                {
                    rm.ResultCode = "1000001";
                    rm.ResultMsg = "删除失败".GetMessage(1001013, language);
                    return rm;
                }
            }
            catch (Exception ex)
            {
                rm.ResultCode = "1000001";
                rm.ResultMsg = ex.Message;
            }
            return rm;
        }

        /// <summary>
        /// 下载模板
        /// </summary>
        public async Task<ResultModel> DownloadTemplate()
        {
            ResultModel rm = new ResultModel();
            try
            {
                string filePath = $"Template/piresultDetLoad{language}.xlsx";
                if (!File.Exists(filePath))
                {
                    rm.ResultCode = "1000001";
                    rm.ResultMsg = "找不到该模板：".GetMessage(1001025, language) + filePath;
                    return rm;
                }
                rm.Data = filePath;
            }
            catch (Exception ex)
            {
                rm.ResultCode = "1000001";
                rm.ResultMsg = ex.Message;
            }
            return rm;
        }

        /// <summary>
        /// 导入
        /// </summary>
        public async Task<ResultModel> Import(IFormFile file)
        {
            ResultModel rm = new ResultModel();
            //_unitOfWork.BeginTran();
            try
            {
                if (file != null)
                {
                    var fileDir = AppDomain.CurrentDomain.BaseDirectory + @"UploadFile";
                    if (!Directory.Exists(fileDir))
                    {
                        Directory.CreateDirectory(fileDir);
                    }
                    //文件名称
                    string fileName = file.FileName;
                    //文件类型
                    string fileType = file.FileName.Substring(fileName.LastIndexOf("."));
                    //上传的文件的路径
                    string filePath = fileDir + $@"/{fileName}";
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                    using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        file.CopyTo(fs);
                    }

                    if (fileType == ".xlsx" || fileType == ".xls")
                    {
                        ExcelHelper eh = new ExcelHelper(filePath);
                        DataTable dt = eh.ExcelToDataTable("", true);
                        eh.Dispose();
                        if (dt == null || dt.Rows.Count == 0)
                        {
                            rm.ResultCode = "1000001";
                            rm.ResultMsg = "文件为空！".GetMessage(1001009, language);
                            return rm;
                        }

                        #region "校验模板字段"
                        if (!dt.Columns.Contains("产线")||!dt.Columns.Contains("部门")||!dt.Columns.Contains("设备")||!dt.Columns.Contains("工序")||!dt.Columns.Contains("零件")||!dt.Columns.Contains("二维码")||!dt.Columns.Contains("批次号")||!dt.Columns.Contains("检测项目")||!dt.Columns.Contains("检测值")||!dt.Columns.Contains("单位")||!dt.Columns.Contains("检测时间")||!dt.Columns.Contains("采集方式")||!dt.Columns.Contains("判定结果")||!dt.Columns.Contains("检测类型（inprocess/incoming/periodic）")||!dt.Columns.Contains("NG Issue编号")||!dt.Columns.Contains("检查频率"))
                        {
                            rm.ResultCode = "1000001";
                            rm.ResultMsg = "文件模板错误！请重新下载模板".GetMessage(1001009, language);
                            return rm;
                        }
                        #endregion

                        //检查唯一索引
                        Expression<Func<PiresultDet, bool>> whereExpression = we =>
                            we.PiresultCorpId.ToLower() == corp.ToLower()
                            && we.PiresultDomainId.ToLower() == domain.ToLower()
                        ;
                        var data = await _dal.Query(whereExpression);

                        List<PiresultDet> listLoadSave = new List<PiresultDet>(dt.Rows.Count);
                        List<PiresultDet> listLoadMod = new List<PiresultDet>(dt.Rows.Count);
                        List<PiresultDetLoad> listLoad = new List<PiresultDetLoad>(dt.Rows.Count);
                        var count = 0;
                        foreach (DataRow row in dt.Rows)
                        {
                            if (row["零件"].GetIsNotEmptyOrNull() && row["检测类型（inprocess/incoming/periodic）"].GetIsNotEmptyOrNull())
                            {
                                PiresultDet piresultDet = new PiresultDet
                                {
                                    #region "字段"
                                    Id = Guid.NewGuid().ToString(),
                                    PiresultCorpId = corp,
                                    PiresultDomainId = domain,
                                    PiresultProdline = row["产线"].GetCString(),
                                    PiresultDept = row["部门"].GetCString(),
                                    PiresultMachine = row["设备"].GetCString(),
                                    PiresultOp = row["工序"].GetCString(),
                                    PiresultPart = row["零件"].GetCString(),
                                    PiresultQrcode = row["二维码"].GetCString(),
                                    PiresultLot = row["批次号"].GetCString(),
                                    PiresultDatatype = row["检测项目"].GetCString(),
                                    PiresultValue = row["检测值"].GetCDecimal(),
                                    PiresultUm = row["单位"].GetCString(),
                                    PiresultDatetime = row["检测时间"].GetCDate(),
                                    PiresultMthd = row["采集方式"].GetCString(),
                                    PiresultResult = row["判定结果"].GetCString(),
                                    PiresultType = row["检测类型（inprocess/incoming/periodic）"].GetCString(),
                                    PiresultIssue = row["NG Issue编号"].GetCString(),
                                    PiresultFrequency = row["检查频率"].GetCString(),
                                    PiresultReleased = false,
                                    PiresultCrtDatetime = DateTime.Now,
                                    PiresultCrtProg = "PiresultDetService-Import",
                                    PiresultCrtUser = user,
                                    PiresultModDatetime = DateTime.Now,
                                    PiresultModProg = "PiresultDetService-Import",
                                    PiresultModUser = user
                                    #endregion
                                };
                                listLoadSave.Add(piresultDet);
                                continue;
                            }
                            //SerilogHelper.WriteLog("PiresultDetService", "Import", (++count).ToString());
                            
                            //PiresultDetLoad piresultDetLoad = (PiresultDetLoad)JsonHelper.JsonToObject(JsonHelper.ObjectToJson(piresultDet),typeof(PiresultDetLoad));

                            #region "校验必填项"

                            #endregion

                            ////检查唯一索引
                            //Expression<Func<PiresultDet, bool>> whereExpression = we => 
                            //    we.PiresultCorpId.ToLower() == corp.ToLower()
                            //    && we.PiresultDomainId.ToLower() == domain.ToLower()
                            //    && we.PiresultProdline == piresultDet.PiresultProdline
                            //    && we.PiresultDept == piresultDet.PiresultDept
                            //    && we.PiresultMachine == piresultDet.PiresultMachine
                            //    && we.PiresultOp == piresultDet.PiresultOp
                            //    && we.PiresultPart == piresultDet.PiresultPart
                            //    && we.PiresultQrcode == piresultDet.PiresultQrcode
                            //    && we.PiresultLot == piresultDet.PiresultLot
                            //    && we.PiresultDatatype == piresultDet.PiresultDatatype
                            //    && we.PiresultDatetime == piresultDet.PiresultDatetime
                            //    && we.PiresultType == piresultDet.PiresultType
                            //;
                            //var data = await _dal.Query(whereExpression);
                            //if (data.Count == 0)
                            //{
                            //    #region "自定义校验"

                            //    ////校验设备是否存在
                            //    //if (!dataMachine.Exists(item => item.PimachineMachine.GetCString().ToLower() == piresultDet.PiresultMachine.GetCString().ToLower()))
                            //    //{
                            //    //    piresultDetLoad.Canpass = false;
                            //    //    piresultDetLoad.Errormessage = "该设备不存在".GetMessage(1001023, language);
                            //    //}

                            //    ////校验工序是否存在
                            //    //if (!dataOp.Exists(item => item.PiopOp.GetCString().ToLower() == piresultDet.PiresultOp.GetCString().ToLower()))
                            //    //{
                            //    //    piresultDetLoad.Canpass = false;
                            //    //    piresultDetLoad.Errormessage = "该工序不存在".GetMessage(1001023, language);
                            //    //}

                            //    ////校验零件是否存在
                            //    //if (!dataPart.Exists(item => item.PiptPart.GetCString().ToLower() == piresultDet.PiresultPart.GetCString().ToLower()))
                            //    //{
                            //    //    piresultDetLoad.Canpass = false;
                            //    //    piresultDetLoad.Errormessage = "该零件不存在".GetMessage(1001023, language);
                            //    //}

                            //    ////校验检测项是否存在
                            //    //if (!dataStd.Exists(item => item.PistdDatatype.GetCString().ToLower() == piresultDet.PiresultDatatype.GetCString().ToLower()))
                            //    //{
                            //    //    piresultDetLoad.Canpass = false;
                            //    //    piresultDetLoad.Errormessage = "该检测项不存在".GetMessage(1001023, language);
                            //    //}
                            //    #endregion

                            //    if (piresultDetLoad.Canpass == true)
                            //    {
                            //        var rtAdd = await _dal.Add(piresultDet);
                            //        if (rtAdd != 1)
                            //        {
                            //            piresultDetLoad.Canpass = false;
                            //            piresultDetLoad.Errormessage = "插入失败".GetMessage(1001009, language);
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            //    #region "自定义校验"

                            //    #endregion

                            //    piresultDet.Id = data.FirstOrDefault().Id;
                            //    var rtUpdate = await _dal.Update(piresultDet);
                            //    if (!rtUpdate)
                            //    {
                            //        piresultDetLoad.Canpass = false;
                            //        piresultDetLoad.Errormessage = "更新失败".GetMessage(1001009, language);
                            //    }
                            //}

                            //listLoad.Add(piresultDetLoad);
                        }

                        //导入数据
                        return await Import(listLoadSave);

                        rm.Data = listLoad;

                        //_unitOfWork.CommitTran();

                        if (listLoad.Where(piresultDetLoad => piresultDetLoad.Canpass == false).ToList().Count > 0)
                        {
                            rm.ResultCode = "1000001";
                            rm.ResultMsg = "存在导入失败数据".GetMessage(1001009, language);
                            return rm;
                        }
                    }
                    else
                    {
                        rm.ResultCode = "1000001";
                        rm.ResultMsg = "文件类型错误！".GetMessage(1001009, language);
                        return rm;
                    }
                }
                else
                {
                    rm.ResultCode = "1000001";
                    rm.ResultMsg = "请选择上传文件！";
                    return rm;
                }
            }
            catch (Exception ex)
            {
                //_unitOfWork.RollbackTran();
                rm.ResultCode = "1000001";
                rm.ResultMsg = ex.Message;
            }
            return rm;
        }


        /// <summary>
        /// 导入
        /// </summary>
        public async Task<ResultModel> Import(List<PiresultDet> piresultDets)
        {
            ResultModel rm = new ResultModel();
            _unitOfWork.BeginTran();
            try
            {
                #region
                //Expression<Func<PiresultDet, bool>> whereExpression = we => 
                //    we.PiresultCorpId.ToLower() == corp.ToLower()
                //    && we.PiresultDomainId.ToLower() == domain.ToLower()
                //    && we.PiresultProdline == piresultDet.PiresultProdline
                //    && we.PiresultDept == piresultDet.PiresultDept
                //    && we.PiresultMachine == piresultDet.PiresultMachine
                //    && we.PiresultOp == piresultDet.PiresultOp
                //    && we.PiresultPart == piresultDet.PiresultPart
                //    && we.PiresultQrcode == piresultDet.PiresultQrcode
                //    && we.PiresultLot == piresultDet.PiresultLot
                //    && we.PiresultDatatype == piresultDet.PiresultDatatype
                //    && we.PiresultDatetime == piresultDet.PiresultDatetime
                //    && we.PiresultType == piresultDet.PiresultType
                //;

                //Expression<Func<PiresultDet, object>> checkColumns = we => new
                //{
                //    we.PiresultProdline,
                //    we.PiresultDept,
                //    we.PiresultMachine,
                //    we.PiresultOp,
                //    we.PiresultPart,
                //    we.PiresultQrcode,
                //    we.PiresultLot,
                //    we.PiresultDatatype,
                //    we.PiresultDatetime,
                //    we.PiresultType,
                //};
                //var data = await _dal.InportInsertOrUpdate(piresultDets, checkColumns);

                //_unitOfWork.CommitTran();
                //var s = data;
                //return rm;

                ////获取所有设备
                //Expression<Func<PimachineMstr, bool>> whereExpressionMachine = we =>
                //            we.PimachineCorpId.ToLower() == corp.ToLower()
                //            && we.PimachineDomainId.ToLower() == domain.ToLower()
                //        ;
                //var dataMachine = await _dalMachine.Query(whereExpressionMachine);

                ////获取所有工序
                //Expression<Func<PiopMstr, bool>> whereExpressionOp = we =>
                //            we.PiopCorpId.ToLower() == corp.ToLower()
                //            && we.PiopDomainId.ToLower() == domain.ToLower()
                //        ;
                //var dataOp = await _dalOp.Query(whereExpressionOp);

                ////获取所有零件
                //Expression<Func<PiptMstr, bool>> whereExpressionPart = we =>
                //    we.PiptCorpId.ToLower() == corp.ToLower()
                //    && we.PiptDomainId.ToLower() == domain.ToLower()
                //;
                //var dataPart = await _dalPart.Query(whereExpressionPart);

                ////获取所有检测项
                //Expression<Func<PistdMstr, bool>> whereExpressionStd = we =>
                //    we.PistdCorpId.ToLower() == corp.ToLower()
                //    && we.PistdDomainId.ToLower() == domain.ToLower()
                //;
                //var dataStd = await _dalStd.Query(whereExpressionStd);
                #endregion
                List<PiresultDet> listLoadSave = new List<PiresultDet>(piresultDets.Count);
                List<PiresultDetLoad> listLoad = new List<PiresultDetLoad>(piresultDets.Count);
                #region
                //var count = 0;
                //foreach (PiresultDet piresultDet in piresultDets)
                //{
                //    SerilogHelper.WriteLog("PiresultDetService", "Import", (++count).ToString());
                //    //pishipMstr.Id = Guid.NewGuid().ToString();
                //    //pishipMstr.PishipCorpId = corp;
                //    //pishipMstr.PishipDomainId = Guid.NewGuid().ToString();
                //    //pishipMstr.PishipCrtDatetime = DateTime.Now;
                //    //pishipMstr.PishipCrtProg = "PishipMstrService-Import";
                //    //pishipMstr.PishipCrtUser = user;
                //    //pishipMstr.PishipModDatetime = DateTime.Now;
                //    //pishipMstr.PishipModProg = "PishipMstrService-Import";
                //    //pishipMstr.PishipModUser = user;

                //    PiresultDetLoad piresultDetLoad = JsonHelper.JsonToObject<PiresultDetLoad>(JsonHelper.ObjectToJson(piresultDet));

                //    #region "校验必填项"

                //    #endregion

                //    #region "自定义校验"

                //    //校验设备是否存在
                //    if (!dataMachine.Exists(item => item.PimachineMachine.GetCString().ToLower() == piresultDet.PiresultMachine.GetCString().ToLower()))
                //    {
                //        piresultDetLoad.Canpass = false;
                //        piresultDetLoad.Errormessage = "该设备不存在".GetMessage(1001023, language);
                //        listLoad.Add(piresultDetLoad);
                //        continue;
                //    }

                //    //校验工序是否存在
                //    if (!dataOp.Exists(item => item.PiopOp.GetCString().ToLower() == piresultDet.PiresultOp.GetCString().ToLower()))
                //    {
                //        piresultDetLoad.Canpass = false;
                //        piresultDetLoad.Errormessage = "该工序不存在".GetMessage(1001023, language);
                //        listLoad.Add(piresultDetLoad);
                //        continue;
                //    }

                //    //校验零件是否存在
                //    if (!dataPart.Exists(item => item.PiptPart.GetCString().ToLower() == piresultDet.PiresultPart.GetCString().ToLower()))
                //    {
                //        piresultDetLoad.Canpass = false;
                //        piresultDetLoad.Errormessage = "该零件不存在".GetMessage(1001023, language);
                //        listLoad.Add(piresultDetLoad);
                //        continue;
                //    }

                //    //校验检测项是否存在
                //    if (!dataStd.Exists(item => item.PistdDatatype.GetCString().ToLower() == piresultDet.PiresultDatatype.GetCString().ToLower() && item.PistdType.GetCString().ToLower() == piresultDet.PiresultType.GetCString().ToLower()))
                //    {
                //        piresultDetLoad.Canpass = false;
                //        piresultDetLoad.Errormessage = "该检测项不存在".GetMessage(1001023, language);
                //        listLoad.Add(piresultDetLoad);
                //        continue;
                //    }
                //    #endregion

                //    listLoadSave.Add(piresultDetLoad);
                //}
                #endregion

                SerilogHelper.WriteLog("PiresultDetService", "SaveStrat", "");
                try
                {
                    var rtAdd = await _dal.AddMuch(piresultDets);
                    SerilogHelper.WriteLog("PiresultDetService", "rtAdd", rtAdd.ToString());
                }
                catch
                {

                }
                //try
                //{
                //    var rtUpdate = await _dal.UpdateMuch(piresultDets);
                //    SerilogHelper.WriteLog("PiresultDetService", "rtUpdate", rtUpdate.ToString());
                //}
                //catch
                //{
                //}

                rm.Data = listLoad;
                _unitOfWork.CommitTran();

                if (listLoad.Where(pishipMstrLoad => pishipMstrLoad.Canpass == false).ToList().Count > 0)
                {
                    rm.ResultCode = "1000001";
                    rm.ResultMsg = "存在导入失败数据".GetMessage(1001009, language);
                    return rm;
                }
            }
            catch (Exception ex)
            {
                _unitOfWork.RollbackTran();
                rm.ResultCode = "1000001";
                rm.ResultMsg = ex.Message;
            }
            return rm;
        }

        /// <summary>
        /// 列表导入
        /// </summary>
        public async Task<ResultModel> ImportList(List<PiresultDet> piresultDets)
        {
            ResultModel rm = new ResultModel();
            try
            {
                if (piresultDets != null && piresultDets.Count > 0) 
                {
                    List<PiresultDet> listLoadSave = new List<PiresultDet>(piresultDets.Count);
                    foreach (var item in piresultDets)
                    {
                        if (item.PiresultPart.GetIsNotEmptyOrNull() && item.PiresultType.GetIsNotEmptyOrNull())
                        {
                            PiresultDet piresultDet = new PiresultDet
                            {
                                #region "字段"
                                Id = Guid.NewGuid().ToString(),
                                PiresultCorpId = corp,
                                PiresultDomainId = domain,
                                PiresultProdline = item.PiresultProdline.GetCString(),
                                PiresultDept = item.PiresultDept.GetCString(),
                                PiresultMachine = item.PiresultMachine.GetCString(),
                                PiresultOp = item.PiresultOp.GetCString(),
                                PiresultPart = item.PiresultPart.GetCString(),
                                PiresultQrcode = item.PiresultQrcode.GetCString(),
                                PiresultLot = item.PiresultLot.GetCString(),
                                PiresultDatatype = item.PiresultDatatype.GetCString(),
                                PiresultValue = item.PiresultValue.GetCDecimal(),
                                PiresultUm = item.PiresultUm.GetCString(),
                                PiresultDatetime = item.PiresultDatetime.GetCDate(),
                                PiresultMthd = item.PiresultMthd.GetCString(),
                                PiresultResult = item.PiresultResult.GetCString(),
                                PiresultType = item.PiresultType.GetCString(),
                                PiresultIssue = item.PiresultIssue.GetCString(),
                                PiresultFrequency = item.PiresultFrequency.GetCString(),
                                PiresultReleased = false,
                                PiresultCrtDatetime = DateTime.Now,
                                PiresultCrtProg = "PiresultDetService-ImportList",
                                PiresultCrtUser = user,
                                PiresultModDatetime = DateTime.Now,
                                PiresultModProg = "PiresultDetService-ImportList",
                                PiresultModUser = user
                                #endregion
                            };
                            listLoadSave.Add(piresultDet);
                            continue;
                        }
                    }
                    //导入数据
                    return await Import(listLoadSave);
                }
                else
                {
                    rm.ResultCode = "1000001";
                    rm.ResultMsg = "数据不能为空！";
                    return rm;
                }
            }
            catch (Exception ex)
            {
                rm.ResultCode = "1000001";
                rm.ResultMsg = ex.Message;
            }
            return rm;
        }

        /// <summary>
        /// 输出
        /// </summary>
        public async Task<ResultModel> Export(PiresultDetSearchModel piresultDetSearchModel)
        {
            ResultModel rm = new ResultModel();
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            try
            {
                Expression<Func<PiresultDet, bool>> whereExpression = we => 
                    we.PiresultCorpId.ToLower() == corp.ToLower()
                    && we.PiresultDomainId.ToLower() == domain.ToLower()
                    && (piresultDetSearchModel.PiresultProdline.GetIsEmptyOrNull() || we.PiresultProdline.ToLower().Contains(piresultDetSearchModel.PiresultProdline.ToLower()))
                    && (piresultDetSearchModel.PiresultDept.GetIsEmptyOrNull() || we.PiresultDept.ToLower().Contains(piresultDetSearchModel.PiresultDept.ToLower()))
                    && (piresultDetSearchModel.PiresultMachine.GetIsEmptyOrNull() || we.PiresultMachine.ToLower().Contains(piresultDetSearchModel.PiresultMachine.ToLower()))
                    && (piresultDetSearchModel.PiresultOp.GetIsEmptyOrNull() || we.PiresultOp.ToLower().Contains(piresultDetSearchModel.PiresultOp.ToLower()))
                    && (piresultDetSearchModel.PiresultPart.GetIsEmptyOrNull() || we.PiresultPart.ToLower().Contains(piresultDetSearchModel.PiresultPart.ToLower()))
                    && (piresultDetSearchModel.PiresultQrcode.GetIsEmptyOrNull() || we.PiresultQrcode.ToLower().Contains(piresultDetSearchModel.PiresultQrcode.ToLower()))
                    && (piresultDetSearchModel.PiresultLot.GetIsEmptyOrNull() || we.PiresultLot.ToLower().Contains(piresultDetSearchModel.PiresultLot.ToLower()))
                    && (piresultDetSearchModel.PiresultDatatype.GetIsEmptyOrNull() || we.PiresultDatatype.ToLower().Contains(piresultDetSearchModel.PiresultDatatype.ToLower()))
                    && (piresultDetSearchModel.PiresultDatetimeFrom.GetIsEmptyOrNull() || we.PiresultDatetime >= piresultDetSearchModel.PiresultDatetimeFrom.GetCDate())
                    && (piresultDetSearchModel.PiresultDatetimeTo.GetIsEmptyOrNull() || we.PiresultDatetime <= piresultDetSearchModel.PiresultDatetimeTo.GetCDate())
                    && (piresultDetSearchModel.PiresultResult.GetIsEmptyOrNull() || we.PiresultResult.ToLower().Contains(piresultDetSearchModel.PiresultResult.ToLower()))
                    && (piresultDetSearchModel.PiresultType.GetIsEmptyOrNull() || we.PiresultType.ToLower().Contains(piresultDetSearchModel.PiresultType.ToLower()))
                    && (piresultDetSearchModel.PiresultReleased.GetIsEmptyOrNull() || we.PiresultReleased == piresultDetSearchModel.PiresultReleased.GetCBool())
                ;
                var pageSort = ClassMapSqlsugar.GetDBSort<PiresultDet>(piresultDetSearchModel.SortList);
                DataTable dt = await _dal.QueryTable(whereExpression, pageSort);

                List<TableHeaderModel> listTableHeader = piresultDetSearchModel.TableHead;
                if (listTableHeader == null || listTableHeader.Count == 0)
                {
                    rm.ResultCode = "1000001";
                    rm.ResultMsg = "表格头信息错误".GetMessage(1001024, language);
                    return rm;
                }

                DataTable dtHeader = dt.Clone();
                foreach (DataColumn col in dtHeader.Columns)
                {
                    if (listTableHeader != null && listTableHeader.Count > 0 && listTableHeader.Where(p => ClassMapSqlsugar.GetDBColumnName<PiresultDet>(p.Property).ToLower() == col.ColumnName.ToLower() && p.IsShow == true).ToList().Count > 0)
                    {
                        dt.Columns[col.ColumnName].ColumnName = listTableHeader.Where(p => ClassMapSqlsugar.GetDBColumnName<PiresultDet>(p.Property).ToLower() == col.ColumnName.ToLower()).ToList().FirstOrDefault().Label;
                    }
                    else
                    {
                        dt.Columns.Remove(col.ColumnName);
                    }
                }
                int index = 0;
                foreach (TableHeaderModel p in listTableHeader.Where(p => p.IsShow == true).ToList())
                {
                    dt.Columns[p.Label].SetOrdinal(index);
                    index++;
                }

                string path = "Download";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string fileName = "PiresultDet" + DateTime.Now.Ticks + new Random().Next(10000000, 99999999) + ".xlsx";
                string filePath = path + @"/" + fileName;
                ExcelHelper eh = new ExcelHelper(filePath);
                eh.DataTableToExcel(dt);

                rm.Data = filePath;
            }
            catch (Exception ex)
            {
                rm.ResultCode = "1000001";
                rm.ResultMsg = ex.Message;
            }
            return rm;
        }

        #endregion

        #region CustomInterface

        /// <summary>
        /// 查询明细
        /// </summary>
        public async Task<ResultModel> GetDetail(PiresultDetSearchModel piresultDetSearchModel)
        {
            ResultModel rm = new ResultModel();
            //try
            //{
            //    Expression<Func<PiresultDet, bool>> whereExpressionQrcode = we =>
            //        we.PiresultCorpId.ToLower() == corp.ToLower()
            //        && we.PiresultDomainId.ToLower() == domain.ToLower()
            //        && (piresultDetSearchModel.PiresultQrcode.GetIsEmptyOrNull() || we.PiresultQrcode.ToLower().Equals(piresultDetSearchModel.PiresultQrcode.ToLower()))
            //        && (piresultDetSearchModel.PiresultType.GetIsEmptyOrNull() || we.PiresultType.ToLower().Equals(piresultDetSearchModel.PiresultType.ToLower()))
            //    ;
            //    var pageSort = piresultDetSearchModel.SortList.GetNotNull().Count() == 0 ? "piresult_datetime desc" : ClassMapSqlsugar.GetDBSort<PiresultDet>(piresultDetSearchModel.SortList);
            //    var dataQrcode = await _dal.QueryPage(whereExpressionQrcode, piresultDetSearchModel.PageIndex, piresultDetSearchModel.PageSize, pageSort);

            //    List<PiresultDet> listQrcode = (List<PiresultDet>)dataQrcode.List;
            //    List<string> listLot = new();
            //    var lot = string.Empty;
            //    foreach(PiresultDet piresult in listQrcode)
            //    {
            //        lot = piresult.PiresultLot.GetCString();
            //        listLot.Add(piresult.PiresultLot.GetCString());
            //    }
            //    if(piresultDetSearchModel.PiresultType.GetCString().ToLower() == "incoming")
            //    {
            //        lot = piresultDetSearchModel.PiresultQrcode.GetCString();
            //    }

            //    Expression <Func<PiresultDet, PiptMstr, PimachineMstr, PiopMstr, PistdMstr, object[]>> joinExpression = (we, pt, machine, op, std) => new object[]
            //    {
            //        JoinType.Left,we.PiresultCorpId == pt.PiptCorpId
            //        && we.PiresultDomainId == pt.PiptDomainId
            //        && we.PiresultPart == pt.PiptPart,
            //        JoinType.Left,we.PiresultCorpId == machine.PimachineCorpId
            //        && we.PiresultDomainId == machine.PimachineDomainId
            //        && we.PiresultMachine == machine.PimachineMachine,
            //        JoinType.Left,we.PiresultCorpId == op.PiopCorpId
            //        && we.PiresultDomainId == op.PiopDomainId
            //        && we.PiresultPart == op.PiopPart
            //        && we.PiresultOp == op.PiopOp,
            //        JoinType.Left,we.PiresultCorpId == std.PistdCorpId
            //        && we.PiresultDomainId == std.PistdDomainId
            //        && we.PiresultPart == std.PistdPart
            //        && we.PiresultOp == std.PistdOp
            //        && we.PiresultDatatype == std.PistdDatatype
            //    };

            //    Expression<Func<PiresultDet, PiptMstr, PimachineMstr, PiopMstr, PistdMstr, PiresultDetViewModel>> selectExpression = (we, pt, machine, op, std) => new PiresultDetViewModel
            //    {
            //        PiresultCorpId = we.PiresultCorpId,
            //        PiresultDomainId = we.PiresultDomainId,
            //        PiresultProdline = we.PiresultProdline,
            //        PiresultDept = we.PiresultDept,
            //        PiresultMachine = machine.PimachineMachineName,
            //        PiresultOp = op.PiopOpName,
            //        PiresultPart = pt.PiptCustPart,
            //        PiresultQrcode = we.PiresultQrcode,
            //        PiresultLot = we.PiresultLot,
            //        PiresultDatatype = we.PiresultDatatype,
            //        PiresultValue = we.PiresultValue,
            //        PiresultUpperLimit = std.PistdUpperLimit,
            //        PiresultLowerLimit = std.PistdLowerLimit,
            //        PiresultUm = we.PiresultUm,
            //        PiresultDatetime = we.PiresultDatetime,
            //        PiresultMthd = we.PiresultMthd,
            //        PiresultResult = we.PiresultResult,
            //        PiresultFrequency = we.PiresultFrequency,
            //    };

            //    Expression<Func<PiresultDet, PiptMstr, PimachineMstr, PiopMstr, PistdMstr, bool>> whereExpression = (we, pt, machine, op, std) =>
            //        we.PiresultCorpId.ToLower() == corp.ToLower()
            //        && we.PiresultDomainId.ToLower() == domain.ToLower()
            //        && (((we.PiresultQrcode == null || we.PiresultQrcode == "") && we.PiresultLot.ToLower() == lot.ToLower()) || we.PiresultQrcode.ToLower().Equals(piresultDetSearchModel.PiresultQrcode.ToLower()))
            //        //&& (piresultDetSearchModel.PiresultQrcode.GetIsEmptyOrNull() || we.PiresultQrcode.ToLower().Contains(piresultDetSearchModel.PiresultQrcode.ToLower()))
            //        //&& (piresultDetSearchModel.PiresultDatetimeFrom.GetIsEmptyOrNull() || we.PiresultDatetime >= piresultDetSearchModel.PiresultDatetimeFrom.GetCDate())
            //        //&& (piresultDetSearchModel.PiresultDatetimeTo.GetIsEmptyOrNull() || we.PiresultDatetime <= piresultDetSearchModel.PiresultDatetimeTo.GetCDate())
            //        && (piresultDetSearchModel.PiresultProdline.GetIsEmptyOrNull() || we.PiresultProdline.ToLower().Equals(piresultDetSearchModel.PiresultProdline.ToLower()))
            //        && (piresultDetSearchModel.PiresultOp.GetIsEmptyOrNull() || we.PiresultOp.ToLower().Equals(piresultDetSearchModel.PiresultOp.ToLower()))
            //        && (piresultDetSearchModel.PiresultDatatype.GetIsEmptyOrNull() || we.PiresultDatatype.ToLower().Equals(piresultDetSearchModel.PiresultDatatype.ToLower()))
            //        && (piresultDetSearchModel.PiresultType.GetIsEmptyOrNull() || we.PiresultType.ToLower().Equals(piresultDetSearchModel.PiresultType.GetCString().ToLower()))
            //        && (piresultDetSearchModel.PiresultResult.GetIsEmptyOrNull() || we.PiresultResult.ToLower().Equals(piresultDetSearchModel.PiresultResult.ToLower()))
            //    ;
            //    //if (piresultDetSearchModel.PiresultType.GetCString().ToLower() == "incoming")
            //    //{
            //    //    whereExpression = (we, pt, machine, op, std) =>
            //    //        we.PiresultCorpId == corp
            //    //        && we.PiresultDomainId == domain
            //    //        && (((we.PiresultQrcode == null || we.PiresultQrcode == "") && we.PiresultLot.ToLower() == lot.ToLower()) || we.PiresultQrcode.ToLower().Contains(piresultDetSearchModel.PiresultQrcode.ToLower()))
            //    //        //&& (piresultDetSearchModel.PiresultQrcode.GetIsEmptyOrNull() || we.PiresultQrcode.ToLower().Contains(piresultDetSearchModel.PiresultQrcode.ToLower()))
            //    //        && (piresultDetSearchModel.PiresultDatetimeFrom.GetIsEmptyOrNull() || we.PiresultDatetime >= piresultDetSearchModel.PiresultDatetimeFrom.GetCDate())
            //    //        && (piresultDetSearchModel.PiresultDatetimeTo.GetIsEmptyOrNull() || we.PiresultDatetime <= piresultDetSearchModel.PiresultDatetimeTo.GetCDate())
            //    //        && (piresultDetSearchModel.PiresultProdline.GetIsEmptyOrNull() || we.PiresultProdline.ToLower().Contains(piresultDetSearchModel.PiresultProdline.ToLower()))
            //    //        && (piresultDetSearchModel.PiresultOp.GetIsEmptyOrNull() || we.PiresultOp.ToLower().Contains(piresultDetSearchModel.PiresultOp.ToLower()))
            //    //        && (piresultDetSearchModel.PiresultDatatype.GetIsEmptyOrNull() || we.PiresultDatatype.ToLower().Contains(piresultDetSearchModel.PiresultDatatype.ToLower()))
            //    //        && (piresultDetSearchModel.PiresultType.GetIsEmptyOrNull() || we.PiresultType.ToLower().Contains(piresultDetSearchModel.PiresultType.GetCString().ToLower()))
            //    //        && (piresultDetSearchModel.PiresultResult.GetIsEmptyOrNull() || we.PiresultResult.ToLower().Equals(piresultDetSearchModel.PiresultResult.ToLower()))
            //    //    ;
            //    //}

            //    //var pageSort = ClassMapSqlsugar.GetDBSort<PiresultDet>(piresultDetSearchModel.SortList);
            //    var data = await _dal.QueryMuchPage(joinExpression, selectExpression, whereExpression, piresultDetSearchModel.PageIndex, piresultDetSearchModel.PageSize, pageSort);

            //    rm.Data = data;
            //}
            //catch (Exception ex)
            //{
            //    rm.ResultCode = "1000001";
            //    rm.ResultMsg = ex.Message;
            //}
            return rm;
        }

        /// <summary>
        /// 查询Bom明细
        /// </summary>
        public async Task<ResultModel> GetBomDetail(PiresultDetSearchModel piresultDetSearchModel)
        {

            ResultModel rm = new ResultModel();
            //try
            //{

            //    Expression<Func<PiresultDet, PiptMstr, PimachineMstr, PiopMstr, PistdMstr, object[]>> joinExpression = (we, pt, machine, op, std) => new object[]
            //    {
            //        JoinType.Left,we.PiresultCorpId == pt.PiptCorpId
            //        && we.PiresultDomainId == pt.PiptDomainId
            //        && we.PiresultPart == pt.PiptPart,
            //        JoinType.Left,we.PiresultCorpId == machine.PimachineCorpId
            //        && we.PiresultDomainId == machine.PimachineDomainId
            //        && we.PiresultMachine == machine.PimachineMachine,
            //        JoinType.Left,we.PiresultCorpId == op.PiopCorpId
            //        && we.PiresultDomainId == op.PiopDomainId
            //        && we.PiresultPart == op.PiopPart
            //        && we.PiresultOp == op.PiopOp,
            //        JoinType.Left,we.PiresultCorpId == std.PistdCorpId
            //        && we.PiresultDomainId == std.PistdDomainId
            //        && we.PiresultPart == std.PistdPart
            //        && we.PiresultOp == std.PistdOp
            //        && we.PiresultDatatype == std.PistdDatatype
            //    };

            //    Expression<Func<PiresultDet, PiptMstr, PimachineMstr, PiopMstr, PistdMstr, PiresultDetViewModel>> selectExpression = (we, pt, machine, op, std) => new PiresultDetViewModel
            //    {
            //        PiresultCorpId = we.PiresultCorpId,
            //        PiresultDomainId = we.PiresultDomainId,
            //        PiresultProdline = we.PiresultProdline,
            //        PiresultDept = we.PiresultDept,
            //        PiresultMachine = machine.PimachineMachineName,
            //        PiresultOp = op.PiopOpName,
            //        PiresultPart = pt.PiptCustPart,
            //        PiresultQrcode = we.PiresultQrcode,
            //        PiresultLot = we.PiresultLot,
            //        PiresultDatatype = we.PiresultDatatype,
            //        PiresultValue = we.PiresultValue,
            //        PiresultUm = we.PiresultUm,
            //        PiresultDatetime = we.PiresultDatetime,
            //        PiresultMthd = we.PiresultMthd,
            //        PiresultResult = we.PiresultResult,
            //    };

            //    Expression<Func<PiresultDet, PiptMstr, PimachineMstr, PiopMstr, PistdMstr, bool>> whereExpression = (we, pt, machine, op, std) =>
            //        we.PiresultCorpId.ToLower() == corp.ToLower() 
            //        && we.PiresultDomainId.ToLower() == domain.ToLower()
            //        && (piresultDetSearchModel.PiresultDatetimeFrom.GetIsEmptyOrNull() || we.PiresultDatetime >= piresultDetSearchModel.PiresultDatetimeFrom.GetCDate())
            //        && (piresultDetSearchModel.PiresultDatetimeTo.GetIsEmptyOrNull() || we.PiresultDatetime <= piresultDetSearchModel.PiresultDatetimeTo.GetCDate())
            //        && (piresultDetSearchModel.PiresultPart.GetIsEmptyOrNull() || pt.PiptCustPart.ToLower().Equals(piresultDetSearchModel.PiresultPart.ToLower()))
            //        && (piresultDetSearchModel.PiresultType.GetIsEmptyOrNull() || we.PiresultType.ToLower().Equals(piresultDetSearchModel.PiresultType.ToLower()))
            //    ;
            //    var pageSort = piresultDetSearchModel.SortList.GetNotNull().Count() == 0 ? "piresult_datetime desc" : ClassMapSqlsugar.GetDBSort<PiresultDet>(piresultDetSearchModel.SortList);
            //    var data = await _dal.QueryMuchPage(joinExpression, selectExpression, whereExpression, piresultDetSearchModel.PageIndex, piresultDetSearchModel.PageSize, pageSort);

            //    rm.Data = data;
            //}
            //catch (Exception ex)
            //{
            //    rm.ResultCode = "1000001";
            //    rm.ResultMsg = ex.Message;
            //}
            return rm;
        }

        /// <summary>
        /// 查询NG明细
        /// </summary>
        public async Task<ResultModel> GetNGDetail(PiresultDetSearchModel piresultDetSearchModel)
        {
            ResultModel rm = new ResultModel();
            //try
            //{
            //    Expression<Func<PiresultDet, PiptMstr, PimachineMstr, PiopMstr, PistdMstr, object[]>> joinExpression = (we, pt, machine, op, std) => new object[]
            //    {
            //        JoinType.Left,we.PiresultCorpId == pt.PiptCorpId
            //        && we.PiresultDomainId == pt.PiptDomainId
            //        && we.PiresultPart == pt.PiptPart,
            //        JoinType.Left,we.PiresultCorpId == machine.PimachineCorpId
            //        && we.PiresultDomainId == machine.PimachineDomainId
            //        && we.PiresultMachine == machine.PimachineMachine,
            //        JoinType.Left,we.PiresultCorpId == op.PiopCorpId
            //        && we.PiresultDomainId == op.PiopDomainId
            //        && we.PiresultPart == op.PiopPart
            //        && we.PiresultOp == op.PiopOp,
            //        JoinType.Left,we.PiresultCorpId == std.PistdCorpId
            //        && we.PiresultDomainId == std.PistdDomainId
            //        && we.PiresultPart == std.PistdPart
            //        && we.PiresultOp == std.PistdOp
            //        && we.PiresultDatatype == std.PistdDatatype
            //    };

            //    Expression<Func<PiresultDet, PiptMstr, PimachineMstr, PiopMstr, PistdMstr, PiresultDetViewModel>> selectExpression = (we, pt, machine, op, std) => new PiresultDetViewModel
            //    {
            //        PiresultCorpId = we.PiresultCorpId,
            //        PiresultDomainId = we.PiresultDomainId,
            //        PiresultProdline = we.PiresultProdline,
            //        PiresultDept = we.PiresultDept,
            //        PiresultMachine = machine.PimachineMachineName,
            //        PiresultOp = op.PiopOpName,
            //        PiresultPart = pt.PiptCustPart,
            //        PiresultQrcode = we.PiresultQrcode,
            //        PiresultLot = we.PiresultLot,
            //        PiresultDatatype = we.PiresultDatatype,
            //        PiresultValue = we.PiresultValue,
            //        PiresultUpperLimit = std.PistdUpperLimit,
            //        PiresultLowerLimit = std.PistdLowerLimit,
            //        PiresultUm = we.PiresultUm,
            //        PiresultDatetime = we.PiresultDatetime,
            //        PiresultMthd = we.PiresultMthd,
            //        PiresultResult = we.PiresultResult,
            //    };

            //    Expression<Func<PiresultDet, PiptMstr, PimachineMstr, PiopMstr, PistdMstr, bool>> whereExpression = (we, pt, machine, op, std) =>
            //        we.PiresultCorpId.ToLower() == corp.ToLower() 
            //        && we.PiresultDomainId.ToLower() == domain.ToLower()
            //        && we.PiresultIssue.ToLower().Equals(piresultDetSearchModel.PiresultIssue.ToLower())
            //    ;
            //    var pageSort = piresultDetSearchModel.SortList.GetNotNull().Count() == 0 ? "piresult_datetime desc" : ClassMapSqlsugar.GetDBSort<PiresultDet>(piresultDetSearchModel.SortList);
            //    var data = await _dal.QueryMuchPage(joinExpression, selectExpression, whereExpression, piresultDetSearchModel.PageIndex, piresultDetSearchModel.PageSize, pageSort);

            //    rm.Data = data;
            //}
            //catch (Exception ex)
            //{
            //    rm.ResultCode = "1000001";
            //    rm.ResultMsg = ex.Message;
            //}
            return rm;
        }
        #endregion

    }
}

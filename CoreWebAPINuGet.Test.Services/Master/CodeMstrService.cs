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
using CoreWebAPINuGet.Test.Models;
using CoreWebAPINuGet.Test.Models.Models;
using CoreWebAPINuGet.Test.Models.ViewModels;
using CoreWebAPINuGet.Test.IServices.Master;

namespace CoreWebAPINuGet.Test.Services.Master
{
    public class CodeMstrService : BaseService<CodeMstr>, ICodeMstrService
    {
        private readonly IBaseRepository<CodeMstr> _dal;
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
        public CodeMstrService(IBaseRepository<CodeMstr> dal, IUnitOfWork unitOfWork, IHttpContextAccessor accessor)
        {
            this._dal = dal;
            base.BaseDal = dal;
            _unitOfWork = unitOfWork;
            _accessor = accessor;

            user = _accessor.HttpContext.Request.Headers["x-user"].GetCString();
            corp = _accessor.HttpContext.Request.Headers["x-corp"].GetCString();
            domain = _accessor.HttpContext.Request.Headers["x-domain"].GetCString();
            language = _accessor.HttpContext.Request.Headers["x-language"].GetCString();

            _dal._db.Aop.OnLogExecuting = (sql, e) =>
            {

            };
        }

        #region StandardInterface
        /// <summary>
        /// 查询
        /// </summary>
        public async Task<ResultModel> Get(BasicSearchModel searchModel)
        {
            ResultModel rm = new ResultModel();
            try
            {
                var where = DbDynamicQueryHelper.PgsqlDynamicWhereSqlParaGet<CodeMstr>(searchModel);
                var pageSort = ClassMapSqlsugar.GetDBSort<CodeMstr>(searchModel.SortList);
                var data = await _dal.Query(where.Item1, where.Item2);

                rm.Data = data;
            }
            catch (Exception ex)
            {
                rm.ResultCode = "10001";
                rm.ResultMsg = ex.Message;
            }
            return rm;
        }

        /// <summary>
        /// 新增
        /// </summary>
        public async Task<ResultModel> Post(CodeMstr codeMstr)
        {
            ResultModel rm = new ResultModel();
            try
            {
                #region "校验"
                Expression<Func<CodeMstr, bool>> whereExpression = we => 
                (
                    we.CodeCorpId == corp
                    && we.CodeDomainId == domain
                    && we.CodeName == codeMstr.CodeName
                    && we.CodeValue == codeMstr.CodeValue
                );
                var data = await _dal.Query(whereExpression);
                if(data.Count > 0)
                {
                    rm.ResultCode = "10001";
                    rm.ResultMsg = "该记录已存在".GetMessage(1001023, language);
                    return rm;
                }
                #endregion

                codeMstr.CodeId = Guid.NewGuid().ToString();
                var rtAdd = await _dal.Add(codeMstr);
                if (rtAdd != 1)
                {
                    rm.ResultCode = "10001";
                    rm.ResultMsg = "插入失败".GetMessage(1001009, language);
                    return rm;
                }
            }
            catch (Exception ex)
            {
                rm.ResultCode = "10001";
                rm.ResultMsg = ex.Message;
            }
            return rm;
        }

        /// <summary>
        /// 编辑
        /// </summary>
        public async Task<ResultModel> Put(CodeMstr codeMstr)
        {
            ResultModel rm = new ResultModel();
            try
            {
                #region "校验"
                Expression<Func<CodeMstr, bool>> whereExpression = we => 
                (
                    we.CodeCorpId == corp
                    && we.CodeDomainId == domain
                    && we.CodeName == codeMstr.CodeName
                    && we.CodeValue == codeMstr.CodeValue
                );
                var data = await _dal.Query(whereExpression);
                if(data.Count == 0)
                {
                    rm.ResultCode = "10001";
                    rm.ResultMsg = "该记录不存在".GetMessage(1001024, language);
                    return rm;
                }
                #endregion

                codeMstr.CodeId = data.FirstOrDefault().CodeId;
                var rtUpdate = await _dal.Update(codeMstr);
                if (!rtUpdate)
                {
                    rm.ResultCode = "10001";
                    rm.ResultMsg = "更新失败".GetMessage(1001010, language);
                    return rm;
                }
            }
            catch (Exception ex)
            {
                rm.ResultCode = "10001";
                rm.ResultMsg = ex.Message;
            }
            return rm;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="listCodeMstr"></param>
        public new async Task<ResultModel> Delete(List<CodeMstr> listCodeMstr)
        {
            ResultModel rm = new ResultModel();
            try
            {
                #region "校验"
                foreach (var codeMstr in listCodeMstr)  
                {
                    Expression<Func<CodeMstr, bool>> whereExpression = we =>
                    (
                    we.CodeCorpId == corp
                    && we.CodeDomainId == domain
                    && we.CodeName == codeMstr.CodeName
                    && we.CodeValue == codeMstr.CodeValue
                    );
                    var data = await _dal.Query(whereExpression);
                    if(data.Count == 0)
                    {
                        rm.ResultCode = "10001";
                        rm.ResultMsg = "该记录不存在".GetMessage(1001024, language);
                        return rm;
                    }
                }
                #endregion

                var rtDelete = await _dal.Delete(listCodeMstr);
                if (!rtDelete)
                {
                    rm.ResultCode = "10001";
                    rm.ResultMsg = "删除失败".GetMessage(1001013, language);
                    return rm;
                }
            }
            catch (Exception ex)
            {
                rm.ResultCode = "10001";
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
                string filePath = $"Template/codeMstrLoad{language}.xlsx";
                if (!File.Exists(filePath))
                {
                    rm.ResultCode = "10001";
                    rm.ResultMsg = "找不到该模板：".GetMessage(1001025, language) + filePath;
                    return rm;
                }
                rm.Data = filePath;
            }
            catch (Exception ex)
            {
                rm.ResultCode = "10001";
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
            _unitOfWork.BeginTran();
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
                    string fileType = file.FileName.Substring(fileName.IndexOf("."));
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
                            rm.ResultCode = "10001";
                            rm.ResultMsg = "文件为空！".GetMessage(1001009, language);
                            return rm;
                        }

                        #region "校验模板字段"
                        if (!dt.Columns.Contains("ID")||!dt.Columns.Contains("名称")||!dt.Columns.Contains("值")||!dt.Columns.Contains("描述")||!dt.Columns.Contains("激活"))
                        {
                            rm.ResultCode = "10001";
                            rm.ResultMsg = "文件为空！".GetMessage(1001009, language);
                            return rm;
                        }
                        #endregion

                        List<CodeMstrLoad> listLoad = new List<CodeMstrLoad>();
                        foreach (DataRow row in dt.Rows)
                        {
                            CodeMstr codeMstr = new CodeMstr
                            {
                                #region "字段"
                                CodeId = Guid.NewGuid().ToString(),
                                CodeCorpId = corp,
                                CodeDomainId = domain,
                                CodeName = row["名称"].GetCString(),
                                CodeValue = row["值"].GetCString(),
                                CodeDesc = row["描述"].GetCString(),
                                CodeActive = row["激活"].GetCBool(),
                                CodeCrtDatetime = DateTime.Now,
                                CodeCrtProg = "CodeMstrService-Import",
                                CodeCrtUser = user,
                                CodeModDatetime = DateTime.Now,
                                CodeModProg = "CodeMstrService-Import",
                                CodeModUser = user,
                                #endregion
                            };

                            CodeMstrLoad codeMstrLoad = (CodeMstrLoad)JsonHelper.JsonToObject(JsonHelper.ObjectToJson(codeMstr),typeof(CodeMstrLoad));

                            #region "校验必填项"

                            #endregion

                            //检查唯一索引
                            Expression<Func<CodeMstr, bool>> whereExpression = we => 
                            (
                                we.CodeCorpId == corp
                                && we.CodeDomainId == domain
                                && we.CodeName == codeMstr.CodeName
                                && we.CodeValue == codeMstr.CodeValue
                            );
                            if (_dal.Query(whereExpression).Result.Count == 0)
                            {
                                #region "自定义校验"

                                #endregion

                                var rtAdd = await _dal.Add(codeMstr);
                                if (rtAdd != 1)
                                {
                                    codeMstrLoad.Canpass = false;
                                    codeMstrLoad.Errormessage = "插入失败".GetMessage(1001009, language);
                                }
                            }
                            else
                            {
                                #region "自定义校验"

                                #endregion

                                var rtUpdate = await _dal.Update(codeMstr);
                                if (!rtUpdate)
                                {
                                    codeMstrLoad.Canpass = false;
                                    codeMstrLoad.Errormessage = "更新失败".GetMessage(1001009, language);
                                }
                            }

                            listLoad.Add(codeMstrLoad);
                        }
                        rm.Data = listLoad;
                        _unitOfWork.CommitTran();

                        if (listLoad.Where(codeMstrLoad => codeMstrLoad.Canpass == false).ToList().Count > 0)
                        {
                            rm.ResultCode = "10001";
                            rm.ResultMsg = "存在导入失败数据".GetMessage(1001009, language);
                            return rm;
                        }
                    }
                    else
                    {
                        rm.ResultCode = "10001";
                        rm.ResultMsg = "文件类型错误！".GetMessage(1001009, language);
                        return rm;
                    }
                }
                else
                {
                    rm.ResultCode = "10001";
                    rm.ResultMsg = "请选择上传文件！";
                    return rm;
                }
            }
            catch (Exception ex)
            {
                _unitOfWork.RollbackTran();
                rm.ResultCode = "10001";
                rm.ResultMsg = ex.Message;
            }
            return rm;
        }

        /// <summary>
        /// 输出
        /// </summary>
        public async Task<ResultModel> Export(BasicSearchModel searchModel)
        {
            ResultModel rm = new ResultModel();
            try
            {
                var where = DbDynamicQueryHelper.PgsqlDynamicWhereSqlParaGet<CodeMstr>(searchModel);
                var pageSort = ClassMapSqlsugar.GetDBSort<CodeMstr>(searchModel.SortList);
                DataTable dt = await _dal.QueryTable(where.Item1, where.Item2, pageSort);

                List<TableHeaderModel> listTableHeader = searchModel.TableHead;
                if (listTableHeader == null || listTableHeader.Count == 0)
                {
                    rm.ResultCode = "10001";
                    rm.ResultMsg = "表格头信息json错误".GetMessage(1001024, language);
                    return rm;
                }

                DataTable dtHeader = dt.Clone();
                foreach (DataColumn col in dtHeader.Columns)
                {
                    if (listTableHeader != null && listTableHeader.Count > 0 && listTableHeader.Where(p => ClassMapSqlsugar.GetDBColumnName<CodeMstr>(p.Property).ToLower() == col.ColumnName.ToLower() && p.IsShow == true).ToList().Count > 0)
                    {
                        dt.Columns[col.ColumnName].ColumnName = listTableHeader.Where(p => ClassMapSqlsugar.GetDBColumnName<CodeMstr>(p.Property).ToLower() == col.ColumnName.ToLower()).ToList().FirstOrDefault().Label;
                    }
                    else
                    {
                        dt.Columns.Remove(col.ColumnName);
                    }
                }
                int index = 0;
                foreach (TableHeaderModel p in listTableHeader.Where(p => p.IsShow == true).ToList())
                {
                    if (dt.Columns.Contains(p.Label))
                        dt.Columns[p.Label].SetOrdinal(index);
                    index++;
                }

                string path = "Download";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string fileName = "CodeMstr" + DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(10000000, 99999999) + ".xlsx";
                string filePath = path + @"/" + fileName;
                ExcelHelper eh = new ExcelHelper(filePath);
                eh.DataTableToExcel(dt);

                rm.Data = filePath;
            }
            catch (Exception ex)
            {
                rm.ResultCode = "10001";
                rm.ResultMsg = ex.Message;
            }
            return rm;
        }

        #endregion

        #region CustomInterface
        #endregion

    }
}

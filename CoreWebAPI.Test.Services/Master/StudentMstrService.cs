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
using CoreWebAPI.Common.LogHelper;
using System.Reflection;
using CoreWebAPI.Common.Redis;
using SqlSugar;
using CoreWebAPI.Repository;

namespace CoreWebAPI.Test.Services.Master
{
    public class StudentMstrService : BaseService<StudentMstr>, IStudentMstrService
    {
        private readonly IBaseRepository<StudentMstr> _dal;
        private readonly IBaseRepository<StudentMstrNew> _dalNew;
        private readonly IRedisBasketRepository _redis;
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
        public StudentMstrService(IBaseRepository<StudentMstr> dal,IBaseRepository<StudentMstrNew> dalNew, IRedisBasketRepository redis, IUnitOfWork unitOfWork, IHttpContextAccessor accessor)
        {
            this._dal = dal;
            this._dalNew = dalNew;
            base.BaseDal = dal;
            _redis = redis;
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
        public async Task<ResultModel> Get(StudentMstrSearchModel studentMstrSearchModel)
        {
            ResultModel rm = new ResultModel();
            try
            {
                //Expression<Func<StudentMstr, bool>> whereExpression = we =>
                //    we.StudentCorpId == corp
                //    && (studentMstrSearchModel.StudentId.GetIsEmptyOrNull() || we.StudentId.ToLower().Equals(studentMstrSearchModel.StudentId.ToLower()))
                //    && (studentMstrSearchModel.StudentName.GetIsEmptyOrNull() || we.StudentName.ToLower().Equals(studentMstrSearchModel.StudentName.ToLower()))
                //    && (studentMstrSearchModel.StudentAge.GetIsEmptyOrNull() || we.StudentAge == studentMstrSearchModel.StudentAge.GetCDecimal())
                //    && (studentMstrSearchModel.StudentEnrolmentDate.GetIsEmptyOrNull() || we.StudentEnrolmentDate == studentMstrSearchModel.StudentEnrolmentDate.GetCDate())
                //    && (studentMstrSearchModel.StudentAddr.GetIsEmptyOrNull() || we.StudentAddr.ToLower().Equals(studentMstrSearchModel.StudentAddr.ToLower()))
                //    && (studentMstrSearchModel.StudentAddr.GetIsEmptyOrNull() || we.StudentGraduated == studentMstrSearchModel.StudentGraduated.GetCBool())
                //;
                //var pageSort = ClassMapSqlsugar.GetDBSort<StudentMstr>(studentMstrSearchModel.SortList);
                //var data = await _dal.QueryPage(whereExpression, studentMstrSearchModel.PageIndex, studentMstrSearchModel.PageSize, pageSort);

                _unitOfWork.BeginTran();
                //var data = await _dal.QueryLock(whereExpression);

                _unitOfWork.CommitTran();

                var data = await _dal.Query();
                var dataNew = await _dalNew.Query();

                _dal.ChangeDB("MYDB_PGSQLNew");
                var data1 = await _dal.Query();
                var dataNew1 = await _dalNew.Query();

                //var rtadd = await _dal.Add(new StudentMstr() { Id=Guid.NewGuid().ToString()});
                //_dal.ChangeDB("MYDB_PGSQL");
                //var data2 = await _dal.Query();
                //var dataNew2 = await _dalNew.Query();
                //var rtadd2 = await _dal.Add(new StudentMstr() { Id = Guid.NewGuid().ToString() });


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
        /// 查询
        /// </summary>
        public async Task<ResultModel> Get(BasicSearchModel searchModel)
        {
            ResultModel rm = new ResultModel();
            try
            {
                var s = Appsettings.App("test");
                await _redis.Set("key1", "test1", TimeSpan.FromSeconds(3600000));
                await _redis.Set("key2", "test2", TimeSpan.FromSeconds(3600000));

                var isex =await _redis.Exist("key2");



                //Expression<Func<StudentMstr, PiptMstr, PimachineMstr, object[]>> joinExpression = (we, pt, machine) => new object[]
                //{
                //    JoinType.Left,we.StudentCorpId == pt.PiptCorpId
                //    && we.StudentDomainId == pt.PiptDomainId
                //    && we.StudentId == pt.PiptPart,
                //    JoinType.Left,we.StudentCorpId == machine.PimachineCorpId
                //    && we.StudentDomainId == machine.PimachineDomainId
                //    && we.StudentId == machine.PimachineMachine
                //};

                //Expression<Func<StudentMstr, PiptMstr, PimachineMstr, PiresultDetViewModel>> selectExpression = (we, pt, machine) => new PiresultDetViewModel
                //{
                //    Id = we.Id.SelectAll()
                //};

                var prop = typeof(StudentMstr).GetProperties(BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
                var where = DbDynamicQueryHelper.PgsqlDynamicWhereSqlParaGet<StudentMstr>(searchModel);
                var pageSort = ClassMapSqlsugar.GetDBSort<StudentMstr>(searchModel.SortList);
                var data = await _dal.Query(where.Item1, where.Item2);

                //var listRt = data.Where(item => item.Id.Contains(""));

                var data1 = data.GetPageWithSelect(new List<string>(), searchModel.PageIndex, searchModel.PageSize, searchModel.SortList);

                var data1Test = (PageModel<StudentMstr>)data1;

                var data2 = data.GetSelectPage(new List<string>() { "StudentId", "StudentName" }, searchModel.PageIndex, searchModel.PageSize);

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
        public async Task<ResultModel> Post(StudentMstr studentMstr)
        {
            ResultModel rm = new ResultModel();
            try
            {
                _redis.RemoveBegin("key");

                #region "校验"
                Expression<Func<StudentMstr, bool>> whereExpression = we => 
                    we.StudentCorpId == corp
                    && we.StudentDomainId == domain
                    && we.StudentId == studentMstr.StudentId
                ;
                var data = await _dal.Query(whereExpression);
                if(data.Count > 0)
                {
                    rm.ResultCode = "1000001";
                    rm.ResultMsg = "该记录已存在".GetMessage(1001023, language);
                    return rm;
                }

                whereExpression = we =>
                   we.StudentCorpId == corp
                   && we.StudentDomainId == domain
               ;
                var data1 = await _dal.Query(whereExpression);
                #endregion

                studentMstr.Id = Guid.NewGuid().ToString();
                studentMstr.StudentCorpId = corp;
                studentMstr.StudentDomainId = domain;
                studentMstr.StudentCrtDatetime = DateTime.Now;
                studentMstr.StudentCrtUser = user;
                studentMstr.StudentCrtProg = "StudentMstrService-Post";
                //var rtAdd = await _dal.AddWithSplitTable(studentMstr);
                var rtAdd = await _dal.Add(studentMstr);
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
        public async Task<ResultModel> Put(StudentMstr studentMstr)
        {
            ResultModel rm = new ResultModel();
            try
            {
                #region "校验"
                Expression<Func<StudentMstr, bool>> whereExpression = we => 
                    we.StudentCorpId == corp
                    && we.StudentDomainId == domain
                    && we.StudentId == studentMstr.StudentId
                ;
                var data = await _dal.Query(whereExpression);
                if(data.Count == 0)
                {
                    rm.ResultCode = "1000001";
                    rm.ResultMsg = "该记录不存在".GetMessage(1001024, language);
                    return rm;
                }
                #endregion

                studentMstr.Id = data.FirstOrDefault().Id;
                studentMstr.StudentModDatetime = DateTime.Now;
                studentMstr.StudentModUser = user;
                studentMstr.StudentModProg = "StudentMstrService-Put";
                var rtUpdate = await _dal.UpdateCheckVersion(studentMstr);
                if (rtUpdate != 1)
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
        /// <param name="listStudentMstr"></param>
        public new async Task<ResultModel> Delete(List<StudentMstr> listStudentMstr)
        {
            ResultModel rm = new ResultModel();
            try
            {
                #region "校验"
                foreach (var studentMstr in listStudentMstr)  
                {
                    Expression<Func<StudentMstr, bool>> whereExpression = we => 
                    we.StudentCorpId == corp
                    && we.StudentDomainId == domain
                    && we.StudentId == studentMstr.StudentId
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

                var rtDelete = await _dal.Delete(listStudentMstr);
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
                string filePath = $"Template/studentMstrLoad{language}.xlsx";
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
                        if (!dt.Columns.Contains("学生号")||!dt.Columns.Contains("姓名")||!dt.Columns.Contains("年龄")||!dt.Columns.Contains("入学时间")||!dt.Columns.Contains("地址")||!dt.Columns.Contains("已毕业"))
                        {
                            rm.ResultCode = "1000001";
                            rm.ResultMsg = "文件模板错误！请重新下载模板".GetMessage(1001009, language);
                            return rm;
                        }
                        #endregion

                        List<StudentMstrLoad> listLoad = new List<StudentMstrLoad>();
                        foreach (DataRow row in dt.Rows)
                        {
                            StudentMstr studentMstr = new StudentMstr
                            {
                                #region "字段"
                                Id = Guid.NewGuid().ToString(),
                                StudentCorpId = corp,
                                StudentDomainId = domain,
                                StudentId = row["学生号"].GetCString(),
                                StudentName = row["姓名"].GetCString(),
                                StudentAge = row["年龄"].GetCDecimal(),
                                StudentEnrolmentDate = row["入学时间"].GetCDate(),
                                StudentAddr = row["地址"].GetCString(),
                                StudentGraduated = row["已毕业"].GetCBool(),
                                StudentCrtDatetime = DateTime.Now,
                                StudentCrtProg = "StudentMstrService-Import",
                                StudentCrtUser = user,
                                StudentModDatetime = DateTime.Now,
                                StudentModProg = "StudentMstrService-Import",
                                StudentModUser = user
                                #endregion
                            };
                            StudentMstrLoad studentMstrLoad = (StudentMstrLoad)JsonHelper.JsonToObject(JsonHelper.ObjectToJson(studentMstr),typeof(StudentMstrLoad));

                            //#region "校验必填项"

                            //#endregion

                            ////检查唯一索引
                            //Expression<Func<StudentMstr, bool>> whereExpression = we => 
                            //    we.StudentCorpId == corp
                            //    && we.StudentDomainId == domain
                            //    && we.StudentId == studentMstr.StudentId
                            //;
                            //var data = await _dal.Query(whereExpression);
                            //if (data.Count == 0)
                            //{
                            //    #region "自定义校验"

                            //    #endregion

                            //    var rtAdd = await _dal.Add(studentMstr);
                            //    if (rtAdd != 1)
                            //    {
                            //        studentMstrLoad.Canpass = false;
                            //        studentMstrLoad.Errormessage = "插入失败".GetMessage(1001009, language);
                            //    }
                            //}
                            //else
                            //{
                            //    #region "自定义校验"

                            //    #endregion

                            //    studentMstr.Id = data.FirstOrDefault().Id;
                            //    var rtUpdate = await _dal.Update(studentMstr);
                            //    if (!rtUpdate)
                            //    {
                            //        studentMstrLoad.Canpass = false;
                            //        studentMstrLoad.Errormessage = "更新失败".GetMessage(1001009, language);
                            //    }
                            //}

                            listLoad.Add(studentMstrLoad);
                        }
                        rm.Data = listLoad;
                        _unitOfWork.CommitTran();

                        if (listLoad.Where(studentMstrLoad => studentMstrLoad.Canpass == false).ToList().Count > 0)
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
                _unitOfWork.RollbackTran();
                rm.ResultCode = "1000001";
                rm.ResultMsg = ex.Message;
            }
            return rm;
        }

        /// <summary>
        /// 列表导入
        /// </summary>
        public async Task<ResultModel> ImportList(List<StudentMstr> listStudentMstr)
        {
            ResultModel rm = new ResultModel();
            _unitOfWork.BeginTran();
            try
            {
                if (listStudentMstr != null && listStudentMstr.Count > 0) 
                {
                    List<StudentMstrLoad> listLoad = new List<StudentMstrLoad>();
                    foreach (var item in listStudentMstr)
                    {
                        StudentMstr studentMstr = new StudentMstr
                        {
                            #region "字段"
                            Id = Guid.NewGuid().ToString(),
                            StudentCorpId = corp,
                            StudentDomainId = domain,
                            StudentId = item.StudentId.GetCString(),
                            StudentName = item.StudentName.GetCString(),
                            StudentAge = item.StudentAge.GetCDecimal(),
                            StudentEnrolmentDate = item.StudentEnrolmentDate.GetCDate(),
                            StudentAddr = item.StudentAddr.GetCString(),
                            StudentGraduated = item.StudentGraduated.GetCBool(),
                            StudentCrtDatetime = DateTime.Now,
                            StudentCrtProg = "StudentMstrService-ImportList",
                            StudentCrtUser = user,
                            StudentModDatetime = DateTime.Now,
                            StudentModProg = "StudentMstrService-ImportList",
                            StudentModUser = user
                            #endregion
                        };
                        StudentMstrLoad studentMstrLoad = (StudentMstrLoad)JsonHelper.JsonToObject(JsonHelper.ObjectToJson(studentMstr), typeof(StudentMstrLoad));

                        #region "校验必填项"

                        #endregion

                        //检查唯一索引
                        Expression<Func<StudentMstr, bool>> whereExpression = we =>
                            we.StudentCorpId == corp
                            && we.StudentDomainId == domain
                            && we.StudentId == studentMstr.StudentId
                        ;
                        var data = await _dal.Query(whereExpression);
                        if (data.Count == 0)
                        {
                            #region "自定义校验"

                            #endregion

                            var rtAdd = await _dal.Add(studentMstr);
                            if (rtAdd != 1)
                            {
                                studentMstrLoad.Canpass = false;
                                studentMstrLoad.Errormessage = "插入失败".GetMessage(1001009, language);
                            }
                        }
                        else
                        {
                            #region "自定义校验"

                            #endregion

                            studentMstr.Id = data.FirstOrDefault().Id;
                            var rtUpdate = await _dal.Update(studentMstr);
                            if (!rtUpdate)
                            {
                                studentMstrLoad.Canpass = false;
                                studentMstrLoad.Errormessage = "更新失败".GetMessage(1001009, language);
                            }
                        }

                        listLoad.Add(studentMstrLoad);
                    }
                    rm.Data = listLoad;
                    _unitOfWork.CommitTran();

                    if (listLoad.Where(studentMstrLoad => studentMstrLoad.Canpass == false).ToList().Count > 0)
                    {
                        rm.ResultCode = "1000001";
                        rm.ResultMsg = "存在导入失败数据".GetMessage(1001009, language);
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
                _unitOfWork.RollbackTran();
                rm.ResultCode = "1000001";
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
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            try
            {
                var where = DbDynamicQueryHelper.PgsqlDynamicWhereSqlGet<StudentMstr>(searchModel);
                var pageSort = ClassMapSqlsugar.GetDBSort<StudentMstr>(searchModel.SortList);
                DataTable dt = await _dal.QueryTable(where,new { }, pageSort);

                List<TableHeaderModel> listTableHeader = searchModel.TableHead;
                if (listTableHeader == null || listTableHeader.Count == 0)
                {
                    rm.ResultCode = "1000001";
                    rm.ResultMsg = "表格头信息错误".GetMessage(1001024, language);
                    return rm;
                }

                DataTable dtHeader = dt.Clone();
                foreach (DataColumn col in dtHeader.Columns)
                {
                    if (listTableHeader != null && listTableHeader.Count > 0 && listTableHeader.Where(p => ClassMapSqlsugar.GetDBColumnName<StudentMstr>(p.Property).ToLower() == col.ColumnName.ToLower() && p.IsShow == true).ToList().Count > 0)
                    {
                        dt.Columns[col.ColumnName].ColumnName = listTableHeader.Where(p => ClassMapSqlsugar.GetDBColumnName<StudentMstr>(p.Property).ToLower() == col.ColumnName.ToLower()).ToList().FirstOrDefault().Label;
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
                string fileName = "StudentMstr" + DateTime.Now.Ticks + new Random().Next(10000000, 99999999) + ".xlsx";
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
        #endregion

    }
}

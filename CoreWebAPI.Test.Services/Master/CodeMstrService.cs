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
using System.Text;
using System.Net;
using SqlSugar;
using NetTaste;
using System.Runtime.InteropServices;

namespace CoreWebAPI.Test.Services.Master
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
            _dal._db.Aop.OnLogExecuting = (sql, p) =>
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
                RefAsync<int> totalCount = 0;
                RefAsync<int> pageCount = 0;
                _dal._db.InitMappingInfo<CodeMstrSalve>();
                var data = await _dal._db.AsTenant().QueryableWithAttr<CodeMstr>().Includes(x => x.CodeSalve
                    .MappingField(z => z.CodeSalveCorpId, () => x.CodeCorpId)
                    //.Where(z => z.CodeSalveDomainId == "1")
                    .ToList()
                )
                //.Where(x => SqlFunc.GreaterThan(x.CodeName, "name5550000"))
                .OrderBy(x => x.CodeId)
                //.ToListAsync();
                //.Where(x => x.CodeSalve.Any(z => z.CodeSalveDomainId == "1"))
                .ToPageListAsync(1, 100, totalCount, pageCount);

                var list = data.Where(item => /*item.CodeName == "name6666666" &&*/ item.CodeSalve != null && item.CodeSalve.Exists(salve => string.Compare(salve.CodeSalveName, "name5550000") > 0)).ToList();
                rm.Data = list;
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
        public async Task<ResultModel> Import(List<IFormFile> files)
        {
            ResultModel rm = new ResultModel();
            _unitOfWork.BeginTran();
            try
            {
                //string str = "http://10.124.0.109:19001/api/serialNumber/newSN";  //获取配置文件
                //str += $"?code=PO&sndUseFunc=";
                //HttpRequestHelper httpHelper = new HttpRequestHelper
                //{
                //    Uri = str,
                //    Type = HttpType.GET
                //};
                //rm.Data = httpHelper.Request();

                HttpRequestHelper httpHelper = new HttpRequestHelper
                {
                    Uri = "http://10.124.0.109:8077/api/DocdDet/Post",
                    Type = HttpType.POST,
                    BodyType = BodyType.Multipart,
                    Header = new Dictionary<string, string>
                    {
                        { "x-domain","ARN"}
                    }
                };
                FormItemModel dataJson = new FormItemModel()
                {
                    Key = "jsonDocHandleStructure",
                    Value = "{\"typeCode\":\"wbzft\",\"listField\":[{\"fieldName\":\"wbzft_column1\",\"fieldDesc\":\"零件号\",\"fieldUnique\":true,\"fieldValue\":\"pt1\"},{\"fieldName\":\"wbzft_column2\",\"fieldDesc\":\"名称\",\"fieldUnique\":false,\"fieldValue\":\"\"}]}"
                };
                httpHelper.FormModelList.Add(dataJson);
                foreach(var fileItem in files)
                {
                    Stream fs = fileItem.OpenReadStream();//将文件转为流
                    byte[] bt = new byte[fileItem.Length];
                    fs.Read(bt, 0, bt.Length);
                    MemoryStream ms = new MemoryStream(bt);
                    FormItemModel formItem = new FormItemModel()
                    {
                        Key = "listFile",
                        FileName = fileItem.FileName,
                        FileContent = ms
                    };
                    httpHelper.FormModelList.Add(formItem);

                }
                rm.Data = httpHelper.Request();

                return rm;
                var file = files[0];
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


                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"http://10.124.0.109:8077/api/DocdDet/Post");
                        request.Method = "POST";
                        Stream postStream = new MemoryStream();

                        //通过表单上传文件
                        string boundary = "----" + DateTime.Now.Ticks.ToString("x");
                        try
                        {
                            string formdataTemplate = "\r\n--" + boundary + "\r\nContent-Disposition: form-data; name=\"{0}\"\r\n\r\n";
                            var formdata = string.Format(formdataTemplate, "jsonDocHandleStructure");
                            string data = "{\"typeCode\":\"wbzft\",\"listField\":[{\"fieldName\":\"wbzft_column1\",\"fieldDesc\":\"零件号\",\"fieldUnique\":true,\"fieldValue\":\"pt1\"},{\"fieldName\":\"wbzft_column2\",\"fieldDesc\":\"名称\",\"fieldUnique\":false,\"fieldValue\":\"\"}]}";

                            var formdataBytes = Encoding.UTF8.GetBytes(postStream.Length == 0 ? formdata.Substring(2, formdata.Length - 2) : formdata);//第一行不需要换行
                            postStream.Write(formdataBytes, 0, formdataBytes.Length);
                            byte[] buffer = Encoding.UTF8.GetBytes(data);
                            postStream.Write(buffer, 0, buffer.Length);
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }


                        try
                        {
                            string formdataTemplate = "\r\n--" + boundary + "\r\nContent-Disposition: form-data; name=\"{0}\"filename=\"{1}\"\r\nContent-Type: application/octet-stream\r\n\r\n";

                            var formdata = string.Format(formdataTemplate, "listFile", System.IO.Path.GetFileName(fileName));
                            var formdataBytes = Encoding.UTF8.GetBytes(postStream.Length == 0 ? formdata.Substring(2, formdata.Length - 2) : formdata);//第一行不需要换行
                            postStream.Write(formdataBytes, 0, formdataBytes.Length);
                            //写入文件
                            byte[] buffer = new byte[1024];
                            int bytesRead = 0;
                            while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) != 0)
                            {
                                postStream.Write(buffer, 0, bytesRead);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }


                        //结尾
                        var footer = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
                        postStream.Write(footer, 0, footer.Length);
                        request.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);

                        request.ContentLength = postStream != null ? postStream.Length : 0;
                        request.Accept = "*/*";
                        //request.Accept "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*;q=0.8";
                        request.KeepAlive = true;
                        request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.1650.5Safari/537.36";
                        if (postStream != null)
                        {
                            postStream.Position = 0;
                            //直接写入流
                            Stream requestStream = request.GetRequestStream();
                            byte[] buffer = new byte[1024];
                            int bytesRead = 0;
                            while ((bytesRead = postStream.Read(buffer, 0, buffer.Length)) != 0)
                            {
                                requestStream.Write(buffer, 0, bytesRead);
                            }

                            postStream.Close();//关闭文件访问
                        }

                        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                        using (Stream responseStream = response.GetResponseStream())
                        {
                            using (StreamReader myStreamReader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8")))
                            {
                                string retString = myStreamReader.ReadToEnd();
                                string rt = retString;
                            }
                        }
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

        ///// <summary>
        ///// 请求类型
        ///// </summary>
        //public enum HttpType
        //{
        //    PUT = 0,
        //    GET = 1,
        //    POST = 2,
        //    DELETE = 3
        //}

        ///// <summary>
        ///// Body类型
        ///// </summary>
        //public enum BodyType
        //{
        //    Form, FileForm, Json, XML
        //}

        //public class HttpRequestHelper
        //{
        //    public string Uri;
        //    public HttpType Type;
        //    public BodyType BodyType;
        //    public Encoding ContentEncoding = Encoding.UTF8;
        //    public Dictionary<string, string> Query = new Dictionary<string, string>();
        //    public Dictionary<string, string> Header = new Dictionary<string, string>();
        //    public Dictionary<string, object> Body = new Dictionary<string, object>();
        //    public string BodyStr;
        //    public int Timeout = 300000;

        //    //拼接Query到url
        //    public string UriWithQuery
        //    {
        //        get
        //        {
        //            var query = ParseQueryString(Query);
        //            return Uri + (query == "" ? "" : "?" + query);
        //        }
        //    }
        //    public string Request()
        //    {
        //        string rt = string.Empty;
        //        HttpWebRequest myRequest = null;
        //        HttpWebResponse myResponse = null;
        //        Stream reqStream = null;
        //        StreamReader reader = null;
        //        try
        //        {
        //            //创建一个HTTP请求
        //            myRequest = (HttpWebRequest)WebRequest.Create(UriWithQuery);
        //            //myRequest.ProtocolVersion = HttpVersion.Version10;
        //            myRequest.Method = Type.ToString();
        //            //内容类型
        //            myRequest.ContentType = "application/x-www-form-urlencoded";
        //            myRequest.AutomaticDecompression = DecompressionMethods.GZip;
        //            myRequest.MaximumAutomaticRedirections = 1;
        //            myRequest.AllowAutoRedirect = true;
        //            myRequest.Timeout = Timeout;
        //            //HttpRequestCachePolicy noCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
        //            //myRequest.CachePolicy = noCachePolicy;
        //            //myRequest.Headers.Add("body", body);
        //            foreach (var item in Header)
        //            {
        //                myRequest.Headers.Add(item.Key, item.Value);
        //            }

        //            //将Body字符串转化为字节
        //            byte[] bodyByte = null;
        //            switch (BodyType)
        //            {
        //                case BodyType.Form:
        //                    {
        //                        myRequest.ContentType = "application/x-www-form-urlencoded";
        //                        var bodydata = Body.Select(pair => pair.Key + "=" + pair.Value.GetCString())
        //                            .DefaultIfEmpty("")
        //                            .Aggregate((a, b) => a + "&" + b);
        //                        bodyByte = ContentEncoding.GetBytes(bodydata);
        //                        break;
        //                    }
        //                case BodyType.Json:
        //                    {
        //                        myRequest.ContentType = "application/json";
        //                        var bodydata = JsonHelper.ObjectToJson(Body);
        //                        bodyByte = ContentEncoding.GetBytes(bodydata);
        //                        break;
        //                    }
        //                case BodyType.XML:
        //                    {
        //                        myRequest.ContentType = "text/xml; charset=utf-8";
        //                        bodyByte = ContentEncoding.GetBytes(BodyStr);
        //                        break;
        //                    }
        //                case BodyType.FileForm:
        //                    {
        //                        //通过表单上传文件
        //                        string boundary = "----" + DateTime.Now.Ticks.ToString("x");
        //                        myRequest.ContentType = $"multipart/form-data; boundary={boundary}";

        //                        foreach (var item in Body)
        //                        {
        //                            if (item.Value.GetType().ToString() == "IFormFile")
        //                            {

        //                            }
        //                            else
        //                            {

        //                            }
        //                        }

        //                        //bodyByte = ContentEncoding.GetBytes(bodydata);
        //                        break;
        //                    }
        //            }
        //            if (bodyByte != null && bodyByte.Length > 0)
        //            {
        //                //byte[] buf = ContentEncoding.GetBytes(Body);
        //                myRequest.ContentLength = bodyByte.Length;

        //                //发送请求，获得请求流 
        //                reqStream = myRequest.GetRequestStream();
        //                reqStream.Flush();
        //                reqStream.Write(bodyByte, 0, bodyByte.Length);
        //                reqStream.Flush();
        //                reqStream.Close();
        //            }

        //            // 获得接口返回值
        //            myResponse = (HttpWebResponse)myRequest.GetResponse();
        //            reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
        //            rt = reader.ReadToEnd();
        //            reader.Close();
        //            myResponse.Close();
        //            myRequest.Abort();
        //        }
        //        catch (Exception ex)
        //        {
        //            rt = ex.Message;
        //        }
        //        finally
        //        {
        //            if (reqStream != null) reqStream.Close();
        //            if (reader != null) reader.Close();
        //            if (myResponse != null) myResponse.Close();
        //            if (myRequest != null) myRequest.Abort();
        //        }
        //        return rt;
        //    }

        //    public static string ParseQueryString(Dictionary<string, string> querys)
        //    {
        //        if (querys.Count == 0)
        //            return "";
        //        return querys
        //            .Select(pair => pair.Key + "=" + pair.Value)
        //            .Aggregate((a, b) => a + "&" + b);
        //    }


        //}

        private byte[] StringToBytes(string s)
        {
            string[] str = s.Split(' ');
            int n = str.Length;

            byte[] cmdBytes = null;
            int p = 0;


            for (int k = 0; k < n; k++)
            {
                int sLen = str[k].Length;
                int bytesLen = sLen / 2;
                int position = 0;
                byte[] bytes = new byte[bytesLen];
                for (int i = 0; i < bytesLen; i++)
                {
                    string abyte = str[k].Substring(position, 2);
                    bytes[i] = Convert.ToByte(abyte, 16);
                    position += 2;
                }

                if (position >= 2)
                {
                    byte[] cmdBytes2 = new byte[p + bytesLen];
                    if (cmdBytes != null)
                    {
                        Array.Copy(cmdBytes, 0, cmdBytes2, 0, p);
                    }
                    Array.Copy(bytes, 0, cmdBytes2, p, bytesLen);
                    cmdBytes = cmdBytes2;
                    p += bytesLen;
                }
            }

            return cmdBytes;
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
                DataTable dt = await _dal.QueryTable(where.Item1,where.Item2, pageSort);

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

using NPOI.OpenXmlFormats;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Net.Security;

namespace CoreWebAPI.Common.Helper
{
    /// <summary>
    /// 请求类型
    /// </summary>
    public enum HttpType
    {
        PUT = 0,
        GET = 1,
        POST = 2,
        DELETE = 3
    }

    /// <summary>
    /// Body类型
    /// </summary>
    public enum BodyType
    {
        Form, Json, ListJson, XML , Multipart
    }

    /// <summary>
    /// 上传文件流用
    /// </summary>
    public class FormItemModel
    {
        /// <summary>
        /// 表单键，request["key"]
        /// </summary>
        public string Key { set; get; }

        /// <summary>
        /// 表单值,上传文件时忽略，request["key"].value
        /// </summary>
        public string Value { set; get; }

        /// <summary>
        /// 是否是文件
        /// </summary>
        public bool IsFile
        {
            get
            {
                if (FileContent == null || FileContent.Length == 0)
                { return false; }
                else { return true; }
            }
        }

        /// <summary>
        /// 上传的文件名
        /// </summary>
        public string FileName { set; get; }

        /// <summary>
        /// 上传的文件内容
        /// </summary>
        public Stream FileContent { set; get; }
    }

    public class HttpRequestHelper
    {
        public string Uri;
        public HttpType Type;
        public BodyType BodyType;
        public Encoding ContentEncoding = Encoding.UTF8;
        public Dictionary<string, string> Query = new Dictionary<string, string>();
        public Dictionary<string, string> Header = new Dictionary<string, string>();
        public Dictionary<string, object> Body = new Dictionary<string, object>();
        public List<Dictionary<string, object>> BodyList = new List<Dictionary<string, object>>();
        public List <FormItemModel> FormModelList = new List<FormItemModel>();
        public string BodyStr;
        public byte[] BodyBytes;
        public int Timeout = 300000;
        static string ProxyUrl = "";
        static string ProxyPort = "";
        //拼接Query到url
        public string UriWithQuery
        {
            get
            {
                var query = ParseQueryString(Query);
                return Uri + (query == "" ? "" : "?" + query);
            }
        }

        public string Request()
        {
            string rt = string.Empty;
            HttpWebRequest myRequest = null;
            HttpWebResponse myResponse = null;
            Stream reqStream = null;
            StreamReader reader = null;
            try
            {
                //创建一个HTTP请求
                myRequest = (HttpWebRequest)WebRequest.Create(UriWithQuery);
                //myRequest.ProtocolVersion = HttpVersion.Version10;
                myRequest.Method = Type.ToString();
                //内容类型
                //myRequest.ContentType = Type == "xml" ? "text/xml; charset=utf-8" : "application/json; charset=UTF-8";
                myRequest.AutomaticDecompression = DecompressionMethods.GZip;
                myRequest.MaximumAutomaticRedirections = 1;
                myRequest.AllowAutoRedirect = true;
                myRequest.Timeout = Timeout;
                //HttpRequestCachePolicy noCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
                //myRequest.CachePolicy = noCachePolicy;
                //myRequest.Headers.Add("body", body);
                foreach (var item in Header)
                {
                    myRequest.Headers.Add(item.Key, item.Value);
                }

                //将Body字符串转化为字节
                byte[] bodyByte = null;
                Stream postStream = new MemoryStream(); //流定义
                switch (BodyType)
                {
                    case BodyType.Form:
                        {
                            myRequest.ContentType = "application/x-www-form-urlencoded";
                            var bodydata = Body.Select(pair => pair.Key + "=" + pair.Value.GetCString())
                                .DefaultIfEmpty("")
                                .Aggregate((a, b) => a + "&" + b);
                            bodyByte = ContentEncoding.GetBytes(bodydata);
                            break;
                        }
                    case BodyType.Json:
                        {
                            myRequest.ContentType = "application/json";
                            var bodydata = JsonHelper.ObjectToJson(Body);
                            bodyByte = ContentEncoding.GetBytes(bodydata);
                            break;
                        }
                    case BodyType.ListJson:
                        {
                            myRequest.ContentType = "application/json";
                            var bodydata = JsonHelper.ObjectToJson(BodyList);
                            bodyByte = ContentEncoding.GetBytes(bodydata);
                            break;
                        }
                    case BodyType.XML:
                        {
                            myRequest.ContentType = "text/xml; charset=utf-8";
                            bodyByte = ContentEncoding.GetBytes(BodyStr);
                            break;
                        }
                    case BodyType.Multipart:
                        {
                            //表单上传文件
                            string boundary = "----" + DateTime.Now.Ticks.ToString("x"); //分隔符
                            myRequest.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);

                            //判断是否用Form上传文件
                            var formUploadFile = FormModelList != null && FormModelList.Count > 0;
                            if (formUploadFile)
                            {
                                //文件数据模板
                                string fileFormdataTemplate =
                                    "\r\n--" + boundary +
                                    "\r\nContent-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"" +
                                    "\r\nContent-Type: application/octet-stream" +
                                    "\r\n\r\n";
                                //文本数据模板
                                string dataFormdataTemplate =
                                    "\r\n--" + boundary +
                                    "\r\nContent-Disposition: form-data; name=\"{0}\"" +
                                    "\r\n\r\n{1}";

                                foreach (var item in FormModelList)
                                {
                                    string formdata = null;
                                    if (item.IsFile)
                                    {
                                        //上传文件
                                        formdata = string.Format(
                                            fileFormdataTemplate,
                                            item.Key, //表单键
                                            item.FileName //文件名
                                        );
                                    }
                                    else
                                    {
                                        //上传文本
                                        formdata = string.Format(
                                            dataFormdataTemplate,
                                            item.Key,  //表单键
                                            item.Value //文件值 Json等等
                                        );
                                    }

                                    //统一处理
                                    byte[] formdataBytes = null;
                                    //第一行不需要换行
                                    if (postStream.Length == 0)
                                        formdataBytes = Encoding.UTF8.GetBytes(formdata.Substring(2, formdata.Length - 2));
                                    else
                                        formdataBytes = Encoding.UTF8.GetBytes(formdata);
                                    postStream.Write(formdataBytes, 0, formdataBytes.Length);

                                    //写入文件内容
                                    if (item.FileContent != null && item.FileContent.Length > 0)
                                    {
                                        using (var stream = item.FileContent)
                                        {
                                            byte[] buffer = new byte[1024];
                                            int bytesRead = 0;
                                            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                                            {
                                                postStream.Write(buffer, 0, bytesRead);
                                            }
                                        }
                                    }
                                }
                                //结尾
                                var footer = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
                                postStream.Write(footer, 0, footer.Length);
                            }
                            break;
                        }
                }
                if ((bodyByte != null && bodyByte.Length > 0)   )
                {
                    //byte[] buf = ContentEncoding.GetBytes(Body);
                    myRequest.ContentLength = bodyByte.Length;


                    //发送请求，获得请求流
                    reqStream = myRequest.GetRequestStream();
                    reqStream.Flush();
                    reqStream.Write(bodyByte, 0, bodyByte.Length);
                    reqStream.Flush();
                    reqStream.Close();
                }

                if  (postStream != null && postStream.Length > 0)
                {
                    myRequest.ContentLength = postStream.Length;
                    myRequest.Accept = "*/*";
                    myRequest.KeepAlive = true;
                    //myRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.1650.5Safari/537.36";
                    //写入二进制流
                    postStream.Position = 0;
                    //直接写入流
                    Stream requestStream = myRequest.GetRequestStream();

                    byte[] buffer = new byte[1024];
                    int bytesRead = 0;
                    while ((bytesRead = postStream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        requestStream.Write(buffer, 0, bytesRead);
                    }
                    postStream.Close();//关闭文件访问
                }

                // 获得接口返回值
                myResponse = (HttpWebResponse)myRequest.GetResponse();
                reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
                rt = reader.ReadToEnd();
                reader.Close();
                myResponse.Close();
                myRequest.Abort();
            }
            catch (Exception ex)
            {
                rt = ex.Message;
            }
            finally
            {
                if (reqStream != null) reqStream.Close();
                if (reader != null) reader.Close();
                if (myResponse != null) myResponse.Close();
                if (myRequest != null) myRequest.Abort();
            }
            return rt;
        }

        public static string ParseQueryString(Dictionary<string, string> querys)
        {
            if (querys.Count == 0)
                return "";
            return querys
                .Select(pair => pair.Key + "=" + pair.Value)
                .Aggregate((a, b) => a + "&" + b);
        }

    }
}
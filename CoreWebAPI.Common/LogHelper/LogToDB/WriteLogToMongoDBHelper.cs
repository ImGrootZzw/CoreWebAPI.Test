using CoreWebAPI.Common.Helper;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System;
using CoreWebAPI.Common.DB.MongoDB;

namespace CoreWebAPI.Common.LogHelper
{
    /// <summary>
    /// 向MongoDB中插入log数据的帮助类
    /// </summary>
    public class WriteLogToMongoDBHelper
    {
        /// <summary>
        /// 写入操作日志到数据库
        /// </summary>
        /// <param name="solutionName"></param>
        /// <param name="domain"></param>
        /// <param name="timestamp"></param>
        /// <param name="user"></param>
        /// <param name="session"></param>
        /// <param name="ip"></param>
        /// <param name="menu"></param>
        /// <param name="apiName"></param>
        /// <param name="callStartTime"></param>
        /// <param name="apiTime"></param>
        /// <param name="apiTotaltime"></param>
        /// <param name="uiOuttime"></param>
        /// <param name="logsMsg"></param>
        /// <param name="outMsgSize"></param>
        /// <param name="inMsgSize"></param>
        public void WriteLogToMongoDB(string solutionName, string domain, string timestamp ,string user, string session, string ip, string menu,
            string apiName, DateTime? callStartTime, DateTime? apiTime, string apiTotaltime, DateTime? uiOuttime, string logsMsg, 
              string outMsgSize, string inMsgSize)
        {
            try
            {
                string conn = Appsettings.App("Serilog", "WriteToMongoDB", "Connection").GetCString();
                string dbName = Appsettings.App("Serilog", "WriteToMongoDB", "DataBase").GetCString();
                string collectionName = Appsettings.App("Serilog", "WriteToMongoDB", "CollectionName").GetCString();
                MongoDBHelper dbHelper = new MongoDBHelper(conn, dbName, collectionName);
                try
                {

                    dbHelper.Add(new
                    {
                        Id = Guid.NewGuid().ToString(),
                        SolutionName = solutionName,
                        Timestamp = timestamp,
                        User = user,
                        Session = session,
                        Ip = ip,
                        Menu = menu,
                        ApiName = apiName,
                        CallStarttime = callStartTime,
                        CallApitime = apiTime,
                        ApiTotaltime = apiTotaltime,
                        UiOuttime = uiOuttime,
                        MsgOutSize = outMsgSize,
                        MsgInSize = inMsgSize,
                        Msg = logsMsg,
                        crtDatetime = DateTime.Now,
                        CrtProg = apiName,
                        CrtUser = user,
                        Domain = domain,
                    });
                }
                catch (Exception ex)
                {
                    SerilogHelper.WriteErrorLog("MongoDBLogHelper", "WriteLogToMongoDB", ex);
                }
                finally
                {
                    dbHelper.Dispose();
                }
            }
            catch (Exception ex)
            {
                SerilogHelper.WriteErrorLog("MongoDBLogHelper", "WriteLogToMongoDB", ex);
            }
        }


        /// <summary>
        /// 写入Debug日志到数据库
        /// </summary>
        /// <param name="solutionName"></param>
        /// <param name="domain"></param>
        /// <param name="timestamp"></param>
        /// <param name="user"></param>
        /// <param name="session"></param>
        /// <param name="ip"></param>
        /// <param name="menu"></param>
        /// <param name="apiName"></param>
        /// <param name="callStartTime"></param>
        /// <param name="apiTime"></param>
        /// <param name="apiTotaltime"></param>
        /// <param name="logsMsg"></param>
        public void WriteDebugLogToMongoDB(string solutionName, string domain, string timestamp, string user, string session, string ip,
             string menu, string apiName, DateTime? callStartTime, DateTime? apiTime, string apiTotaltime, string logsMsg)
        {
            try
            {
                string conn = Appsettings.App("Serilog", "WriteToMongoDB", "Connection").GetCString();
                string dbName = Appsettings.App("Serilog", "WriteToMongoDB", "DataBase").GetCString();
                string collectionName = Appsettings.App("Serilog", "WriteToMongoDB", "CollectionName").GetCString();
                MongoDBHelper dbHelper = new MongoDBHelper(conn, dbName, collectionName);

                try
                {
                    dbHelper.Add(new
                    {
                        Id = Guid.NewGuid().ToString(),
                        SolutionName = solutionName,
                        Domain = domain,
                        Timestamp = timestamp.GetIsEmptyOrNull() ? new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString() : timestamp,
                        User = user,
                        Session = session,
                        Ip = ip,
                        Menu = menu,
                        ApiName = apiName,
                        CallStarttime = callStartTime,
                        CallApitime = apiTime,
                        ApiTotaltime = apiTotaltime,
                        Msg = logsMsg,
                        crtDatetime = DateTime.Now,
                        CrtProg = apiName,
                        CrtUser = user
                    });
                }
                catch (Exception ex)
                {
                    SerilogHelper.WriteErrorLog("MongoDBLogHelper", "WriteDebugLogToMongoDB", ex);
                }
                finally
                {
                    dbHelper.Dispose();
                }
                
            }
            catch (Exception ex)
            {
                SerilogHelper.WriteErrorLog("MongoDBLogHelper", "WriteDebugLogToMongoDB", ex);
            }
        }
    }
}

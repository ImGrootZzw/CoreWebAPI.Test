using CoreWebAPI.Common.DB;
using CoreWebAPI.Common.Helper;
using CoreWebAPI.Common.HttpContext;
using CoreWebAPI.Common.HttpContextUser;
using CoreWebAPI.Common.LogHelper;
using CoreWebAPI.Common.Redis;
using Google.Protobuf.WellKnownTypes;
using Nacos.V2.Naming.Dtos;
using NPOI.OpenXml4Net.OPC.Internal;
using NPOI.SS.Formula.Functions;
using SqlSugar;
using SugarRedis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CoreWebAPI.Common.LogHelper
{
    /// <summary>
    /// PGSQL日志帮助类
    /// </summary>
    public static class WriteLogToPgsqlHelper
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
        public static void WriteLogToPgsql(string solutionName, string domain, string timestamp, string user, string session, string ip, string menu,
            string apiName, DateTime? callStartTime, DateTime? apiTime, string apiTotaltime, DateTime? uiOuttime, string logsMsg,
              string outMsgSize, string inMsgSize)
        {
            try
            {
                var db = new SqlSugarScope(new List<ConnectionConfig>()
                        {
                            new ConnectionConfig(){
                                ConfigId = "WriteLogToPgsql",
                                DbType = DbType.PostgreSQL,
                                ConnectionString = Appsettings.App("Serilog","WriteToPgsql","Connection").GetCString(),
                                IsAutoCloseConnection = true,
                            }
                        });
                try
                {
                    var bl = new BlockLogsMstr();
                    bl.Id = Guid.NewGuid().ToString();
                    bl.BlockLogsSollutionName = solutionName;
                    bl.BlockLogsTimeStamp = timestamp;
                    bl.BlockLogsUser = user;
                    bl.BlockLogsSession = session;
                    bl.BlockLogsIp = ip;//请求ip;
                    bl.BlockLogsMenu = menu;
                    bl.BlockLogsApiName = apiName;
                    bl.BlockLogsCallStartTime = callStartTime;
                    bl.BlockLogsCallApiTime = apiTime;
                    bl.BlockLogsApiTotalTime = apiTotaltime;
                    bl.BlockLogsUiOutTime = uiOuttime;
                    bl.BlockLogsMsgInsize = inMsgSize;
                    bl.BlockLogsMsgOutSize = outMsgSize;
                    bl.BlockLogsMsg = logsMsg;
                    bl.CrtUser = user;
                    bl.CrtDatetime = DateTime.Now;
                    bl.CrtProg = apiName;
                    bl.Domain = domain;
                    var res = db.Insertable(bl).ExecuteCommandAsync().Result;
                }
                catch (Exception ex)
                {
                    SerilogHelper.WriteLog("LogRecHelper", "WriteLogToPgDB", "err :" + ex.Message);
                }
                finally
                {
                    db.Close();
                }
            }
            catch (Exception ex)
            {
                SerilogHelper.WriteErrorLog("LogRecHelper", "WriteDebugLogToPgDB", ex);
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
        public static void WriteDebugLogToPgsql(string solutionName, string domain, string timestamp, string user, string session, string ip,
             string menu, string apiName, DateTime? callStartTime, DateTime? apiTime, string apiTotaltime, string logsMsg)
        {
            try
            {
                var db = new SqlSugarScope(new List<ConnectionConfig>()
                        {
                            new ConnectionConfig(){
                                ConfigId = "WriteDebugLogToPgsql",
                                DbType = DbType.PostgreSQL,
                                ConnectionString = Appsettings.App("Serilog","WriteToPgsql","Connection").GetCString(),
                                IsAutoCloseConnection = true,
                            }
                        });
                try
                {
                    var dl = new DebugLogsMstr();
                    dl.Id = Guid.NewGuid().ToString();
                    dl.DebugLogsSollutionName = solutionName;
                    dl.Domain = domain;
                    dl.DebugLogsTimeStamp = timestamp;
                    dl.DebugLogsUser = user;
                    dl.DebugLogsSession = session;
                    dl.DebugLogsIp = ip;//请求ip;
                    dl.DebugLogsMenu = menu;
                    dl.DebugLogsApiName = apiName;
                    dl.DebugLogsCallStartTime = callStartTime;
                    dl.DebugLogsCallApiTime = apiTime;
                    dl.DebugLogsApiTotalTime = apiTotaltime;
                    dl.DebugLogsMsg = logsMsg;
                    dl.CrtUser = user;
                    dl.CrtDatetime = DateTime.Now;
                    dl.CrtProg = apiName;

                    var res = db.Insertable(dl).ExecuteCommandAsync().Result;
                }
                catch (Exception ex)
                {
                    SerilogHelper.WriteErrorLog("LogRecHelper", "WriteDebugLogToPgDB", ex);
                }
                finally 
                {
                    db.Close(); 
                }
            }
            catch (Exception ex)
            {
                SerilogHelper.WriteErrorLog("LogRecHelper", "WriteDebugLogToPgDB", ex);
            }
        }

        #region Model
        ///<summary>
        ///操作记录日志 
        ///</summary>
        [SugarTable("blocklogs_mstr", tableDescription: "dam")]
        private class BlockLogsMstr
        {
            /// <summary>
            /// 描述 : ID
            /// 允许空值 : False
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "id", IsNullable = false, IsPrimaryKey = true)]
            public string Id { get; set; }

            /// <summary>
            /// 描述 : 项目
            /// 允许空值 : True
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "blocklogs_solutionname")]
            public string BlockLogsSollutionName { get; set; }

            /// <summary>
            /// 描述 : 时间戳
            /// 允许空值 : False
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "blocklogs_timestamp", IsNullable = false, IsPrimaryKey = true)]
            public string BlockLogsTimeStamp { get; set; }

            /// <summary>
            /// 描述 : 用户
            /// 允许空值 : True
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "blocklogs_user", IndexGroupNameList = new string[] { "uk_blocklogs_mstr" })]
            public string BlockLogsUser { get; set; }

            /// <summary>
            /// 描述 : session
            /// 允许空值 : True
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "blocklogs_session", IndexGroupNameList = new string[] { "uk_blocklogs_mstr" })]
            public string BlockLogsSession { get; set; }

            /// <summary>
            /// 描述 : ip
            /// 允许空值 : True
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "blocklogs_ip", IndexGroupNameList = new string[] { "uk_blocklogs_mstr" })]
            public string BlockLogsIp { get; set; }

            /// <summary>
            /// 描述 : 菜单功能
            /// 允许空值 : True
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "blocklogs_menu", IndexGroupNameList = new string[] { "uk_blocklogs_mstr" })]
            public string BlockLogsMenu { get; set; }

            /// <summary>
            /// 描述 : 接口名
            /// 允许空值 : True
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "blocklogs_api_name", IndexGroupNameList = new string[] { "uk_blocklogs_mstr" })]
            public string BlockLogsApiName { get; set; }

            /// <summary>
            /// 描述 : ID
            /// 允许空值 : 调用开始时间
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "blocklogs_call_starttime", IndexGroupNameList = new string[] { "uk_blocklogs_mstr" })]
            public DateTime? BlockLogsCallStartTime { get; set; }

            /// <summary>
            /// 描述 : 调到接口时间
            /// 允许空值 : True
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "blocklogs_call_apitime")]
            public DateTime? BlockLogsCallApiTime { get; set; }

            /// <summary>
            /// 描述 : 接口耗时时间
            /// 允许空值 : True
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "blocklogs_api_totaltime")]
            public string BlockLogsApiTotalTime { get; set; }

            /// <summary>
            /// 描述 : 界面输出时间
            /// 允许空值 : True
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "blocklogs_ui_outtime")]
            public DateTime? BlockLogsUiOutTime { get; set; }

            /// <summary>
            /// 描述 : 报文输出大小
            /// 允许空值 : True
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "blocklogs_msg_outsize")]
            public string BlockLogsMsgOutSize { get; set; }

            /// <summary>
            /// 描述 : 报文输入大小
            /// 允许空值 : True
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "blocklogs_msg_insize")]
            public string BlockLogsMsgInsize { get; set; }

            /// <summary>
            /// 描述 : ID
            /// 允许空值 : 日志信息
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "blocklogs_msg")]
            public string BlockLogsMsg { get; set; }

            /// <summary>
            /// 描述 : 创建时间
            /// 允许空值 : True
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "blocklogs_crt_datetime")]
            public DateTime? CrtDatetime { get; set; }

            /// <summary>
            /// 描述 : 程序名
            /// 允许空值 : True
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "blocklogs_crt_prog")]
            public string CrtProg { get; set; }

            /// <summary>
            /// 描述 : 创建用户
            /// 允许空值 : True
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "blocklogs_crt_user")]
            public string CrtUser { get; set; }

            /// <summary>
            /// 描述 : 修改日期
            /// 允许空值 : True
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "blocklogs_mod_datetime")]
            public DateTime? ModDatetime { get; set; }

            /// <summary>
            /// 描述 : 修改程序名
            /// 允许空值 : True
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "blocklogs_mod_prog")]
            public string ModProg { get; set; }


            /// <summary>
            /// 描述 : 修改用户
            /// 允许空值 : True
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "blocklogs_mod_user")]
            public string ModUser { get; set; }

            /// <summary>
            /// 描述 : 域
            /// 允许空值 : True
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "blocklogs_domain", IndexGroupNameList = new string[] { "uk_blocklogs_mstr" })]
            public string Domain { get; set; }


        }

        ///<summary>
        ///Debug记录日志 
        ///</summary>
        [SugarTable("debuglogs_mstr", tableDescription: "dam")]
        private class DebugLogsMstr
        {
            /// <summary>
            /// 描述 : ID
            /// 允许空值 : False
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "id", IsNullable = false, IsPrimaryKey = true)]
            public string Id { get; set; }

            /// <summary>
            /// 描述 : 项目
            /// 允许空值 : True
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "debuglogs_solutionname")]
            public string DebugLogsSollutionName { get; set; }

            /// <summary>
            /// 描述 : 时间戳
            /// 允许空值 : False
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "debuglogs_timestamp", IsNullable = false, IsPrimaryKey = true)]
            public string DebugLogsTimeStamp { get; set; }

            /// <summary>
            /// 描述 : 用户
            /// 允许空值 : True
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "debuglogs_user", IndexGroupNameList = new string[] { "uk_debuglogs_mstr" })]
            public string DebugLogsUser { get; set; }

            /// <summary>
            /// 描述 : session
            /// 允许空值 : True
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "debuglogs_session", IndexGroupNameList = new string[] { "uk_debuglogs_mstr" })]
            public string DebugLogsSession { get; set; }

            /// <summary>
            /// 描述 : ip
            /// 允许空值 : True
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "debuglogs_ip", IndexGroupNameList = new string[] { "uk_debuglogs_mstr" })]
            public string DebugLogsIp { get; set; }

            /// <summary>
            /// 描述 : 菜单功能
            /// 允许空值 : True
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "debuglogs_menu", IndexGroupNameList = new string[] { "uk_debuglogs_mstr" })]
            public string DebugLogsMenu { get; set; }

            /// <summary>
            /// 描述 : 接口名
            /// 允许空值 : True
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "debuglogs_api_name", IndexGroupNameList = new string[] { "uk_debuglogs_mstr" })]
            public string DebugLogsApiName { get; set; }

            /// <summary>
            /// 描述 : ID
            /// 允许空值 : 调用开始时间
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "debuglogs_call_starttime", IndexGroupNameList = new string[] { "uk_debuglogs_mstr" })]
            public DateTime? DebugLogsCallStartTime { get; set; }

            /// <summary>
            /// 描述 : 调到接口时间
            /// 允许空值 : True
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "debuglogs_call_apitime")]
            public DateTime? DebugLogsCallApiTime { get; set; }

            /// <summary>
            /// 描述 : 接口耗时时间
            /// 允许空值 : True
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "debuglogs_api_totaltime")]
            public string DebugLogsApiTotalTime { get; set; }


            /// <summary>
            /// 描述 : ID
            /// 允许空值 : 日志信息
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "debuglogs_msg")]
            public string DebugLogsMsg { get; set; }

            /// <summary>
            /// 描述 : 创建时间
            /// 允许空值 : True
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "debuglogs_crt_datetime")]
            public DateTime? CrtDatetime { get; set; }

            /// <summary>
            /// 描述 : 程序名
            /// 允许空值 : True
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "debuglogs_crt_prog")]
            public string CrtProg { get; set; }

            /// <summary>
            /// 描述 : 创建用户
            /// 允许空值 : True
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "debuglogs_crt_user")]
            public string CrtUser { get; set; }

            /// <summary>
            /// 描述 : 修改日期
            /// 允许空值 : True
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "debuglogs_mod_datetime")]
            public DateTime? ModDatetime { get; set; }

            /// <summary>
            /// 描述 : 修改程序名
            /// 允许空值 : True
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "debuglogs_mod_prog")]
            public string ModProg { get; set; }


            /// <summary>
            /// 描述 : 修改用户
            /// 允许空值 : True
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "debuglogs_mod_user")]
            public string ModUser { get; set; }

            /// <summary>
            /// 描述 : 域
            /// 允许空值 : True
            /// 默认值 : 
            /// </summary>        
            [SugarColumn(ColumnName = "debuglogs_domain", IndexGroupNameList = new string[] { "uk_debuglogs_mstr" })]
            public string Domain { get; set; }

        }



        #endregion

    }
}

using CoreWebAPI.Common.Helper;
using CoreWebAPI.Common.DB;
//using CoreWebAPI.Common.LogHelper;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using StackExchange.Profiling;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreWebAPI.Common.LogHelper;
using CoreWebAPI.Common.Redis;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Newtonsoft.Json.Linq;
using CoreWebAPI.Common.HttpContext;

namespace CoreWebAPI.Extensions
{
    /// <summary>
    /// SqlSugar 启动服务
    /// </summary>
    public static class SqlsugarSetup
    {
        static RabbitHelper rabbitHelper;
        public static void AddSqlsugarSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            ICacheService myCache = new SqlSugarRedisCache();
            if (Appsettings.App("AppSettings", "DBAOP", "Enabled").GetCBool())
            {

                var hostNames = Appsettings.App<string>("AppSettings", "DBAOP", "RabbitMQ", "HostNames");
                var userName = Appsettings.App("AppSettings", "DBAOP", "RabbitMQ", "UserName");
                var password = Appsettings.App("AppSettings", "DBAOP", "RabbitMQ", "Password");
                var port = Appsettings.App("AppSettings", "DBAOP", "RabbitMQ", "Port").GetCInt();
                var virtualHost = Appsettings.App("AppSettings", "DBAOP", "RabbitMQ", "VirtualHost");
                var exchangeName = Appsettings.App("AppSettings", "DBAOP", "RabbitMQ", "ExchangeName");
                var queueName = Appsettings.App("AppSettings", "DBAOP", "RabbitMQ", "QueueName");
                var routeKey = Appsettings.App("AppSettings", "DBAOP", "RabbitMQ", "RouteKey");
                rabbitHelper = new RabbitHelper(hostNames, userName, password, port, virtualHost, exchangeName, queueName, routeKey);
            }

            // 把多个连接对象注入服务，这里必须采用Scope，因为有事务操作
            services.AddScoped<ISqlSugarClient>(o =>
            {
                // 默认添加主数据库连接
                MainDb.CurrentDbConnId = Appsettings.App(new string[] { "MainDB" });

                // 连接字符串
                var listConfig = new List<ConnectionConfig>();

                BaseDBConfig.MutiConnectionString.allDbs.ForEach(m =>
                {
                    // 从库
                    var listConfig_Slave = new List<SlaveConnectionConfig>();
                    BaseDBConfig.MutiConnectionString.slaveDbs.ForEach(s =>
                    {
                        if (s.MainConnId.GetCString().ToLower() == m.ConnId.GetCString().ToLower())
                            listConfig_Slave.Add(new SlaveConnectionConfig()
                            {
                                HitRate = s.HitRate,
                                ConnectionString = s.Connection
                            });
                    });
                    listConfig.Add(new ConnectionConfig()
                    {
                        ConfigId = m.ConnId.GetCString().ToLower(),
                        ConnectionString = m.Connection,
                        DbType = (DbType)m.DbType,
                        IsAutoCloseConnection = true,
                        //IsShardSameThread = false,
                        AopEvents = new AopEvents(),
                        MoreSettings = new ConnMoreSettings()
                        {
                            //IsWithNoLockQuery = true,
                            IsAutoRemoveDataCache = true,
                            PgSqlIsAutoToLower = true
                        },
                        // 从库
                        SlaveConnectionConfigs = listConfig_Slave,
                        // 自定义特性
                        ConfigureExternalServices = new ConfigureExternalServices()
                        {
                            //DataInfoCacheService = myCache,
                            DataInfoCacheService = Appsettings.App("Startup", "SqlSugar", "RedisEnabled").GetCBool() ? myCache : null,
                            AppendDataReaderTypeMappings = new List<KeyValuePair<string, CSharpDataType>>() {
                                new KeyValuePair<string, CSharpDataType>("varchar",CSharpDataType.@string),
                                new KeyValuePair<string, CSharpDataType>("tid",CSharpDataType.@object),
                                new KeyValuePair<string, CSharpDataType>("public.citext",CSharpDataType.@string)
                             }
                            //EntityService = (property, column) =>
                            //{
                            //    if (column.IsPrimarykey && property.PropertyType == typeof(int))
                            //    {
                            //        column.IsIdentity = true;
                            //    }
                            //}
                        },
                        InitKeyType = InitKeyType.Attribute
                    }
                   );
                });
                SqlSugarClient db = new SqlSugarClient(listConfig);
                //db.Aop.DataExecuting = (oldValue, entityInfo) =>
                //{
                //    if (entityInfo.PropertyName.EndsWith("Timestamp"))
                //    {
                //        var s1 = entityInfo.EntityValue.GetType().GetProperty(entityInfo.PropertyName).GetValue(entityInfo.EntityValue);
                //        var s2 = s1.GetCDate() == oldValue.GetCDate();
                //        var s22 = s1 == oldValue;
                //        entityInfo.SetValue(DateTime.Now);
                //    }

                //    //update生效
                //    if (entityInfo.OperationType == DataFilterType.UpdateByObject && entityInfo.PropertyName.EndsWith("ModDatetime") && entityInfo.EntityValue.GetType().GetProperty(entityInfo.PropertyName).GetValue(entityInfo.EntityValue).GetCDate() != oldValue.GetCDate())
                //    {
                //        entityInfo.SetValue(DateTime.Now);//修改UpdateTime字段
                //    }
                //};
                if (Appsettings.App(new string[] { "AppSettings", "SqlAOP", "Enabled" }).GetCBool())
                {
                    db.Aop.OnLogExecuted = (sql, p) =>
                    {
                        //代码CS文件名
                        var fileName = db.Ado.SqlStackTrace.FirstFileName;
                        //代码行数
                        var fileLine = db.Ado.SqlStackTrace.FirstLine;
                        //方法名
                        var FirstMethodName = db.Ado.SqlStackTrace.FirstMethodName;
                        var tmp = db.Ado.SqlStackTrace.MyStackTraceList;// 获取上层方法的信息

                        Parallel.For(0, 1, e =>
                        {
                            DbLog dbLog = new DbLog
                            {
                                Ip = StaticHttpContext.IP,
                                Api = StaticHttpContext.API,
                                User = StaticHttpContext.User,
                                Class = db.Ado.SqlStackTrace.FirstFileName,
                                Method = db.Ado.SqlStackTrace.FirstMethodName,
                                Line = db.Ado.SqlStackTrace.FirstLine,
                                Sql = sql,
                                Para = JsonHelper.ObjectToJson(p),
                                ExecutionTime = db.Ado.SqlExecutionTime.TotalMilliseconds
                            };
                            SerilogHelper.WriteLog("SqlLog", "ExecutLog", JsonHelper.JsonFormat(JsonHelper.ObjectToJson(dbLog)));
                        });
                    };
                    db.Aop.OnError = (SqlSugarException ex) =>
                    {
                        Parallel.For(0, 1, e =>
                        {
                            DbLog dbLog = new DbLog
                            {
                                Ip = StaticHttpContext.IP,
                                Api = StaticHttpContext.API,
                                User = StaticHttpContext.User,
                                Class = db.Ado.SqlStackTrace.FirstFileName,
                                Method = db.Ado.SqlStackTrace.FirstMethodName,
                                Line = db.Ado.SqlStackTrace.FirstLine,
                                Sql = db.Ado.SqlParameterKeyWord,
                                ExecutionTime = db.Ado.SqlExecutionTime.TotalMilliseconds,
                                Ex = ex
                            };
                            SerilogHelper.WriteLog("SqlLog", "ErrorLog", JsonHelper.JsonFormat(JsonHelper.ObjectToJson(dbLog)));
                        });
                    };
                    db.Aop.OnDiffLogEvent = (DiffLogModel) =>
                    {
                        Parallel.For(0, 1, e =>
                        {
                            DbLog dbLog = new DbLog
                            {
                                Ip = StaticHttpContext.IP,
                                Api = StaticHttpContext.API,
                                User = StaticHttpContext.User,
                                Class = db.Ado.SqlStackTrace.FirstFileName,
                                Method = db.Ado.SqlStackTrace.FirstMethodName,
                                Line = db.Ado.SqlStackTrace.FirstLine,
                                Sql = db.Ado.SqlParameterKeyWord,
                                ExecutionTime = db.Ado.SqlExecutionTime.TotalMilliseconds,
                                BeforeData = DiffLogModel.BeforeData,
                                AfterData = DiffLogModel.AfterData
                            };
                            SendDbLog(DiffLogModel, db.Ado.Connection.ConnectionString, db.Ado.Connection.Database);
                            SerilogHelper.WriteLog("SqlLog", "DiffLog", JsonHelper.JsonFormat(JsonHelper.ObjectToJson(dbLog)));
                        });
                    };
                    db.Aop.OnLogExecuting = (sql, p) =>
                    {
                        Parallel.For(0, p.Length, (i, loopstate) =>
                        {
                            if (p[i].DbType == System.Data.DbType.DateTimeOffset)
                            {
                                p[i].Value = TimeZoneInfo.ConvertTime(p[i].Value.GetCDate(), TimeZoneInfo.Utc);
                            }
                        });
                        SerilogHelper.WriteLog("SqlLog", "ExecutingLog", JsonHelper.ObjectToJson(p));
                    };
                }

                return db;
            });
        }


        private static string GetParas(SugarParameter[] pars)
        {
            string key = "【SQL参数】：";
            foreach (var param in pars)
            {
                key += $"{param.ParameterName}:{param.Value};\n";
            }

            return key;
        }

        private static void SendDbLog(DiffLogModel diffLog, string dbConnection, string dbName)
        {
            // 发送接口日志到消息队列
            Task task3 = Task.Run(() =>
            {
                if (!Appsettings.App("AppSettings", "DBAOP", "Enabled").GetCBool())
                    return;
                try
                {
                    var tableName = string.Empty;
                    var tableDesc = string.Empty;
                    var fieldData = new JArray();
                    var beforeData = new JArray();
                    var afterData = new JArray();
                    if (diffLog.BeforeData is null)
                    {
                        tableName = diffLog.AfterData.FirstOrDefault().TableName;
                        tableDesc = diffLog.AfterData.FirstOrDefault().TableDescription;
                    }
                    else if (diffLog.AfterData is null)
                    {
                        tableName = diffLog.BeforeData.FirstOrDefault().TableName;
                        tableDesc = diffLog.BeforeData.FirstOrDefault().TableDescription;
                    }
                    else
                    {
                        tableName = diffLog.BeforeData.FirstOrDefault().TableName;
                        tableDesc = diffLog.BeforeData.FirstOrDefault().TableDescription;
                    }

                    //获取变更前数据json
                    foreach (var rowData in diffLog.BeforeData.GetNotNull())
                    {
                        var rowJO = new JObject();
                        foreach (var colData in rowData.Columns)
                        {
                            rowJO[colData.ColumnName] = JToken.FromObject(colData.Value);
                        }
                        beforeData.Add(rowJO);

                        //添加字段描述
                        if (fieldData.Count == 0)
                        {
                            var fieldJO = new JObject();
                            foreach (var colData in rowData.Columns)
                            {
                                fieldJO[colData.ColumnName] = JToken.FromObject(colData.ColumnDescription);
                            }
                            fieldData.Add(fieldJO);
                        }
                    }

                    //获取变更后数据json
                    foreach (var rowData in diffLog.AfterData.GetNotNull())
                    {
                        var rowJO = new JObject();
                        foreach (var colData in rowData.Columns)
                        {
                            rowJO[colData.ColumnName] = JToken.FromObject(colData.Value);
                        }
                        afterData.Add(rowJO);

                        //添加字段描述
                        if (fieldData.Count == 0)
                        {
                            var fieldJO = new JObject();
                            foreach (var colData in rowData.Columns)
                            {
                                fieldJO[colData.ColumnName] = JToken.FromObject(colData.ColumnDescription);
                            }
                            fieldData.Add(fieldJO);
                        }
                    }

                    DblogHist dblogHist = new()
                    {
                        DblogSql = diffLog.Sql,
                        DblogPara = JsonHelper.ObjectToJson(diffLog.Parameters),
                        DblogCorpId = "",
                        DblogDomainId = "",
                        DblogDbConnection = dbConnection,
                        DblogDbName = dbName,
                        DblogTableName = tableName,
                        DblogTableDesc = tableDesc,
                        DblogFields = fieldData.ToString(),
                        DblogActionmode = diffLog.DiffType.ToString(),
                        DblogBeforeData = beforeData.ToString(),
                        DblogAfterData = afterData.ToString(),
                        DblogCostTime = diffLog.Time.Value.TotalSeconds,
                        DblogIP = StaticHttpContext.IP,
                        DblogApi = StaticHttpContext.API,
                        DblogUser = StaticHttpContext.User,
                        DblogDatetime = DateTime.Now
                    };
                    SerilogHelper.WriteLog("SqlLog-Hist", "dblogHist", JsonHelper.ObjectToJson(dblogHist));

                    rabbitHelper.SendMsg(dblogHist);
                }
                catch (Exception ex)
                {
                    SerilogHelper.WriteErrorLog("SqlLog-Hist", "发送接口请求日志到消息队列", ex);
                }
            });
        }
    }

    class DbLog
    {
        public string Ip { get; set; }
        public string Api { get; set; }
        public string User { get; set; }
        public string Class { get; set; }
        public string Method { get; set; }
        public int Line { get; set; }
        public double ExecutionTime { get; set; }
        public string Sql { get; set; }
        public string Para { get; set; }
        public List<DiffLogTableInfo> BeforeData { get; set; }
        public List<DiffLogTableInfo> AfterData { get; set; }
        public Exception Ex { get; set; }
    }

    ///<summary>
    ///数据库操作日志
    ///</summary>
    [SugarTable("dblog_hist")]
    public class DblogHist
    {
        /// <summary>
        /// 描述 : 
        /// 允许空值 : False
        /// 默认值 : nextval('dblog_hist_id_seq'::regclass)
        /// </summary>        
        [SugarColumn(ColumnName = "id", IsNullable = false, IsPrimaryKey = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// 描述 : corp
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "dblog_corp_id", IndexGroupNameList = new string[] { "search_index_dblog_hist" })]
        public string DblogCorpId { get; set; }

        /// <summary>
        /// 描述 : domain
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "dblog_domain_id", IndexGroupNameList = new string[] { "search_index_dblog_hist" })]
        public string DblogDomainId { get; set; }

        /// <summary>
        /// 描述 : SQL语句
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "dblog_sql")]
        public string DblogSql { get; set; }

        /// <summary>
        /// 描述 : 查询参数
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "dblog_para")]
        public string DblogPara { get; set; }

        /// <summary>
        /// 描述 : 数据库连接配置
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "dblog_db_connection")]
        public string DblogDbConnection { get; set; }

        /// <summary>
        /// 描述 : 数据库名称
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "dblog_db_name", IndexGroupNameList = new string[] { "search_index_dblog_hist" })]
        public string DblogDbName { get; set; }

        /// <summary>
        /// 描述 : 表名
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "dblog_table_name", IndexGroupNameList = new string[] { "search_index_dblog_hist" })]
        public string DblogTableName { get; set; }

        /// <summary>
        /// 描述 : 表描述
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "dblog_table_desc")]
        public string DblogTableDesc { get; set; }

        /// <summary>
        /// 描述 : 字段
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "dblog_fields")]
        public string DblogFields { get; set; }

        /// <summary>
        /// 描述 : 操作类型（insert/update/delete）
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "dblog_actionmode")]
        public string DblogActionmode { get; set; }

        /// <summary>
        /// 描述 : 变更前数据
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "dblog_before_data")]
        public string DblogBeforeData { get; set; }

        /// <summary>
        /// 描述 : 变更后数据
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "dblog_after_data")]
        public string DblogAfterData { get; set; }

        /// <summary>
        /// 描述 : 花费时间
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "dblog_cost_time")]
        public double DblogCostTime { get; set; }

        /// <summary>
        /// 描述 : 请求端IP
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "dblog_ip")]
        public string DblogIP { get; set; }

        /// <summary>
        /// 描述 : 执行接口
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "dblog_api")]
        public string DblogApi { get; set; }

        /// <summary>
        /// 描述 : 执行用户
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "dblog_user", IndexGroupNameList = new string[] { "search_index_dblog_hist" })]
        public string DblogUser { get; set; }

        /// <summary>
        /// 描述 : 执行时间
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "dblog_datetime", IndexGroupNameList = new string[] { "search_index_dblog_hist" })]
        public DateTime? DblogDatetime { get; set; }

        /// <summary>
        /// 描述 : 创建日期
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "dblog_crt_datetime", IsOnlyIgnoreUpdate = true)]
        public DateTime? DblogCrtDatetime { get; set; }

        /// <summary>
        /// 描述 : 程序名
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "dblog_crt_prog", IsOnlyIgnoreUpdate = true)]
        public string DblogCrtProg { get; set; }

        /// <summary>
        /// 描述 : 创建用户
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "dblog_crt_user", IsOnlyIgnoreUpdate = true)]
        public string DblogCrtUser { get; set; }

        /// <summary>
        /// 描述 : 修改日期
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "dblog_mod_datetime", IsOnlyIgnoreInsert = true)]
        public DateTime? DblogModDatetime { get; set; }

        /// <summary>
        /// 描述 : 修改程序名
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "dblog_mod_prog", IsOnlyIgnoreInsert = true)]
        public string DblogModProg { get; set; }

        /// <summary>
        /// 描述 : 修改用户
        /// 允许空值 : True
        /// 默认值 : 
        /// </summary>        
        [SugarColumn(ColumnName = "dblog_mod_user", IsOnlyIgnoreInsert = true)]
        public string DblogModUser { get; set; }

    }

}
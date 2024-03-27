using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CoreWebAPI.Common.Helper;

namespace CoreWebAPI.Common.DB
{
    public class BaseDBConfig
    {
        /* 之前的单库操作已经删除，如果想要之前的代码，可以查看我的GitHub的历史记录
         * 目前是多库操作，默认加载的是appsettings.json设置为true的第一个db连接。
         */
        public static (List<MutiDBOperate> allDbs, List<MutiDBOperate> slaveDbs) MutiConnectionString => MutiInitConn();
        private static string DifDBConnOfSecurity(string connFile, string conn)
        {
            if (File.Exists(connFile))
            {
                return File.ReadAllText(connFile).Trim();
            }

            return conn;
        }


        public static (List<MutiDBOperate>, List<MutiDBOperate>) MutiInitConn()
        {
            //主库
            List<MutiDBOperate> listDatabase = Appsettings.App<MutiDBOperate>("DBS").Where(db => db.Enabled && db.MainConnId.GetIsEmptyOrNull()).ToList();
            //所有主库的从库
            List<MutiDBOperate> listDatabaseSlave = Appsettings.App<MutiDBOperate>("DBS").Where(db => db.Enabled && db.MainConnId.GetIsNotEmptyOrNull()).ToList();

            return (listDatabase, listDatabaseSlave);
        }

        /// <summary>
        /// 定制Db字符串
        /// 目的是保证安全：优先从本地txt文件获取，若没有文件则从appsettings.json中获取
        /// </summary>
        /// <param name="mutiDBOperate"></param>
        /// <returns></returns>
        private static MutiDBOperate SpecialDbString(MutiDBOperate mutiDBOperate)
        {
            if (mutiDBOperate.DbType == DataBaseType.Sqlite)
            {
                mutiDBOperate.Connection = $"DataSource=" + Path.Combine(Environment.CurrentDirectory, mutiDBOperate.Connection);
            }
            else if (mutiDBOperate.DbType == DataBaseType.SqlServer)
            {
                mutiDBOperate.Connection = DifDBConnOfSecurity(@"D:\my-file\dbCountPsw1_SqlserverConn.txt", mutiDBOperate.Connection);
            }
            else if (mutiDBOperate.DbType == DataBaseType.MySql)
            {
                mutiDBOperate.Connection = DifDBConnOfSecurity(@"D:\my-file\dbCountPsw1_MySqlConn.txt", mutiDBOperate.Connection);
            }
            else if (mutiDBOperate.DbType == DataBaseType.Oracle)
            {
                mutiDBOperate.Connection = DifDBConnOfSecurity(@"D:\my-file\dbCountPsw1_OracleConn.txt", mutiDBOperate.Connection);
            }

            return mutiDBOperate;
        }
    }


    public enum DataBaseType
    {
        MySql = 0,
        SqlServer = 1,
        Sqlite = 2,
        Oracle = 3,
        PostgreSQL = 4
    }
    public class MutiDBOperate
    {
        /// <summary>
        /// 连接ID
        /// </summary>
        public string ConnId { get; set; }
        /// <summary>
        /// 作为从库时，主库连接ID
        /// </summary>
        public string MainConnId { get; set; }
        /// <summary>
        /// 连接启用开关
        /// </summary>
        public bool Enabled { get; set; }
        /// <summary>
        /// 从库执行级别，越大越先执行
        /// </summary>
        public int HitRate { get; set; }
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string Connection { get; set; }
        /// <summary>
        /// 数据库类型
        /// </summary>
        public DataBaseType DbType { get; set; }
    }
}
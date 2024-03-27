using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Grpc.Net.Client;
using MessageGrpc;
using CoreWebAPI.Common.Redis;

namespace CoreWebAPI.Common.Helper
{
    public static class MultiLangHelper
    {
        /// <summary>
        /// 获取错误多语言信息
        /// </summary>
        /// <param name="thisValue">默认信息</param>
        /// <param name="code">错误代码</param>
        /// <param name="lang">语言</param>
        /// <returns></returns>
        public static string GetMessage(this string thisValue, int code, string lang)
        {
            try
            {
                RedisCacheManager redisCache = new RedisCacheManager();
                if (redisCache.Exist("messageJson"))
                {
                    string messageJson = redisCache.Get("messageJson");
                    DataTable dt = JsonHelper.NewJsonToDataTable(messageJson);
                    DataRow[] rows = dt.Select($"mesg_nbr = '{ code }' and mesg_lang = '{ lang }'");
                    if (rows.Length > 0)
                    {
                        return rows[0]["mesg_desc"].GetCString();
                    }
                    //List<MessageModel> listMessage = (List<MessageModel>)JsonHelper.JsonToObject(messageJson, typeof(List<MessageModel>));
                    //MessageModel message = listMessage.Find(m => m.messageId == code.GetCInt() && m.languageId == lang && m.active == true);
                    //if (message != null)
                    //{
                    //    return message.info;
                    //}
                }

                var messageGrpcServiceUrl = Appsettings.App("MessageGrpcServiceConn");
                using var channel = GrpcChannel.ForAddress(messageGrpcServiceUrl, new GrpcChannelOptions() { MaxReceiveMessageSize = int.MaxValue,MaxSendMessageSize=int.MaxValue }) ;
                var client = new Message.MessageClient(channel);
                ReplyModel rm = client.GetMessage(new RequestModel { Code = code.ToString(),Lang = lang,DefaultValue=thisValue });
                if(rm.Code == 10000)
                {
                    DataTable dt = (DataTable)JsonHelper.JsonToObject(rm.Data, typeof(DataTable));
                    redisCache.Set("messageJson", dt, 60);
                    DataRow[] rows = dt.Select($"mesg_nbr = '{ code }' and mesg_lang = '{ lang }'");
                    if (rows.Length > 0)
                    {
                        return rows[0]["mesg_desc"].GetCString();
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return thisValue;
        }
    }

    public class MessageModel
    {
        /// <summary>
        /// 描述 : Id
        /// 空值 : False
        /// 默认 : 
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 描述 : DbName
        /// 空值 : False
        /// 默认 : 
        /// </summary>
        public string DbName { get; set; }

        /// <summary>
        /// 描述 : TableName
        /// 空值 : False
        /// 默认 : 
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 描述 : TableIndex
        /// 空值 : False
        /// 默认 : 
        /// </summary>
        public string TableIndex { get; set; }

        /// <summary>
        /// 描述 : TableData
        /// 空值 : False
        /// 默认 : 
        /// </summary>
        public string TableData { get; set; }

        /// <summary>
        /// 描述 : ActionMode
        /// 空值 : False
        /// 默认 : 
        /// </summary>
        public string ActionMode { get; set; }


    }

    public class MessageModelJava
    {
        public int id;
        public string code;
        public int messageId;
        public string languageId;
        public bool active;
        public string info;
        public DateTime createDatetime;
        public string createProgram;
        public string createUserid;
        public DateTime modifyDatetime;
        public string modifyProgram;
        public string modifyUserid;
    }
}

using CoreWebAPI.Common.LogHelper;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web;

namespace CoreWebAPI.Common.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public class RabbitHelper
    {
        private readonly IConnection _connection;
        private readonly IModel channel;
        private readonly string _exchangeName;
        private readonly string _queueName;
        private readonly string _routeKey;

        /// <summary>
        /// 
        /// </summary>
        public RabbitHelper(List<string> hostNames, string userName, string password, int port, string virtualHost, string exchangeName, string queueName, string routeKey)
        {
            //创建连接工厂
            var factory = new ConnectionFactory()
            {
                //HostName = options.Value.RabbitHost,//集群不在此处声明
                UserName = userName,
                Password = password,
                Port = port,
                VirtualHost = virtualHost
            };
            _exchangeName = exchangeName;
            _queueName = queueName;
            _routeKey = routeKey;

            //创建连接
            _connection = factory.CreateConnection(hostNames);
            //创建通道
            channel = _connection.CreateModel();
            //声明交换机
            channel.ExchangeDeclare(_exchangeName, ExchangeType.Topic);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        public void SendMsg(object msg, [CallerMemberName] string memberName = "")
        {
            try
            {
                //声明一个队列
                channel.QueueDeclare(_queueName, true, false, false, null);
                //绑定队列，交换机，路由键
                channel.QueueBind(_queueName, _exchangeName, _routeKey);

                var basicProperties = channel.CreateBasicProperties();
                //1：非持久化 2：可持久化
                basicProperties.DeliveryMode = 2;
                var address = new PublicationAddress(ExchangeType.Direct, _exchangeName, _routeKey);

                string msgJson = JsonConvert.SerializeObject(msg);
                var payload = Encoding.UTF8.GetBytes(msgJson);
                channel.BasicPublish(address, basicProperties, payload);
            }
            catch (Exception ex)
            {
                SerilogHelper.WriteErrorLog(memberName, "SendMsg", ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            _connection.Close();
        }
    }
}
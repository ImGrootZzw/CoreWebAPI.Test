using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using CoreWebAPI.Common.Helper;
using CoreWebAPI.Common.LogHelper;

namespace CoreWebAPI.Extensions.ServiceExtensions
{
    public class RabbitListener : IHostedService
    {

        private readonly IConnection _connection;
        private readonly IModel _channel;
        protected string ExchangeName = "message";
        protected string RouteKey = "test";
        protected string QueueName = "test";

        public RabbitListener()
        {
            try
            {
                //创建连接工厂
                var factory = new ConnectionFactory()
                {
                    //HostName = "10.124.0.107",//集群在创建连接时声明多个host
                    UserName = Appsettings.App("Startup", "RabbitMQ", "UserName"),
                    Password = Appsettings.App("Startup", "RabbitMQ", "Password"),
                    Port = Appsettings.App("Startup", "RabbitMQ", "Port").GetCInt(),
                    VirtualHost = Appsettings.App("Startup", "RabbitMQ", "VirtualHost")
                };
                //创建连接
                _connection = factory.CreateConnection(Appsettings.App<string>("Startup", "RabbitMQ", "HostNames"));
                //创建通道
                _channel = _connection.CreateModel();
            }
            catch (Exception ex)
            {
                SerilogHelper.WriteErrorLog("RabbitListener", "init error", ex);
                Console.WriteLine($"RabbitListener init error,ex:{ex.Message}");
            }
        }
        public RabbitListener(String[] hostName, string virtualHost, string userName, string password, int port)
        {
            try
            {
                //创建连接工厂
                var factory = new ConnectionFactory()
                {
                    //HostName = "10.124.0.107",//集群在创建连接时声明多个host
                    UserName = userName,
                    Password = password,
                    Port = port,
                    VirtualHost = virtualHost
                };
                //创建连接
                _connection = factory.CreateConnection(hostName);
                //创建通道
                _channel = _connection.CreateModel();
            }
            catch (Exception ex)
            {
                SerilogHelper.WriteErrorLog("RabbitListener", "init error", ex);
                Console.WriteLine($"RabbitListener init error,ex:{ex.Message}");
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Register();
            return Task.CompletedTask;
        }

        /// <summary>
        /// 处理消息的方法
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public virtual async Task<bool> Process(string message) => throw new NotImplementedException();

        /// <summary>
        /// 注册消费者监听在这里
        /// </summary>
        public void Register()
        {
            //声明交换机
            _channel.ExchangeDeclare(ExchangeName, ExchangeType.Topic);
            _channel.QueueDeclare(queue: QueueName, durable:true, exclusive: false, autoDelete:false);
            _channel.QueueBind(queue: QueueName, exchange: ExchangeName, routingKey: RouteKey);
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var result =await Process(message);

                if(!Appsettings.App("Startup", "RabbitMQ", "DisableRetry").GetCBool())
                {
                    while (!result)
                    {
                        result = await Process(message);
                    }
                }
                
                if (result)
                {
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                else
                {
                    // 重新排队，减少网络请求，提高性能
                    _channel.BasicNack(ea.DeliveryTag, multiple: true, requeue: true);
                }
            };
            _channel.BasicConsume(queue: QueueName, consumer: consumer);
        }

        public void DeRegister()
        {
            _connection.Close();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _connection.Close();
            return Task.CompletedTask;
        }
    }


}

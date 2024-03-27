using CoreWebAPI.Extensions.ServiceExtensions.Kafka;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoreWebAPI.Extensions.ServiceExtensions.Kafka.Consumer
{
    public class KafkaConsumerBuilder : IKafkaConsumerBuilder
    {
        public KafkaConsumerBuilder(IServiceCollection services, KafkaConsumerOptions kafkaConsumerOptions)
        {
            _services = services;
            _kafkaConsumerOptions = kafkaConsumerOptions;
        }

        public IServiceCollection _services { get; }

        public KafkaConsumerOptions _kafkaConsumerOptions { get; }

        /// <summary>
        /// 添加队列监听
        /// </summary>
        /// <param name="onMessageRecieved"></param>
        /// <returns></returns>
        public IKafkaConsumerBuilder AddListener(Action<IServiceProvider, RecieveResult> onMessageRecieved)
        {
            _services.AddSingleton<IKafkaConsumerProvider>(serviceProvider =>
            {
                return new KafkaConsumerProvider(_kafkaConsumerOptions, result =>
                {
                    using (var scope = serviceProvider.CreateScope())
                    {
                         onMessageRecieved?.Invoke(scope.ServiceProvider, result);
                    }
                });
            });

            return this;
        }
    }
}

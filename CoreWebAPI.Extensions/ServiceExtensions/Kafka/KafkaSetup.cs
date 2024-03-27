using CoreWebAPI.Extensions.ServiceExtensions.Kafka.Consumer;
using CoreWebAPI.Extensions.ServiceExtensions.Kafka.Consumers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreWebAPI.Extensions.ServiceExtensions.Kafka
{
    public static class KafkaSetup
    {
        /// <summary>
        /// 注册消费者服务，获取消费创建对象
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IKafkaConsumerBuilder AddKafkaConsumerSetup(this IServiceCollection services, Action<KafkaConsumerOptions> options)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            KafkaConsumerOptions kafkaConsumerOptions = new KafkaConsumerOptions();
            options?.Invoke(kafkaConsumerOptions);

            // 注册消费者服务
            if (!services.Any(f => f.ImplementationType == typeof(KafkaConsumerHostedService)))
            {
                services.AddSingleton<IHostedService, KafkaConsumerHostedService>();
            }

            return new KafkaConsumerBuilder(services, kafkaConsumerOptions);
        }
        
        /// <summary>
        /// 添加自定义监听
        /// </summary>
        /// <param name="builder"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IKafkaConsumerBuilder AddListener<T>(this IKafkaConsumerBuilder builder) where T : class, IKafkaConsumerListener
        {
            return builder.AddListener(typeof(T));
        }

        /// <summary>
        /// 添加自定义监听
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="listenerType"></param>
        /// <returns></returns>
        public static IKafkaConsumerBuilder AddListener(this IKafkaConsumerBuilder builder, Type listenerType)
        {
            if (!typeof(IKafkaConsumerListener).IsAssignableFrom(listenerType) || !listenerType.IsClass || listenerType.IsAbstract)
            {
                throw new ArgumentException($"the listener type must be implement {nameof(IKafkaConsumerListener)} and none abstract class", nameof(listenerType));
            }

            builder._services.AddTransient(listenerType);
            return builder.AddListener((serviceProvider, result) =>
            {
                var listenner = serviceProvider.GetService(listenerType) as IKafkaConsumerListener;
                while (!listenner.ConsumeAsync(result).Result)
                {
                    if(builder._kafkaConsumerOptions.EnableAutoCommit)
                        break;
                };
                result.Commit();
            });
        }

    }
}

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoreWebAPI.Extensions.ServiceExtensions.Kafka.Consumer
{
    public interface IKafkaConsumerBuilder
    {
        KafkaConsumerOptions _kafkaConsumerOptions { get; }

        IServiceCollection _services { get; }

        IKafkaConsumerBuilder AddListener(Action<IServiceProvider, RecieveResult> onMessageRecieved);
    }
}

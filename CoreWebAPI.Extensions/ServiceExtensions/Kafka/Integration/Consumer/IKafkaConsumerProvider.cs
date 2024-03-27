using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoreWebAPI.Extensions.ServiceExtensions.Kafka.Consumer
{
    public interface IKafkaConsumerProvider : IDisposable
    {
        //KafkaConsumer Consumer { get; }

        Task ListenAsync();
    }
}

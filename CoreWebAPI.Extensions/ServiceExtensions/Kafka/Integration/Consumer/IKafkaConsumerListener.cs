using CoreWebAPI.Extensions.ServiceExtensions.Kafka.Consumer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreWebAPI.Extensions.ServiceExtensions.Kafka.Consumer
{
    public interface IKafkaConsumerListener
    {
        Task<bool> ConsumeAsync(RecieveResult recieveResult);
    }
}

using CoreWebAPI.Extensions.ServiceExtensions.Kafka.Consumer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreWebAPI.Test.WebAPI.Listener
{
    /// <summary>
    /// 
    /// </summary>
    public class KafkaConsumerListener : IKafkaConsumerListener
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="recieveResult"></param>
        /// <returns></returns>
        public async Task<bool> ConsumeAsync(RecieveResult recieveResult)
        {
            Console.WriteLine("KafkaConsumerListener-Offset:" + recieveResult.Offset);
            Console.WriteLine("KafkaConsumerListener-Partition:" + recieveResult.Partition);
            Console.WriteLine("KafkaConsumerListener-Message:" + recieveResult.Message);
            recieveResult.Commit();
            if (recieveResult.Offset > 7)
                return false;

            return true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreWebAPI.Extensions.ServiceExtensions.Kafka.Consumer
{
    public class KafkaConsumerProvider : IKafkaConsumerProvider
    {
        KafkaConsumerOptions kafkaConsumerOptions;
        ListenResult listenResult;
        Action<RecieveResult> onMessageRecieved;
        bool disposed = false;

        public KafkaConsumer Consumer { get; }

        public KafkaConsumerProvider(KafkaConsumerOptions kafkaConsumerOptions, Action<RecieveResult> onMessageRecieved)
        {
            this.kafkaConsumerOptions = kafkaConsumerOptions;
            this.onMessageRecieved = onMessageRecieved;

            Consumer = KafkaConsumer.Create(kafkaConsumerOptions.GroupId, kafkaConsumerOptions);
            Consumer.EnableAutoCommit = kafkaConsumerOptions.EnableAutoCommit;
        }

        public async Task ListenAsync()
        {
            if (disposed) throw new ObjectDisposedException(nameof(KafkaConsumerProvider));

            if (listenResult == null)
            {
                listenResult = await Consumer.ListenAsync(kafkaConsumerOptions.Subscribers, recieveResult =>
                {
                    onMessageRecieved.Invoke(recieveResult);
                });
            }
        }

        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                listenResult?.Stop();
                Consumer?.Dispose();
            }
        }

        public override string ToString()
        {
            return $"Consumer-{Consumer}    Subscribers:{string.Join(",", kafkaConsumerOptions.Subscribers.Select(f => f.Partition == null ? f.Topic : $"{f.Topic}:{f.Partition}"))}";
        }
    }
}

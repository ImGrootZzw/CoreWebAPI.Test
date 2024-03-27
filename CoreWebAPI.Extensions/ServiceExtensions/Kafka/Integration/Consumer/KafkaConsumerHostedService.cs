using CoreWebAPI.Extensions.ServiceExtensions.Kafka.Consumer;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoreWebAPI.Extensions.ServiceExtensions.Kafka.Consumers
{
    public class KafkaConsumerHostedService : IHostedService
    {
        ILoggerFactory _loggerFactory;
        IEnumerable<IKafkaConsumerProvider> _kafkaConsumerProviders;
        public KafkaConsumerHostedService(ILoggerFactory loggerFactory, IEnumerable<IKafkaConsumerProvider> kafkaConsumerProviders)
        {
            this._kafkaConsumerProviders = kafkaConsumerProviders;
            this._loggerFactory = loggerFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var logger = _loggerFactory.CreateLogger<KafkaConsumerHostedService>();

            foreach (var provider in _kafkaConsumerProviders)
            {
                await provider.ListenAsync();

                logger.LogInformation($"Consumer Listen:{provider}");
                Console.WriteLine($"Consumer Listen:{provider}");
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            cancellationToken.Register(() =>
            {
                var logger = _loggerFactory.CreateLogger<KafkaConsumerHostedService>();

                foreach (var provider in _kafkaConsumerProviders)
                {
                    provider.Dispose();

                    logger.LogInformation($"Consumer Stoped:{provider}");
                    Console.WriteLine($"Consumer Stoped:{provider}");
                }
            });
            await Task.CompletedTask;
        }
    }
}

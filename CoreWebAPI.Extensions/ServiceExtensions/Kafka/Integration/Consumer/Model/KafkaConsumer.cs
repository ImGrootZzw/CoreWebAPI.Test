using Confluent.Kafka;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoreWebAPI.Extensions.ServiceExtensions.Kafka.Consumer
{
    public class KafkaConsumer : KafkaBase, IDisposable
    {
        ConsumerBuilder<string, object> builder;

        private readonly List<IConsumer<string, object>> consumers;

        bool disposed = false;

        /// <summary>
        /// 消费群组
        /// </summary>
        public string GroupId { get; private set; }

        /// <summary>
        /// 是否允许自动提交（enable.auto.commit）
        /// </summary>
        public bool EnableAutoCommit { get; set; } = false;

        /// <summary>
        /// 异常事件
        /// </summary>
        public event Action<object, Exception> ErrorHandler;

        /// <summary>
        /// 统计事件
        /// </summary>
        public event Action<object, string> StatisticsHandler;

        /// <summary>
        /// 日志事件
        /// </summary>
        public event Action<object, KafkaLogMessage> LogHandler;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="groupId">消费者组</param>
        /// <param name="bootstrapServers">Bootstrap Servers</param>
        public KafkaConsumer(string groupId, params string[] bootstrapServers)
        {
            if (bootstrapServers == null || bootstrapServers.Length == 0)
            {
                throw new Exception("at least one server must be assigned");
            }

            this.GroupId = groupId;
            this.BootstrapServers = string.Join(",", bootstrapServers);
            this.consumers = new List<IConsumer<string, object>>();
        }

        #region Private
        /// <summary>
        /// 创建消费者生成器
        /// </summary>
        private void CreateConsumerBuilder()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(KafkaConsumer));
            }

            if (builder == null)
            {
                lock (this)
                {
                    if (builder == null)
                    {
                        ConsumerConfig config = new ConsumerConfig();
                        config.BootstrapServers = BootstrapServers;
                        config.GroupId = GroupId;
                        config.AutoOffsetReset = AutoOffsetReset.Earliest;
                        config.EnableAutoCommit = EnableAutoCommit;
                        if (!string.IsNullOrEmpty(SaslUsername))
                        {
                            config.SaslUsername = SaslUsername;
                            config.SaslPassword = SaslPassword;
                            config.SaslMechanism = SaslMechanism.Plain;
                            config.SecurityProtocol = SecurityProtocol.SaslPlaintext;
                        }
                        //config.EnableAutoOffsetStore = true;
                        //config.IsolationLevel = IsolationLevel.ReadCommitted;
                        config.MaxPollIntervalMs = 300000;


                        //List<KeyValuePair<string, string>> config = new List<KeyValuePair<string, string>>();
                        //config.Add(new KeyValuePair<string, string>("bootstrap.servers", BootstrapServers));
                        //config.Add(new KeyValuePair<string, string>("group.id", GroupId));
                        //config.Add(new KeyValuePair<string, string>("auto.offset.reset", "earliest"));
                        //config.Add(new KeyValuePair<string, string>("enable.auto.commit", EnableAutoCommit.ToString().ToLower()));
                        //if (!string.IsNullOrEmpty(SaslUsername))
                        //{
                        //    config.Add(new KeyValuePair<string, string>("security.protocol", "SASL_PLAINTEXT"));
                        //    config.Add(new KeyValuePair<string, string>("sasl.mechanism", "PLAIN"));
                        //    config.Add(new KeyValuePair<string, string>("sasl.username", SaslUsername));
                        //    config.Add(new KeyValuePair<string, string>("sasl.password", SaslPassword));
                        //}
                        //config.Add(new KeyValuePair<string, string>("max.poll.interval.ms", "10000"));
                        //config.Add(new KeyValuePair<string, string>("session.timeout.ms", "10000"));
                        //config.Add(new KeyValuePair<string, string>("isolation.level", "read_uncommitted"));

                        builder = new ConsumerBuilder<string, object>(config);

                        Action<Delegate, object> tryCatchWrap = (@delegate, arg) =>
                        {
                            try
                            {
                                @delegate?.DynamicInvoke(arg);
                            }
                            catch { }
                        };

                        // 待完善--错误处理时间
                        builder.SetErrorHandler((p, e) => tryCatchWrap(ErrorHandler, new Exception(e.Reason)));
                        builder.SetStatisticsHandler((p, e) => tryCatchWrap(StatisticsHandler, e));
                        builder.SetLogHandler((p, e) => tryCatchWrap(LogHandler, new KafkaLogMessage(e)));
                        builder.SetValueDeserializer(new KafkaConverter());
                    }
                }
            }
        }

        /// <summary>
        /// 内部处理消息
        /// </summary>
        /// <param name="consumer"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="action"></param>
        private void InternalListen(IConsumer<string, object> consumer, CancellationToken cancellationToken, Action<RecieveResult> action)
        {
            try
            {
                var result = consumer.Consume(cancellationToken);
                if (!cancellationToken.IsCancellationRequested && result != null)
                {
                    CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                    if (!EnableAutoCommit)
                    {
                        cancellationTokenSource.Token.Register(() =>
                        {
                            consumer.Commit(result);
                        });
                    }
                     action?.Invoke(new RecieveResult(result, cancellationTokenSource));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 验证消费主题和分区
        /// </summary>
        /// <param name="subscribers"></param>
        private void CheckSubscribers(params KafkaSubscriber[] subscribers)
        {
            if (subscribers == null || subscribers.Length == 0)
            {
                throw new InvalidOperationException("subscriber cann't be empty");
            }

            if (subscribers.Any(f => string.IsNullOrEmpty(f.Topic)))
            {
                throw new InvalidOperationException("topic cann't be empty");
            }
        }

        /// <summary>
        /// 设置消费主题，偏移量
        /// </summary>
        /// <param name="consumer"></param>
        private void SetSubscribers(IConsumer<string, object> consumer, params KafkaSubscriber[] subscribers)
        {
            var topics = subscribers.Where(f => f.Topic != null).Select(f => f.Topic).ToArray();
            var topicPartitions = subscribers.Where(f => f.Partition != null).Select(f => new TopicPartition(f.Topic, new Partition(f.Partition.Value))).ToArray();

            if (topics.Length > 0)
            {
                consumer.Subscribe(topics);
            }

            if (topicPartitions.Length > 0)
            {
                consumer.Assign(topicPartitions);
            }
        }

        #region Listen        
        /// <summary>
        /// 监听
        /// </summary>
        /// <param name="topics"></param>
        /// <param name="action"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public void Listen(string[] topics, Action<RecieveResult> action = null, CancellationToken cancellationToken = default)
        {
            Listen(topics.Select(f => new KafkaSubscriber() { Partition = null, Topic = f }).ToArray(), action, cancellationToken);
        }

        /// <summary>
        /// 监听
        /// </summary>
        /// <param name="subscribers"></param>
        /// <param name="action"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public void Listen(KafkaSubscriber[] subscribers, Action<RecieveResult> action = null, CancellationToken cancellationToken = default)
        {
            ListenResult result = new ListenResult();
            cancellationToken.Register(() =>
            {
                result.Stop();
            });
            using (var consumer = CreateConsumer(result, subscribers))
            {
                while (!result.Stoped)
                {
                    InternalListen(consumer, result.Token, action);
                }
            }
        }

        /// <summary>
        /// 异步监听
        /// </summary>
        /// <param name="topics"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public async Task<ListenResult> ListenAsync(string[] topics, Action<RecieveResult> action = null)
        {
            return await ListenAsync(topics.Select(f => new KafkaSubscriber() { Partition = null, Topic = f }).ToArray(), action);
        }

        /// <summary>
        /// 异步监听
        /// </summary>
        /// <param name="subscribers"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public async Task<ListenResult> ListenAsync(KafkaSubscriber[] subscribers, Action<RecieveResult> action = null)
        {
            ListenResult result = new ListenResult();
            new Task(() =>
            {
                using (var consumer = CreateConsumer(result, subscribers))
                {
                    while (!result.Stoped)
                    {
                        InternalListen(consumer, result.Token, action);
                    }
                }
            }).Start();
            return await Task.FromResult(result);
        }
        #endregion

        /// <summary>
        /// 创建一个消费者
        /// </summary>
        /// <param name="listenResult"></param>
        /// <param name="subscribers"></param>
        /// <returns></returns>
        private IConsumer<string, object> CreateConsumer(ListenResult listenResult, params KafkaSubscriber[] subscribers)
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(KafkaConsumer));
            }

            CheckSubscribers(subscribers);

            CreateConsumerBuilder();

            var consumer = builder.Build();
            listenResult.Token.Register(() =>
            {
                // fixed issue: https://gitee.com/shanfeng1000/dotnetcore-demo/issues/I5BSCJ
                try
                {
                    consumers.Remove(consumer);
                    consumer.Close();
                    consumer.Dispose();
                }
                catch { }
            });

            SetSubscribers(consumer, subscribers);

            consumers.Add(consumer);

            return consumer;
        }

        #endregion

        

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            disposed = true;
            builder = null;

            // fixed issue: https://gitee.com/shanfeng1000/dotnetcore-demo/issues/I5BSCJ
            foreach (var consumer in consumers)
            {
                try
                {
                    consumer?.Close();
                    consumer?.Dispose();
                }
                catch { }
            }
            consumers.Clear();

            GC.Collect();
        }

        public static KafkaConsumer Create(string groupId, KafkaBaseOptions kafkaBaseOptions)
        {
            return new KafkaConsumer(groupId, kafkaBaseOptions.BootstrapServers)
            {
                SaslUsername = kafkaBaseOptions.SaslUsername,
                SaslPassword = kafkaBaseOptions.SaslPassword
            };
        }
        public override string ToString()
        {
            return BootstrapServers;
        }
    }

}

using System;
using System.Collections.Generic;
using System.Text;

namespace CoreWebAPI.Extensions.ServiceExtensions.Kafka
{
    public abstract class KafkaBase
    {
        /// <summary>
        /// kafka服务节点
        /// </summary>
        public string BootstrapServers { get; protected set; }

        /// <summary>
        /// SASL PLAINTEXT认证用户名
        /// </summary>
        public string SaslUsername { get; set; }

        /// <summary>
        /// SASL PLAINTEXT认证密码
        /// </summary>
        public string SaslPassword { get; set; }
    }
}

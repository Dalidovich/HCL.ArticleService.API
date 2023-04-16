using Amazon.Runtime.Internal.Util;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using HCL.ArticleService.API.BLL.Interfaces;
using HCL.ArticleService.API.Domain.DTO;
using HCL.ArticleService.API.Domain.Enums;
using HCL.ArticleService.API.Domain.InnerResponse;
using Microsoft.Extensions.Logging;

namespace HCL.ArticleService.API.BLL.Services
{
    public class KafkaProducerService: IKafkaProducerService
    {
        private readonly string _topic= "new_article";
        private readonly string _bootstrapServers;
        private readonly IProducer<Null, string> _producer;
        private readonly ILogger<KafkaProducerService> _logger;

        public KafkaProducerService(KafkaSettings kafkaSettings)
        {
            _bootstrapServers = kafkaSettings.BootstrapServers;
            var config = new ProducerConfig
            {
                BootstrapServers = _bootstrapServers
            };
            _producer = new ProducerBuilder<Null, string>(config).Build();
        }

        public async Task<BaseResponse<bool>> createTopicAsync()
        {
            using (var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = _bootstrapServers }).Build())
            {
                await adminClient.CreateTopicsAsync(new TopicSpecification[] { new TopicSpecification { Name = _topic, ReplicationFactor = 1, NumPartitions = 1 } });
            }

            return new StandartResponse<bool>()
            {
                Data = true,
                StatusCode = StatusCode.TopicCreate,
            };
        }

        public async Task<BaseResponse<bool>> CreateMessage(string messageContent)
        {
            var deliveryReport = await _producer.ProduceAsync(_topic, new Message<Null, string> { Value = messageContent, });

            return new StandartResponse<bool>()
            {
                Data = true,
                StatusCode = StatusCode.MessageSend,
            };
        }
    }
}

using Amazon.Runtime.Internal.Util;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using HCL.ArticleService.API.BLL.Interfaces;
using HCL.ArticleService.API.Domain.DTO;
using HCL.ArticleService.API.Domain.Enums;
using HCL.ArticleService.API.Domain.InnerResponse;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace HCL.ArticleService.API.BLL.Services
{
    public class KafkaProducerService: IKafkaProducerService
    {
        private readonly string _topic;
        private readonly string _bootstrapServers;
        private readonly IProducer<string, string> _producer;

        public KafkaProducerService(KafkaSettings kafkaSettings)
        {
            _bootstrapServers = kafkaSettings.BootstrapServers;
            _topic = kafkaSettings.Topic;
            var config = new ProducerConfig
            {
                BootstrapServers = _bootstrapServers,
            };
            _producer = new ProducerBuilder<string, string>(config)
                .Build();
        }

        public async Task<BaseResponse<bool>> CreateMessage(KafkaArticleCreateNotification messageContent)
        {
            var deliveryReport = await _producer.ProduceAsync(_topic, new Message<string, string> {
                Key=messageContent.ArticleId,
                Value = messageContent.AuthorId
            });

            return new StandartResponse<bool>()
            {
                Data = true,
                StatusCode = StatusCode.MessageSend,
            };
        }
    }
}

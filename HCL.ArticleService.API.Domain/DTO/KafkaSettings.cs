﻿namespace HCL.ArticleService.API.Domain.DTO
{
    public class KafkaSettings
    {
        public string BootstrapServers { get; set; }
        public string Topic { get; set; }
    }
}
namespace HCL.ArticleService.API.Domain.DTO.AppSettingsDTO
{
    public class KafkaSettings
    {
        public string BootstrapServers { get; set; }
        public string Topic { get; set; }
    }
}
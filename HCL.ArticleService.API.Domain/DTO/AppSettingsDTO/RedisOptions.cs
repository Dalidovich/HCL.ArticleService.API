namespace HCL.ArticleService.API.Domain.DTO.AppSettingsDTO
{
    public class RedisOptions
    {
        public string Host { get; set; }
        public string Resource { get; set; }
        public TimeSpan Expiry { get; set; }
        public TimeSpan Wait { get; set; }
        public TimeSpan Retry { get; set; }
        public TimeSpan StoreDuration { get; set; }
    }
}
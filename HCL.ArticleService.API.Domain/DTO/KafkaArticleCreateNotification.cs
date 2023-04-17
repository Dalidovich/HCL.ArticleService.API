using HCL.ArticleService.API.Domain.Entities;

namespace HCL.ArticleService.API.Domain.DTO
{
    public class KafkaArticleCreateNotification
    {
        public string ArticleId { get; set; }
        public string AuthorId { get; set; }

        public KafkaArticleCreateNotification(Article article)
        {
            ArticleId = article.Id;
            AuthorId = article.Author;
        }
    }
}

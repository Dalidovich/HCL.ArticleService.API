using HCL.ArticleService.API.BLL.Interfaces;
using HCL.ArticleService.API.DAL.Repositories.Interfaces;
using HCL.ArticleService.API.Domain.DTO;
using HCL.ArticleService.API.Domain.Entities;
using HCL.ArticleService.API.Domain.Enums;
using HCL.ArticleService.API.Domain.InnerResponse;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace HCL.ArticleService.API.BLL.Services
{
    public class ArticleControllService : IArticleControllService
    {
        private readonly IArticleRepository _articleRepository;
        private readonly IKafkaProducerService _kafkaProducerService;

        public ArticleControllService(IArticleRepository articleRepository, IKafkaProducerService kafkaProducerService)
        {
            _articleRepository = articleRepository;
            _kafkaProducerService = kafkaProducerService;
        }

        public async Task<BaseResponse<Article>> CreateArticle(Article article)
        {
            var createdArticle = await _articleRepository.AddAsync(article);
            await _kafkaProducerService.CreateMessage(new KafkaArticleCreateNotification(createdArticle));

            return new StandartResponse<Article>()
            {
                Data = createdArticle,
                StatusCode = StatusCode.ArticleCreate
            };
        }

        public async Task<BaseResponse<bool>> DeleteArticle(Expression<Func<Article, bool>> expression)
        {

            return new StandartResponse<bool>()
            {
                Data = await _articleRepository.DeleteAsync(expression),
                StatusCode = StatusCode.ArticleDelete
            };
        }

        public BaseResponse<IQueryable<Article>> GetArticleOData()
        {
            var contents = _articleRepository.GetArticlesAsync();
            if (contents.Count() == 0)
            {
                throw new KeyNotFoundException("[GetArticleOData]");
            }

            return new StandartResponse<IQueryable<Article>>()
            {
                Data = contents,
                StatusCode = StatusCode.ArticleRead
            };
        }
    }
}
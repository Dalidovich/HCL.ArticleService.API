using Grpc.Net.Client;
using HCL.ArticleService.API.BLL.gRPCClients;
using HCL.ArticleService.API.BLL.Interfaces;
using HCL.ArticleService.API.DAL.Repositories.Interfaces;
using HCL.ArticleService.API.Domain.DTO;
using HCL.ArticleService.API.Domain.DTO.AppSettingsDTO;
using HCL.ArticleService.API.Domain.Entities;
using HCL.ArticleService.API.Domain.Enums;
using HCL.ArticleService.API.Domain.InnerResponse;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Linq.Expressions;

namespace HCL.ArticleService.API.BLL.Services
{
    public class ArticleControllService : IArticleControllService
    {
        private readonly IArticleRepository _articleRepository;
        private readonly IKafkaProducerService _kafkaProducerService;
        private readonly IRedisLockService _redisLockService;
        private readonly IGrpcService _grpcService;

        public ArticleControllService(IArticleRepository articleRepository, IKafkaProducerService kafkaProducerService
            , IRedisLockService redisLockService, IGrpcService grpcService)
        {
            _articleRepository = articleRepository;
            _kafkaProducerService = kafkaProducerService;
            _redisLockService = redisLockService;
            _grpcService = grpcService;
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
            var contents = _articleRepository.GetArticlesOdata();
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

        public async Task<BaseResponse<ArticleWithAthorDTO>> GetFullArticleInfo(string articleId)
        {
            var rawArticel = _articleRepository.GetArticlesOdata().Where(x => x.Id == articleId).SingleOrDefault();
            if (rawArticel == null)
            {

                return new StandartResponse<ArticleWithAthorDTO>();
            }

            var resource = await _redisLockService.GetAthor(rawArticel.Author);

            if (resource.StatusCode == StatusCode.RedisReceive)
            {
                ArticleWithAthorDTO article = new ArticleWithAthorDTO
                    (resource.Data.Login, resource.Data.Status, resource.Data.CreateDate.ToDateTime(), rawArticel);

                return new StandartResponse<ArticleWithAthorDTO>()
                {
                    Data = article,
                    StatusCode = StatusCode.ArticleRead
                };
            }

            return await _grpcService.GetFullArticleInfoGrpc(rawArticel);
        }

        public async Task<BaseResponse<bool>> UpdateArticlesActualState()
        {
            var filter = Builders<Article>.Filter.Lte(x => x.CreateDate, DateTime.Now.AddYears(-2));
            var updeteSettings = Builders<Article>.Update.Set(x => x.IsActual, false);

            return new StandartResponse<bool>()
            {
                Data = await _articleRepository.UpdateManyAsync(filter, updeteSettings),
                StatusCode = StatusCode.ArticleUpdate
            };
        }
    }
}
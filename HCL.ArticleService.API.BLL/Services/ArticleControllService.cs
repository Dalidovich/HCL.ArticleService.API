using HCL.ArticleService.API.BLL.Interfaces;
using HCL.ArticleService.API.DAL.Repositories.Interfaces;
using HCL.ArticleService.API.Domain.Entities;
using HCL.ArticleService.API.Domain.Enums;
using HCL.ArticleService.API.Domain.InnerResponse;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HCL.ArticleService.API.BLL.Services
{
    public class ArticleControllService : IArticleControllService
    {
        private readonly IArticleRepository _articleRepository;
        protected readonly ILogger<IArticleControllService> _logger;

        public ArticleControllService(IArticleRepository articleRepository, ILogger<IArticleControllService> logger)
        {
            _articleRepository = articleRepository;
            _logger = logger;
        }

        private async Task<BaseResponse<Article>> GetArticleBase(Article? entity)
        {
            try
            {
                if (entity == null)
                {
                    return new StandartResponse<Article>()
                    {
                        Message = "entity not found"
                    };
                }
                return new StandartResponse<Article>()
                {
                    Data = entity,
                    StatusCode = StatusCode.ArticleRead
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[GetArticleBase] : {ex.Message}");
                return new StandartResponse<Article>()
                {
                    Message = ex.Message,
                    StatusCode = StatusCode.InternalServerError,
                };
            }
        }

        public async Task<BaseResponse<Article>> GetArticle(BsonDocument filter)
        {
            try
            {
                var entity = await _articleRepository.GetArticleAsync(filter);
                return await GetArticleBase(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[GetArticle_BsonDocument] : {ex.Message}");
                return new StandartResponse<Article>()
                {
                    Message = ex.Message,
                    StatusCode = StatusCode.InternalServerError,
                };
            }
        }

        public async Task<BaseResponse<Article>> GetArticle(Expression<Func<Article, bool>> expression)
        {
            try
            {
                var entity = await _articleRepository.GetArticleAsync(expression);
                return await GetArticleBase(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[GetArticle_Expression] : {ex.Message}");
                return new StandartResponse<Article>()
                {
                    Message = ex.Message,
                    StatusCode = StatusCode.InternalServerError,
                };
            }
        }

        public async Task<BaseResponse<Article>> CreateArticle(Article account)
        {
            try
            {
                var createdAccount = await _articleRepository.AddAsync(account);
                return new StandartResponse<Article>()
                {
                    Data = createdAccount,
                    StatusCode = StatusCode.ArticleCreate
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[CreateArticle] : {ex.Message}");
                return new StandartResponse<Article>()
                {
                    Message = ex.Message,
                    StatusCode = StatusCode.InternalServerError,
                };
            }
        }

        public async Task<BaseResponse<bool>> DeleteArticle(Expression<Func<Article, bool>> expression)
        {
            try
            {
                var entity = await _articleRepository.GetArticlesAsync(expression,0,1);
                if (entity == null)
                {
                    return new StandartResponse<bool>()
                    {
                        Message = "entity not found"
                    };
                }
                return new StandartResponse<bool>()
                {
                    Data = await _articleRepository.DeleteAsync(expression),
                    StatusCode = StatusCode.ArticleDelete
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[DeleteArticle] : {ex.Message}");
                return new StandartResponse<bool>()
                {
                    Message = ex.Message,
                    StatusCode = StatusCode.InternalServerError,
                };
            }
        }

        private async Task<BaseResponse<IEnumerable<Article>>> GetAllArticlesBase(IEnumerable<Article>? contents)
        {
            try
            {
                if (contents == null)
                {
                    return new StandartResponse<IEnumerable<Article>>()
                    {
                        Message = "entity not found"
                    };
                }
                return new StandartResponse<IEnumerable<Article>>()
                {
                    Data = contents,
                    StatusCode = StatusCode.ArticleRead
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[GetAllArticles] : {ex.Message}");
                return new StandartResponse<IEnumerable<Article>>()
                {
                    Message = ex.Message,
                    StatusCode = StatusCode.InternalServerError,
                };
            }
        }

        public async Task<BaseResponse<IEnumerable<Article>>> GetAllArticles(Expression<Func<Article, bool>> expression, int skip, int loadCount)
        {
            try
            {
                var contents = await _articleRepository.GetArticlesAsync(expression,skip,loadCount);
                return await GetAllArticlesBase(contents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[GetAllArticles_Expression] : {ex.Message}");
                return new StandartResponse<IEnumerable<Article>>()
                {
                    Message = ex.Message,
                    StatusCode = StatusCode.InternalServerError,
                };
            }
        }

        public async Task<BaseResponse<IEnumerable<Article>>> GetAllArticles(BsonDocument filter, int skip, int loadCount)
        {
            try
            {
                var contents = await _articleRepository.GetArticlesAsync(filter,skip,loadCount);
                return await GetAllArticlesBase(contents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[GetAllArticles_BsonDocument] : {ex.Message}");
                return new StandartResponse<IEnumerable<Article>>()
                {
                    Message = ex.Message,
                    StatusCode = StatusCode.InternalServerError,
                };
            }
        }
    }
}

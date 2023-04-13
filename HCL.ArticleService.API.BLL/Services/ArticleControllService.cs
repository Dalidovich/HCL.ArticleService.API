using HCL.ArticleService.API.BLL.Interfaces;
using HCL.ArticleService.API.DAL.Repositories.Interfaces;
using HCL.ArticleService.API.Domain.Entities;
using HCL.ArticleService.API.Domain.Enums;
using HCL.ArticleService.API.Domain.InnerResponse;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using MongoDB.Bson;
using SharpCompress.Common;
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
                throw new Exception($"[CreateArticle] : {ex.Message}");
            }
        }

        public async Task<BaseResponse<bool>> DeleteArticle(Expression<Func<Article, bool>> expression)
        {
            try
            {
                return new StandartResponse<bool>()
                {
                    Data = await _articleRepository.DeleteAsync(expression),
                    StatusCode = StatusCode.ArticleDelete
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"[DeleteArticle] : {ex.Message}");
            }
        }

        public BaseResponse<IQueryable<Article>> GetArticleOData()
        {
            try
            {
                var contents=_articleRepository.GetArticlesAsync();
                if (contents == null)
                {
                    return new StandartResponse<IQueryable<Article>>()
                    {
                        Message = "entity not found"
                    };
                }
                return new StandartResponse<IQueryable<Article>>()
                {
                    Data = contents,
                    StatusCode = StatusCode.ArticleRead
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"[GetArticleOData] : {ex.Message}");
            }
        }

    }
}

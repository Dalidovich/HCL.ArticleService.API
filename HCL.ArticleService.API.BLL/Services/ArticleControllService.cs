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
            var createdAccount = await _articleRepository.AddAsync(account);
            return new StandartResponse<Article>()
            {
                Data = createdAccount,
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
            var contents=_articleRepository.GetArticlesAsync();
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

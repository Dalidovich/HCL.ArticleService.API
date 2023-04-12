using HCL.ArticleService.API.Domain.Entities;
using HCL.ArticleService.API.Domain.InnerResponse;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HCL.ArticleService.API.BLL.Interfaces
{
    public interface IArticleControllService
    {
        public Task<BaseResponse<Article>> GetArticle(Expression<Func<Article, bool>> expression);
        public Task<BaseResponse<Article>> GetArticle(BsonDocument filter);
        public Task<BaseResponse<IEnumerable<Article>>> GetAllArticles(Expression<Func<Article, bool>> expression,int skip,int loadCount);
        public Task<BaseResponse<IEnumerable<Article>>> GetAllArticles(BsonDocument filter, int skip,int loadCount);
        public Task<BaseResponse<Article>> CreateArticle(Article account);
        public Task<BaseResponse<bool>> DeleteArticle(Expression<Func<Article, bool>> expression);
    }
}

using HCL.ArticleService.API.Domain.Entities;
using MongoDB.Bson;
using System.Linq.Expressions;

namespace HCL.ArticleService.API.DAL.Repositories.Interfaces
{
    public interface IArticleRepository
    {
        public Task<Article> AddAsync(Article article);
        public Task<bool> DeleteAsync(Expression<Func<Article, bool>> expression);
        public Task<IEnumerable<Article>> GetArticlesAsync(Expression<Func<Article, bool>> expression, int skip, int loadCount);
        public Task<IEnumerable<Article>> GetArticlesAsync(BsonDocument filter, int skip, int loadCount);
        public Task<Article> GetArticleAsync(Expression<Func<Article, bool>> expression);
        public Task<Article> GetArticleAsync(BsonDocument filter);
    }
}

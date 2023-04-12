using HCL.ArticleService.API.Domain.Entities;
using MongoDB.Bson;
using System.Linq.Expressions;

namespace HCL.ArticleService.API.DAL.Repositories.Interfaces
{
    public interface IArticleRepository
    {
        public Task<Article> AddAsync(Article article);
        public Task<bool> DeleteAsync(Expression<Func<Article, bool>> expression);
        public IQueryable<Article> GetArticlesAsync();
        public Task<Article> GetArticleAsync(Expression<Func<Article, bool>> expression);
    }
}

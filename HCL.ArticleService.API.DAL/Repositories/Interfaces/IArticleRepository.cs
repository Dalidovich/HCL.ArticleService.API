using HCL.ArticleService.API.Domain.Entities;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace HCL.ArticleService.API.DAL.Repositories.Interfaces
{
    public interface IArticleRepository
    {
        public Task<Article> AddAsync(Article article);
        public Task<bool> DeleteAsync(Expression<Func<Article, bool>> expression);
        public IQueryable<Article> GetArticlesOdata();
        public Task<bool> UpdateManyAsync(FilterDefinition<Article> filter, UpdateDefinition<Article> updateDefinition);
    }
}
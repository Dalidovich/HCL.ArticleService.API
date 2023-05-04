using HCL.ArticleService.API.DAL.Repositories.Interfaces;
using HCL.ArticleService.API.Domain.DTO.AppSettingsDTO;
using HCL.ArticleService.API.Domain.Entities;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace HCL.ArticleService.API.DAL.Repositories
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly MongoClient _client;
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<Article> _articlesTable;

        public ArticleRepository(MongoDBSettings mongoDBSettings)
        {
            _client = new MongoClient(mongoDBSettings.Host);
            _database = _client.GetDatabase(mongoDBSettings.Database);
            _articlesTable = _database.GetCollection<Article>(mongoDBSettings.Collection);
        }

        public async Task<Article> AddAsync(Article article)
        {
            await _articlesTable.InsertOneAsync(article);

            return article;
        }

        public async Task<bool> DeleteAsync(Expression<Func<Article, bool>> expression)
        {
            var deletedArticle = await _articlesTable.DeleteManyAsync(expression);

            return deletedArticle.IsAcknowledged;
        }

        public IQueryable<Article> GetArticlesOdata()
        {

            return _articlesTable.AsQueryable();
        }

        public async Task<bool> UpdateManyAsync(FilterDefinition<Article> filter, UpdateDefinition<Article> updateDefinition)
        {
            var updatedArticle = await _articlesTable.UpdateManyAsync(filter, updateDefinition);

            return updatedArticle.IsAcknowledged;
        }
    }
}
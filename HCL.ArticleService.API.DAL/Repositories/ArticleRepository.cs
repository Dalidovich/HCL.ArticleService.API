using HCL.ArticleService.API.DAL.Repositories.Interfaces;
using HCL.ArticleService.API.Domain.DTO;
using HCL.ArticleService.API.Domain.Entities;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HCL.ArticleService.API.DAL.Repositories
{
    public class ArticleRepository : IArticleRepository
    {
        private MongoClient _client;
        private IMongoDatabase _database;
        private IMongoCollection<Article> _articlesTable;

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
            var deletedArticle= await _articlesTable.DeleteManyAsync(expression);
            return deletedArticle.IsAcknowledged;
        }

        public async Task<Article> GetArticleAsync(Expression<Func<Article, bool>> expression)
        {
            return await _articlesTable.Find(expression).SingleOrDefaultAsync();
        }

        public IQueryable<Article> GetArticlesAsync()
        {
            return _articlesTable.AsQueryable();
        }
    }
}

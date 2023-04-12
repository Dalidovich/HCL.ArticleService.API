using HCL.ArticleService.API.DAL.Repositories.Interfaces;
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

        public ArticleRepository()
        {
            _client = new MongoClient("mongodb://localhost:27017");
            _database = _client.GetDatabase("HCL_Article");
            _articlesTable = _database.GetCollection<Article>("articles");
        }

        public async Task<Article> AddAsync(Article article)
        {
            await _articlesTable.InsertOneAsync(article);
            return article;
        }

        public async Task<bool> DeleteAsync(Expression<Func<Article, bool>> expression)
        {
            var deletedArticle= await _articlesTable.DeleteManyAsync(expression);
            return true;
        }

        public async Task<Article> GetArticleAsync(Expression<Func<Article, bool>> expression)
        {
            return await _articlesTable.Find(expression).SingleOrDefaultAsync();
        }

        public async Task<Article> GetArticleAsync(BsonDocument filter)
        {
            return await _articlesTable.Find(filter).SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<Article>> GetArticlesAsync(Expression<Func<Article, bool>> expression, int skip, int loadCount)
        {
            return await _articlesTable.Find(expression).Skip(skip).Limit(loadCount).ToListAsync();
        }

        public async Task<IEnumerable<Article>> GetArticlesAsync(BsonDocument filter, int skip, int loadCount)
        {
            return await _articlesTable.Find(filter).Skip(skip).Limit(loadCount).ToListAsync();
        }
    }
}

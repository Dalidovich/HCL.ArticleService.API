using HCL.ArticleService.API.DAL.Repositories.Interfaces;
using HCL.ArticleService.API.Domain.DTO;
using HCL.ArticleService.API.Domain.Entities;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Connections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HCL.ArticleService.API.DAL.Repositories
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly MongoClient _client;
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<Article> _articlesTable;
        private readonly IClientSession clientSession;

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

        public IQueryable<Article> GetArticlesAsync()
        {
            return _articlesTable.AsQueryable();
        }
    }
}

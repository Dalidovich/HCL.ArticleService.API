using HCL.ArticleService.API.BLL.Interfaces;
using HCL.ArticleService.API.Domain.DTO;
using HCL.ArticleService.API.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Data;

namespace HCL.ArticleService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArticleController : ControllerBase
    {
        private readonly ILogger<ArticleController> _logger;
        private readonly IArticleControllService _articleControllService;

        public ArticleController(ILogger<ArticleController> logger, IArticleControllService articleControllService)
        {
            _logger = logger;
            _articleControllService= articleControllService;
        }

        [Authorize]
        [HttpPost("v1/CreateArticle/")]
        public async Task<IResult> CreateArticle(ArticleDTO articleDTO)
        {
            if (articleDTO == null)
            {
                return Results.NotFound();
            }
            var resourse = await _articleControllService.CreateArticle(new Article(articleDTO));
            if (resourse.Data == null)
            {
                return Results.NoContent();
            }
            else
            {
                return Results.Json(new { articleId = resourse.Data.Id});
            }
        }

        [Authorize]
        [HttpDelete("v1/DeleteOwnArticle/{ownId}/{articleId}")]
        public async Task<IResult> DeleteOwnArticle(string ownId, string articleId)
        {
            var article=await _articleControllService.GetArticle(x=>x.Id==articleId);
            if (article.Data == null)
            {
                return Results.NoContent();
            }
            else if (article.StatusCode == Domain.Enums.StatusCode.ArticleRead && article.Data.Author==ownId)
            {
                await _articleControllService.DeleteArticle(x=>x.Id==articleId);
                return Results.Ok();
            }
            return Results.StatusCode(403);
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("v1/DeleteArticle/{articleId}")]
        public async Task<IResult> DeleteOwnArticle(string articleId)
        {
            var article = await _articleControllService.GetArticle(x => x.Id == articleId);
            if (article.StatusCode == Domain.Enums.StatusCode.ArticleRead)
            {
                await _articleControllService.DeleteArticle(x => x.Id == articleId);
                return Results.Ok();
            }
            return Results.NoContent();
        }

        [HttpGet("v1/GetArticle/{filterOperator}/{field}/{value}")]
        public async Task<IResult> GetArticle(string filterOperator,string field, string value)
        {
            var filter = new BsonDocument { { field, new BsonDocument(filterOperator, value) } };
            var article = await _articleControllService.GetArticle(filter);
            if (article.StatusCode == Domain.Enums.StatusCode.ArticleRead)
            {
                return Results.Json(article.Data);
            }
            return Results.NoContent();
        }

        [HttpGet("v1/GetArticle/{filterOperator}/{field}/{value}/{skipCount}/{takeCount}")]
        public async Task<IResult> GetArticles(string filterOperator, string field, string value,int skipCount,int takeCount)
        {
            var filter = new BsonDocument { { field, new BsonDocument(filterOperator, value) } };
            var article = await _articleControllService.GetAllArticles(filter, skipCount, takeCount);
            if (article.StatusCode == Domain.Enums.StatusCode.ArticleRead)
            {
                return Results.Json(article.Data);
            }
            return Results.NoContent();
        }
    }
}
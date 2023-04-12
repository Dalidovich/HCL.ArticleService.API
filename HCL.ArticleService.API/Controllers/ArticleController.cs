using HCL.ArticleService.API.BLL.Interfaces;
using HCL.ArticleService.API.Domain.DTO;
using HCL.ArticleService.API.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Data;

namespace HCL.ArticleService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArticleController : ODataController
    {
        private readonly ILogger<ArticleController> _logger;
        private readonly IArticleControllService _articleControllService;

        public ArticleController(ILogger<ArticleController> logger, IArticleControllService articleControllService)
        {
            _logger = logger;
            _articleControllService= articleControllService;
        }

        [Authorize]
        [HttpPost("v1/Article")]
        public async Task<IResult> CreateArticle([FromQuery] ArticleDTO articleDTO)
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
                return Results.Json(new { articleId = resourse.Data.Id });
            }
        }

        [Authorize]
        [HttpDelete("v1/OwnArticle")]
        public async Task<IResult> DeleteOwnArticle([FromQuery] string ownId, [FromQuery] string articleId)
        {
            var article = _articleControllService.GetArticleOData().Data.Where(x=>x.Id== articleId).SingleOrDefault();
            if (article == null)
            {
                return Results.NoContent();
            }
            else if (article.Author == ownId)
            {
                await _articleControllService.DeleteArticle(x => x.Id == articleId);
                return Results.Ok();
            }
            return Results.StatusCode(403);
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("v1/Article")]
        public async Task<IResult> DeleteOwnArticle([FromQuery] string articleId)
        {
            var article = _articleControllService.GetArticleOData().Data.Where(x => x.Id == articleId).SingleOrDefault();
            if (article == null)
            {
                return Results.NoContent();
            }
            else
            {
                await _articleControllService.DeleteArticle(x => x.Id == articleId);
                return Results.Ok();
            }
        }

        [HttpGet("odata/v1/Article")]
        [EnableQuery]
        public IQueryable<Article> GetArticle()
        {
            return _articleControllService.GetArticleOData().Data;
        }
    }
}
using HCL.ArticleService.API.BLL.Interfaces;
using HCL.ArticleService.API.Domain.DTO;
using HCL.ArticleService.API.Domain.DTO.Builders;
using HCL.ArticleService.API.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Data;
using System.Text.Json;

namespace HCL.ArticleService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArticleController : ControllerBase
    {
        private readonly IArticleControllService _articleControllService;
        private readonly ILogger<ArticleController> _logger;

        public ArticleController(IArticleControllService articleControllService, ILogger<ArticleController> logger)
        {
            _articleControllService = articleControllService;
            _logger = logger;
        }

        [Authorize]
        [HttpPost("v1/article")]
        public async Task<IActionResult> CreateArticle([FromQuery] ArticleDTO articleDTO)
        {
            var resourse = await _articleControllService.CreateArticle(new Article(articleDTO));
            if (resourse.StatusCode == Domain.Enums.StatusCode.ArticleCreate)
            {

                var log = new LogDTOBuidlder("CreateArticle(articleDTO)")
                    .BuildMessage("create article")
                    .BuildStatusCode(201)
                    .BuildSuccessState(true)
                    .Build();
                _logger.LogInformation(JsonSerializer.Serialize(log));

                return Created("", new { articleId = resourse.Data.Id });
            }

            return NotFound();
        }

        [Authorize]
        [HttpDelete("v1/article/account")]
        public async Task<IActionResult> DeleteOwnArticle([FromQuery] string ownId, [FromQuery] string articleId)
        {
            var article = _articleControllService.GetArticleOData().Data
                ?.Where(x => x.Id == articleId)
                .SingleOrDefault();
            if (article == null)
            {

                return NotFound();
            }
            else if (article.Author == ownId)
            {
                await _articleControllService.DeleteArticle(x => x.Id == articleId);
                var log = new LogDTOBuidlder("DeleteOwnArticle(ownId,articleId)")
                    .BuildMessage("delete article")
                    .BuildStatusCode(204)
                    .BuildSuccessState(true)
                    .Build();
                _logger.LogInformation(JsonSerializer.Serialize(log));

                return NoContent();
            }

            return Forbid();
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("v1/article")]
        public async Task<IActionResult> DeleteOwnArticle([FromQuery] string articleId)
        {
            var article = _articleControllService.GetArticleOData().Data
                ?.Where(x => x.Id == articleId)
                .SingleOrDefault();
            if (article == null)
            {
                var log = new LogDTOBuidlder("DeleteOwnArticle(articleId)")
                .BuildMessage("admin account delete article")
                .BuildStatusCode(204)
                .Build();
                _logger.LogInformation(JsonSerializer.Serialize(log));

                return NotFound();
            }
            await _articleControllService.DeleteArticle(x => x.Id == articleId);

            return NoContent();
        }

        [HttpGet("v1/article/withAthor")]
        public async Task<IActionResult> GetArticleWithAthor([FromQuery] string articleId)
        {
            var articleWithAthor = await _articleControllService.GetFullArticleInfo(articleId);
            if (articleWithAthor.StatusCode == Domain.Enums.StatusCode.EntityNotFound)
            {
                var log = new LogDTOBuidlder("GetArticleWithAthor(articleId)")
                    .BuildMessage("get article with athor")
                    .BuildStatusCode(204)
                    .BuildSuccessState(true)
                    .Build();
                _logger.LogInformation(JsonSerializer.Serialize(log));

                return NotFound();
            }

            return Ok(articleWithAthor.Data);
        }
    }
}
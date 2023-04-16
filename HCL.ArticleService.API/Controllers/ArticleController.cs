using HCL.ArticleService.API.BLL.Interfaces;
using HCL.ArticleService.API.BLL.Services;
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
    public class ArticleController : ControllerBase
    {
        private readonly ILogger<ArticleController> _logger;
        private readonly IArticleControllService _articleControllService;

        public ArticleController(ILogger<ArticleController> logger, IArticleControllService articleControllService
            , KafkaProducerService kafkaProducerService)
        {
            _logger = logger;
            _articleControllService= articleControllService;
        }

        [Authorize]
        [HttpPost("v1/Article")]
        public async Task<IActionResult> CreateArticle([FromQuery] ArticleDTO articleDTO)
        {
            if (ModelState.IsValid)
            {
                var resourse = await _articleControllService.CreateArticle(new Article(articleDTO));
                if (resourse.Data != null)
                {
                    return Created("",new { articleId = resourse.Data.Id });
                }
                return NotFound();
            }
            return BadRequest();
        }

        [Authorize]
        [HttpDelete("v1/OwnArticle")]
        public async Task<IActionResult> DeleteOwnArticle([FromQuery] string ownId, [FromQuery] string articleId)
        {
            var article = _articleControllService.GetArticleOData().Data.Where(x=>x.Id== articleId).SingleOrDefault();
            if (article == null)
            {
                return NotFound();
            }
            else if (article.Author == ownId)
            {
                await _articleControllService.DeleteArticle(x => x.Id == articleId);
                return NoContent();
            }
            return Forbid();
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("v1/Article")]
        public async Task<IActionResult> DeleteOwnArticle([FromQuery] string articleId)
        {
            var article = _articleControllService.GetArticleOData().Data.Where(x => x.Id == articleId).SingleOrDefault();
            if (article == null)
            {
                return NotFound();
            }
            await _articleControllService.DeleteArticle(x => x.Id == articleId);
            return NoContent();
        }
    }
}
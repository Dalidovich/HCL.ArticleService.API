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
    public class ArticleODataController : ODataController
    {
        private readonly ILogger<ArticleODataController> _logger;
        private readonly IArticleControllService _articleControllService;

        public ArticleODataController(ILogger<ArticleODataController> logger, IArticleControllService articleControllService)
        {
            _logger = logger;
            _articleControllService= articleControllService;
        }

        [HttpGet("odata/v1/Article")]
        [EnableQuery]
        public IQueryable<Article> GetArticle()
        {
            return _articleControllService.GetArticleOData().Data;
        }
    }
}
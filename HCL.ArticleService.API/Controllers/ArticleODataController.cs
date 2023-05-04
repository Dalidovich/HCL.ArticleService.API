using HCL.ArticleService.API.BLL.Interfaces;
using HCL.ArticleService.API.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace HCL.ArticleService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArticleODataController : ODataController
    {
        private readonly IArticleControllService _articleControllService;

        public ArticleODataController(IArticleControllService articleControllService)
        {
            _articleControllService = articleControllService;
        }

        [HttpGet("odata/v1/Article")]
        [EnableQuery]
        public IQueryable<Article> GetArticle()
        {

            return _articleControllService.GetArticleOData().Data;
        }
    }
}
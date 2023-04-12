
using HCL.ArticleService.API.BLL.Interfaces;
using HCL.ArticleService.API.BLL.Services;
using HCL.ArticleService.API.DAL.Repositories;
using HCL.ArticleService.API.DAL.Repositories.Interfaces;

namespace HCL.ArticleService.API
{
    public static class DIManger
    {
        public static void AddRepositores(this WebApplicationBuilder webApplicationBuilder)
        {
            webApplicationBuilder.Services.AddScoped<IArticleRepository, ArticleRepository>();
        }

        public static void AddServices(this WebApplicationBuilder webApplicationBuilder)
        {
            webApplicationBuilder.Services.AddScoped<IArticleControllService, ArticleControllService>();
        }
    }
}
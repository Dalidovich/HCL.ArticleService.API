
using HCL.ArticleService.API.BLL.Interfaces;
using HCL.ArticleService.API.BLL.Services;
using HCL.ArticleService.API.DAL.Repositories;
using HCL.ArticleService.API.DAL.Repositories.Interfaces;
using HCL.ArticleService.API.Domain.DTO;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.Options;
using Microsoft.OData.ModelBuilder;

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
            webApplicationBuilder.Services.AddScoped<IKafkaProducerService, KafkaProducerService>();
        }

        public static void AddODataProperty(this WebApplicationBuilder webApplicationBuilder)
        {
            webApplicationBuilder.Services.AddControllers().AddOData(opt =>
            {
                opt.Count().Filter().Expand().Select().OrderBy().SetMaxTop(5000)
                    .AddRouteComponents("odata", new ODataConventionModelBuilder().GetEdmModel());
                opt.TimeZone = TimeZoneInfo.Utc;
            });
        }

        public static void AddMongoDBConnection(this WebApplicationBuilder webApplicationBuilder)
        {
            webApplicationBuilder.Services.Configure<MongoDBSettings>(webApplicationBuilder.Configuration.GetSection("MongoDbSettings"));
            webApplicationBuilder.Services.AddSingleton(serviceProvider =>serviceProvider.GetRequiredService<IOptions<MongoDBSettings>>().Value);            
        }

        public static void AddKafkaProperty(this WebApplicationBuilder webApplicationBuilder)
        {
            webApplicationBuilder.Services.Configure<KafkaSettings>(webApplicationBuilder.Configuration.GetSection("KafkaSettings"));
            webApplicationBuilder.Services.AddSingleton(serviceProvider => serviceProvider.GetRequiredService<IOptions<KafkaSettings>>().Value);
        }

    }
}
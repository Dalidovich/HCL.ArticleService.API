using HCL.ArticleService.API.BLL.Interfaces;
using HCL.ArticleService.API.BLL.Services;
using HCL.ArticleService.API.DAL.Repositories;
using HCL.ArticleService.API.DAL.Repositories.Interfaces;
using HCL.ArticleService.API.Domain.DTO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.ModelBuilder;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

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

        public static void AddAuthProperty(this WebApplicationBuilder webApplicationBuilder)
        {
            var secretKey = webApplicationBuilder.Configuration.GetSection("JWTSettings:SecretKey").Value;
            var issuer = webApplicationBuilder.Configuration.GetSection("JWTSettings:Issuer").Value;
            var audience = webApplicationBuilder.Configuration.GetSection("JWTSettings:Audience").Value;
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            webApplicationBuilder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {

                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    IssuerSigningKey = signingKey,
                    ValidateIssuerSigningKey = true,
                };
            });
        }
    }
}
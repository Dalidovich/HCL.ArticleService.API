using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using HCL.ArticleService.API.BackgroundHostedServices;
using HCL.ArticleService.API.BLL.Interfaces;
using HCL.ArticleService.API.BLL.Services;
using HCL.ArticleService.API.DAL.Repositories;
using HCL.ArticleService.API.DAL.Repositories.Interfaces;
using HCL.ArticleService.API.Domain.DTO.AppSettingsDTO;
using HCL.ArticleService.API.Midleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.ModelBuilder;
using Serilog;
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
            webApplicationBuilder.Services.AddScoped<IRedisLockService, RedisLockService>();
            webApplicationBuilder.Services.AddScoped<IGrpcService, GrpcService>();
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
            webApplicationBuilder.Services.AddSingleton(serviceProvider => serviceProvider.GetRequiredService<IOptions<MongoDBSettings>>().Value);
        }

        public static void AddGrpcProperty(this WebApplicationBuilder webApplicationBuilder)
        {
            webApplicationBuilder.Services.Configure<IdentityGrpcSettings>(webApplicationBuilder.Configuration.GetSection("IdentityGrpcSettings"));
            webApplicationBuilder.Services.AddSingleton(serviceProvider => serviceProvider.GetRequiredService<IOptions<IdentityGrpcSettings>>().Value);
        }

        public static void AddHostedServices(this WebApplicationBuilder webApplicationBuilder)
        {
            webApplicationBuilder.Services.AddHostedService<HangfireRecurringHostJob>();
        }

        public static void AddKafkaProperty(this WebApplicationBuilder webApplicationBuilder)
        {
            webApplicationBuilder.Services.Configure<KafkaSettings>(webApplicationBuilder.Configuration.GetSection("KafkaSettings"));
            webApplicationBuilder.Services.AddSingleton(serviceProvider => serviceProvider.GetRequiredService<IOptions<KafkaSettings>>().Value);
        }

        public static void AddElasticserchProperty(this WebApplicationBuilder webApplicationBuilder)
        {
            ElasticsearchHelper.ConfigureLogging();
            webApplicationBuilder.Host.UseSerilog();
        }

        public static void AddHangfireProperty(this WebApplicationBuilder webApplicationBuilder)
        {
            var migrationOptions = new MongoMigrationOptions
            {
                MigrationStrategy = new DropMongoMigrationStrategy(),
                BackupStrategy = new CollectionMongoBackupStrategy()
            };
            var storageOptions = new MongoStorageOptions
            {
                MigrationOptions = migrationOptions,
                CheckQueuedJobsStrategy = CheckQueuedJobsStrategy.TailNotificationsCollection
            };

            var hangfireSettings = webApplicationBuilder.Configuration
                .GetSection("HangfireMongoDbSettings")
                .Get<HangfireMongoDBSettings>();

            webApplicationBuilder.Services.AddHangfire(x =>
                x.UseMongoStorage(
                    hangfireSettings.Host
                    , hangfireSettings.HangfireDatabase
                    , storageOptions));
            webApplicationBuilder.Services.AddHangfireServer();
        }

        public static void AddAuthProperty(this WebApplicationBuilder webApplicationBuilder)
        {
            var jwtSettings = webApplicationBuilder.Configuration
                .GetRequiredSection("JWTSettings")
                .Get<JWTSettings>();

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));

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
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateLifetime = true,
                    IssuerSigningKey = signingKey,
                    ValidateIssuerSigningKey = true,
                };
            });
        }

        public static void AddRedisPropperty(this WebApplicationBuilder webApplicationBuilder)
        {
            webApplicationBuilder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = webApplicationBuilder.Configuration.GetSection("RedisOptions:Host").Value;
            });

            webApplicationBuilder.Services.Configure<RedisOptions>(webApplicationBuilder.Configuration.GetSection("RedisOptions"));
            webApplicationBuilder.Services.AddSingleton(serviceProvider => serviceProvider.GetRequiredService<IOptions<RedisOptions>>().Value);
        }

        public static void AddMiddleware(this WebApplication webApplication)
        {
            webApplication.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
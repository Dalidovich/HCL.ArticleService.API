using HCL.ArticleService.API.BLL.Midleware;
using HCL.ArticleService.API.Domain.DTO;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.OData.ModelBuilder;

namespace HCL.ArticleService.API
{

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.AddRepositores();
            builder.AddServices();
            builder.AddMongoDBConnection();
            builder.AddKafkaProperty();
            builder.AddODataProperty();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
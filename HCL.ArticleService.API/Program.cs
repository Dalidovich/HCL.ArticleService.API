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

            builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDbSettings"));
            builder.Services.AddSingleton(serviceProvider =>
        serviceProvider.GetRequiredService<IOptions<MongoDBSettings>>().Value);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddControllers().AddOData(opt =>
            {
                opt.Count().Filter().Expand().Select().OrderBy().SetMaxTop(5000)
                    .AddRouteComponents("odata", new ODataConventionModelBuilder().GetEdmModel());
                opt.TimeZone = TimeZoneInfo.Utc;
            });


            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
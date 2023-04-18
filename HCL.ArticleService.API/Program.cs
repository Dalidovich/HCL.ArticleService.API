using HCL.ArticleService.API.BLL.Midleware;

namespace HCL.ArticleService.API
{

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.AddRepositores();
            builder.AddServices();
            builder.AddKafkaProperty();
            builder.AddMongoDBConnection();
            builder.AddODataProperty();
            builder.AddAuthProperty();            

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
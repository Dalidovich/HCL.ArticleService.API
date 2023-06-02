using HCL.ArticleService.API.Domain.DTO.AppSettingsDTO;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace HCL.ArticleService.API.Test.IntegrationTest
{
    public class CustomTestHostBuilder
    {
        public static WebApplicationFactory<Program> Build(MongoDBSettings mongoDBSettings)
        {
            return new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(async services =>
                {
                    var settings = services.SingleOrDefault(d =>
                             d.ServiceType == typeof(MongoDBSettings));

                    services.Remove(settings);

                    services.AddSingleton(mongoDBSettings);
                });
            });
        }
    }
}

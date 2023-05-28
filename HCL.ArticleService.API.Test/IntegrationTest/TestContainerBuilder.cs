using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace HCL.ArticleService.API.Test.IntegrationTest
{
    public class TestContainerBuilder
    {
        public static readonly string mongoUser = "mongo";
        public static readonly string mongoPassword = "mongo";

        public static readonly string database = "HCL_Article";
        public static readonly string collection = "articles";

        public static IContainer CreateMongoDBContainer()
        {

            return new ContainerBuilder()
                .WithName(Guid.NewGuid().ToString("N"))
                .WithImage("mongo:latest")
                .WithAutoRemove(true)
                .WithHostname(Guid.NewGuid().ToString("N"))
                .WithExposedPort(27017)
                .WithPortBinding(27017, true)
                .WithEnvironment("MONGO_INITDB_ROOT_USERNAME", "mongo")
                .WithEnvironment("MONGO_INITDB_ROOT_PASSWORD", "mongo")
                .WithTmpfsMount("/mongodata")
                .Build();
        }
    }
}

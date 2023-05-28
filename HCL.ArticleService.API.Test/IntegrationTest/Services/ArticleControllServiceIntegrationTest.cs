using DotNet.Testcontainers.Containers;
using FluentAssertions;
using HCL.ArticleService.API.BLL.Services;
using HCL.ArticleService.API.DAL.Repositories;
using HCL.ArticleService.API.Domain.DTO;
using HCL.ArticleService.API.Domain.DTO.AppSettingsDTO;
using HCL.ArticleService.API.Domain.Entities;
using HCL.ArticleService.API.Test.IntegrationTest;
using MongoDB.Bson;
using Xunit;

namespace HCL.ArticleService.API.Test.Service
{
    public class ArticleControllServiceIntegrationTest : IAsyncLifetime
    {
        private IContainer mongoContainer = TestContainerBuilder.CreateMongoDBContainer();
        private MongoDBSettings mongoDBSettings = new MongoDBSettings();

        public async Task InitializeAsync()
        {
            await mongoContainer.StartAsync();
            mongoDBSettings = new MongoDBSettings()
            {
                Collection = TestContainerBuilder.collection,
                Database = TestContainerBuilder.database,
                Host = $"mongodb://{TestContainerBuilder.mongoUser}:{TestContainerBuilder.mongoPassword}" +
                $"@localhost:{mongoContainer.GetMappedPublicPort(27017)}"
            };
        }

        public async Task DisposeAsync()
        {
            await mongoContainer.StopAsync();
        }
        [Fact]
        public async Task CreateArticle_WithRightData_ReturnNewArticle()
        {
            //Arrange
            var kafkaProdMock = StandartMockBuilder.CreateKafkaProducerMock();
            var redisMock = StandartMockBuilder.CreateSuccessReceiveMessageRedisLockServiceMock();
            var grpcServMock = StandartMockBuilder.CreateGrpcServiceMock();

            var articRep = new ArticleRepository(mongoDBSettings);
            var articServ = new ArticleControllService(articRep, kafkaProdMock.Object
                , redisMock.Object, grpcServMock.Object);

            var article = new Article(new ArticleDTO())
            {
                Author = "Ilia",
                Content = "1",
                Theme = "test",
                Title = "Test"
            };

            //Act
            var result = await articServ.CreateArticle(article);

            //Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(Domain.Enums.StatusCode.ArticleCreate);
        }

        [Fact]
        public async Task DeleteArticle_WithExistArticle_ReturnBooleanTrue()
        {
            //Arrange
            var kafkaProdMock = StandartMockBuilder.CreateKafkaProducerMock();
            var redisMock = StandartMockBuilder.CreateSuccessReceiveMessageRedisLockServiceMock();
            var grpcServMock = StandartMockBuilder.CreateGrpcServiceMock();

            var articRep = new ArticleRepository(mongoDBSettings);

            var addedEntity = await articRep.AddAsync(
                new Article(new ArticleDTO())
                {
                    Author = Guid.NewGuid().ToString(),
                    IsActual = true,
                    Content = "test",
                    CreateDate = DateTime.Now,
                    Theme = "test",
                    Title = "Test"
                }
            );

            var articServ = new ArticleControllService(articRep, kafkaProdMock.Object
                , redisMock.Object, grpcServMock.Object);

            //Act
            var result = await articServ.DeleteArticle(x => x.Id == addedEntity.Id);

            //Assert
            result.Should().NotBeNull();
            result.Data.Should().BeTrue();
            result.StatusCode.Should().Be(Domain.Enums.StatusCode.ArticleDelete);
        }

        [Fact]
        public async Task DeleteArticle_WithNotExistArticle_ReturnBooleanTrue()
        {
            //Arrange
            var kafkaProdMock = StandartMockBuilder.CreateKafkaProducerMock();
            var redisMock = StandartMockBuilder.CreateSuccessReceiveMessageRedisLockServiceMock();
            var grpcServMock = StandartMockBuilder.CreateGrpcServiceMock();

            var articRep = new ArticleRepository(mongoDBSettings);

            var addedEntity = await articRep.AddAsync(
                new Article(new ArticleDTO())
                {
                    Author = Guid.NewGuid().ToString(),
                    IsActual = true,
                    Content = "test",
                    CreateDate = DateTime.Now,
                    Theme = "test",
                    Title = "Test"
                }
            );

            var articServ = new ArticleControllService(articRep, kafkaProdMock.Object
                , redisMock.Object, grpcServMock.Object);

            //Act
            var result = await articServ.DeleteArticle(x => x.Id == ObjectId.GenerateNewId().ToString());

            //Assert
            result.Should().NotBeNull();
            result.Data.Should().BeTrue();
            result.StatusCode.Should().Be(Domain.Enums.StatusCode.ArticleDelete);
        }

        [Fact]
        public async Task GetFullArticle_WithExistArticleAndSuccessReceiveRedis_ReturnFullInfo()
        {
            //Arrange
            var kafkaProdMock = StandartMockBuilder.CreateKafkaProducerMock();
            var redisMock = StandartMockBuilder.CreateSuccessReceiveMessageRedisLockServiceMock();
            var grpcServMock = StandartMockBuilder.CreateGrpcServiceMock();

            var articRep = new ArticleRepository(mongoDBSettings);

            var addedEntity = await articRep.AddAsync(
                new Article(new ArticleDTO())
                {
                    Author = Guid.NewGuid().ToString(),
                    IsActual = true,
                    Content = "test",
                    CreateDate = DateTime.Now,
                    Theme = "test",
                    Title = "Test"
                }
            );

            var articServ = new ArticleControllService(articRep, kafkaProdMock.Object
                , redisMock.Object, grpcServMock.Object);

            //Act
            var result = await articServ.GetFullArticleInfo(addedEntity.Id);

            //Assert
            result.Should().NotBeNull();
            result.Data.Should().NotBeNull();
            result.Data.AuthorLogin.Should().Be("Ilia");
            result.StatusCode.Should().Be(Domain.Enums.StatusCode.ArticleRead);
        }

        [Fact]
        public async Task GetFullArticle_WithExistArticleAndFailReceiveRedis_ReturnFullInfo()
        {
            //Arrange
            var kafkaProdMock = StandartMockBuilder.CreateKafkaProducerMock();
            var redisMock = StandartMockBuilder.CreateFailReceiveMessageRedisLockServiceMock();
            var grpcServMock = StandartMockBuilder.CreateGrpcServiceMock();

            var articRep = new ArticleRepository(mongoDBSettings);

            var addedEntity = await articRep.AddAsync(
                new Article(new ArticleDTO())
                {
                    Author = Guid.NewGuid().ToString(),
                    IsActual = true,
                    Content = "test",
                    CreateDate = DateTime.Now,
                    Theme = "test",
                    Title = "Test"
                }
            );

            var articServ = new ArticleControllService(articRep, kafkaProdMock.Object
                , redisMock.Object, grpcServMock.Object);

            //Act
            var result = await articServ.GetFullArticleInfo(addedEntity.Id);

            //Assert
            result.Should().NotBeNull();
            result.Data.Should().NotBeNull();
            result.Data.AuthorLogin.Should().Be("Dima");
            result.StatusCode.Should().Be(Domain.Enums.StatusCode.ArticleRead);
        }

        [Fact]
        public async Task GetFullArticle_WithNotExistArticleAndSuccessReceiveRedis_ReturnFullInfo()
        {
            //Arrange
            var kafkaProdMock = StandartMockBuilder.CreateKafkaProducerMock();
            var redisMock = StandartMockBuilder.CreateSuccessReceiveMessageRedisLockServiceMock();
            var grpcServMock = StandartMockBuilder.CreateGrpcServiceMock();

            var articRep = new ArticleRepository(mongoDBSettings);
            var articServ = new ArticleControllService(articRep, kafkaProdMock.Object
                , redisMock.Object, grpcServMock.Object);

            //Act
            var result = await articServ.GetFullArticleInfo(ObjectId.GenerateNewId().ToString());

            //Assert
            result.Should().NotBeNull();
            result.Data.Should().BeNull();
            result.StatusCode.Should().Be(Domain.Enums.StatusCode.EntityNotFound);
        }

        [Fact]
        public async Task GetFullArticle_WithNotExistArticleAndFailReceiveRedis_ReturnFullInfo()
        {
            //Arrange
            var kafkaProdMock = StandartMockBuilder.CreateKafkaProducerMock();
            var redisMock = StandartMockBuilder.CreateFailReceiveMessageRedisLockServiceMock();
            var grpcServMock = StandartMockBuilder.CreateGrpcServiceMock();

            var articRep = new ArticleRepository(mongoDBSettings);
            var articServ = new ArticleControllService(articRep, kafkaProdMock.Object
                , redisMock.Object, grpcServMock.Object);

            //Act
            var result = await articServ.GetFullArticleInfo(ObjectId.GenerateNewId().ToString());

            //Assert
            result.Should().NotBeNull();
            result.Data.Should().BeNull();
            result.StatusCode.Should().Be(Domain.Enums.StatusCode.EntityNotFound);
        }

        [Fact]
        public async Task UpdateArticleActualState_WithExistArticle_ReturnBooleanTrue()
        {
            //Arrange
            var kafkaProdMock = StandartMockBuilder.CreateKafkaProducerMock();
            var redisMock = StandartMockBuilder.CreateSuccessReceiveMessageRedisLockServiceMock();
            var grpcServMock = StandartMockBuilder.CreateGrpcServiceMock();

            var articRep = new ArticleRepository(mongoDBSettings);

            await articRep.AddAsync(
                new Article(new ArticleDTO())
                {
                    Author = Guid.NewGuid().ToString(),
                    IsActual = true,
                    Content = "test",
                    CreateDate = DateTime.Now,
                    Theme = "test",
                    Title = "Test"
                }
            );
            await articRep.AddAsync(
                new Article(new ArticleDTO())
                {
                    Author = Guid.NewGuid().ToString(),
                    IsActual = true,
                    Content = "test",
                    CreateDate = DateTime.Now.AddYears(-4),
                    Theme = "test",
                    Title = "Test"
                }
            );

            var articServ = new ArticleControllService(articRep, kafkaProdMock.Object
                , redisMock.Object, grpcServMock.Object);

            //Act
            var result = await articServ.UpdateArticlesActualState();

            //Assert
            result.Should().NotBeNull();
            result.Data.Should().BeTrue();
            result.StatusCode.Should().Be(Domain.Enums.StatusCode.ArticleUpdate);
        }
    }
}

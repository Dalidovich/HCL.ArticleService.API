using FluentAssertions;
using HCL.ArticleService.API.BLL.Services;
using HCL.ArticleService.API.Controllers;
using HCL.ArticleService.API.DAL.Repositories;
using HCL.ArticleService.API.Domain.DTO;
using HCL.ArticleService.API.Domain.DTO.AppSettingsDTO;
using HCL.ArticleService.API.Domain.Entities;
using HCL.ArticleService.API.Test.IntegrationTest;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace HCL.ArticleService.API.Test.Controllers
{
    public class ArticleControllerIntegrationTest
    {
        [Fact]
        public async Task CreateArticle_WithRightData_ReturnNewArticle()
        {
            //Arrange
            var mongoContainer = TestContainerBuilder.CreateMongoDBContainer();
            await mongoContainer.StartAsync();

            MongoDBSettings mongoDBSettings = new MongoDBSettings()
            {
                Collection = TestContainerBuilder.collection,
                Database = TestContainerBuilder.database,
                Host = $"mongodb://{TestContainerBuilder.mongoUser}:{TestContainerBuilder.mongoPassword}" +
                    $"@localhost:{mongoContainer.GetMappedPublicPort(27017)}"
            };

            var kafkaProdMock = StandartMockBuilder.CreateKafkaProducerMock();
            var redisMock = StandartMockBuilder.CreateSuccessReceiveMessageRedisLockServiceMock();
            var grpcServMock = StandartMockBuilder.CreateGrpcServiceMock();

            var articRep = new ArticleRepository(mongoDBSettings);
            var articServ = new ArticleControllService(articRep, kafkaProdMock.Object
                , redisMock.Object, grpcServMock.Object);
            var controller = new ArticleController(articServ, StandartMockBuilder.mockLoggerArticleController);

            var articleDto = new ArticleDTO()
            {
                Author = Guid.NewGuid(),
                Content = "fisrt",
                Theme = "test",
                Title = "Test"
            };

            //Act
            var createdResult = await controller.CreateArticle(articleDto) as CreatedResult;

            //Assert
            createdResult.Should().NotBeNull();

            await mongoContainer.StopAsync();
        }

        [Fact]
        public async Task DeleteArticle_WithExistArticleIsMine_ReturnNoContent()
        {
            //Arrange
            var articleId = Guid.NewGuid().ToString();
            var accountId = Guid.NewGuid();

            var mongoContainer = TestContainerBuilder.CreateMongoDBContainer();
            await mongoContainer.StartAsync();

            MongoDBSettings mongoDBSettings = new MongoDBSettings()
            {
                Collection = TestContainerBuilder.collection,
                Database = TestContainerBuilder.database,
                Host = $"mongodb://{TestContainerBuilder.mongoUser}:{TestContainerBuilder.mongoPassword}" +
                    $"@localhost:{mongoContainer.GetMappedPublicPort(27017)}"
            };

            var kafkaProdMock = StandartMockBuilder.CreateKafkaProducerMock();
            var redisMock = StandartMockBuilder.CreateSuccessReceiveMessageRedisLockServiceMock();
            var grpcServMock = StandartMockBuilder.CreateGrpcServiceMock();

            var articRep = new ArticleRepository(mongoDBSettings);

            var addedEntity = await articRep.AddAsync(
                new Article(new ArticleDTO())
                {
                    Author = accountId.ToString(),
                    IsActual = true,
                    Content = "test",
                    CreateDate = DateTime.Now,
                    Theme = "test",
                    Title = "Test"
                }
            );
            articleId = addedEntity.Id;

            var articServ = new ArticleControllService(articRep, kafkaProdMock.Object
                , redisMock.Object, grpcServMock.Object);
            var controller = new ArticleController(articServ, StandartMockBuilder.mockLoggerArticleController);

            //Act
            var result = await controller.DeleteOwnArticle(accountId.ToString(), articleId) as NoContentResult;

            //Assert
            result.Should().NotBeNull();

            await mongoContainer.StopAsync();
        }

        [Fact]
        public async Task DeleteArticle_WithExistArticleIsNotMine_ReturnForbid()
        {
            //Arrange
            var articleId = Guid.NewGuid().ToString();

            var mongoContainer = TestContainerBuilder.CreateMongoDBContainer();
            await mongoContainer.StartAsync();

            MongoDBSettings mongoDBSettings = new MongoDBSettings()
            {
                Collection = TestContainerBuilder.collection,
                Database = TestContainerBuilder.database,
                Host = $"mongodb://{TestContainerBuilder.mongoUser}:{TestContainerBuilder.mongoPassword}" +
                    $"@localhost:{mongoContainer.GetMappedPublicPort(27017)}"
            };

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
            articleId = addedEntity.Id;

            var articServ = new ArticleControllService(articRep, kafkaProdMock.Object
                , redisMock.Object, grpcServMock.Object);
            var controller = new ArticleController(articServ, StandartMockBuilder.mockLoggerArticleController);

            //Act
            var result = await controller.DeleteOwnArticle(Guid.NewGuid().ToString(), articleId) as ForbidResult;

            //Assert
            result.Should().NotBeNull();

            await mongoContainer.StopAsync();
        }

        [Fact]
        public async Task DeleteArticle_WithNotExistArticleIsNotMine_ReturnNotFound()
        {
            //Arrange
            var mongoContainer = TestContainerBuilder.CreateMongoDBContainer();
            await mongoContainer.StartAsync();

            MongoDBSettings mongoDBSettings = new MongoDBSettings()
            {
                Collection = TestContainerBuilder.collection,
                Database = TestContainerBuilder.database,
                Host = $"mongodb://{TestContainerBuilder.mongoUser}:{TestContainerBuilder.mongoPassword}" +
                    $"@localhost:{mongoContainer.GetMappedPublicPort(27017)}"
            };

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
            var controller = new ArticleController(articServ, StandartMockBuilder.mockLoggerArticleController);

            //Act
            var result = await controller.DeleteOwnArticle(Guid.NewGuid().ToString(), "64395070590f1a86560058c3") as NotFoundResult;

            //Assert
            result.Should().NotBeNull();

            await mongoContainer.StopAsync();
        }

        [Fact]
        public async Task DeleteArticle_WithNotExistArticle_ReturnNotFound()
        {
            //Arrange
            var mongoContainer = TestContainerBuilder.CreateMongoDBContainer();
            await mongoContainer.StartAsync();

            MongoDBSettings mongoDBSettings = new MongoDBSettings()
            {
                Collection = TestContainerBuilder.collection,
                Database = TestContainerBuilder.database,
                Host = $"mongodb://{TestContainerBuilder.mongoUser}:{TestContainerBuilder.mongoPassword}" +
                    $"@localhost:{mongoContainer.GetMappedPublicPort(27017)}"
            };

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

            var articServ = new ArticleControllService(articRep, kafkaProdMock.Object
                , redisMock.Object, grpcServMock.Object);
            var controller = new ArticleController(articServ, StandartMockBuilder.mockLoggerArticleController);

            //Act
            var result = await controller.DeleteOwnArticle("64395070590f1a86560058c3") as NotFoundResult;

            //Assert
            result.Should().NotBeNull();

            await mongoContainer.StopAsync();
        }

        [Fact]
        public async Task DeleteArticle_WithExistArticle_ReturnNoContent()
        {
            //Arrange
            var articleId = Guid.NewGuid().ToString();
            var mongoContainer = TestContainerBuilder.CreateMongoDBContainer();
            await mongoContainer.StartAsync();

            MongoDBSettings mongoDBSettings = new MongoDBSettings()
            {
                Collection = TestContainerBuilder.collection,
                Database = TestContainerBuilder.database,
                Host = $"mongodb://{TestContainerBuilder.mongoUser}:{TestContainerBuilder.mongoPassword}" +
                    $"@localhost:{mongoContainer.GetMappedPublicPort(27017)}"
            };

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
            articleId = addedEntity.Id;
            var articServ = new ArticleControllService(articRep, kafkaProdMock.Object
                , redisMock.Object, grpcServMock.Object);
            var controller = new ArticleController(articServ, StandartMockBuilder.mockLoggerArticleController);

            //Act
            var result = await controller.DeleteOwnArticle(articleId) as NoContentResult;

            //Assert
            result.Should().NotBeNull();

            await mongoContainer.StopAsync();
        }

        [Fact]
        public async Task GetArticle_WithExistArticleAndSuccessRedisResieve_ReturnOk()
        {
            //Arrange
            var articleId = Guid.NewGuid().ToString();
            var mongoContainer = TestContainerBuilder.CreateMongoDBContainer();
            await mongoContainer.StartAsync();

            MongoDBSettings mongoDBSettings = new MongoDBSettings()
            {
                Collection = TestContainerBuilder.collection,
                Database = TestContainerBuilder.database,
                Host = $"mongodb://{TestContainerBuilder.mongoUser}:{TestContainerBuilder.mongoPassword}" +
                    $"@localhost:{mongoContainer.GetMappedPublicPort(27017)}"
            };

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
            articleId = addedEntity.Id;

            var articServ = new ArticleControllService(articRep, kafkaProdMock.Object
                , redisMock.Object, grpcServMock.Object);
            var controller = new ArticleController(articServ, StandartMockBuilder.mockLoggerArticleController);

            //Act
            var result = await controller.GetArticleWithAthor(articleId) as OkObjectResult;

            //Assert
            result.Should().NotBeNull();

            await mongoContainer.StopAsync();
        }

        [Fact]
        public async Task GetArticle_WithNotExistArticleAndSuccessRedisResieve_ReturnNotFound()
        {
            //Arrange
            var mongoContainer = TestContainerBuilder.CreateMongoDBContainer();
            await mongoContainer.StartAsync();

            MongoDBSettings mongoDBSettings = new MongoDBSettings()
            {
                Collection = TestContainerBuilder.collection,
                Database = TestContainerBuilder.database,
                Host = $"mongodb://{TestContainerBuilder.mongoUser}:{TestContainerBuilder.mongoPassword}" +
                    $"@localhost:{mongoContainer.GetMappedPublicPort(27017)}"
            };

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

            var articServ = new ArticleControllService(articRep, kafkaProdMock.Object
                , redisMock.Object, grpcServMock.Object);
            var controller = new ArticleController(articServ, StandartMockBuilder.mockLoggerArticleController);

            //Act
            var result = await controller.GetArticleWithAthor("64395070590f1a86560058c3") as NotFoundResult;

            //Assert
            result.Should().NotBeNull();

            await mongoContainer.StopAsync();
        }

        [Fact]
        public async Task GetArticle_WithExistArticleAndFailRedisResieve_ReturnOk()
        {
            //Arrange
            var articleId = Guid.NewGuid().ToString();
            var mongoContainer = TestContainerBuilder.CreateMongoDBContainer();
            await mongoContainer.StartAsync();

            MongoDBSettings mongoDBSettings = new MongoDBSettings()
            {
                Collection = TestContainerBuilder.collection,
                Database = TestContainerBuilder.database,
                Host = $"mongodb://{TestContainerBuilder.mongoUser}:{TestContainerBuilder.mongoPassword}" +
                    $"@localhost:{mongoContainer.GetMappedPublicPort(27017)}"
            };

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
            articleId = addedEntity.Id;

            var articServ = new ArticleControllService(articRep, kafkaProdMock.Object
                , redisMock.Object, grpcServMock.Object);
            var controller = new ArticleController(articServ, StandartMockBuilder.mockLoggerArticleController);

            //Act
            var result = await controller.GetArticleWithAthor(articleId) as OkObjectResult;

            //Assert
            result.Should().NotBeNull();

            await mongoContainer.StopAsync();
        }

        [Fact]
        public async Task GetArticle_WithNotExistArticleAndFailRedisResieve_ReturnNotFound()
        {
            //Arrange
            var mongoContainer = TestContainerBuilder.CreateMongoDBContainer();
            await mongoContainer.StartAsync();

            MongoDBSettings mongoDBSettings = new MongoDBSettings()
            {
                Collection = TestContainerBuilder.collection,
                Database = TestContainerBuilder.database,
                Host = $"mongodb://{TestContainerBuilder.mongoUser}:{TestContainerBuilder.mongoPassword}" +
                    $"@localhost:{mongoContainer.GetMappedPublicPort(27017)}"
            };

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

            var articServ = new ArticleControllService(articRep, kafkaProdMock.Object
                , redisMock.Object, grpcServMock.Object);
            var controller = new ArticleController(articServ, StandartMockBuilder.mockLoggerArticleController);

            //Act
            var result = await controller.GetArticleWithAthor("64395070590f1a86560058c3") as NotFoundResult;

            //Assert
            result.Should().NotBeNull();

            await mongoContainer.StopAsync();
        }
    }
}

using FluentAssertions;
using HCL.ArticleService.API.BLL.Services;
using HCL.ArticleService.API.Controllers;
using HCL.ArticleService.API.Domain.DTO;
using HCL.ArticleService.API.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace HCL.ArticleService.API.Test.Controllers
{
    public class ArticleControllerTest
    {
        [Fact]
        public async Task CreateArticle_WithRightData_ReturnNewArticle()
        {
            //Arrange
            List<Article> articles = new List<Article>();

            var articRepMock = StandartMockBuilder.CreateAccountRepositoryMock(articles);
            var kafkaProdMock = StandartMockBuilder.CreateKafkaProducerMock();
            var redisMock = StandartMockBuilder.CreateSuccessReceiveMessageRedisLockServiceMock();
            var grpcServMock = StandartMockBuilder.CreateGrpcServiceMock();

            var articServ = new ArticleControllService(articRepMock.Object, kafkaProdMock.Object
                , redisMock.Object, grpcServMock.Object);
            var controller = new ArticleController(articServ);

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
            articles.Should().NotBeEmpty();
            articles.First().Title.Should().Be(articleDto.Title);
        }

        [Fact]
        public async Task DeleteArticle_WithExistArticleIsMine_ReturnNoContent()
        {
            //Arrange
            var articleId = Guid.NewGuid().ToString();
            var accountId = Guid.NewGuid();
            List<Article> articles = new List<Article>()
            {
                new Article(new ArticleDTO())
                {
                    Id = articleId,
                    Author = accountId.ToString(),
                    IsActual = true,
                    Content = "test",
                    CreateDate = DateTime.Now,
                    Theme = "test",
                    Title = "Test"
                }
            };

            var articRepMock = StandartMockBuilder.CreateAccountRepositoryMock(articles);
            var kafkaProdMock = StandartMockBuilder.CreateKafkaProducerMock();
            var redisMock = StandartMockBuilder.CreateSuccessReceiveMessageRedisLockServiceMock();
            var grpcServMock = StandartMockBuilder.CreateGrpcServiceMock();

            var articServ = new ArticleControllService(articRepMock.Object, kafkaProdMock.Object
                , redisMock.Object, grpcServMock.Object);
            var controller = new ArticleController(articServ);

            //Act
            var result = await controller.DeleteOwnArticle(accountId.ToString(), articleId) as NoContentResult;

            //Assert
            result.Should().NotBeNull();
            articles.Should().BeEmpty();
        }

        [Fact]
        public async Task DeleteArticle_WithExistArticleIsNotMine_ReturnForbid()
        {
            //Arrange
            var articleId = Guid.NewGuid().ToString();
            List<Article> articles = new List<Article>()
            {
                new Article(new ArticleDTO())
                {
                    Id = articleId,
                    Author = Guid.NewGuid().ToString(),
                    IsActual = true,
                    Content = "test",
                    CreateDate = DateTime.Now,
                    Theme = "test",
                    Title = "Test"
                }
            };

            var articRepMock = StandartMockBuilder.CreateAccountRepositoryMock(articles);
            var kafkaProdMock = StandartMockBuilder.CreateKafkaProducerMock();
            var redisMock = StandartMockBuilder.CreateSuccessReceiveMessageRedisLockServiceMock();
            var grpcServMock = StandartMockBuilder.CreateGrpcServiceMock();

            var articServ = new ArticleControllService(articRepMock.Object, kafkaProdMock.Object
                , redisMock.Object, grpcServMock.Object);
            var controller = new ArticleController(articServ);

            //Act
            var result = await controller.DeleteOwnArticle(Guid.NewGuid().ToString(), articleId) as ForbidResult;

            //Assert
            result.Should().NotBeNull();
            articles.Should().NotBeEmpty();
        }

        [Fact]
        public async Task DeleteArticle_WithNotExistArticleIsNotMine_ReturnNotFound()
        {
            //Arrange
            List<Article> articles = new List<Article>()
            {
                new Article(new ArticleDTO())
                {
                    Id = Guid.NewGuid().ToString(),
                    Author = Guid.NewGuid().ToString(),
                    IsActual = true,
                    Content = "test",
                    CreateDate = DateTime.Now,
                    Theme = "test",
                    Title = "Test"
                }
            };

            var articRepMock = StandartMockBuilder.CreateAccountRepositoryMock(articles);
            var kafkaProdMock = StandartMockBuilder.CreateKafkaProducerMock();
            var redisMock = StandartMockBuilder.CreateSuccessReceiveMessageRedisLockServiceMock();
            var grpcServMock = StandartMockBuilder.CreateGrpcServiceMock();

            var articServ = new ArticleControllService(articRepMock.Object, kafkaProdMock.Object
                , redisMock.Object, grpcServMock.Object);
            var controller = new ArticleController(articServ);

            //Act
            var result = await controller.DeleteOwnArticle(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()) as NotFoundResult;

            //Assert
            result.Should().NotBeNull();
            articles.Should().NotBeEmpty();
        }

        [Fact]
        public async Task DeleteArticle_WithNotExistArticle_ReturnNotFound()
        {
            //Arrange
            List<Article> articles = new List<Article>()
            {
                new Article(new ArticleDTO())
                {
                    Id = Guid.NewGuid().ToString(),
                    Author = Guid.NewGuid().ToString(),
                    IsActual = true,
                    Content = "test",
                    CreateDate = DateTime.Now,
                    Theme = "test",
                    Title = "Test"
                }
            };

            var articRepMock = StandartMockBuilder.CreateAccountRepositoryMock(articles);
            var kafkaProdMock = StandartMockBuilder.CreateKafkaProducerMock();
            var redisMock = StandartMockBuilder.CreateSuccessReceiveMessageRedisLockServiceMock();
            var grpcServMock = StandartMockBuilder.CreateGrpcServiceMock();

            var articServ = new ArticleControllService(articRepMock.Object, kafkaProdMock.Object
                , redisMock.Object, grpcServMock.Object);
            var controller = new ArticleController(articServ);

            //Act
            var result = await controller.DeleteOwnArticle(Guid.NewGuid().ToString()) as NotFoundResult;

            //Assert
            result.Should().NotBeNull();
            articles.Should().NotBeEmpty();
        }

        [Fact]
        public async Task DeleteArticle_WithExistArticle_ReturnNoContent()
        {
            //Arrange
            var articleId = Guid.NewGuid().ToString();
            List<Article> articles = new List<Article>()
            {
                new Article(new ArticleDTO())
                {
                    Id = articleId,
                    Author = Guid.NewGuid().ToString(),
                    IsActual = true,
                    Content = "test",
                    CreateDate = DateTime.Now,
                    Theme = "test",
                    Title = "Test"
                }
            };

            var articRepMock = StandartMockBuilder.CreateAccountRepositoryMock(articles);
            var kafkaProdMock = StandartMockBuilder.CreateKafkaProducerMock();
            var redisMock = StandartMockBuilder.CreateSuccessReceiveMessageRedisLockServiceMock();
            var grpcServMock = StandartMockBuilder.CreateGrpcServiceMock();

            var articServ = new ArticleControllService(articRepMock.Object, kafkaProdMock.Object
                , redisMock.Object, grpcServMock.Object);
            var controller = new ArticleController(articServ);

            //Act
            var result = await controller.DeleteOwnArticle(articleId) as NoContentResult;

            //Assert
            result.Should().NotBeNull();
            articles.Should().BeEmpty();
        }

        [Fact]
        public async Task GetArticle_WithExistArticleAndSuccessRedisResieve_ReturnOk()
        {
            //Arrange
            var articleId = Guid.NewGuid().ToString();
            List<Article> articles = new List<Article>()
            {
                new Article(new ArticleDTO())
                {
                    Id = articleId,
                    Author = Guid.NewGuid().ToString(),
                    IsActual = true,
                    Content = "test",
                    CreateDate = DateTime.Now,
                    Theme = "test",
                    Title = "Test"
                }
            };

            var articRepMock = StandartMockBuilder.CreateAccountRepositoryMock(articles);
            var kafkaProdMock = StandartMockBuilder.CreateKafkaProducerMock();
            var redisMock = StandartMockBuilder.CreateSuccessReceiveMessageRedisLockServiceMock();
            var grpcServMock = StandartMockBuilder.CreateGrpcServiceMock();

            var articServ = new ArticleControllService(articRepMock.Object, kafkaProdMock.Object
                , redisMock.Object, grpcServMock.Object);
            var controller = new ArticleController(articServ);

            //Act
            var result = await controller.GetArticleWithAthor(articleId) as OkObjectResult;

            //Assert
            result.Should().NotBeNull();
            articles.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetArticle_WithNotExistArticleAndSuccessRedisResieve_ReturnNotFound()
        {
            //Arrange
            List<Article> articles = new List<Article>()
            {
                new Article(new ArticleDTO())
                {
                    Id = Guid.NewGuid().ToString(),
                    Author = Guid.NewGuid().ToString(),
                    IsActual = true,
                    Content = "test",
                    CreateDate = DateTime.Now,
                    Theme = "test",
                    Title = "Test"
                }
            };

            var articRepMock = StandartMockBuilder.CreateAccountRepositoryMock(articles);
            var kafkaProdMock = StandartMockBuilder.CreateKafkaProducerMock();
            var redisMock = StandartMockBuilder.CreateSuccessReceiveMessageRedisLockServiceMock();
            var grpcServMock = StandartMockBuilder.CreateGrpcServiceMock();

            var articServ = new ArticleControllService(articRepMock.Object, kafkaProdMock.Object
                , redisMock.Object, grpcServMock.Object);
            var controller = new ArticleController(articServ);

            //Act
            var result = await controller.GetArticleWithAthor(Guid.NewGuid().ToString()) as NotFoundResult;

            //Assert
            result.Should().NotBeNull();
            articles.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetArticle_WithExistArticleAndFailRedisResieve_ReturnOk()
        {
            //Arrange
            var articleId = Guid.NewGuid().ToString();
            List<Article> articles = new List<Article>()
            {
                new Article(new ArticleDTO())
                {
                    Id = articleId,
                    Author = Guid.NewGuid().ToString(),
                    IsActual = true,
                    Content = "test",
                    CreateDate = DateTime.Now,
                    Theme = "test",
                    Title = "Test"
                }
            };

            var articRepMock = StandartMockBuilder.CreateAccountRepositoryMock(articles);
            var kafkaProdMock = StandartMockBuilder.CreateKafkaProducerMock();
            var redisMock = StandartMockBuilder.CreateFailReceiveMessageRedisLockServiceMock();
            var grpcServMock = StandartMockBuilder.CreateGrpcServiceMock();

            var articServ = new ArticleControllService(articRepMock.Object, kafkaProdMock.Object
                , redisMock.Object, grpcServMock.Object);
            var controller = new ArticleController(articServ);

            //Act
            var result = await controller.GetArticleWithAthor(articleId) as OkObjectResult;

            //Assert
            result.Should().NotBeNull();
            articles.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetArticle_WithNotExistArticleAndFailRedisResieve_ReturnNotFound()
        {
            //Arrange
            List<Article> articles = new List<Article>()
            {
                new Article(new ArticleDTO())
                {
                    Id = Guid.NewGuid().ToString(),
                    Author = Guid.NewGuid().ToString(),
                    IsActual = true,
                    Content = "test",
                    CreateDate = DateTime.Now,
                    Theme = "test",
                    Title = "Test"
                }
            };

            var articRepMock = StandartMockBuilder.CreateAccountRepositoryMock(articles);
            var kafkaProdMock = StandartMockBuilder.CreateKafkaProducerMock();
            var redisMock = StandartMockBuilder.CreateFailReceiveMessageRedisLockServiceMock();
            var grpcServMock = StandartMockBuilder.CreateGrpcServiceMock();

            var articServ = new ArticleControllService(articRepMock.Object, kafkaProdMock.Object
                , redisMock.Object, grpcServMock.Object);
            var controller = new ArticleController(articServ);

            //Act
            var result = await controller.GetArticleWithAthor(Guid.NewGuid().ToString()) as NotFoundResult;

            //Assert
            result.Should().NotBeNull();
            articles.Should().NotBeEmpty();
        }
    }
}

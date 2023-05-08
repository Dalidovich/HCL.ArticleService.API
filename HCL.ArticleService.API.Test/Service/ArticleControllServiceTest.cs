using FluentAssertions;
using HCL.ArticleService.API.BLL.Services;
using HCL.ArticleService.API.Controllers;
using HCL.ArticleService.API.Domain.DTO;
using HCL.ArticleService.API.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace HCL.ArticleService.API.Test.Service
{
    public class ArticleControllServiceTest
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

            var article= new Article(new ArticleDTO())
            {
                Author="Ilia",
                Content="1",
                Theme="test",
                Title="Test"
            };

            //Act
            var result = await articServ.CreateArticle(article);

            //Assert
            result.Should().NotBeNull();
            articles.Should().NotBeEmpty();
            result.StatusCode.Should().Be(Domain.Enums.StatusCode.ArticleCreate);
        }

        [Fact]
        public async Task DeleteArticle_WithExistArticle_ReturnBooleanTrue()
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

            //Act
            var result = await articServ.DeleteArticle(x=>x.Id== articleId);

            //Assert
            result.Should().NotBeNull();
            articles.Should().BeEmpty();
            result.Data.Should().BeTrue();
            result.StatusCode.Should().Be(Domain.Enums.StatusCode.ArticleDelete);
        }

        [Fact]
        public async Task DeleteArticle_WithNotExistArticle_ReturnBooleanFalse()
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

            //Act
            var result = await articServ.DeleteArticle(x => x.Id == Guid.NewGuid().ToString());

            //Assert
            result.Should().NotBeNull();
            articles.Should().NotBeEmpty();
            result.Data.Should().BeFalse();
            result.StatusCode.Should().Be(Domain.Enums.StatusCode.ArticleDelete);
        }

        [Fact]
        public async Task GetFullArticle_WithExistArticleAndSuccessReceiveRedis_ReturnFullInfo()
        {
            //Arrange
            var articleId= Guid.NewGuid().ToString();
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

            //Act
            var result = await articServ.GetFullArticleInfo(articleId);

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

            //Act
            var result = await articServ.GetFullArticleInfo(articleId);

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

            //Act
            var result = await articServ.GetFullArticleInfo(Guid.NewGuid().ToString());

            //Assert
            result.Should().NotBeNull();
            result.Data.Should().BeNull();
            result.StatusCode.Should().Be(Domain.Enums.StatusCode.EntityNotFound);
        }

        [Fact]
        public async Task GetFullArticle_WithNotExistArticleAndFailReceiveRedis_ReturnFullInfo()
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

            //Act
            var result = await articServ.GetFullArticleInfo(Guid.NewGuid().ToString());

            //Assert
            result.Should().NotBeNull();
            result.Data.Should().BeNull();
            result.StatusCode.Should().Be(Domain.Enums.StatusCode.EntityNotFound);
        }

        [Fact]
        public async Task UpdateArticleActualState_WithExistArticle_ReturnBooleanTrue()
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
                },
                new Article(new ArticleDTO())
                {
                    Id = Guid.NewGuid().ToString(),
                    Author = Guid.NewGuid().ToString(),
                    IsActual = true,
                    Content = "test",
                    CreateDate = DateTime.Now.AddYears(-4),
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

            //Act
            var result = await articServ.UpdateArticlesActualState();

            //Assert
            result.Should().NotBeNull();
            result.Data.Should().BeTrue();
            result.StatusCode.Should().Be(Domain.Enums.StatusCode.ArticleUpdate);
            articles.Where(x=>x.IsActual).Count().Should().Be(1);
        }
    }
}

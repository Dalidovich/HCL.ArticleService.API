using Google.Protobuf.WellKnownTypes;
using HCL.ArticleService.API.BLL.gRPCClients;
using HCL.ArticleService.API.BLL.Interfaces;
using HCL.ArticleService.API.DAL.Repositories.Interfaces;
using HCL.ArticleService.API.Domain.DTO;
using HCL.ArticleService.API.Domain.DTO.AppSettingsDTO;
using HCL.ArticleService.API.Domain.Entities;
using HCL.ArticleService.API.Domain.InnerResponse;
using MockQueryable.Moq;
using Moq;
using System.Linq.Expressions;

namespace HCL.ArticleService.API.Test
{
    public class StandartMockBuilder
    {
        private static Article _addAccount(Article article, List<Article> articles)
        {
            var ar = new Article(new ArticleDTO())
            {
                Id = Guid.NewGuid().ToString(),
                Author = article.Author,
                IsActual = true,
                Content = article.Content,
                CreateDate = DateTime.Now,
                Theme = article.Theme,
                Title = article.Title
            };
            articles.Add(ar);

            return ar;
        }

        public static readonly IdentityGrpcSettings identityGrpcSettings = new IdentityGrpcSettings()
        {
            Host = "localhost",
        };

        public static Mock<IGrpcService> CreateGrpcServiceMock()
        {
            var grpcService = new Mock<IGrpcService>();
            grpcService.Setup(s => s.GetFullArticleInfoGrpc(It.IsAny<Article>()))
                .ReturnsAsync((Article article) =>
                {
                    ArticleWithAthorDTO articleWithAthorDTO = new ArticleWithAthorDTO
                    ("Ilia", "normal", DateTime.Now, article);

                    return new StandartResponse<ArticleWithAthorDTO>()
                    {
                        Data = articleWithAthorDTO,
                        StatusCode = Domain.Enums.StatusCode.ArticleRead
                    };
                });

            return grpcService;
        }

        public static Mock<IRedisLockService> CreateSuccessReceiveMessageRedisLockServiceMock()
        {
            var redisLockServ = new Mock<IRedisLockService>();
            redisLockServ
                .Setup(r => r.GetAthor(It.IsAny<string>()))
                .ReturnsAsync(() =>
                {

                    return new StandartResponse<AthorPublicProfileReply>()
                    {
                        Data = new AthorPublicProfileReply()
                        {
                            CreateDate = Timestamp.FromDateTimeOffset(DateTime.Now),
                            Login = "Ilia",
                            Status = "normal"
                        },
                        StatusCode = Domain.Enums.StatusCode.RedisReceive
                    };
                });

            return redisLockServ;
        }

        public static Mock<IRedisLockService> CreateFailReceiveMessageRedisLockServiceMock()
        {
            var redisLockServ = new Mock<IRedisLockService>();
            redisLockServ
                .Setup(r => r.GetAthor(It.IsAny<string>()))
                .ReturnsAsync(() =>
                {

                    return new StandartResponse<AthorPublicProfileReply>()
                    {
                        StatusCode = Domain.Enums.StatusCode.RedisEmpty
                    };
                });

            return redisLockServ;
        }

        public static Mock<IArticleRepository> CreateAccountRepositoryMock(List<Article> articles)
        {
            var arRep = new Mock<IArticleRepository>();
            var collectionQuerybleMock = articles.BuildMock();
            arRep
                .Setup(r => r.AddAsync(It.IsAny<Article>()))
                .ReturnsAsync((Article account) =>
                {

                    return _addAccount(account, articles);
                });

            arRep.Setup(r => r.DeleteAsync(It.IsAny<Expression<Func<Article, bool>>>()))
                .ReturnsAsync((Expression<Func<Article, bool>> expression) =>
                {
                    var del = articles.Where(expression.Compile()).SingleOrDefault();

                    if (del != null)
                    {
                        articles.Remove(del);

                        return true;
                    }

                    return false;
                });

            arRep.Setup(r => r.GetArticlesOdata())
                .Returns(collectionQuerybleMock);

            return arRep;
        }

        public static Mock<IKafkaProducerService> CreateKafkaProducerMock()
        {
            var KafProdMock = new Mock<IKafkaProducerService>();
            KafProdMock.Setup(p => p.CreateMessage(It.IsAny<KafkaArticleCreateNotification>()))
                .ReturnsAsync(() => new StandartResponse<bool>()
                {
                    Data = true,
                    StatusCode = Domain.Enums.StatusCode.MessageSend
                });

            return KafProdMock;
        }
    }
}

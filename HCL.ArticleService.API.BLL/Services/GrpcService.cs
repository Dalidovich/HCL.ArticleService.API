using Grpc.Net.Client;
using HCL.ArticleService.API.BLL.gRPCClients;
using HCL.ArticleService.API.BLL.Interfaces;
using HCL.ArticleService.API.Domain.DTO;
using HCL.ArticleService.API.Domain.DTO.AppSettingsDTO;
using HCL.ArticleService.API.Domain.Entities;
using HCL.ArticleService.API.Domain.Enums;
using HCL.ArticleService.API.Domain.InnerResponse;

namespace HCL.ArticleService.API.BLL.Services
{
    public class GrpcService : IGrpcService
    {
        private readonly IdentityGrpcSettings _identityGrpcSettings;

        public GrpcService(IdentityGrpcSettings identityGrpcSettings)
        {
            _identityGrpcSettings = identityGrpcSettings;
        }

        public async Task<BaseResponse<ArticleWithAthorDTO>> GetFullArticleInfoGrpc(Article rawArticel)
        {
            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            AthorPublicProfileReply reply;
            using (var channel = GrpcChannel.ForAddress(_identityGrpcSettings.Host, new GrpcChannelOptions { HttpHandler = httpHandler }))
            {
                var client = new AthorPublicProfile.AthorPublicProfileClient(channel);
                reply = await client.GetProfileAsync(new AthorIdRequest { AccountId = rawArticel.Author });
            }
            ArticleWithAthorDTO article = new ArticleWithAthorDTO(reply.Login, reply.Status, reply.CreateDate.ToDateTime(), rawArticel);

            return new StandartResponse<ArticleWithAthorDTO>()
            {
                Data = article,
                StatusCode = StatusCode.ArticleRead
            };            
        }
    }
}
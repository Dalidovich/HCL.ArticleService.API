using HCL.ArticleService.API.BLL.gRPCClients;
using HCL.ArticleService.API.Domain.InnerResponse;

namespace HCL.ArticleService.API.BLL.Interfaces
{
    public interface IRedisLockService
    {
        public Task<BaseResponse<AthorPublicProfileReply>> GetAthor(string authorId);
    }
}
using HCL.ArticleService.API.Domain.DTO;
using HCL.ArticleService.API.Domain.Entities;
using HCL.ArticleService.API.Domain.InnerResponse;

namespace HCL.ArticleService.API.BLL.Interfaces
{
    public interface IGrpcService
    {
        public Task<BaseResponse<ArticleWithAthorDTO>> GetFullArticleInfoGrpc(Article rawArticel);
    }
}
using HCL.ArticleService.API.Domain.DTO;
using HCL.ArticleService.API.Domain.InnerResponse;

namespace HCL.ArticleService.API.BLL.Interfaces
{
    public interface IKafkaProducerService
    {
        public Task<BaseResponse<bool>> CreateMessage(KafkaArticleCreateNotification messageContent);
    }
}
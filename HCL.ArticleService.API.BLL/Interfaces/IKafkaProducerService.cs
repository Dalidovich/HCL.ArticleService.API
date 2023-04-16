using HCL.ArticleService.API.Domain.InnerResponse;

namespace HCL.ArticleService.API.BLL.Interfaces
{
    public interface IKafkaProducerService
    {
        public Task<BaseResponse<bool>> createTopicAsync();
        public Task<BaseResponse<bool>> CreateMessage(string messageContent);
    }
}

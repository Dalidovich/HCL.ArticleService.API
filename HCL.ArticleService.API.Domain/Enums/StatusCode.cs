namespace HCL.ArticleService.API.Domain.Enums
{
    public enum StatusCode
    {
        EntityNotFound = 0,

        ArticleCreate = 1,
        ArticleDelete = 2,
        ArticleRead = 3,

        //Kafka status code
        TopicCreate = 11,
        TopicAlreadyExists = 12,
        MessageSend = 13,

        OK = 200,
        OKNoContent = 204,
        InternalServerError = 500,
    }
}
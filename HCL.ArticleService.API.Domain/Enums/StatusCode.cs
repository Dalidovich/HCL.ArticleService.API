namespace HCL.ArticleService.API.Domain.Enums
{
    public enum StatusCode
    {
        EntityNotFound = 0,

        ArticleCreate = 1,
        ArticleUpdate = 2,
        ArticleDelete = 3,
        ArticleRead = 4,

        //Kafka status code
        TopicCreate = 11,
        TopicAlreadyExists = 12,
        MessageSend = 13,

        //Redis status code
        RedisLock = 21,
        RedisEmpty = 22,
        RedisReceive = 23,

        OK = 200,
        OKNoContent = 204,
        InternalServerError = 500,
    }
}
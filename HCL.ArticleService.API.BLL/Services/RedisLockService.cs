using HCL.ArticleService.API.BLL.gRPCClients;
using HCL.ArticleService.API.BLL.Interfaces;
using HCL.ArticleService.API.Domain.DTO.AppSettingsDTO;
using HCL.ArticleService.API.Domain.Enums;
using HCL.ArticleService.API.Domain.InnerResponse;
using Microsoft.Extensions.Caching.Distributed;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;
using System.Text.Json;

namespace HCL.ArticleService.API.BLL.Services
{
    public class RedisLockService : IRedisLockService
    {
        private readonly RedisOptions _redisOptions;
        private readonly IDistributedCache _distributedCache;

        public RedisLockService(RedisOptions redisOptions, IDistributedCache distributedCache)
        {
            _redisOptions = redisOptions;
            _distributedCache = distributedCache;
        }

        public async Task<BaseResponse<AthorPublicProfileReply>> GetAthor(string authorId)
        {
            var existingConnectionMultiplexer = ConnectionMultiplexer.Connect(_redisOptions.Host);
            var multiplexers = new List<RedLockMultiplexer>
            {
                existingConnectionMultiplexer
            };
            var redlockFactory = RedLockFactory.Create(multiplexers);

            await using (var redLock = await redlockFactory.CreateLockAsync
                (_redisOptions.Resource, _redisOptions.Expiry, _redisOptions.Wait, _redisOptions.Retry))
            {
                if (redLock.IsAcquired)
                {
                    AthorPublicProfileReply? reply = null;
                    var replyString = await _distributedCache.GetStringAsync(authorId);
                    if (replyString != null)
                    {
                        reply = JsonSerializer.Deserialize<AthorPublicProfileReply>(replyString);

                        return new StandartResponse<AthorPublicProfileReply>()
                        {
                            Data = reply,
                            StatusCode = StatusCode.RedisReceive
                        };
                    }

                    return new StandartResponse<AthorPublicProfileReply>()
                    {
                        StatusCode = StatusCode.RedisEmpty
                    };
                }

                return new StandartResponse<AthorPublicProfileReply>()
                {
                    StatusCode = StatusCode.RedisLock
                };
            }
        }
    }
}
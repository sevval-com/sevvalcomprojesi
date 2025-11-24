using Sevval.Application.Interfaces.RedisCache;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sevval.Application.Behaviours
{
    public class RedisCacheBehaviour<TRequest, TResponse>(IRedisCacheService redisCacheService)
    : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IRedisCacheService redisCacheService = redisCacheService;

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (request is ICacheableQuery query)
            {
                var cachekey = query.CacheKey;
                var cacheTime = query.CacheTime;
                var cacheData = await redisCacheService.GetAsync<TResponse>(cachekey);

                if (cacheData != null) { return cacheData; }

                var response = await next();

                if (response is not null)
                    await redisCacheService.SetAsync(cachekey, response, DateTime.Now.AddMinutes(cacheTime));

                return response;
            }
            else if (request is ICacheRefreshCommand command)
            {
                var cachekey = command.CacheKey;
                var keys = redisCacheService.GetKeys(cachekey);

                foreach (var item in keys)
                {
                    await redisCacheService.Clear(item);
                }
                var response = await next();
                return response;
            }

            return await next();
        }
    }
}

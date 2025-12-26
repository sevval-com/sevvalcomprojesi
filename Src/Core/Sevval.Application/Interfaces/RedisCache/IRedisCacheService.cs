using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sevval.Application.Interfaces.RedisCache
{
    public interface IRedisCacheService
    {
        Task Clear(string key);
        void ClearAll();
        Task<T> GetAsync<T>(string key);
        IList<string> GetKeys(string cacheKey);
        Task SetAsync<T>(string key, T value, DateTime? cacheExpireTime = null);
    }
}


using Sevval.Application.Interfaces.RedisCache;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sevval.Infrastructure.RedisCache
{
    public class RedisCacheService : IRedisCacheService
    {

        private readonly ConnectionMultiplexer connectionMultiplexer;
        private readonly StackExchange.Redis.IDatabase database;
        private readonly RedisCacheSettings redisCacheSettings;


        public RedisCacheService(IOptions<RedisCacheSettings> options)
        {
            redisCacheSettings = options.Value;
            var opt = ConfigurationOptions.Parse(redisCacheSettings.ConnectionString);
            connectionMultiplexer = ConnectionMultiplexer.Connect(opt);
            database = connectionMultiplexer.GetDatabase();
        }
        public async Task<T> GetAsync<T>(string key)
        {
            var value = await database.StringGetAsync(key);
            if (value.HasValue) return JsonConvert.DeserializeObject<T>(value);
            return default;
        }

        public async Task SetAsync<T>(string key, T value, DateTime? cacheExpireTime = null)
        {
            TimeSpan timeUnitExpiration = cacheExpireTime.Value - DateTime.Now;
            await database.StringSetAsync(key, JsonConvert.SerializeObject(value), timeUnitExpiration);
        }

        public async Task Clear(string key) => await database.KeyDeleteAsync(key);

        public void ClearAll()
        {
            var endpoints = connectionMultiplexer.GetEndPoints(true);
            foreach (var endpoint in endpoints)
            {
                var server = connectionMultiplexer.GetServer(endpoint);
                server.FlushAllDatabases();
            }
        }

        public IList<string> GetKeys(string cacheKey)
        {
            IList<string> keys = new List<string>();
            keys.Add($"GetAll{cacheKey}");
            keys.Add($"{cacheKey}ByPagination");
            return keys;
        }
    }

}

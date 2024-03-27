using CoreWebAPI.Common.Helper;
using SqlSugar;
using SugarRedis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreWebAPI.Common.Redis
{
    public class SqlSugarRedisCache : ICacheService
    {

        //默认:127.0.0.1:6379,password=,connectTimeout=3000,connectRetry=1,syncTimeout=10000,DefaultDatabase=0
        public static SugarRedisClient _service = new SugarRedisClient(Appsettings.App("Startup", "SqlSugar", "RedisConnectionString"));

        public void Add<V>(string key, V value)
        {
            _service.Set(key, value);
        }

        public void Add<V>(string key, V value, int cacheDurationInSeconds)
        {
            _service.Set(key, value, cacheDurationInSeconds);
        }

        public bool ContainsKey<V>(string key)
        {
            return _service.Exists(key);
        }

        public V Get<V>(string key)
        {
            return _service.Get<V>(key);
        }

        public IEnumerable<string> GetAllKey<V>()
        {
            try
            {
                return _service.SearchCacheRegex("SqlSugarDataCache.*");
            }
            catch
            {
                throw new Exception("redis连接失败，请检查SqlSugar的RedisConnectionString配置");
            }
        }

        public V GetOrCreate<V>(string cacheKey, Func<V> create, int cacheDurationInSeconds = int.MaxValue)
        {
            if (this.ContainsKey<V>(cacheKey))
            {
                var result = this.Get<V>(cacheKey);
                if (result == null)
                {
                    return create();
                }
                else
                {
                    return result;
                }
            }
            else
            {
                var result = create();
                this.Add(cacheKey, result, cacheDurationInSeconds);
                return result;
            }
        }

        public void Remove<V>(string key)
        {
            _service.Remove(key);
        }
    }
}

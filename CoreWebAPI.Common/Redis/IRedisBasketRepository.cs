using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreWebAPI.Common.Redis
{
    /// <summary>
    /// Redis缓存接口
    /// </summary>
    public interface IRedisBasketRepository
    {
        /// <summary>
        /// 获取缓存值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<string> GetValue(string key);

        /// <summary>
        /// 获取值，并序列化
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<TEntity> Get<TEntity>(string key);

        /// <summary>
        /// 新增、修改
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expirySecond"></param>
        /// <returns></returns>
        Task<bool> Set(string key, object value, double expirySecond = 3600);

        /// <summary>
        /// 新增、修改
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="cacheTime"></param>
        /// <returns></returns>
        Task Set(string key, object value, TimeSpan cacheTime);

        //判断是否存在
        Task<bool> Exist(string key);

        //移除某一个缓存值
        Task<bool> Remove(string key);

        //移除某一个缓存值
        Task<long> RemoveBegin(string key);

        //全部清除
        Task Clear();

        Task<RedisValue[]> ListRangeAsync(string redisKey);
        Task<long> ListLeftPushAsync(string redisKey, string redisValue, int db = -1);
        Task<long> ListRightPushAsync(string redisKey, string redisValue, int db = -1);
        Task<long> ListRightPushAsync(string redisKey, IEnumerable<string> redisValue, int db = -1);
        Task<T> ListLeftPopAsync<T>(string redisKey, int db = -1) where T : class;
        Task<T> ListRightPopAsync<T>(string redisKey, int db = -1) where T : class;
        Task<string> ListLeftPopAsync(string redisKey, int db = -1);
        Task<string> ListRightPopAsync(string redisKey, int db = -1);
        Task<long> ListLengthAsync(string redisKey, int db = -1);
        Task<IEnumerable<string>> ListRangeAsync(string redisKey, int db = -1);
        Task<IEnumerable<string>> ListRangeAsync(string redisKey, int start, int stop, int db = -1);
        Task<long> ListDelRangeAsync(string redisKey, string redisValue, long type = 0, int db = -1);
        Task ListClearAsync(string redisKey, int db = -1);

    }
}

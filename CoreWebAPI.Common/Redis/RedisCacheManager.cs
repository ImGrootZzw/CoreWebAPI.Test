using StackExchange.Redis;
using System;
using CoreWebAPI.Common.Helper;

namespace CoreWebAPI.Common.Redis
{
    public class RedisCacheManager : IRedisCacheManager
    {

        private readonly string redisConnenctionString;
        private readonly string redisPassword;

        public volatile ConnectionMultiplexer redisConnection;

        private readonly object redisConnectionLock = new object();

        public RedisCacheManager()
        {
            string redisConfiguration = Appsettings.App(new string[] { "RedisAOP", "ConnectionString" });//获取连接字符串
            this.redisPassword = Appsettings.App(new string[] { "RedisAOP", "Password" });//获取连接密码

            if (string.IsNullOrWhiteSpace(redisConfiguration))
            {
                throw new ArgumentException("redis config is empty", nameof(redisConfiguration));
            }
            this.redisConnenctionString = redisConfiguration;
            this.redisConnection = GetRedisConnection();
        }

        /// <summary>
        /// 核心代码，获取连接实例
        /// 通过双if 夹lock的方式，实现单例模式
        /// </summary>
        /// <returns></returns>
        private ConnectionMultiplexer GetRedisConnection()
        {
            //如果已经连接实例，直接返回
            if (this.redisConnection != null && this.redisConnection.IsConnected)
            {
                return this.redisConnection;
            }
            //加锁，防止异步编程中，出现单例无效的问题
            lock (redisConnectionLock)
            {
                if (this.redisConnection != null)
                {
                    //释放redis连接
                    this.redisConnection.Dispose();
                }
                try
                {
                    var config = new ConfigurationOptions
                    {
                        AbortOnConnectFail = false,
                        AllowAdmin = true,
                        ConnectTimeout = 15000,//改成15s
                        SyncTimeout = 5000,
                        Password = redisPassword,//Redis数据库密码
                        EndPoints = { redisConnenctionString }// connectionString 为IP:Port 如”192.168.2.110:6379”
                    };
                    this.redisConnection = ConnectionMultiplexer.Connect(config);
                }
                catch (Exception)
                {
                    throw new Exception("Redis服务未启用，请开启该服务。");
                }
            }
            return this.redisConnection;
        }

        /// <summary>
        /// 判断是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Exist(string key)
        {
            return redisConnection.GetDatabase().KeyExists(key);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string Get(string key)
        {
            return redisConnection.GetDatabase().StringGet(key);
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public TEntity Get<TEntity>(string key)
        {
            var value = redisConnection.GetDatabase().StringGet(key);
            if (value.HasValue)
            {
                //需要用的反序列化，将Redis存储的Byte[]，进行反序列化
                return JsonHelper.Deserialize<TEntity>(value);
            }
            else
            {
                return default;
            }
        }

        /// <summary>
        /// 增加/修改
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expirySecond"></param>
        /// <returns></returns>
        public bool Set(string key, object value, double expirySecond = 3600)
        {
            if (value != null)
            {
                return redisConnection.GetDatabase().StringSet(key, JsonHelper.Serialize(value), TimeSpan.FromSeconds(expirySecond));
            }
            return false;
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            redisConnection.GetDatabase().KeyDelete(key);
        }

        /// <summary>
        /// 移除key以指定字符串开头的所有缓存
        /// </summary>
        /// <param name="key"></param>
        public long RemoveByBegin(string key)
        {
            var redisResult = redisConnection.GetDatabase().ScriptEvaluate(LuaScript.Prepare(
                //Redis的keys模糊查询：
                " local res = redis.call('KEYS', @keypattern) " +
                " return res "), new { @keypattern = key + "*" });
            if (!redisResult.IsNull)
                return redisConnection.GetDatabase().KeyDelete((RedisKey[])redisResult);  //删除一组key
            return 0;
        }

        /// <summary>
        /// 移除key以指定字符串结尾的所有缓存
        /// </summary>
        /// <param name="key"></param>
        public long RemoveByEnd(string key)
        {
            var redisResult = redisConnection.GetDatabase().ScriptEvaluate(LuaScript.Prepare(
                //Redis的keys模糊查询：
                " local res = redis.call('KEYS', @keypattern) " +
                " return res "), new { @keypattern = "*" + key });
            if (!redisResult.IsNull)
                return redisConnection.GetDatabase().KeyDelete((RedisKey[])redisResult);  //删除一组key
            return 0;
        }

        /// <summary>
        /// 移除包含以指定字符串的所有缓存
        /// </summary>
        /// <param name="key"></param>
        public long RemoveByContains(string key)
        {
            var redisResult = redisConnection.GetDatabase().ScriptEvaluate(LuaScript.Prepare(
                //Redis的keys模糊查询：
                " local res = redis.call('KEYS', @keypattern) " +
                " return res "), new { @keypattern = "*" + key + "*" });
            if (!redisResult.IsNull)
                return redisConnection.GetDatabase().KeyDelete((RedisKey[])redisResult);  //删除一组key
            return 0;
        }

        /// <summary>
        /// 清除
        /// </summary>
        public void Clear()
        {
            foreach (var endPoint in this.GetRedisConnection().GetEndPoints())
            {
                var server = this.GetRedisConnection().GetServer(endPoint);
                foreach (var key in server.Keys())
                {
                    redisConnection.GetDatabase().KeyDelete(key);
                }
            }
        }

    }
}

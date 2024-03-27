using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreWebAPI.Common.Redis
{
    /// <summary>
    /// Redis缓存接口
    /// </summary>
    public interface IRedisCacheManager
    {

        //获取 Reids 缓存值
        string Get(string key);

        //获取值，并序列化
        TEntity Get<TEntity>(string key);

        //保存
        bool Set(string key, object value, double expirySecond = 3600);

        //判断是否存在
        bool Exist(string key);

        /// <summary>
        /// 移除某一个缓存值
        /// </summary>
        /// <param name="key"></param>
        void Remove(string key);

        /// <summary>
        /// 移除key值以***开头的缓存
        /// </summary>
        /// <param name="key"></param>
        long RemoveByBegin(string key);

        /// <summary>
        /// 移除key值以***结尾的缓存
        /// </summary>
        /// <param name="key"></param>
        long RemoveByEnd(string key);

        /// <summary>
        /// 移除key值包含***的缓存
        /// </summary>
        /// <param name="key"></param>
        long RemoveByContains(string key);


        //全部清除
        void Clear();
    }
}

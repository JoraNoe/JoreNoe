
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace JoreNoe.Cache.Redis
{
    public class RedisManager :Register,IRedisManager
    {
        private readonly IDatabase RedisDataBase;

        public RedisManager()
        {
            this.RedisDataBase = GetDatabase();
        }

        /// <summary>
        /// 添加字符串
        /// </summary>
        /// <returns></returns>
        public bool Add(string KeyName,string Context,int Expire = 180)
        {
            var Result = this.RedisDataBase.StringSet(KeyName,Context);
            this.RedisDataBase.KeyExpire(KeyName, TimeSpan.FromSeconds(Expire));
            return Result;
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="KeyName"></param>
        /// <returns></returns>
        public bool Remove(string KeyName)
        {
            if (this.RedisDataBase.KeyExists(KeyName))
                throw new Exception("键不存在");

            return this.RedisDataBase.KeyDelete(KeyName);
        }
        /// <summary>
        /// 添加泛型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="KeyName"></param>
        /// <param name="Context"></param>
        /// <param name="Expire"></param>
        /// <returns></returns>
        public T Add<T>(string KeyName,T Context,int Expire = 180)
        {
            var Result = this.RedisDataBase.StringSet(KeyName,JsonConvert.SerializeObject(Context));
            this.RedisDataBase.KeyExpire(KeyName, TimeSpan.FromSeconds(Expire));
            if (!Result)
                throw new Exception("存储失败");

            //查询
            var TData = JsonConvert.DeserializeObject<T>(this.RedisDataBase.StringGet(KeyName));
            return TData;
        }

        /// <summary>
        /// 添加或者获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="KeyName"></param>
        /// <param name="Context"></param>
        /// <param name="Expire"></param>
        /// <returns></returns>
        public T AddOrGet<T>(string KeyName, T Context, int Expire = 180)
        {
            if (this.RedisDataBase.KeyExists(KeyName))
                return JsonConvert.DeserializeObject<T>(this.RedisDataBase.StringGet(KeyName));

            var Result = this.RedisDataBase.StringSet(KeyName, JsonConvert.SerializeObject(Context));
            this.RedisDataBase.KeyExpire(KeyName, TimeSpan.FromSeconds(Expire));
            if (!Result)
                throw new Exception("存储失败");

            //查询
            var TData = JsonConvert.DeserializeObject<T>(this.RedisDataBase.StringGet(KeyName));
            return TData;
        }

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="KeyName"></param>
        /// <returns></returns>
        public bool Exists(string KeyName)
        {
            return this.RedisDataBase.KeyExists(KeyName);
        }
    }
}


using JoreNoe.Limit;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;

namespace JoreNoe.Cache.Redis
{
    public class RedisManager : IRedisManager
    {
        private readonly IDatabase RedisDataBase;
        private readonly IJoreNoeRedisBaseService JoreNoeRedisBaseService;

        public RedisManager(IJoreNoeRedisBaseService JoreNoeRedisBaseService)
        {
            this.JoreNoeRedisBaseService = JoreNoeRedisBaseService;
            this.RedisDataBase = this.JoreNoeRedisBaseService.RedisDataBase;
        }

        /// <summary>
        /// 检查Key是否为空以及是否存在
        /// </summary>
        private void ValidateKey(string keyName)
        {
            // 是否可用
            RequireMethod.CheckMethod();
            if (string.IsNullOrEmpty(keyName))
                throw new ArgumentNullException(nameof(keyName));
        }

        /// <summary>
        /// 添加字符串
        /// </summary>
        /// <returns></returns>
        public bool Add(string KeyName, string Context, int Expire = 180)
        {
            this.ValidateKey(KeyName);
            var Result = this.RedisDataBase.StringSet(KeyName, Context);
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
            this.ValidateKey(KeyName);
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
        public T Add<T>(string KeyName, T Context, int Expire = 180)
        {
            this.ValidateKey(KeyName);
            var Result = this.RedisDataBase.StringSet(KeyName, JsonConvert.SerializeObject(Context));
            this.RedisDataBase.KeyExpire(KeyName, TimeSpan.FromSeconds(Expire));
            if (!Result)
                throw new Exception("存储失败");
            return Context;
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
            this.ValidateKey(KeyName);
            if (this.RedisDataBase.KeyExists(KeyName))
                return JsonConvert.DeserializeObject<T>(this.RedisDataBase.StringGet(KeyName));

            this.Add<T>(KeyName,Context,Expire);
            return Context;
        }

        public T AddOrGet<T>(string keyName, Func<T> contentProvider, int expire = 180)
        {
            this.ValidateKey(keyName);
            if (contentProvider == null)
                throw new ArgumentNullException(nameof(contentProvider));

            // Check if the key already exists in the Redis database
            if (this.RedisDataBase.KeyExists(keyName))
            {
                // Get the value associated with the key and deserialize it
                string value = this.RedisDataBase.StringGet(keyName);
                return JsonConvert.DeserializeObject<T>(value);
            }

            // Get the context from the content provider
            T context = contentProvider();

            // Serialize the context and store it in the Redis database with an expiration time
            string serializedContext = JsonConvert.SerializeObject(context);
            bool isStored = this.RedisDataBase.StringSet(keyName, serializedContext, TimeSpan.FromSeconds(expire));

            if (!isStored)
                throw new Exception("Failed to store the data in Redis");

            // Return the original context if storing was successful
            return context;
        }



        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="KeyName"></param>
        /// <returns></returns>
        public IList<T> Find<T>(string KeyName)
        {
            this.ValidateKey(KeyName);
            if (!this.Exists(KeyName))
                return new List<T>();

            return JsonConvert.DeserializeObject<IList<T>>(this.RedisDataBase.StringGet(KeyName));
        }

        public T Single<T>(string KeyName)
        {
            this.ValidateKey(KeyName);

            if (!this.Exists(KeyName))
                return default(T);

            return JsonConvert.DeserializeObject<T>(this.RedisDataBase.StringGet(KeyName));
        }


        public string Single(string KeyName)
        {
            this.ValidateKey(KeyName);

            if (!this.Exists(KeyName))
                return string.Empty;

            return this.RedisDataBase.StringGet(KeyName);
        }



        /// <summary>
        /// 添加或者获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="KeyName"></param>
        /// <param name="Context"></param>
        /// <param name="Expire"></param>
        /// <returns></returns>
        public IList<T> AddMulitToFolder<T>(string KeyName, IList<T> Context, string FolderName, int Expire = 180)
        {
            this.ValidateKey(KeyName);

            if (this.RedisDataBase.KeyExists(KeyName))
                return JsonConvert.DeserializeObject<IList<T>>(this.RedisDataBase.HashGet(KeyName, FolderName));

            var Result = this.RedisDataBase.HashSet(KeyName, FolderName + ":" + KeyName, JsonConvert.SerializeObject(Context));
            this.RedisDataBase.KeyExpire(KeyName, TimeSpan.FromSeconds(Expire));
            if (!Result)
                throw new Exception("存储失败");

            return Context;
        }

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="KeyName"></param>
        /// <returns></returns>
        public bool Exists(string KeyName)
        {
            this.ValidateKey(KeyName);
            return this.RedisDataBase.KeyExists(KeyName);
        }

        /// <summary>
        /// 添加或者获取
        /// </summary>
        /// <param name="KeyName"></param>
        /// <param name="Context"></param>
        /// <param name="Expire"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public string AddOrGet(string KeyName, string Context, int Expire = 180)
        {
            this.ValidateKey(KeyName);

            if (this.RedisDataBase.KeyExists(KeyName))
                return this.RedisDataBase.StringGet(KeyName);

            var Result = this.RedisDataBase.StringSet(KeyName, Context);
            this.RedisDataBase.KeyExpire(KeyName, TimeSpan.FromSeconds(Expire));
            if (!Result)
                throw new Exception("存储失败");

            return Context;
        }

        public IList<T> AddOrGet<T>(string KeyName, IList<T> Context, int Expire = 180)
        {
            this.ValidateKey(KeyName);

            if (this.RedisDataBase.KeyExists(KeyName))
            {
                var GetContext = this.RedisDataBase.StringGet(KeyName);
                if (string.IsNullOrEmpty(GetContext))
                    return default;
                return JsonConvert.DeserializeObject<IList<T>>(this.RedisDataBase.StringGet(KeyName));
            }

            var Result = this.RedisDataBase.StringSet(KeyName, JsonConvert.SerializeObject(Context));
            this.RedisDataBase.KeyExpire(KeyName, TimeSpan.FromSeconds(Expire));
            if (!Result)
                throw new Exception("存储失败");

            return Context;
        }


        /// <summary>
        /// 获取单个
        /// </summary>
        /// <param name="KeyName"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public string Get(string KeyName)
        {
            this.ValidateKey(KeyName);

            if (!this.Exists(KeyName))
                return String.Empty;

            return this.RedisDataBase.StringGet(KeyName);
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="KeyName"></param>
        /// <param name="Context"></param>
        /// <returns></returns>
        public string Update(string KeyName, string Context)
        {
            this.ValidateKey(KeyName);


            if (!this.Exists(KeyName))
                return String.Empty;

            var Delete = this.RedisDataBase.KeyDelete(KeyName);
            if (Delete)
            {
                var KeyExpire = this.RedisDataBase.KeyTimeToLive(KeyName);
                this.RedisDataBase.StringSet(KeyName, Context);
                this.RedisDataBase.KeyExpire(KeyName, KeyExpire);
            }
            return Context;
        }

        /// <summary>
        /// 修改实体类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="KeyName"></param>
        /// <param name="Context"></param>
        /// <returns></returns>
        public T Update<T>(string KeyName, T Context)
        {
            this.ValidateKey(KeyName);

            if (!this.Exists(KeyName))
                return default(T);

            var Delete = this.RedisDataBase.KeyDelete(KeyName);
            if (Delete)
            {
                var KeyExpire = this.RedisDataBase.KeyTimeToLive(KeyName);
                this.RedisDataBase.StringSet(KeyName, JsonConvert.SerializeObject(Context));
                this.RedisDataBase.KeyExpire(KeyName, KeyExpire);
            }
            return Context;
        }

        /// <summary>
        /// 修改多个
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="KeyName"></param>
        /// <param name="Contexts"></param>
        /// <returns></returns>
        public IList<T> Update<T>(string KeyName, IList<T> Contexts)
        {
            this.ValidateKey(KeyName);

            if (!this.Exists(KeyName))
                return default;

            var Delete = this.RedisDataBase.KeyDelete(KeyName);
            if (Delete)
            {
                var KeyExpire = this.RedisDataBase.KeyTimeToLive(KeyName);
                this.RedisDataBase.StringSet(KeyName, JsonConvert.SerializeObject(Contexts));
                this.RedisDataBase.KeyExpire(KeyName, KeyExpire);
            }
            return Contexts;
        }
    }
}

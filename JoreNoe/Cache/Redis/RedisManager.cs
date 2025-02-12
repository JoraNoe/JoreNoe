﻿using JoreNoe.Middleware;
using Mysqlx.Expr;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JoreNoe.Cache.Redis
{
    public class RedisManager : IRedisManager
    {
        private readonly IDatabase RedisDataBase;
        private readonly IJoreNoeRedisBaseService JoreNoeRedisBaseService;
        public static TimeSpan DefaultExpire = TimeSpan.FromSeconds(180);
        public RedisManager(IJoreNoeRedisBaseService JoreNoeRedisBaseService)
        {
            this.JoreNoeRedisBaseService = JoreNoeRedisBaseService;
            this.RedisDataBase = this.JoreNoeRedisBaseService.RedisDataBase;
        }

        /// <summary>
        /// 检查Key是否为空以及是否存在
        /// </summary>
        private string ValidateKey(string KeyName)
        {
            if (string.IsNullOrEmpty(KeyName))
                throw new ArgumentNullException(nameof(KeyName));

            if (this.JoreNoeRedisBaseService.SettingConfigs.IsEnabledFaieldProjectName)
                return string.Concat(JoreNoeRequestCommonTools.GetReferencingProjectName(), ":", KeyName);
            else
                return KeyName;
        }

        #region 同步

        /// <summary>
        /// 添加字符串
        /// </summary>
        /// <returns></returns>
        public bool Add(string KeyName, string Context, TimeSpan? Expire = null)
        {
            KeyName = this.ValidateKey(KeyName);
            Expire ??= TimeSpan.FromSeconds(180);

            var Result = this.RedisDataBase.StringSet(KeyName, Context);
            this.RedisDataBase.KeyExpire(KeyName, Expire);
            return Result;
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="KeyName"></param>
        /// <returns></returns>
        public bool Remove(string KeyName)
        {
            KeyName = this.ValidateKey(KeyName);
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
        public T Add<T>(string KeyName, T Context, TimeSpan? Expire = null)
        {
            KeyName = this.ValidateKey(KeyName);
            Expire ??= TimeSpan.FromSeconds(180);

            var Result = this.RedisDataBase.StringSet(KeyName, JsonConvert.SerializeObject(Context));
            this.RedisDataBase.KeyExpire(KeyName, Expire);
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
        public T AddOrGet<T>(string KeyName, T Context, TimeSpan? Expire = null)
        {
            KeyName = this.ValidateKey(KeyName);
            Expire ??= TimeSpan.FromSeconds(180);

            this.Add<T>(KeyName, Context, Expire);
            return Context;
        }

        public T AddOrGet<T>(string KeyName, Func<T> contentProvider, TimeSpan? Expire = null)
        {
            KeyName = this.ValidateKey(KeyName);
            if (contentProvider == null)
                throw new ArgumentNullException(nameof(contentProvider));

            Expire ??= TimeSpan.FromSeconds(180);

            // Check if the key already exists in the Redis database
            if (this.RedisDataBase.KeyExists(KeyName))
            {
                // Get the value associated with the key and deserialize it
                string value = this.RedisDataBase.StringGet(KeyName);
                return JsonConvert.DeserializeObject<T>(value);
            }

            // Get the context from the content provider
            T context = contentProvider();

            // Serialize the context and store it in the Redis database with an expiration time
            string serializedContext = JsonConvert.SerializeObject(context);
            bool isStored = this.RedisDataBase.StringSet(KeyName, serializedContext, Expire);

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
            KeyName = this.ValidateKey(KeyName);

            if (!this.Exists(KeyName))
                return new List<T>();

            return JsonConvert.DeserializeObject<IList<T>>(this.RedisDataBase.StringGet(KeyName));
        }

        public T Single<T>(string KeyName)
        {
            KeyName = this.ValidateKey(KeyName);

            if (!this.Exists(KeyName))
                return default(T);

            return JsonConvert.DeserializeObject<T>(this.RedisDataBase.StringGet(KeyName));
        }


        public string Single(string KeyName)
        {
            KeyName = this.ValidateKey(KeyName);

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
        public IList<T> AddMulitToFolder<T>(string KeyName, IList<T> Context, string FolderName, TimeSpan? Expire = null)
        {
            KeyName = this.ValidateKey(KeyName);
            Expire ??= TimeSpan.FromSeconds(180);
            if (this.RedisDataBase.KeyExists(KeyName))
                return JsonConvert.DeserializeObject<IList<T>>(this.RedisDataBase.HashGet(KeyName, FolderName));

            var Result = this.RedisDataBase.HashSet(KeyName, FolderName + ":" + KeyName, JsonConvert.SerializeObject(Context));
            this.RedisDataBase.KeyExpire(KeyName, Expire);
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
            KeyName = this.ValidateKey(KeyName);
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
        public string AddOrGet(string KeyName, string Context, TimeSpan? Expire = null)
        {
            KeyName = this.ValidateKey(KeyName);
            Expire ??= TimeSpan.FromSeconds(180);
            if (this.RedisDataBase.KeyExists(KeyName))
                return this.RedisDataBase.StringGet(KeyName);

            var Result = this.RedisDataBase.StringSet(KeyName, Context);
            this.RedisDataBase.KeyExpire(KeyName, Expire);
            if (!Result)
                throw new Exception("存储失败");

            return Context;
        }

        public IList<T> AddOrGet<T>(string KeyName, IList<T> Context, TimeSpan? Expire = null)
        {
            KeyName = this.ValidateKey(KeyName);
            Expire ??= TimeSpan.FromSeconds(180);
            if (this.RedisDataBase.KeyExists(KeyName))
            {
                var GetContext = this.RedisDataBase.StringGet(KeyName);
                if (string.IsNullOrEmpty(GetContext))
                    return default;
                return JsonConvert.DeserializeObject<IList<T>>(this.RedisDataBase.StringGet(KeyName));
            }

            var Result = this.RedisDataBase.StringSet(KeyName, JsonConvert.SerializeObject(Context));
            this.RedisDataBase.KeyExpire(KeyName, Expire);
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
            KeyName = this.ValidateKey(KeyName);

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
            KeyName = this.ValidateKey(KeyName);


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
            KeyName = this.ValidateKey(KeyName);

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
            KeyName = this.ValidateKey(KeyName);

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

        #endregion

        #region 异步

        public async Task<bool> AddAsync(string KeyName, string Context, TimeSpan? Expire = null)
        {
            KeyName = ValidateKey(KeyName);
            Expire ??= TimeSpan.FromSeconds(180);
            var result = await this.RedisDataBase.StringSetAsync(KeyName, Context);
            await this.RedisDataBase.KeyExpireAsync(KeyName, Expire);
            return result;
        }

        public async Task<bool> RemoveAsync(string KeyName)
        {
            KeyName = ValidateKey(KeyName);
            return await this.RedisDataBase.KeyDeleteAsync(KeyName);
        }

        public async Task<T> AddAsync<T>(string KeyName, T Context, TimeSpan? Expire = null)
        {
            KeyName = ValidateKey(KeyName);
            Expire ??= TimeSpan.FromSeconds(180);
            var result = await this.RedisDataBase.StringSetAsync(KeyName, JsonConvert.SerializeObject(Context));
            await this.RedisDataBase.KeyExpireAsync(KeyName, Expire);

            if (!result)
                throw new Exception("存储失败");

            return Context;
        }

        public async Task<T> AddOrGetAsync<T>(string KeyName, T Context, TimeSpan? Expire = null)
        {
            KeyName = ValidateKey(KeyName);
            if (await this.RedisDataBase.KeyExistsAsync(KeyName))
                return JsonConvert.DeserializeObject<T>(await this.RedisDataBase.StringGetAsync(KeyName));

            await AddAsync(KeyName, Context, Expire);
            return Context;
        }

        public async Task<T> AddOrGetAsync<T>(string KeyName, Func<Task<T>> contentProvider, TimeSpan? Expire = null)
        {
            KeyName = ValidateKey(KeyName);
            if (contentProvider == null)
                throw new ArgumentNullException(nameof(contentProvider));
            Expire ??= TimeSpan.FromSeconds(180);
            if (await this.RedisDataBase.KeyExistsAsync(KeyName))
            {
                string value = await this.RedisDataBase.StringGetAsync(KeyName);
                return JsonConvert.DeserializeObject<T>(value);
            }

            T context = await contentProvider();
            var result = await this.RedisDataBase.StringSetAsync(KeyName, JsonConvert.SerializeObject(context), Expire);

            if (!result)
                throw new Exception("Failed to store the data in Redis");

            return context;
        }

        public async Task<IList<T>> FindAsync<T>(string KeyName)
        {
            KeyName = ValidateKey(KeyName);
            if (!await ExistsAsync(KeyName))
                return new List<T>();

            return JsonConvert.DeserializeObject<IList<T>>(await this.RedisDataBase.StringGetAsync(KeyName));
        }

        public async Task<T> SingleAsync<T>(string KeyName)
        {
            KeyName = ValidateKey(KeyName);
            if (!await ExistsAsync(KeyName))
                return default;

            return JsonConvert.DeserializeObject<T>(await this.RedisDataBase.StringGetAsync(KeyName));
        }

        public async Task<string> SingleAsync(string KeyName)
        {
            KeyName = ValidateKey(KeyName);
            if (!await ExistsAsync(KeyName))
                return string.Empty;

            return await this.RedisDataBase.StringGetAsync(KeyName);
        }

        public async Task<IList<T>> AddMulitToFolderAsync<T>(string KeyName, IList<T> Context, string FolderName, TimeSpan? Expire = null)
        {
            KeyName = ValidateKey(KeyName);
            if (await this.RedisDataBase.KeyExistsAsync(KeyName))
                return JsonConvert.DeserializeObject<IList<T>>(await this.RedisDataBase.HashGetAsync(KeyName, FolderName));
            Expire ??= TimeSpan.FromSeconds(180);
            var result = await this.RedisDataBase.HashSetAsync(KeyName, FolderName + ":" + KeyName, JsonConvert.SerializeObject(Context));
            await this.RedisDataBase.KeyExpireAsync(KeyName, Expire);

            if (!result)
                throw new Exception("存储失败");

            return Context;
        }

        public async Task<bool> ExistsAsync(string KeyName)
        {
            KeyName = ValidateKey(KeyName);
            return await this.RedisDataBase.KeyExistsAsync(KeyName);
        }

        public async Task<string> AddOrGetAsync(string KeyName, string Context, TimeSpan? Expire = null)
        {
            KeyName = ValidateKey(KeyName);
            Expire ??= TimeSpan.FromSeconds(180);
            if (await this.RedisDataBase.KeyExistsAsync(KeyName))
                return await this.RedisDataBase.StringGetAsync(KeyName);

            var result = await this.RedisDataBase.StringSetAsync(KeyName, Context);
            await this.RedisDataBase.KeyExpireAsync(KeyName, Expire);

            if (!result)
                throw new Exception("存储失败");

            return Context;
        }

        public async Task<IList<T>> AddOrGetAsync<T>(string KeyName, IList<T> Context, TimeSpan? Expire = null)
        {
            KeyName = ValidateKey(KeyName);
            Expire ??= TimeSpan.FromSeconds(180);
            if (await this.RedisDataBase.KeyExistsAsync(KeyName))
            {
                var getContext = await this.RedisDataBase.StringGetAsync(KeyName);
                if (string.IsNullOrEmpty(getContext))
                    return default;

                return JsonConvert.DeserializeObject<IList<T>>(getContext);
            }

            var result = await this.RedisDataBase.StringSetAsync(KeyName, JsonConvert.SerializeObject(Context));
            await this.RedisDataBase.KeyExpireAsync(KeyName, Expire);

            if (!result)
                throw new Exception("存储失败");

            return Context;
        }

        public async Task<string> GetAsync(string KeyName)
        {
            KeyName = ValidateKey(KeyName);
            if (!await ExistsAsync(KeyName))
                return string.Empty;

            return await this.RedisDataBase.StringGetAsync(KeyName);
        }

        public async Task<string> UpdateAsync(string KeyName, string Context)
        {
            KeyName = ValidateKey(KeyName);
            if (!await ExistsAsync(KeyName))
                return string.Empty;

            var delete = await this.RedisDataBase.KeyDeleteAsync(KeyName);
            if (delete)
            {
                var keyExpire = await this.RedisDataBase.KeyTimeToLiveAsync(KeyName);
                await this.RedisDataBase.StringSetAsync(KeyName, Context);
                await this.RedisDataBase.KeyExpireAsync(KeyName, keyExpire);
            }
            return Context;
        }

        public async Task<T> UpdateAsync<T>(string KeyName, T Context)
        {
            KeyName = ValidateKey(KeyName);
            if (!await ExistsAsync(KeyName))
                return default;

            var delete = await this.RedisDataBase.KeyDeleteAsync(KeyName);
            if (delete)
            {
                var keyExpire = await this.RedisDataBase.KeyTimeToLiveAsync(KeyName);
                await this.RedisDataBase.StringSetAsync(KeyName, JsonConvert.SerializeObject(Context));
                await this.RedisDataBase.KeyExpireAsync(KeyName, keyExpire);
            }
            return Context;
        }

        public async Task<IList<T>> UpdateAsync<T>(string KeyName, IList<T> Contexts)
        {
            KeyName = ValidateKey(KeyName);
            if (!await ExistsAsync(KeyName))
                return default;

            var delete = await this.RedisDataBase.KeyDeleteAsync(KeyName);
            if (delete)
            {
                var keyExpire = await this.RedisDataBase.KeyTimeToLiveAsync(KeyName);
                await this.RedisDataBase.StringSetAsync(KeyName, JsonConvert.SerializeObject(Contexts));
                await this.RedisDataBase.KeyExpireAsync(KeyName, keyExpire);
            }
            return Contexts;
        }

        public async Task<bool> SetContainsAsync(string KeyName,string Vlaue)
        {
            KeyName = ValidateKey(KeyName);
            return await this.RedisDataBase.SetContainsAsync(KeyName, Vlaue);
        }

        #endregion
    }
}

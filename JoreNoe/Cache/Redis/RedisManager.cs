using JoreNoe.Middleware;
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
        private readonly string ReferenceProjectName;
        public RedisManager(IJoreNoeRedisBaseService JoreNoeRedisBaseService)
        {
            this.JoreNoeRedisBaseService = JoreNoeRedisBaseService;
            this.RedisDataBase = this.JoreNoeRedisBaseService.RedisDataBase;
            this.ReferenceProjectName = JoreNoeRequestCommonTools.GetReferencingProjectName();
        }

        /// <summary>
        /// 检查Key是否为空以及是否存在
        /// </summary>
        private string ValidateKey(string KeyName)
        {
            if (string.IsNullOrEmpty(KeyName))
                throw new ArgumentNullException(nameof(KeyName));

            return this.JoreNoeRedisBaseService.SettingConfigs.IsEnabledFaieldProjectName ?
                 string.Concat(this.ReferenceProjectName, ":", KeyName) : KeyName;
        }

        #region 同步

        /// <summary>
        /// 添加字符串
        /// </summary>
        /// <returns></returns>
        public bool Add(string KeyName, string Context, TimeSpan? Expire = null)
        {
            var AssamblyKey = this.ValidateKey(KeyName);
 
            var Result = this.RedisDataBase.StringSet(AssamblyKey, Context);
            this.RedisDataBase.KeyExpire(AssamblyKey, Expire);
            return Result;
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="KeyName"></param>
        /// <returns></returns>
        public bool Remove(string KeyName)
        {
            var AssamblyKey = this.ValidateKey(KeyName);
            return this.RedisDataBase.KeyDelete(AssamblyKey);
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
            var AssamblyKey = this.ValidateKey(KeyName);
 

            var Result = this.RedisDataBase.StringSet(AssamblyKey, JsonConvert.SerializeObject(Context));
            this.RedisDataBase.KeyExpire(AssamblyKey, Expire);
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
            var AssamblyKey = this.ValidateKey(KeyName);

            var Result = this.RedisDataBase.StringSet(AssamblyKey, JsonConvert.SerializeObject(Context));
            this.RedisDataBase.KeyExpire(AssamblyKey, Expire);
            if (!Result)
                throw new Exception("存储失败");

            return Context;
        }

        public T AddOrGet<T>(string KeyName, Func<T> contentProvider, TimeSpan? Expire = null)
        {
            var AssamblyKey = this.ValidateKey(KeyName);
            if (contentProvider == null)
                throw new ArgumentNullException(nameof(contentProvider));

 

            // Check if the key already exists in the Redis database
            if (this.RedisDataBase.KeyExists(AssamblyKey))
            {
                // Get the value associated with the key and deserialize it
                string value = this.RedisDataBase.StringGet(AssamblyKey);
                return JsonConvert.DeserializeObject<T>(value);
            }

            // Get the context from the content provider
            T context = contentProvider();

            // Serialize the context and store it in the Redis database with an expiration time
            string serializedContext = JsonConvert.SerializeObject(context);
            bool isStored = this.RedisDataBase.StringSet(AssamblyKey, serializedContext, Expire);

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
            var AssamblyKey = this.ValidateKey(KeyName);

            if (!this.Exists(AssamblyKey))
                return new List<T>();

            return JsonConvert.DeserializeObject<IList<T>>(this.RedisDataBase.StringGet(AssamblyKey));
        }

        public T Single<T>(string KeyName)
        {
            var AssamblyKey = this.ValidateKey(KeyName);

            if (!this.RedisDataBase.KeyExists(AssamblyKey))
                return default(T);

            return JsonConvert.DeserializeObject<T>(this.RedisDataBase.StringGet(AssamblyKey));
        }


        public string Single(string KeyName)
        {
            var AssamblyKey = this.ValidateKey(KeyName);

            if (!this.RedisDataBase.KeyExists(AssamblyKey))
                return string.Empty;

            return this.RedisDataBase.StringGet(AssamblyKey);
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
            var AssamblyKey = this.ValidateKey(KeyName);
 
            if (this.RedisDataBase.KeyExists(AssamblyKey))
                return JsonConvert.DeserializeObject<IList<T>>(this.RedisDataBase.HashGet(AssamblyKey, FolderName));

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
            var AssamblyKey = this.ValidateKey(KeyName);
            return this.RedisDataBase.KeyExists(AssamblyKey);
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
            var AssamblyKey = this.ValidateKey(KeyName);
 
            if (this.RedisDataBase.KeyExists(AssamblyKey))
                return this.RedisDataBase.StringGet(AssamblyKey);

            var Result = this.RedisDataBase.StringSet(AssamblyKey, Context);
            this.RedisDataBase.KeyExpire(AssamblyKey, Expire);
            if (!Result)
                throw new Exception("存储失败");

            return Context;
        }

        public IList<T> AddOrGet<T>(string KeyName, IList<T> Context, TimeSpan? Expire = null)
        {
            var AssamblyKey = this.ValidateKey(KeyName);
 
            if (this.RedisDataBase.KeyExists(AssamblyKey))
            {
                var GetContext = this.RedisDataBase.StringGet(AssamblyKey);
                if (string.IsNullOrEmpty(GetContext))
                    return default;
                return JsonConvert.DeserializeObject<IList<T>>(this.RedisDataBase.StringGet(AssamblyKey));
            }

            var Result = this.RedisDataBase.StringSet(AssamblyKey, JsonConvert.SerializeObject(Context));
            this.RedisDataBase.KeyExpire(AssamblyKey, Expire);
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
            var AssamblyKey = this.ValidateKey(KeyName);

            if (!this.RedisDataBase.KeyExists(AssamblyKey))
                return String.Empty;

            return this.RedisDataBase.StringGet(AssamblyKey);
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="KeyName"></param>
        /// <param name="Context"></param>
        /// <returns></returns>
        public string Update(string KeyName, string Context)
        {
            var AssamblyKey = this.ValidateKey(KeyName);


            if (!this.RedisDataBase.KeyExists(AssamblyKey))
                return String.Empty;
            var KeyExpire = this.RedisDataBase.KeyTimeToLive(AssamblyKey);
            var Delete = this.RedisDataBase.KeyDelete(AssamblyKey);
            if (Delete)
            {
                this.RedisDataBase.StringSet(AssamblyKey, Context);
                this.RedisDataBase.KeyExpire(AssamblyKey, KeyExpire);
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
            var AssamblyKey = this.ValidateKey(KeyName);

            if (!this.RedisDataBase.KeyExists(AssamblyKey))
                return default(T);
            var KeyExpire = this.RedisDataBase.KeyTimeToLive(AssamblyKey);
            var Delete = this.RedisDataBase.KeyDelete(AssamblyKey);
            if (Delete)
            {
                this.RedisDataBase.StringSet(AssamblyKey, JsonConvert.SerializeObject(Context));
                this.RedisDataBase.KeyExpire(AssamblyKey, KeyExpire);
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
            var AssamblyKey = this.ValidateKey(KeyName);

            if (!this.RedisDataBase.KeyExists(AssamblyKey))
                return default;
            var KeyExpire = this.RedisDataBase.KeyTimeToLive(AssamblyKey);
            var Delete = this.RedisDataBase.KeyDelete(AssamblyKey);
            if (Delete)
            {
                this.RedisDataBase.StringSet(AssamblyKey, JsonConvert.SerializeObject(Contexts));
                this.RedisDataBase.KeyExpire(AssamblyKey, KeyExpire);
            }
            return Contexts;
        }

        #endregion

        #region 异步

        public async Task<bool> AddAsync(string KeyName, string Context, TimeSpan? Expire = null)
        {
            var AssamblyKey = this.ValidateKey(KeyName);
 
            var result = await this.RedisDataBase.StringSetAsync(AssamblyKey, Context);
            await this.RedisDataBase.KeyExpireAsync(AssamblyKey, Expire);
            return result;
        }

        public async Task<bool> RemoveAsync(string KeyName)
        {
            var AssamblyKey = this.ValidateKey(KeyName);
            return await this.RedisDataBase.KeyDeleteAsync(AssamblyKey);
        }

        public async Task<T> AddAsync<T>(string KeyName, T Context, TimeSpan? Expire = null)
        {
            var AssamblyKey = this.ValidateKey(KeyName);
 
            var result = await this.RedisDataBase.StringSetAsync(AssamblyKey, JsonConvert.SerializeObject(Context));
            await this.RedisDataBase.KeyExpireAsync(AssamblyKey,Expire);

            if (!result)
                throw new Exception("存储失败");

            return Context;
        }

        public async Task<T> AddOrGetAsync<T>(string KeyName, T Context, TimeSpan? Expire = null)
        {
            var AssamblyKey = this.ValidateKey(KeyName);
            if (await this.RedisDataBase.KeyExistsAsync(AssamblyKey))
                return JsonConvert.DeserializeObject<T>(await this.RedisDataBase.StringGetAsync(AssamblyKey));

            await this.RedisDataBase.StringSetAsync(AssamblyKey, JsonConvert.SerializeObject(Context));
            await this.RedisDataBase.KeyExpireAsync(AssamblyKey, Expire);

            return Context;
        }

        public async Task<T> AddOrGetAsync<T>(string KeyName, Func<Task<T>> contentProvider, TimeSpan? Expire = null)
        {
            var AssamblyKey = this.ValidateKey(KeyName);
            if (contentProvider == null)
                throw new ArgumentNullException(nameof(contentProvider));
 
            if (await this.RedisDataBase.KeyExistsAsync(AssamblyKey))
            {
                string value = await this.RedisDataBase.StringGetAsync(AssamblyKey);
                return JsonConvert.DeserializeObject<T>(value);
            }

            T context = await contentProvider();
            var result = await this.RedisDataBase.StringSetAsync(AssamblyKey, JsonConvert.SerializeObject(context), Expire);

            if (!result)
                throw new Exception("Failed to store the data in Redis");

            return context;
        }

        public async Task<IList<T>> FindAsync<T>(string KeyName)
        {
            var AssamblyKey = this.ValidateKey(KeyName);
            if (!await this.RedisDataBase.KeyExistsAsync(AssamblyKey))
                return new List<T>();

            return JsonConvert.DeserializeObject<IList<T>>(await this.RedisDataBase.StringGetAsync(AssamblyKey));
        }

        public async Task<T> SingleAsync<T>(string KeyName)
        {
            var AssamblyKey = this.ValidateKey(KeyName);
            if (!await this.RedisDataBase.KeyExistsAsync(AssamblyKey))
                return default;

            return JsonConvert.DeserializeObject<T>(await this.RedisDataBase.StringGetAsync(AssamblyKey));
        }

        public async Task<string> SingleAsync(string KeyName)
        {
            var AssamblyKey = this.ValidateKey(KeyName);
            if (!await this.RedisDataBase.KeyExistsAsync(AssamblyKey))
                return string.Empty;

            return await this.RedisDataBase.StringGetAsync(AssamblyKey);
        }

        public async Task<IList<T>> AddMulitToFolderAsync<T>(string KeyName, IList<T> Context, string FolderName, TimeSpan? Expire = null)
        {
            var AssamblyKey = this.ValidateKey(KeyName);
            if (await this.RedisDataBase.KeyExistsAsync(AssamblyKey))
                return JsonConvert.DeserializeObject<IList<T>>(await this.RedisDataBase.HashGetAsync(AssamblyKey, FolderName));
 
            var result = await this.RedisDataBase.HashSetAsync(AssamblyKey, FolderName + ":" + AssamblyKey, JsonConvert.SerializeObject(Context));
            await this.RedisDataBase.KeyExpireAsync(AssamblyKey, Expire);

            if (!result)
                throw new Exception("存储失败");

            return Context;
        }

        public async Task<bool> ExistsAsync(string KeyName)
        {
            var AssamblyKey = this.ValidateKey(KeyName);
            return await this.RedisDataBase.KeyExistsAsync(AssamblyKey);
        }

        public async Task<string> GetAsync(string KeyName)
        {
            var AssamblyKey = this.ValidateKey(KeyName);
            if (!await this.RedisDataBase.KeyExistsAsync(AssamblyKey))
                return string.Empty;

            return await this.RedisDataBase.StringGetAsync(AssamblyKey);
        }

        public async Task<string> AddOrGetAsync(string KeyName, string Context, TimeSpan? Expire = null)
        {
            var AssamblyKey = this.ValidateKey(KeyName);
 
            if (await this.RedisDataBase.KeyExistsAsync(AssamblyKey))
                return await this.RedisDataBase.StringGetAsync(AssamblyKey);

            var result = await this.RedisDataBase.StringSetAsync(AssamblyKey, Context);
            await this.RedisDataBase.KeyExpireAsync(AssamblyKey, Expire);

            if (!result)
                throw new Exception("存储失败");

            return Context;
        }

        public async Task<IList<T>> AddOrGetAsync<T>(string KeyName, IList<T> Context, TimeSpan? Expire = null)
        {
            var AssamblyKey = this.ValidateKey(KeyName);
 
            if (await this.RedisDataBase.KeyExistsAsync(AssamblyKey))
            {
                var getContext = await this.RedisDataBase.StringGetAsync(AssamblyKey);
                if (string.IsNullOrEmpty(getContext))
                    return default;

                return JsonConvert.DeserializeObject<IList<T>>(getContext);
            }

            var result = await this.RedisDataBase.StringSetAsync(AssamblyKey, JsonConvert.SerializeObject(Context));
            await this.RedisDataBase.KeyExpireAsync(AssamblyKey, Expire);

            if (!result)
                throw new Exception("存储失败");

            return Context;
        }

       

        public async Task<string> UpdateAsync(string KeyName, string Context)
        {
            var AssamblyKey = this.ValidateKey(KeyName);
            if (!await this.RedisDataBase.KeyExistsAsync(AssamblyKey))
                return string.Empty;
            var keyExpire = await this.RedisDataBase.KeyTimeToLiveAsync(AssamblyKey);
            var delete = await this.RedisDataBase.KeyDeleteAsync(AssamblyKey);
            if (delete)
            {
                await this.RedisDataBase.StringSetAsync(AssamblyKey, Context);
                await this.RedisDataBase.KeyExpireAsync(AssamblyKey, keyExpire);
            }
            return Context;
        }

        public async Task<T> UpdateAsync<T>(string KeyName, T Context)
        {
            var AssamblyKey = this.ValidateKey(KeyName);
            if (!await this.RedisDataBase.KeyExistsAsync(AssamblyKey))
                return default;

            var keyExpire = await this.RedisDataBase.KeyTimeToLiveAsync(AssamblyKey);
            var delete = await this.RedisDataBase.KeyDeleteAsync(AssamblyKey);
            if (delete)
            {
                await this.RedisDataBase.StringSetAsync(AssamblyKey, JsonConvert.SerializeObject(Context));
                await this.RedisDataBase.KeyExpireAsync(AssamblyKey, keyExpire);
            }
            return Context;
        }

        public async Task<IList<T>> UpdateAsync<T>(string KeyName, IList<T> Contexts)
        {
            var AssamblyKey = this.ValidateKey(KeyName);
            if (!await this.RedisDataBase.KeyExistsAsync(AssamblyKey))
                return default;

            var keyExpire = await this.RedisDataBase.KeyTimeToLiveAsync(AssamblyKey);
            var delete = await this.RedisDataBase.KeyDeleteAsync(AssamblyKey);
            if (delete)
            {
                await this.RedisDataBase.StringSetAsync(AssamblyKey, JsonConvert.SerializeObject(Contexts));
                await this.RedisDataBase.KeyExpireAsync(AssamblyKey, keyExpire);
            }
            return Contexts;
        }

        public async Task<bool> SetContainsAsync(string KeyName, string Vlaue)
        {
            var AssamblyKey = this.ValidateKey(KeyName);
            return await this.RedisDataBase.SetContainsAsync(AssamblyKey, Vlaue);
        }

        public async Task<bool> SetAddAsync(string KeyName,string Value)
        {
            var AssamblyKey = this.ValidateKey(KeyName);
            var Set =  await this.RedisDataBase.SetAddAsync(AssamblyKey, Value);
            await this.RedisDataBase.KeyPersistAsync(AssamblyKey);
            return Set;
        }

        #endregion
    }
}

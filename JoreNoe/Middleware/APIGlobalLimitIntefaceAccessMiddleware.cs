using JoreNoe.Cache.Redis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace JoreNoe.Middleware
{

    public interface ILimitInteFaceAccessSetting
    {
        public string ReturnMessage { get; set; }

        public TimeSpan LocalCacheDurationInMinutes { get; set; }
    }

    public class LimitInteFaceAccessSetting : ILimitInteFaceAccessSetting
    {
        public LimitInteFaceAccessSetting(string returnMessage, TimeSpan LocalCacheDurationInMinutes)
        {
            this.ReturnMessage = returnMessage;
            this.LocalCacheDurationInMinutes = LocalCacheDurationInMinutes;
        }
        public string ReturnMessage { get; set; }
        public TimeSpan LocalCacheDurationInMinutes { get; set; }
    }

    public class APIGlobalLimitIntefaceAccessMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILimitInteFaceAccessSetting _limitInteFaceAccessSetting;
        private readonly IRedisManager _redisDb;
        private readonly IMemoryCache MemoryCache;
        public APIGlobalLimitIntefaceAccessMiddleware(RequestDelegate next, ILimitInteFaceAccessSetting Config, IJoreNoeRedisBaseService JoreNoeRedisBaseService, IMemoryCache MemoryCache, IRedisManager redis)
        {
            this._next = next;
            this._limitInteFaceAccessSetting = Config;
            this._redisDb = redis;
            this.MemoryCache = MemoryCache;
        }

        public async Task Invoke(HttpContext context, IServiceProvider serviceProvider)
        {
            var RedisKey = string.Concat(JoreNoeRequestCommonTools.RequestAPIListsName, ":", context.Request.Path);
            if (!await this.MethodPathIsExists(RedisKey, context.Request.Path))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;  // 设置403禁止状态码
                await context.Response.WriteAsync(_limitInteFaceAccessSetting.ReturnMessage);  // 返回消息
                return;  // 结束请求管道
            }
            await _next(context);
        }

        /// <summary>
        /// 查询本地缓存是否存在，不存在则查询Redis ，Redis 不存在则写入 本地和 Redis 中 
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Path"></param>
        /// <returns></returns>
        private async Task<bool> MethodPathIsExists(string Key, string Path)
        {
            if (!this.MemoryCache.TryGetValue(Key, out bool value))
            {
                if (await this._redisDb.ExistsAsync(Key).ConfigureAwait(false))
                {
                    var GetRedisValue = await this._redisDb.SingleAsync<bool>(Key).ConfigureAwait(false);
                    this.MemoryCache.Set(Key, GetRedisValue, this._limitInteFaceAccessSetting.LocalCacheDurationInMinutes);
                    value = GetRedisValue;
                }
                else
                {
                    value = true;
                    this.MemoryCache.Set(Key, true, this._limitInteFaceAccessSetting.LocalCacheDurationInMinutes);
                    await this._redisDb.AddAsync<bool>(Key, true).ConfigureAwait(false);
                }
            }
            return value;
        }
    }

    /// <summary>
    /// 中间件扩展方法
    /// </summary>
    public static class JoreNoeAPIGlobalLimitIntefaceAccesstMiddlewareExtensions
    {
        /// <summary>
        /// 使用记录接口中间件
        /// </summary>
        /// <param name="builder">应用程序构建器</param>
        /// <returns>构建后的应用程序</returns>
        public static IApplicationBuilder UseJoreNoeIntefaceAccessMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<APIGlobalLimitIntefaceAccessMiddleware>();
        }

        /// <summary>
        /// 添加IP黑名单配置到服务容器
        /// </summary>
        /// <param name="services">服务容器</param>
        /// <param name="LocalCacheDurationInMinutes">本地缓存MemoryCache失效时间 默认不设置为30分钟失效时间</param>
        /// <param name="ReturnMessage">返回消息内容 默认不设置为 Access Denied</param>
        public static void AddJoreNoeJoreNoeIntefaceAccessMiddleware(this IServiceCollection services, TimeSpan LocalCacheDurationInMinutes = default, string ReturnMessage = "Access Denied")
        {
            services.AddSingleton<ILimitInteFaceAccessSetting>(new LimitInteFaceAccessSetting(ReturnMessage, LocalCacheDurationInMinutes == TimeSpan.Zero ? TimeSpan.FromMinutes(30) : LocalCacheDurationInMinutes));
            services.AddMemoryCache();
        }
    }
}

﻿using JoreNoe.Cache.Redis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JoreNoe.Middleware
{

    public interface ILimitInteFaceAccessSetting
    {
        public string ReturnMessage { get; set; }

        public TimeSpan LocalCacheDurationInMinutes { get; set; }

        public string ApiLassterWite { get; set; }
    }

    public class LimitInteFaceAccessSetting : ILimitInteFaceAccessSetting
    {
        public LimitInteFaceAccessSetting(string returnMessage, TimeSpan LocalCacheDurationInMinutes, string ApiLassterWite = "api")
        {
            this.ReturnMessage = returnMessage;
            this.LocalCacheDurationInMinutes = LocalCacheDurationInMinutes;
            this.ApiLassterWite = ApiLassterWite;
        }
        public string ReturnMessage { get; set; }
        public TimeSpan LocalCacheDurationInMinutes { get; set; }
        public string ApiLassterWite { get; set; }
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
            Task.Run(async () => { await this.SaveSystemAllControllerMethod().ConfigureAwait(false); }).ConfigureAwait(false);

        }

        /// <summary>
        /// 存储所有方法和控制器
        /// </summary>
        /// <returns></returns>
        public async Task SaveSystemAllControllerMethod()
        {
            var GetAllRequestMethoss = JoreNoeRequestCommonTools.ApiControllerEndpoints();
            foreach (var item in GetAllRequestMethoss)
            {
                var CoontrollerName = item.ControllerName.Substring(0, item.ControllerName.Length - "Controller".Length);
                var RouteAPIAddress = string.Concat("/", CoontrollerName, "/", item.MethodName);
                var RedisKey = string.Concat(JoreNoeRequestCommonTools.RequestAPIListsName, ":", RouteAPIAddress);
                await this.MethodPathIsExists(RedisKey).ConfigureAwait(false);
            }
        }

        public async Task Invoke(HttpContext context, IServiceProvider serviceProvider)
        {
            var ActionName = context.Request.Path;
            var Path = ExtractPath(ActionName, this._limitInteFaceAccessSetting.ApiLassterWite);
            var RedisKey = string.Concat(JoreNoeRequestCommonTools.RequestAPIListsName, ":", Path);
            if (!await this.MethodPathIsExists(RedisKey))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;  // 设置403禁止状态码
                await context.Response.WriteAsync(_limitInteFaceAccessSetting.ReturnMessage);  // 返回消息
                return;  // 结束请求管道
            }
            await _next(context);
        }

        private static string ExtractPath(string url, string ApiLassterWite)
        {
            // 使用正则表达式匹配 /api/ 后面的路径部分
            Regex regex = new Regex(@$"/{ApiLassterWite}/([^/]+(?:/[^/]+)?)");
            Match match = regex.Match(url);

            if (match.Success)
            {
                // 返回匹配到的第一个捕获组
                return match.Groups[1].Value;
            }

            return null;
        }

        /// <summary>
        /// 查询本地缓存是否存在，不存在则查询Redis ，Redis 不存在则写入 本地和 Redis 中 
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Path"></param>
        /// <returns></returns>
        private async Task<bool> MethodPathIsExists(string Key)
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
        /// 将项目所有控制器存入到Redis中
        /// </summary>
        /// <param name="services">服务容器</param>
        /// <param name="LocalCacheDurationInMinutes">本地缓存MemoryCache失效时间 默认不设置为30分钟失效时间</param>
        /// <param name="ReturnMessage">返回消息内容 默认不设置为 Access Denied</param>
        public static void AddJoreNoeJoreNoeIntefaceAccessMiddleware(this IServiceCollection services, TimeSpan LocalCacheDurationInMinutes = default, string ReturnMessage = "Access Denied", string ApiLassterWite = "api")
        {
            services.AddSingleton<ILimitInteFaceAccessSetting>(new LimitInteFaceAccessSetting(ReturnMessage, LocalCacheDurationInMinutes == TimeSpan.Zero ? TimeSpan.FromMinutes(30) : LocalCacheDurationInMinutes, ApiLassterWite));
            services.AddMemoryCache();
        }
    }
}

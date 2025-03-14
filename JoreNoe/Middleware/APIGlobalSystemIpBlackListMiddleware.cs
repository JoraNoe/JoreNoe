﻿using JoreNoe.Cache.Redis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace JoreNoe.Middleware
{
    /// <summary>
    /// 限制IP访问参数配置接口
    /// </summary>
    public interface IJoreNoeSystemIpBlackListRedisSettingConfig
    {
        /// <summary>
        /// 是否启用每分钟请求次数限制
        /// </summary>
        bool IsEnabledRequestLimit { get; set; }

        /// <summary>
        /// 最大请求次数
        /// </summary>
        int MaxRequestCount { get; set; }

        /// <summary>
        /// 时间窗口长度（允许的请求次数时限）
        /// </summary>
        TimeSpan TimeSpanTime { get; set; }

        /// <summary>
        /// 本地缓存
        /// </summary>
        TimeSpan TimeSpanLocalCache { get; set; }
    }

    public class LocalCacheItem
    {
        public int Value { get; set; }                 // 记录的值
        public DateTime AbsoluteExpiration { get; set; } // 绝对过期时间
    }

    /// <summary>
    /// 限制IP访问的Redis配置实现
    /// </summary>
    public class JoreNoeSystemIpBlackListRedisSettingConfig : IJoreNoeSystemIpBlackListRedisSettingConfig
    {
        public bool IsEnabledRequestLimit { get; set; }
        public int MaxRequestCount { get; set; }
        public TimeSpan TimeSpanTime { get; set; }
        public TimeSpan TimeSpanLocalCache { get; set; }

        /// <summary>
        /// 构造函数，初始化配置参数
        /// </summary>
        /// <param name="RedisConnection">Redis连接字符串</param>
        /// <param name="MaxRequestCount">最大请求次数</param>
        /// <param name="TimeSpanTime">时间窗口长度</param>
        /// <param name="IsEnabledRequestLimit">是否启用请求限制</param>
        public JoreNoeSystemIpBlackListRedisSettingConfig(int MaxRequestCount, TimeSpan TimeSpanTime, TimeSpan TimeSpanLocalCache, bool IsEnabledRequestLimit = false)
        {
            this.IsEnabledRequestLimit = IsEnabledRequestLimit;
            this.MaxRequestCount = MaxRequestCount;
            this.TimeSpanTime = TimeSpanTime;
            this.TimeSpanLocalCache = TimeSpanLocalCache;
        }
    }


    /// <summary>
    /// 全局IP黑名单中间件
    /// </summary>
    public class APIGlobalSystemIpBlackListMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IJoreNoeSystemIpBlackListRedisSettingConfig _config;
        private readonly IRedisManager _redisDb;
        private readonly IMemoryCache MemoryCache;

        /// <summary>
        /// 构造函数，初始化中间件和Redis数据库连接
        /// </summary>
        /// <param name="next">下一个中间件</param>
        /// <param name="config">配置参数</param>
        public APIGlobalSystemIpBlackListMiddleware(RequestDelegate next, IJoreNoeSystemIpBlackListRedisSettingConfig config, IRedisManager RedisBaseService,
            IMemoryCache MemoryCache)
        {
            _next = next;
            _config = config;
            _redisDb = RedisBaseService;
            this.MemoryCache = MemoryCache;
        }

        /// <summary>
        /// 中间件主逻辑，检测IP并根据请求频率限制访问
        /// </summary>
        /// <param name="context">HTTP上下文</param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context, IServiceProvider ServiceProvider)
        {
            // 获取请求IP
            var remoteIp = JoreNoeRequestCommonTools.GetClientIpAddress(context);

            // 如果启用请求限制，检测IP的请求次数
            if (!string.IsNullOrEmpty(remoteIp))
            {
                var GetBlackListData = await IsBlackListed(remoteIp).ConfigureAwait(false);
                if (GetBlackListData)
                {
                    //黑名单中存在，直接结束管道
                    await this.EndPipeLineReturnErroMessage(context).ConfigureAwait(false);
                    return;// 结束管道、
                }

                // 是否启用限制
                if (_config.IsEnabledRequestLimit)
                {
                    var currentCount = this.AddIpCount(remoteIp);

                    // 如果请求次数超过限制，将IP加入黑名单
                    if (currentCount >= _config.MaxRequestCount)
                    {
                        // 清楚缓存数据
                        this.RemoveAddIpCountLocal(remoteIp);
                        await _redisDb.SetAddAsync(JoreNoeRequestCommonTools.ProjectBlackListsName, remoteIp).ConfigureAwait(false);  // 将IP加入黑名单
                        await this.EndPipeLineReturnErroMessage(context).ConfigureAwait(false);
                        return;//结束管道
                    }
                }
            }

            await _next(context);  // 继续执行下一个中间件
        }

        /// <summary>
        /// 结束管道并且返回错误信息
        /// </summary>
        /// <param name="Context"></param>
        /// <returns></returns>
        private async Task EndPipeLineReturnErroMessage(HttpContext Context)
        {
            var deniedMessage = await QueryDeniedMessage().ConfigureAwait(false);
            Context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await Context.Response.WriteAsync(deniedMessage).ConfigureAwait(false);
        }

        /// <summary>
        /// 查询内存中的IP是否存在
        /// </summary>
        /// <param name="remoteIp"></param>
        /// <returns></returns>
        private async Task<bool> IsBlackListed(string remoteIp)
        {
            var Key = string.Format(JoreNoeRequestCommonTools.MemoryCacheCurrentIpBlackListName, remoteIp);
            if (!this.MemoryCache.TryGetValue(Key, out bool isBlacklisted))
            {
                isBlacklisted = await _redisDb.SetContainsAsync(JoreNoeRequestCommonTools.ProjectBlackListsName, remoteIp).ConfigureAwait(false);
                if (isBlacklisted)
                {
                    MemoryCache.Set(Key, true, _config.TimeSpanLocalCache);
                }
            }
            return isBlacklisted;
        }

        private readonly object _lock = new();

        /// <summary>
        /// 添加计数
        /// </summary>
        /// <param name="remoteIp"></param>
        /// <returns></returns>
        private int AddIpCount(string remoteIp)
        {
            var cacheKey = string.Format(JoreNoeRequestCommonTools.MemoryCacheCurrentIpCountName, remoteIp);
            lock (_lock)
            {
                var isExists = this.MemoryCache.TryGetValue(cacheKey, out LocalCacheItem cacheItem);
                if (isExists)
                {
                    if (DateTime.UtcNow >= cacheItem.AbsoluteExpiration)
                    {
                        this.MemoryCache.Remove(cacheKey);
                        return 1;
                    }

                    cacheItem.Value += 1;
                    this.MemoryCache.Set(cacheKey, cacheItem);
                    return cacheItem.Value;
                }
                else
                {
                    var newItem = new LocalCacheItem
                    {
                        Value = 1,
                        AbsoluteExpiration = DateTime.UtcNow.Add(_config.TimeSpanTime)
                    };
                    this.MemoryCache.Set(cacheKey, newItem, _config.TimeSpanTime);
                    return newItem.Value;
                }
            }
        }

        /// <summary>
        /// 清楚缓存中数据
        /// </summary>
        /// <param name="remoteIp"></param>
        private void RemoveAddIpCountLocal(string remoteIp)
        {
            var cacheKey = string.Format(JoreNoeRequestCommonTools.MemoryCacheCurrentIpCountName, remoteIp);
            this.MemoryCache.Remove(cacheKey);
        }

        /// <summary>
        /// 获取拒绝访问的消息，如果Redis中不存在，则创建默认消息
        /// </summary>
        /// <returns>拒绝访问的HTML消息</returns>
        private async Task<string> QueryDeniedMessage()
        {
            if (this.MemoryCache.TryGetValue(JoreNoeRequestCommonTools.DeniedReturnMessage, out string CachedMessage))
            {
                return CachedMessage;
            }

            if (await _redisDb.ExistsAsync(JoreNoeRequestCommonTools.DeniedReturnMessage).ConfigureAwait(false))
            {
                var message = await _redisDb.GetAsync(JoreNoeRequestCommonTools.DeniedReturnMessage).ConfigureAwait(false);  // 从Redis中获取消息
                this.MemoryCache.Set(JoreNoeRequestCommonTools.DeniedReturnMessage, message, _config.TimeSpanLocalCache);
                return message;
            }
            else
            {
                var defaultMessage = JoreNoeRequestCommonTools.ReturnDeniedHTMLPage();  // 返回默认HTML拒绝消息
                await _redisDb.AddAsync(JoreNoeRequestCommonTools.DeniedReturnMessage, defaultMessage).ConfigureAwait(false);  // 存入Redis
                this.MemoryCache.Set(JoreNoeRequestCommonTools.DeniedReturnMessage, defaultMessage, _config.TimeSpanLocalCache);
                return defaultMessage;
            }
        }
    }

    /// <summary>
    /// 中间件扩展方法
    /// </summary>
    public static class JoreNoeAPIGlobalSystemIpBlackListMiddlewareExtensions
    {
        /// <summary>
        /// 使用IP黑名单中间件
        /// </summary>
        /// <param name="builder">应用程序构建器</param>
        /// <returns>构建后的应用程序</returns>
        public static IApplicationBuilder UseJoreNoeSystemIPBlackListMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<APIGlobalSystemIpBlackListMiddleware>();
        }

        /// <summary>
        /// 添加IP黑名单配置到服务容器
        /// </summary>
        /// <param name="services">服务容器</param>
        /// <param name="maxRequestCount">最大请求次数</param>
        /// <param name="spanTime">多长时间一个IP可以请求接口多少次</param>
        /// <param name="TimeSpanLocalCache">存储限制后的返回文本时间</param>
        /// <param name="isEnabledRequestLimit">是否启用请求限制</param>
        public static void AddJoreNoeSystemIPBlackListMiddleware(this IServiceCollection services, int maxRequestCount, TimeSpan spanTime, TimeSpan TimeSpanLocalCache, bool isEnabledRequestLimit = false)
        {
            services.AddSingleton<IJoreNoeSystemIpBlackListRedisSettingConfig>(new JoreNoeSystemIpBlackListRedisSettingConfig(maxRequestCount, spanTime, TimeSpanLocalCache, isEnabledRequestLimit));
            services.AddMemoryCache();
        }
    }
}

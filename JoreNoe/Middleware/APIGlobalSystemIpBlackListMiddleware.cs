using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System.Reflection;
using System.Threading.Tasks;
using System;
using JoreNoe.Cache.Redis;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;

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
    }

    /// <summary>
    /// 限制IP访问的Redis配置实现
    /// </summary>
    public class JoreNoeSystemIpBlackListRedisSettingConfig : IJoreNoeSystemIpBlackListRedisSettingConfig
    {
        public bool IsEnabledRequestLimit { get; set; }
        public int MaxRequestCount { get; set; }
        public TimeSpan TimeSpanTime { get; set; }

        /// <summary>
        /// 构造函数，初始化配置参数
        /// </summary>
        /// <param name="RedisConnection">Redis连接字符串</param>
        /// <param name="MaxRequestCount">最大请求次数</param>
        /// <param name="TimeSpanTime">时间窗口长度</param>
        /// <param name="IsEnabledRequestLimit">是否启用请求限制</param>
        public JoreNoeSystemIpBlackListRedisSettingConfig(int MaxRequestCount, TimeSpan TimeSpanTime, bool IsEnabledRequestLimit = false)
        {
            this.IsEnabledRequestLimit = IsEnabledRequestLimit;
            this.MaxRequestCount = MaxRequestCount;
            this.TimeSpanTime = TimeSpanTime;
        }
    }


    /// <summary>
    /// 全局IP黑名单中间件
    /// </summary>
    public class APIGlobalSystemIpBlackListMiddleware
    {
        // 常量
        private readonly string KeyTemplateIpCount = "{0}:IP:{1}:count";
        private readonly string KeyTemplateBlacklist = "{0}:SystemBlackIps";
        private readonly string KeyTemplateDeniedMessage = "{0}:DeniedReturnMessage";
        private readonly string MemoryCacheCurrentIpCountKey = "IP{0}Count";
        private readonly string MemoryCacheCurrentIpBlackListKey = "IP{0}Black";

        // 初始化
        private readonly RequestDelegate _next;
        private readonly IJoreNoeSystemIpBlackListRedisSettingConfig _config;
        private readonly IDatabase _redisDb;
        private readonly IMemoryCache MemoryCache;
        /// <summary>
        /// 构造函数，初始化中间件和Redis数据库连接
        /// </summary>
        /// <param name="next">下一个中间件</param>
        /// <param name="config">配置参数</param>
        public APIGlobalSystemIpBlackListMiddleware(RequestDelegate next, IJoreNoeSystemIpBlackListRedisSettingConfig config, IJoreNoeRedisBaseService RedisBaseService,
            IMemoryCache MemoryCache)
        {
            _next = next;
            _config = config;
            _redisDb = RedisBaseService.RedisDataBase;
            this.MemoryCache = MemoryCache;
        }


        private string GetRedisKey(string remoteIp) => string.Format(KeyTemplateIpCount, ProjectName, remoteIp);
        private string GetBlacklistKey => string.Format(KeyTemplateBlacklist, ProjectName);
        private string ProjectName => Assembly.GetEntryAssembly()?.GetName().Name ?? "UnknownProject";


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
                }

                // 是否启用限制
                if (_config.IsEnabledRequestLimit)
                {
                    var currentCount = this.AddIpCount(remoteIp); //await _redisDb.StringIncrementAsync(redisKey).ConfigureAwait(false);  // 增加该IP的请求次数

                    // 如果请求次数超过限制，将IP加入黑名单
                    if (currentCount >= _config.MaxRequestCount)
                    {
                        await _redisDb.SetAddAsync(this.GetBlacklistKey, remoteIp).ConfigureAwait(false);  // 将IP加入黑名单
                        await _redisDb.KeyPersistAsync(this.GetBlacklistKey).ConfigureAwait(false);
                        await this.EndPipeLineReturnErroMessage(context).ConfigureAwait(false);
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
            return;
        }

        /// <summary>
        /// 查询内存中的IP是否存在
        /// </summary>
        /// <param name="remoteIp"></param>
        /// <returns></returns>
        private async Task<bool> IsBlackListed(string remoteIp)
        {
            var Key = string.Format(MemoryCacheCurrentIpBlackListKey, remoteIp);
            if (!this.MemoryCache.TryGetValue(Key, out bool isBlacklisted))
            {
                isBlacklisted = await _redisDb.SetContainsAsync(this.GetBlacklistKey, remoteIp).ConfigureAwait(false);
                if (isBlacklisted)
                {
                    MemoryCache.Set(Key, true, TimeSpan.FromMinutes(6));
                }
            }
            return isBlacklisted;
        }

        /// <summary>
        /// 添加计数
        /// </summary>
        /// <param name="remoteIp"></param>
        /// <returns></returns>
        private int AddIpCount(string remoteIp)
        {
            var cacheKey = string.Format(MemoryCacheCurrentIpCountKey, remoteIp);
            var IsExists = this.MemoryCache.TryGetValue(cacheKey, out int value);
            if (IsExists)
            {
                value += 1;
                this.MemoryCache.Set(cacheKey, value);
            }
            else
            {
                value = 1;
                this.MemoryCache.Set(cacheKey, value, _config.TimeSpanTime);
            }
            return value;
        }

        /// <summary>
        /// 获取拒绝访问的消息，如果Redis中不存在，则创建默认消息
        /// </summary>
        /// <returns>拒绝访问的HTML消息</returns>
        private async Task<string> QueryDeniedMessage()
        {
            var messageKey = $"{ProjectName}:DeniedReturnMessage";
            if (await _redisDb.KeyExistsAsync(messageKey).ConfigureAwait(false))
            {
                return await _redisDb.StringGetAsync(messageKey).ConfigureAwait(false);  // 从Redis中获取消息
            }
            else
            {
                var defaultMessage = JoreNoeRequestCommonTools.ReturnDeniedHTMLPage();  // 返回默认HTML拒绝消息
                await _redisDb.StringSetAsync(messageKey, defaultMessage).ConfigureAwait(false);  // 存入Redis
                await _redisDb.KeyPersistAsync(messageKey).ConfigureAwait(false);
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
        /// <param name="redisConnection">Redis连接字符串</param>
        /// <param name="maxRequestCount">最大请求次数</param>
        /// <param name="spanTime">时间窗口</param>
        /// <param name="isEnabledRequestLimit">是否启用请求限制</param>
        public static void AddJoreNoeSystemIPBlackListMiddleware(this IServiceCollection services, int maxRequestCount, TimeSpan spanTime, bool isEnabledRequestLimit = false)
        {
            services.AddSingleton<IJoreNoeSystemIpBlackListRedisSettingConfig>(new JoreNoeSystemIpBlackListRedisSettingConfig(maxRequestCount, spanTime, isEnabledRequestLimit));
            services.AddMemoryCache();
        }
    }
}

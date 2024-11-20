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
        public APIGlobalSystemIpBlackListMiddleware(RequestDelegate next, IJoreNoeSystemIpBlackListRedisSettingConfig config,IJoreNoeRedisBaseService RedisBaseService,
            IMemoryCache MemoryCache)
        {
            _next = next;
            _config = config;
            _redisDb = RedisBaseService.RedisDataBase;
            this.MemoryCache = MemoryCache;
        }


        private string GetRedisKey(string remoteIp) => string.Format(KeyTemplateIpCount, ProjectName, remoteIp);
        private string GetBlacklistKey() => string.Format(KeyTemplateBlacklist, ProjectName);
        private string ProjectName => Assembly.GetEntryAssembly()?.GetName().Name ?? "UnknownProject";

        /// <summary>
        /// 中间件主逻辑，检测IP并根据请求频率限制访问
        /// </summary>
        /// <param name="context">HTTP上下文</param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context,IServiceProvider ServiceProvider)
        {
            // 获取请求IP
            var remoteIp = JoreNoeRequestCommonTools.GetClientIpAddress(context); 

            // 如果IP已被拉黑，拒绝访问
            if (!string.IsNullOrEmpty(remoteIp) && await IsBlackListed(remoteIp))
            {
                var deniedMessage = await QueryDeniedMessage();  // 获取拒绝访问消息
                context.Response.StatusCode = StatusCodes.Status403Forbidden;  // 设置403禁止状态码
                await context.Response.WriteAsync(deniedMessage);  // 返回消息
                return;  // 结束请求管道
            }

            // 如果启用请求限制，检测IP的请求次数
            if (!string.IsNullOrEmpty(remoteIp) && _config.IsEnabledRequestLimit)
            {
                var redisKey = GetRedisKey(remoteIp);  // 生成用于存储请求次数的Redis键
                var currentCount = await _redisDb.StringIncrementAsync(redisKey);  // 增加该IP的请求次数

                // 首次访问时，设置该IP请求次数的过期时间
                if (currentCount == 1)
                    await _redisDb.KeyExpireAsync(redisKey, _config.TimeSpanTime);

                // 如果请求次数超过限制，将IP加入黑名单
                if (currentCount > _config.MaxRequestCount)
                {
                    await _redisDb.SetAddAsync(GetBlacklistKey(), remoteIp);  // 将IP加入黑名单
                    await _redisDb.KeyPersistAsync(GetBlacklistKey());
                    var deniedMessage = await QueryDeniedMessage();
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync(deniedMessage);
                    return;
                }
            }

            await _next(context);  // 继续执行下一个中间件
        }

        /// <summary>
        /// 查询内存中的IP是否存在
        /// 不存在则写入IP
        /// </summary>
        /// <param name="remoteIp"></param>
        /// <returns></returns>
        private async Task<bool> IsBlackListed(string remoteIp)
        {
            if (this.MemoryCache.TryGetValue(remoteIp, out _))
            {
                return true;
            }

            var isBlacklisted = await _redisDb.SetContainsAsync(GetBlacklistKey(), remoteIp);
            if (isBlacklisted)
            {
                MemoryCache.Set(remoteIp, true, _config.TimeSpanTime);
            }

            return isBlacklisted;
        }

        /// <summary>
        /// 获取拒绝访问的消息，如果Redis中不存在，则创建默认消息
        /// </summary>
        /// <returns>拒绝访问的HTML消息</returns>
        private async Task<string> QueryDeniedMessage()
        {
            var messageKey = $"{ProjectName}:DeniedReturnMessage";
            if (await _redisDb.KeyExistsAsync(messageKey))
            {
                return await _redisDb.StringGetAsync(messageKey);  // 从Redis中获取消息
            }
            else
            {
                var defaultMessage = JoreNoeRequestCommonTools.ReturnDeniedHTMLPage();  // 返回默认HTML拒绝消息
                await _redisDb.StringSetAsync(messageKey, defaultMessage);  // 存入Redis
                await _redisDb.KeyPersistAsync(messageKey);
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
        public static void AddJoreNoeSystemIPBlackListMiddleware(this IServiceCollection services,int maxRequestCount, TimeSpan spanTime, bool isEnabledRequestLimit = false)
        {
            services.AddSingleton<IJoreNoeSystemIpBlackListRedisSettingConfig>(new JoreNoeSystemIpBlackListRedisSettingConfig(maxRequestCount, spanTime, isEnabledRequestLimit));
            services.AddMemoryCache();
        }
    }
}

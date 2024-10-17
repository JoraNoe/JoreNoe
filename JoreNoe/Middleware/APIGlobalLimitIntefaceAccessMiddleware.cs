using JoreNoe.Cache.Redis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoreNoe.Middleware
{

    public interface ILimitInteFaceAccessSetting
    {
        public string ReturnMessage { get; set; }
    }

    public class LimitInteFaceAccessSetting : ILimitInteFaceAccessSetting
    {
        public LimitInteFaceAccessSetting(string returnMessage)
        {
            this.ReturnMessage = returnMessage;
        }
        public string ReturnMessage { get; set; }
    }

    public class APIGlobalLimitIntefaceAccessMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILimitInteFaceAccessSetting _limitInteFaceAccessSetting;
        public APIGlobalLimitIntefaceAccessMiddleware(RequestDelegate next, ILimitInteFaceAccessSetting Config)
        {
            _next = next;
            _limitInteFaceAccessSetting = Config;
        }

        public async Task Invoke(HttpContext context, IServiceProvider serviceProvider)
        {
            var JoreNoeRedisBase = serviceProvider.GetRequiredService<IJoreNoeRedisBaseService>();
            var _redisDb = JoreNoeRedisBase.RedisDataBase;

            var Key = JoreNoeRequestCommonTools.GetReferencingProjectName() + ":RequestPathLists:" + context.Request.Path;
            if (await _redisDb.KeyExistsAsync(Key).ConfigureAwait(false))
            {
                var Single = await _redisDb.StringGetAsync(Key).ConfigureAwait(false);
                if (Single == false)
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;  // 设置403禁止状态码
                    await context.Response.WriteAsync(_limitInteFaceAccessSetting.ReturnMessage);  // 返回消息
                    return;  // 结束请求管道
                }
            }
            else
            {
                await _redisDb.StringSetAsync(Key, true);
                await _redisDb.KeyPersistAsync(Key);
            }
            await _next(context);
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
        /// <param name="redisConnection">Redis连接字符串</param>
        /// <param name="maxRequestCount">最大请求次数</param>
        /// <param name="spanTime">时间窗口</param>
        /// <param name="isEnabledRequestLimit">是否启用请求限制</param>
        public static void AddJoreNoeJoreNoeIntefaceAccessMiddleware(this IServiceCollection services, string ReturnMessage = "Access Denied")
        {
            services.AddSingleton<ILimitInteFaceAccessSetting>(new LimitInteFaceAccessSetting(ReturnMessage));
        }
    }
}

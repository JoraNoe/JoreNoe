using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace JoreNoe.Middleware
{
    public interface IJorenoeRuningRequestLogging
    {
        /// <summary>
        /// 返回信息
        /// </summary>
        /// <returns></returns>
        Task RunningRequestLogging(JorenoeRuningRequestLoggingModel Context);
    }

    /// <summary>
    /// 运行日志获取内容
    /// </summary>
    public class JorenoeRuningRequestLoggingModel
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 请求方法
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// 请求路径
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Get请求参数
        /// </summary>
        public QueryString QueryString { get; set; }

        /// <summary>
        /// 请求URI 方案 HTTPS HTTP
        /// </summary>
        public string Scheme { get; set; }

        /// <summary>
        /// 主机
        /// </summary>
        public string Hsot { get; set; }

        /// <summary>
        /// 完整路径
        /// </summary>
        public string FullPathUrl { get; set; }

        /// <summary>
        /// 请求头集合
        /// </summary>
        public string Headers { get; set; }

        /// <summary>
        /// Body请求参数
        /// </summary>
        public string RequestBody { get; set; }

        /// <summary>
        /// 时长
        /// </summary>
        public TimeSpan Duration { get; set; }
    }

    /// <summary>
    /// 回调逻辑
    /// </summary>
    public class APIGlobalRuningRequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Action<JorenoeRuningRequestLoggingModel> _callback;
        public APIGlobalRuningRequestLoggingMiddleware(RequestDelegate next, Action<JorenoeRuningRequestLoggingModel> callback)
        {
            _next = next;
            _callback = callback;
        }

        public async Task Invoke(HttpContext context)
        {
            await _next(context);

            var startTime = DateTime.UtcNow;
            var request = context.Request;
            var method = request.Method;
            var path = request.Path;
            var queryString = request.QueryString;
            var requestBody = await JoreNoeRequestCommonTools.GetRequestBodyAsync(request);
            var endTime = DateTime.UtcNow;
            var duration = endTime - startTime;


            var Entity = new JorenoeRuningRequestLoggingModel
            {
                StartTime = startTime,
                Method = method,
                Path = path,
                QueryString = queryString,
                RequestBody = requestBody,
                Duration = duration,
                Headers = JsonConvert.SerializeObject(request.Headers),
                Hsot = request.Host.ToString(),
                Scheme = request.Scheme,
                FullPathUrl = $"{request.Scheme}://{request.Host}{path}{queryString}"
            };
           
            // 回调 
            _callback(Entity);
        }
    }

    /// <summary>
    /// 接口集成逻辑
    /// </summary>
    /// <typeparam name="Entity"></typeparam>
    public class APIGlobalInefaceRuningRequestLoggingMiddleware<Entity>
        where Entity : class, IJorenoeRuningRequestLogging
    {
        private readonly RequestDelegate _next;
        private readonly Entity _entity;
        public APIGlobalInefaceRuningRequestLoggingMiddleware(RequestDelegate next, Entity entity)
        {
            _next = next;
            _entity = entity;
        }

        public async Task Invoke(HttpContext context)
        {
            await _next(context);

            var startTime = DateTime.UtcNow;
            var request = context.Request;
            var method = request.Method;
            var path = request.Path;
            var queryString = request.QueryString;
            var requestBody = await JoreNoeRequestCommonTools.GetRequestBodyAsync(request);
            var endTime = DateTime.UtcNow;
            var duration = endTime - startTime;
            var EntityData = new JorenoeRuningRequestLoggingModel
            {
                StartTime = startTime,
                Method = method,
                Path = path,
                QueryString = queryString,
                RequestBody = requestBody,
                Duration = duration,
                Headers = JsonConvert.SerializeObject(request.Headers),
                Hsot = request.Host.ToString(),
                Scheme = request.Scheme,
                FullPathUrl = $"{request.Scheme}://{request.Host}{path}{queryString}"
            };
            
            await _entity.RunningRequestLogging(EntityData).ConfigureAwait(false);
            
        }
    }



    public static class JoreNoeRequestLoggingMiddlewareExtensions
    {
        /// <summary>
        /// 直接使用的全局系统请求日志中间件
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseJoreNoeRequestLoggingMiddleware(this IApplicationBuilder builder, Action<JorenoeRuningRequestLoggingModel> callback)
        {
            return builder.UseMiddleware<APIGlobalRuningRequestLoggingMiddleware>(callback);
        }

        /// <summary>
        /// 使用全局系统请求日志中间件
        /// </summary>
        /// <param name="builder"></param>
        public static void UseJoreNoeRequestLoggingMiddleware(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<APIGlobalInefaceRuningRequestLoggingMiddleware<IJorenoeRuningRequestLogging>>();
        }

        /// <summary>
        /// 添加全局系统请求日志中间件
        /// </summary>
        /// <typeparam name="Entity">要使用得类</typeparam>
        /// <param name="Service"></param>
        public static void AddJoreNoeRequestLoggingMiddleware<Entity>(this IServiceCollection Service)
            where Entity : class, IJorenoeRuningRequestLogging
        {
            Service.AddSingleton<IJorenoeRuningRequestLogging, Entity>();
        }
    }

}

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoreNoe.Middleware
{
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
            var startTime = DateTime.UtcNow;

            var request = context.Request;
            var method = request.Method;
            var path = request.Path;
            var queryString = request.QueryString;
            var requestBody = await GetRequestBodyAsync(request);

            await _next(context);

            var endTime = DateTime.UtcNow;
            var duration = endTime - startTime;

            _callback(new JorenoeRuningRequestLoggingModel
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
            });
        }

        private async Task<string> GetRequestBodyAsync(HttpRequest request)
        {
            var body = request.Body;
            if (body.CanSeek)
            {
                body.Seek(0, SeekOrigin.Begin);
            }

            var requestBody = await new StreamReader(body).ReadToEndAsync();

            if (body.CanSeek)
            {
                body.Seek(0, SeekOrigin.Begin);
            }

            return requestBody;
        }

    }

    public static class JoreNoeRequestLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseJoreNoeRequestLoggingMiddleware(this IApplicationBuilder builder, Action<JorenoeRuningRequestLoggingModel> callback)
        {
            return builder.UseMiddleware<APIGlobalRuningRequestLoggingMiddleware>(callback);
        }
    }

}

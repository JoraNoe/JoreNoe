using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.IO;
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
        /// 响应体
        /// </summary>
        public string ResponseBody { get; set; }

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
            // 备份原始请求体
            var originalRequestBody = context.Request.Body;
            var originalResponseBodyStream = context.Response.Body;
            string RequestBody = string.Empty;
            string ResponsBody = string.Empty;
            var StartTime = DateTime.UtcNow;
            try
            {
                if(!string.IsNullOrEmpty(context.Request.ContentType) && context.Request.ContentType.ToLowerInvariant().Contains("multipart/form-data"))
                {
                    RequestBody = "Format：" + context.Request.ContentType + "，Length：" + context.Request.ContentLength;
                }
                else
                {
                    // 创建一个新的内存流来保存请求体的副本
                    using (var memStream = new MemoryStream())
                    {
                        // 复制原始请求体到内存流
                        await context.Request.Body.CopyToAsync(memStream);
                        memStream.Seek(0, SeekOrigin.Begin);

                        // 读取请求体内容作为字符串
                        RequestBody = await new StreamReader(memStream).ReadToEndAsync();

                        // 将内存流设置为请求的新请求体
                        memStream.Seek(0, SeekOrigin.Begin);
                        context.Request.Body = memStream;


                    }
                }

                if (!string.IsNullOrEmpty(context.Response.ContentType) && context.Response.ContentType.ToLowerInvariant().Contains("multipart/form-data"))
                {
                    ResponsBody = "Format：" + context.Response.ContentType + "，Length：" + context.Response.ContentLength;
                }
                else
                {
                    using (var responseBody = new MemoryStream())
                    {
                        // 替换响应体为内存流
                        context.Response.Body = responseBody;
                        // 调用下一个中间件
                        await _next(context);

                        // 读取响应体
                        responseBody.Seek(0, SeekOrigin.Begin);
                        ResponsBody = await new StreamReader(responseBody).ReadToEndAsync();
                        responseBody.Seek(0, SeekOrigin.Begin);

                        // 将响应体内容写回原始响应流
                        await responseBody.CopyToAsync(originalResponseBodyStream);
                    }
                }
            }
            finally
            {
                // 恢复原始请求体
                context.Request.Body = originalRequestBody;
                context.Response.Body = originalResponseBodyStream;
            }

           
            var request = context.Request;
            var method = request.Method;
            var path = request.Path;
            var queryString = request.QueryString;
            var requestBody = RequestBody;
            var endTime = DateTime.UtcNow;
            var duration = endTime - StartTime;


            var Entity = new JorenoeRuningRequestLoggingModel
            {
                StartTime = StartTime,
                Method = method,
                Path = path,
                QueryString = queryString,
                RequestBody = requestBody,
                ResponseBody = ResponsBody,
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
            // 备份原始请求体
            var originalRequestBody = context.Request.Body;
            var originalResponseBodyStream = context.Response.Body;
            string RequestBody = string.Empty;
            string ResponsBody = string.Empty;
            var StartTime = DateTime.UtcNow;
            try
            {
                if (!string.IsNullOrEmpty(context.Request.ContentType) && context.Request.ContentType.ToLowerInvariant().Contains("multipart/form-data"))
                {
                    RequestBody = "Format：" + context.Request.ContentType + "，Length：" + context.Request.ContentLength;
                }
                else
                {
                    // 创建一个新的内存流来保存请求体的副本
                    using (var memStream = new MemoryStream())
                    {
                        // 复制原始请求体到内存流
                        await context.Request.Body.CopyToAsync(memStream);
                        memStream.Seek(0, SeekOrigin.Begin);

                        // 读取请求体内容作为字符串
                        RequestBody = await new StreamReader(memStream).ReadToEndAsync();

                        // 将内存流设置为请求的新请求体
                        memStream.Seek(0, SeekOrigin.Begin);
                        context.Request.Body = memStream;
                    }
                }
                if (!string.IsNullOrEmpty(context.Response.ContentType) && context.Response.ContentType.ToLowerInvariant().Contains("multipart/form-data"))
                {
                    ResponsBody = "Format：" + context.Response.ContentType + "，Length：" + context.Response.ContentLength;
                }
                else
                {
                    using (var responseBody = new MemoryStream())
                    {
                        // 替换响应体为内存流
                        context.Response.Body = responseBody;
                        // 调用下一个中间件
                        await _next(context);

                        // 读取响应体
                        responseBody.Seek(0, SeekOrigin.Begin);
                        ResponsBody = await new StreamReader(responseBody).ReadToEndAsync();
                        responseBody.Seek(0, SeekOrigin.Begin);

                        // 将响应体内容写回原始响应流
                        await responseBody.CopyToAsync(originalResponseBodyStream);
                    }
                }
            }
            finally
            {
                // 恢复原始请求体
                context.Request.Body = originalRequestBody;
                context.Response.Body = originalResponseBodyStream;
            }

            // 装填数据
            var FillDataContext = this.FillData(StartTime, context,RequestBody,ResponsBody, DateTime.UtcNow - StartTime);
            await _entity.RunningRequestLogging(FillDataContext).ConfigureAwait(false);
        }

        /// <summary>
        /// 填装数据
        /// </summary>
        /// <param name="StartTime"></param>
        /// <param name="Context"></param>
        /// <param name="RequestBody"></param>
        /// <param name="ResponsBody"></param>
        /// <param name="Duration"></param>
        /// <returns></returns>
        private JorenoeRuningRequestLoggingModel FillData(DateTime StartTime,HttpContext Context,string RequestBody,string ResponsBody,TimeSpan Duration)
        {
            var EntityData = new JorenoeRuningRequestLoggingModel
            {
                StartTime = StartTime,
                Method = Context.Request.Method,
                Path = Context.Request.Path,
                QueryString = Context.Request.QueryString,
                RequestBody = RequestBody,
                ResponseBody = ResponsBody,
                Duration = Duration,
                Headers = JsonConvert.SerializeObject(Context.Request.Headers),
                Hsot = Context.Request.Host.ToString(),
                Scheme = Context.Request.Scheme,
                FullPathUrl = $"{Context.Request.Scheme}://{Context.Request.Host}{Context.Request.Path}{Context.Request.QueryString}"
            };
            return EntityData;
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

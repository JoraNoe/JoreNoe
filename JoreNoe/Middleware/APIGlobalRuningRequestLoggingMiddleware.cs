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

    public interface IJorenoeRuningRequestLoggingSettingConfiguration
    {
        public int LimitSizeMax { get; set; }
    }
    public class JorenoeRuningRequestLoggingSettingConfiguration : IJorenoeRuningRequestLoggingSettingConfiguration
    {
        public int LimitSizeMax { get; set; }
        public JorenoeRuningRequestLoggingSettingConfiguration(int LimitSizeMax = 600)
        {
            this.LimitSizeMax = LimitSizeMax;
        }
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
        public string Host { get; set; }

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
        public string Duration { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string UserAgent { get; set; }
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
            // 备份原始请求体和响应体流
            var originalRequestBody = context.Request.Body;
            var originalResponseBodyStream = context.Response.Body;
            string RequestBody = string.Empty;
            string ResponseBody = string.Empty;
            var StartTime = DateTime.UtcNow;

            try
            {
                // 读取请求体
                if (!string.IsNullOrEmpty(context.Request.ContentType) &&
                    context.Request.ContentType.ToLowerInvariant().Contains("multipart/form-data"))
                {
                    RequestBody = "Format：" + context.Request.ContentType + "，Length：" + context.Request.ContentLength;
                }
                else
                {
                    // 创建内存流并备份请求体
                    var memStream = new MemoryStream();

                    await context.Request.Body.CopyToAsync(memStream);
                    memStream.Seek(0, SeekOrigin.Begin);
                    RequestBody = await new StreamReader(memStream).ReadToEndAsync();

                    // 重要：重新设置 Request.Body 以供后续中间件读取
                    memStream.Seek(0, SeekOrigin.Begin);
                    context.Request.Body = memStream;

                    // 继续传递请求
                    await _next(context);

                }

                // 读取响应体
                var responseMemoryStream = new MemoryStream();

                context.Response.Body = responseMemoryStream;  // 替换响应体流

                // 调用下一个中间件来处理响应
                await _next(context);

                if (!string.IsNullOrEmpty(context.Response.ContentType) && context.Response.ContentType.ToLowerInvariant().Contains("multipart/form-data"))
                {
                    ResponseBody = "Format：" + context.Response.ContentType + "，Length：" + context.Response.ContentLength;
                }
                else
                {
                    // 确保响应体内容已完全写入
                    responseMemoryStream.Seek(0, SeekOrigin.Begin);
                    // 读取响应体内容
                    ResponseBody = await new StreamReader(responseMemoryStream).ReadToEndAsync();
                    // 重置流位置
                    responseMemoryStream.Seek(0, SeekOrigin.Begin);
                    // 写回原始响应流
                    await responseMemoryStream.CopyToAsync(originalResponseBodyStream);
                }

            }
            finally
            {
                // 恢复原始请求体和响应体流
                context.Request.Body = originalRequestBody;
                context.Response.Body = originalResponseBodyStream;

                // 记录请求信息
                var endTime = DateTime.UtcNow;
                var request = context.Request;
                var method = request.Method;
                var path = request.Path;
                var queryString = request.QueryString;
                var duration = (endTime - StartTime).ToString(@"hh\:mm\:ss\.fff");

                var entity = new JorenoeRuningRequestLoggingModel
                {
                    StartTime = StartTime,
                    Method = method,
                    Path = path,
                    QueryString = queryString,
                    RequestBody = RequestBody,
                    ResponseBody = ResponseBody,
                    Duration = duration,
                    Headers = JsonConvert.SerializeObject(request.Headers),
                    Host = request.Host.ToString(),
                    Scheme = request.Scheme,
                    FullPathUrl = $"{request.Scheme}://{request.Host}{path}{queryString}",
                    IpAddress = JoreNoeRequestCommonTools.GetClientIpAddress(context),
                    UserAgent = context.Request.Headers["User-Agent"]
                };

                // 执行回调
                _callback(entity);
            }
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
        private readonly IJorenoeRuningRequestLoggingSettingConfiguration Setting;
        public APIGlobalInefaceRuningRequestLoggingMiddleware(RequestDelegate next, Entity entity,IJorenoeRuningRequestLoggingSettingConfiguration Setting)
        {
            _next = next;
            _entity = entity;
            this.Setting = Setting;
        }

        public async Task Invoke(HttpContext context)
        {
            int MaxBodySize = Setting.LimitSizeMax * 1024;
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
                    var requestStream = new MemoryStream();
                    await context.Request.Body.CopyToAsync(requestStream);
                    requestStream.Seek(0, SeekOrigin.Begin);
                    if (requestStream.Length > MaxBodySize)
                    {
                        RequestBody = $"Request Body too large. Size: {requestStream.Length} bytes";
                    }
                    else
                    {
                        RequestBody = await new StreamReader(requestStream).ReadToEndAsync();
                    }
                    requestStream.Seek(0, SeekOrigin.Begin);
                    context.Request.Body = requestStream;
                }


                // 创建内存流来替代响应体
                var responseMemoryStream = new MemoryStream();
                // 替换响应体为内存流
                context.Response.Body = responseMemoryStream;
                // 调用下一个中间件
                await _next(context).ConfigureAwait(false);
                if (!string.IsNullOrEmpty(context.Response.ContentType) && context.Response.ContentType.ToLowerInvariant().Contains("multipart/form-data"))
                {
                    ResponsBody = "Format：" + context.Response.ContentType + "，Length：" + context.Response.ContentLength;
                }
                else
                {
                    // 确保响应体内容已完全写入
                    responseMemoryStream.Seek(0, SeekOrigin.Begin);
                    // 读取响应体内容
                    if (responseMemoryStream.Length > MaxBodySize)
                    {
                        ResponsBody = $"Response Body too large. Size: {responseMemoryStream.Length} bytes";
                    }
                    else
                    {
                        ResponsBody = await new StreamReader(responseMemoryStream).ReadToEndAsync();
                    }
                    // 重置流位置
                    responseMemoryStream.Seek(0, SeekOrigin.Begin);
                    // 写回原始响应流
                    await responseMemoryStream.CopyToAsync(originalResponseBodyStream);
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                // 恢复原始请求体和响应体
                context.Request.Body = originalRequestBody;
                context.Response.Body = originalResponseBodyStream;
                var duration = (DateTime.UtcNow - StartTime).ToString(@"hh\:mm\:ss\.fff");
                // 装填数据
                var FillDataContext = this.FillData(StartTime, context, RequestBody, ResponsBody, duration);
                await _entity.RunningRequestLogging(FillDataContext).ConfigureAwait(false);
            }
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
        private JorenoeRuningRequestLoggingModel FillData(DateTime StartTime, HttpContext Context, string RequestBody, string ResponsBody, string Duration)
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
                Host = Context.Request.Host.ToString(),
                Scheme = Context.Request.Scheme,
                FullPathUrl = $"{Context.Request.Scheme}://{Context.Request.Host}{Context.Request.Path}{Context.Request.QueryString}",
                IpAddress = JoreNoeRequestCommonTools.GetClientIpAddress(Context),
                UserAgent = Context.Request.Headers["User-Agent"]
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
        public static void AddJoreNoeRequestLoggingMiddleware<Entity>(this IServiceCollection Service,int SizeMaxLimit = 100)
            where Entity : class, IJorenoeRuningRequestLogging
        {
            Service.AddSingleton<IJorenoeRuningRequestLoggingSettingConfiguration>(new JorenoeRuningRequestLoggingSettingConfiguration(SizeMaxLimit));
            Service.AddSingleton<IJorenoeRuningRequestLogging, Entity>();
        }
    }

}

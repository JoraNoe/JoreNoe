using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace JoreNoe.Middleware
{
    public interface IJoreNoeGlobalErrorHandling
    {
        Task GlobalErrorHandling(Exception Ex);
    }

    public interface IJoreNoeGlobalErrorHandlingSettings
    {
        /// <summary>
        /// 是否启用控制台输出
        /// </summary>
        public bool EnableConsoleOut { get; set; }

        /// <summary>
        /// 是否启用返回消息不直接500  返回 错误信息
        /// </summary>
        public bool EnableReturnRecordErrorMessage { get; set; }
    }

    public class JoreNoeGlobalErrorHandlingSettings : IJoreNoeGlobalErrorHandlingSettings
    {
        public JoreNoeGlobalErrorHandlingSettings(bool EnableConsoleOut, bool EnableReturnRecordErrorMessage)
        {
            this.EnableConsoleOut = EnableConsoleOut;
            this.EnableReturnRecordErrorMessage = EnableReturnRecordErrorMessage;
        }
        public bool EnableConsoleOut { get; set; }
        public bool EnableReturnRecordErrorMessage { get; set; }
    }

    /// <summary>
    /// 直接使用方式
    /// </summary>
    public class JoreNoeGlobalErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Action<Exception, HttpContext> _errorHandlingAction;
        private readonly IJoreNoeGlobalErrorHandlingSettings joreNoeGlobalErrorHandlingSettings;

        public JoreNoeGlobalErrorHandlingMiddleware(RequestDelegate next,
            Action<Exception, HttpContext> errorHandlingAction,
            IJoreNoeGlobalErrorHandlingSettings joreNoeGlobalErrorHandlingSettings)
        {
            _next = next;
            _errorHandlingAction = errorHandlingAction;
            this.joreNoeGlobalErrorHandlingSettings = joreNoeGlobalErrorHandlingSettings;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                if (this.joreNoeGlobalErrorHandlingSettings.EnableConsoleOut)
                    await Console.Out.WriteLineAsync(JoreNoeRequestCommonTools.FormatError(ex));
                _errorHandlingAction(ex, context);
            }
        }
    }

    /// <summary>
    /// 接口方式
    /// </summary>
    public class JorNoeGlobalInterfaceErrorHandlingMiddleware<Entity>
        where Entity : class, IJoreNoeGlobalErrorHandling
    {
        private readonly RequestDelegate _next;
        private readonly Entity _Entity;
        private readonly IJoreNoeGlobalErrorHandlingSettings joreNoeGlobalErrorHandlingSettings;
        public JorNoeGlobalInterfaceErrorHandlingMiddleware(RequestDelegate next, Entity Entity, IJoreNoeGlobalErrorHandlingSettings joreNoeGlobalErrorHandlingSettings)
        {
            _next = next;
            _Entity = Entity;
            this.joreNoeGlobalErrorHandlingSettings = joreNoeGlobalErrorHandlingSettings;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                if (this.joreNoeGlobalErrorHandlingSettings.EnableConsoleOut)
                    await Console.Out.WriteLineAsync(JoreNoeRequestCommonTools.FormatError(ex));

                await _Entity.GlobalErrorHandling(ex).ConfigureAwait(false);

                if (this.joreNoeGlobalErrorHandlingSettings.EnableReturnRecordErrorMessage)
                {
                    var response = APIReturnInfo<string>.Error(JoreNoeRequestCommonTools.FormatError(ex), Status: StatusCodes.Status500InternalServerError); // 使用 APIReturnInfo 返回错误
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = StatusCodes.Status200OK;
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(response));

                    return;
                }
            }
        }
    }


    public static class JoreNoeGlobalErrorMiddlewareExtensions
    {
        /// <summary>
        /// 使用全局错误中间件
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="errorHandlingAction"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseJoreNoeGlobalErrorHandlingMiddleware(this IApplicationBuilder builder, Action<Exception, HttpContext> errorHandlingAction)
        {
            return builder.UseMiddleware<JoreNoeGlobalErrorHandlingMiddleware>(errorHandlingAction);
        }


        /// <summary>
        /// 接口方式添加全局错误中间件
        /// </summary>
        /// <typeparam name="Entity"></typeparam>
        /// <param name="Service"></param>
        public static void AddJoreNoeGlobalErrorHandlingMiddleware<Entity>(this IServiceCollection Service, bool EnableConsoleOut = false, bool EnableReturnRecordErrorMessage = false)
            where Entity : class, IJoreNoeGlobalErrorHandling
        {
            Service.AddSingleton<IJoreNoeGlobalErrorHandlingSettings>(new JoreNoeGlobalErrorHandlingSettings(EnableConsoleOut, EnableReturnRecordErrorMessage));
            Service.AddSingleton<IJoreNoeGlobalErrorHandling, Entity>();
        }

        /// <summary>
        /// 接口方式 使用全局错误中间件
        /// </summary>
        /// <param name="builder"></param>
        public static void UseJoreNoeGlobalErrorHandlingMiddleware(this IApplicationBuilder builder)
        {
            //Action ??= (e) => { e.EnableConsoleOut = false; };
            builder.UseMiddleware<JorNoeGlobalInterfaceErrorHandlingMiddleware<IJoreNoeGlobalErrorHandling>>();
        }

    }
}

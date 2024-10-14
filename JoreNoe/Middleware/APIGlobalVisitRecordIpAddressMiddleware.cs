using MathNet.Numerics.Statistics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Mysqlx.Crud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace JoreNoe.Middleware
{
    /// <summary>
    /// IP模型
    /// </summary>
    public class GlobalVisitRecord
    {
        public Guid Id { get; set; } // Matches CHAR(36) in SQL for UUID
        public string IpAddress { get; set; } // Matches VARCHAR(300) in SQL
        public string UserAgent { get; set; } // Matches VARCHAR(300) in SQL
        public DateTime CreateTime { get; set; } // Matches DATETIME in SQL
    }

    /// <summary>
    /// 接口
    /// </summary>
    public interface IJoreNoeAPIGlobalVisitRecordIpAddressMiddleware
    {
        Task VisitRecordIpAddress(GlobalVisitRecord Context);
    }

    /// <summary>
    /// 回调方式使用
    /// </summary>
    public class APIGlobalVisitRecordIpAddressMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Action<GlobalVisitRecord> _callback;
        public APIGlobalVisitRecordIpAddressMiddleware(RequestDelegate _next, Action<GlobalVisitRecord> _callback)
        {
            this._next = _next;
            this._callback = _callback;
        }

        /// <summary>
        /// 回调方式
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                // 获取 IP 地址和 User-Agent
                var ip = JoreNoeRequestCommonTools.GetClientIpAddress(context);
                var userAgent = context.Request.Headers["User-Agent"];

                // 记录访问信息
                _callback(new GlobalVisitRecord
                {
                    CreateTime = DateTime.Now,
                    Id = Guid.NewGuid(),
                    IpAddress = ip,
                    UserAgent = userAgent,
                });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            // 调用下一个中间件
            await _next(context);

            
        }
    }

    /// <summary>
    /// 接口方式
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class APIGlobalInefaceVisitRecordIpAddressMiddleware<TEntity>
        where TEntity : class, IJoreNoeAPIGlobalVisitRecordIpAddressMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly TEntity _entity;

        public APIGlobalInefaceVisitRecordIpAddressMiddleware(RequestDelegate next, TEntity entity)
        {
            _next = next;
            _entity = entity;
        }

        /// <summary>
        /// 处理 HTTP 请求并记录访问者的 IP 地址和 User-Agent
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                // 获取 IP 地址和 User-Agent
                var ip = JoreNoeRequestCommonTools.GetClientIpAddress(context);
                var userAgent = context.Request.Headers["User-Agent"];

                // 记录访问信息
                await _entity.VisitRecordIpAddress(new GlobalVisitRecord
                {
                    CreateTime = DateTime.Now,
                    Id = Guid.NewGuid(),
                    IpAddress = ip,
                    UserAgent = userAgent,
                });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            // 调用下一个中间件
            await _next(context);
        }
    }

    /// <summary>
    /// 扩展方法
    /// 注入方式
    /// </summary>
    public static class JoreNoeRequestGlobalVisitRecordIpAddressMiddlewareExtensions
    {
        /// <summary>
        /// 直接使用全局获取访问者的IP地址
        /// </summary>
        /// <param name="builer"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseJoreNoeRequestVisitRecordIpAddressMiddleware(this IApplicationBuilder builer, Action<GlobalVisitRecord> callback)
        {
            return builer.UseMiddleware<APIGlobalVisitRecordIpAddressMiddleware>(callback);
        }

        /// <summary>
        /// 使用全局获取访问者的IP注入方式
        /// </summary>
        /// <param name="builer"></param>
        public static void UseJoreNoeRequestVisitRecordIpAddressMiddleware(this IApplicationBuilder builer)
        {
            builer.UseMiddleware<APIGlobalInefaceVisitRecordIpAddressMiddleware<IJoreNoeAPIGlobalVisitRecordIpAddressMiddleware>>();
        }

        /// <summary>
        /// 添加全局系统获取访问者的IP地址 添加DI 容器荣 
        /// </summary>
        /// <typeparam name="Entity"></typeparam>
        /// <param name="Service"></param>
        public static void AddJoreNoeRequestVisitRecordIpAddressMiddleware<Entity>(this IServiceCollection Service)
            where Entity : class, IJoreNoeAPIGlobalVisitRecordIpAddressMiddleware
        {
            Service.AddSingleton<IJoreNoeAPIGlobalVisitRecordIpAddressMiddleware, Entity>();
        }
    }
}

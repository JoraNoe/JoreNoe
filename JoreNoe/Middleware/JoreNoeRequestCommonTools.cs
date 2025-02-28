using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JoreNoe.Middleware
{
    public static class JoreNoeRequestCommonTools
    {
        /// <summary>
        /// 请求接口Redis存储名称
        /// </summary>
        public const string RequestAPIListsName = "RequestAPILists";

        /// <summary>
        /// 系统黑名单
        /// </summary>
        public const string ProjectBlackListsName = "ProjectBlackLists";

        /// <summary>
        /// IP请求数量
        /// </summary>
        public const string MemoryCacheCurrentIpCountName = "IP{0}Count";

        /// <summary>
        /// IP黑名单
        /// </summary>
        public const string MemoryCacheCurrentIpBlackListName = "IP{0}Black";

        /// <summary>
        /// 返回缓存拒绝访问模版信息
        /// </summary>
        public const string DeniedReturnMessage = "DeniedReturnMessage";


        /// <summary>
        /// 获取请求中BODY中的参数
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<string> GetRequestBodyAsync(HttpRequest request)
        {
            // 保存原始请求体
            var originalBody = request.Body;

            try
            {
                // 读取请求体的内容作为字符串
                using (var memoryStream = new MemoryStream())
                {
                    // 复制请求体到内存流
                    await originalBody.CopyToAsync(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    // 读取内存流中的内容作为字符串
                    var requestBody = await new StreamReader(memoryStream).ReadToEndAsync();

                    // 重置内存流的位置
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    // 将内存流设置为请求的新请求体
                    request.Body = memoryStream;

                    return requestBody;
                }
            }
            finally
            {
                // 恢复原始请求体
                request.Body = originalBody;
                // 将请求体流位置重置到起始位置，以便后续中间件再次读取
                request.Body.Seek(0, SeekOrigin.Begin);
            }
        }

        /// <summary>
        /// 错误日志格式方式
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string FormatError(Exception ex)
        {
            var formattedMessage = new StringBuilder();
            formattedMessage.AppendLine($"[JorenoeGlobalErrorInfo]  报错时间：{DateTime.Now}  报错内容: {ex.Message}  详情信息如下： ");
            if (ex != null)
            {
                formattedMessage.AppendLine(ex.ToString());
            }
            return formattedMessage.ToString();
        }


        /// <summary>
        /// 获取客户端 IP 地址
        /// </summary>
        /// <param name="context"></param>
        /// <returns>客户端 IP 地址</returns>
        public static string GetClientIpAddress(HttpContext context)
        {
            var ip = context.Connection.RemoteIpAddress?.ToString();

            // 如果存在 X-Forwarded-For 头，优先使用该值
            if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedIps))
            {
                ip = forwardedIps.ToString().Split(',').FirstOrDefault()?.Trim();
            }

            // 如果 IP 为 null，默认设置为 127.0.0.1
            if (string.IsNullOrEmpty(ip) || ip == "::1")
            {
                ip = "127.0.0.1"; // 转换为 IPv4 的本地地址
            }

            return ip;
        }

        /// <summary>
        /// 返回拒绝访问的页面，也可以自定义默认使用
        /// </summary>
        /// <returns></returns>
        public static string ReturnDeniedHTMLPage()
        {
            return @"<!DOCTYPE html>
            <html lang='en'>
            <head>
                <meta charset='UTF-8'>
                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                <title>Access Denied</title>
                <style>
                    body {
                        margin: 0;
                        font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                        background-color: #1c1c1e;
                        color: #f5f5f7;
                        display: flex;
                        justify-content: center;
                        align-items: center;
                        height: 100vh;
                    }
                    .container {
                        text-align: center;
                        background-color: #2c2c2e;
                        padding: 50px;
                        border-radius: 12px;
                        box-shadow: 0 5px 15px rgba(0, 0, 0, 0.4);
                    }
                    h1 {
                        font-size: 3rem;
                        margin-bottom: 20px;
                        color: #ff453a;
                    }
                    p {
                        font-size: 1.2rem;
                        margin-bottom: 25px;
                        color: #a1a1a3;
                    }
                    .footer {
                        font-size: 0.9rem;
                        margin-top: 40px;
                        color: #6c6c70;
                    }
                </style>
            </head>
            <body>
                <div class='container'>
                    <h1>Access Denied</h1>
                    <p>You have been blacklisted and cannot continue accessing.</p>
                    <div class='footer'>
                        Developed by JoreNoe 💖
                    </div>
                </div>
            </body>
            </html>
            ";
        }

        /// <summary>
        /// 获取当前项目名称
        /// </summary>
        /// <returns>项目名称</returns>
        public static string GetReferencingProjectName()
        {
            return Assembly.GetEntryAssembly()?.GetName().Name ?? "UnknownProject";  // 获取项目名称
        }

        /// <summary>
        /// 获取程序集的名称
        /// </summary>
        /// <returns></returns>
        public static string GetAssemblyName()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            return assembly.GetName().Name;
        }

        /// <summary>
        /// 获取所有控制器，方法和路由
        /// </summary>
        /// <returns></returns>
        public static IList<ControllerEndpoints> ApiControllerEndpoints()
        {
            var ApiEndpoints = new List<ControllerEndpoints>();
            var AssemblyData = Assembly.GetEntryAssembly();
            var ControllerTypes = AssemblyData.GetTypes()
            .Where(t => t.IsSubclassOf(typeof(ControllerBase)))
            .ToList();
            foreach (var controllerType in ControllerTypes)
            {

                // 获取控制器上的所有方法（接口）
                var methods = controllerType.GetMethods()
                    .Where(m => m.DeclaringType == controllerType) // 只取本类的，避免继承父类的方法
                    .Where(m => m.IsPublic && m.GetCustomAttributes<HttpMethodAttribute>().Any()); // 只取HTTP方法

                foreach (var method in methods)
                {
                    // 获取该方法的所有 HTTP 方法特性（如 [HttpPost("sdf")]）
                    var httpMethodAttributes = method.GetCustomAttributes<HttpMethodAttribute>();
                    foreach (var httpMethod in httpMethodAttributes)
                    {
                        var SingleControllerEndpoints = new ControllerEndpoints();
                        SingleControllerEndpoints.ControllerName = controllerType.Name;
                        SingleControllerEndpoints.MethodName = method.Name;
                        SingleControllerEndpoints.HttpMethodName = httpMethod.HttpMethods.First();
                        SingleControllerEndpoints.RouteName = httpMethod.Template;
                        ApiEndpoints.Add(SingleControllerEndpoints);
                    }
                }
            }
            return ApiEndpoints;
        }
    }

    public class ControllerEndpoints
    {
        /// <summary>
        /// 控制器名称
        /// </summary>
        public string ControllerName { get; set; }

        /// <summary>
        /// 方法名称
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// 路由名称
        /// </summary>
        public string RouteName { get; set; }

        /// <summary>
        /// 请求方式GET,POST,PUT,DELTE
        /// </summary>
        public string HttpMethodName { get; set; }
    }
}

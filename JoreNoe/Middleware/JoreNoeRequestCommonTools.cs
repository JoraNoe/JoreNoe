using Microsoft.AspNetCore.Http;
using System;
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
                formattedMessage.AppendLine(ex .ToString());
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
        public static  string GetReferencingProjectName()
        {
            return Assembly.GetEntryAssembly()?.GetName().Name ?? "UnknownProject";  // 获取项目名称
        }
    }
}

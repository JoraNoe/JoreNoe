using Microsoft.AspNetCore.Http;
using System;
using System.IO;
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
            formattedMessage.AppendLine($"[JorenoeGlobalErrorInfo] ,报错时间：{DateTime.Now},报错内容: {ex.Message}, 详情信息如下： ");
            if (ex != null)
            {
                formattedMessage.AppendLine(ex.ToString());
            }
            return formattedMessage.ToString();
        }

    }
}

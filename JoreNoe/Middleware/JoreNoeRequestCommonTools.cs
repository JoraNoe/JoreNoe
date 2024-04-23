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

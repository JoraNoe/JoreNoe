using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace JoreNoe.JoreHttpClient
{
    public class HttpClientApi
    {
        private static readonly HttpClient client = new HttpClient();

        /// <summary>
        /// 发送Post请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="content">参数内容</param>
        /// <param name="contentType">内容类型</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="HttpRequestException"></exception>
        public static async Task<string> PostAsync(string url, string content, string contentType = "application/x-www-form-urlencoded")
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException(nameof(url));

            var requestContent = new StringContent(content, Encoding.UTF8, contentType);

            using var response = await client.PostAsync(url, requestContent);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                throw new HttpRequestException($"HTTP request failed with status code {(int)response.StatusCode}: {response.ReasonPhrase}");
            }
        }

        /// <summary>
        /// 发送Get 请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="charset">编码格式</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="HttpRequestException"></exception>
        public static async Task<string> GetAsync(string url, string charset="UTF-8")
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException(nameof(url));

            using var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                using var reader = new StreamReader(responseStream, Encoding.GetEncoding(charset));
                return await reader.ReadToEndAsync();
            }
            else
            {
                throw new HttpRequestException($"HTTP request failed with status code {(int)response.StatusCode}: {response.ReasonPhrase}");
            }
        }
    }
}

using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using JoreNoe.Limit;
using Microsoft.Extensions.DependencyInjection;

namespace JoreNoe.JoreHttpClient
{
    public class HttpClientApi
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HttpClientApi(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private HttpClient CreateClient()
        {
            return _httpClientFactory.CreateClient("ignoreSslClient");
        }

        /// <summary>
        /// 发送Post请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="content">参数内容</param>
        /// <param name="contentType">内容类型</param>
        /// <returns></returns>
        public async Task<string> PostAsync(string url, string content, string contentType = "application/json")
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException(nameof(url));

            var client = CreateClient();
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
        /// 发送Get请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="charset">编码格式</param>
        /// <returns></returns>
        public async Task<string> GetAsync(string url, string charset = "UTF-8")
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException(nameof(url));

            var client = CreateClient();

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

        /// <summary>
        /// 发送Put请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="content">参数内容</param>
        /// <param name="contentType">内容类型</param>
        /// <returns></returns>
        public async Task<string> PutAsync(string url, string content, string contentType = "application/json")
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException(nameof(url));

            var client = CreateClient();
            var requestContent = new StringContent(content, Encoding.UTF8, contentType);

            using var response = await client.PutAsync(url, requestContent);

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
        /// 发送Delete请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <returns></returns>
        public async Task<string> DeleteAsync(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException(nameof(url));

            var client = CreateClient();

            using var response = await client.DeleteAsync(url);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                throw new HttpRequestException($"HTTP request failed with status code {(int)response.StatusCode}: {response.ReasonPhrase}");
            }
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHttpClientApi(this IServiceCollection services)
        {
            services.AddHttpClient("ignoreSslClient")
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
                });

            services.AddSingleton<HttpClientApi>();
            return services;
        }
    }
}

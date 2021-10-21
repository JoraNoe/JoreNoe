using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace JoreNoe.JoreHttpClient
{
    public class HttpClientApi
    {
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) => true;

        /// <summary>
        /// http请求 post 同步
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="Parament"></param>
        /// <returns></returns>
        public static string PostSync(string Url, string Parament)
        {
            if (string.IsNullOrEmpty(Url))
                throw new ArgumentNullException(nameof(Url));

            var httpClientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true
            };

            byte[] result = null;
            using (HttpClient http = new HttpClient(httpClientHandler))
            {
                //http.DefaultRequestHeaders.Add("User-Agent", @"Mozilla/5.0 (compatible; Baiduspider/2.0; +http://www.baidu.com/search/spider.html)");
                http.DefaultRequestHeaders.Add("Accept", @"text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
                HttpResponseMessage message = null;
                var Byte = Encoding.UTF8.GetBytes(Parament);
                using (Stream dataStream = new MemoryStream(Byte))
                {
                    using (HttpContent content = new StreamContent(dataStream))
                    {
                        content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                        ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);//验证服务器证书回调自动验证
                        message = http.PostAsync(Url, content).Result;
                    }
                }
                if (message != null && message.StatusCode == HttpStatusCode.OK)
                {
                    using (message)
                    {
                        using (Stream responseStream = message.Content.ReadAsStreamAsync().Result)
                        {
                            if (responseStream != null)
                            {
                                byte[] responseData = new byte[responseStream.Length];
                                responseStream.Read(responseData, 0, responseData.Length);
                                result = responseData;
                            }
                        }
                    }
                }
            }
            return Encoding.UTF8.GetString(result);
        }

        /// <summary>
        /// http请求 post 异步
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="Parament"></param>
        /// <returns></returns>
        public async static Task<string> PostAsync(string Url, string Parament)
        {
            if (string.IsNullOrEmpty(Url))
                throw new ArgumentNullException(nameof(Url));


            var httpClientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true
            };

            byte[] result = null;
            using (HttpClient http = new HttpClient(httpClientHandler))
            {
                //http.DefaultRequestHeaders.Add("User-Agent", @"Mozilla/5.0 (compatible; Baiduspider/2.0; +http://www.baidu.com/search/spider.html)");
                http.DefaultRequestHeaders.Add("Accept", @"text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
                HttpResponseMessage message = null;
                var Byte = Encoding.UTF8.GetBytes(Parament);
                using (Stream dataStream = new MemoryStream(Byte))
                {
                    using (HttpContent content = new StreamContent(dataStream))
                    {
                        content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                        ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);//验证服务器证书回调自动验证
                        message = await http.PostAsync(Url, content);
                    }
                }
                if (message != null && message.StatusCode == HttpStatusCode.OK)
                {
                    using (message)
                    {
                        using (Stream responseStream = await message.Content.ReadAsStreamAsync())
                        {
                            if (responseStream != null)
                            {
                                byte[] responseData = new byte[responseStream.Length];
                                responseStream.Read(responseData, 0, responseData.Length);
                                result = responseData;
                            }
                        }
                    }
                }
            }
            return Encoding.UTF8.GetString(result);
        }
    }
}

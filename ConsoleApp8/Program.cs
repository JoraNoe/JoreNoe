using System.Security.Cryptography;
using System.Text;

namespace ConsoleApp8
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // 创建 HttpClient 实例
            using (var client = new HttpClient())
            {
                // 设置目标 URL
                var url = "https://manage.allinone.ouc-online.com.cn/api/Notice/PageBulletin?PageNum=1&PageSize=10&TitleSerch=";  // 替换为目标 URL

                // 修改 User-Agent
                client.DefaultRequestHeaders.UserAgent.ParseAdd("1234/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");  // 更改 User-Agent
                client.DefaultRequestHeaders.Referrer = new Uri("https://555-23-123--23-123-1--23.example.com");  // 更改 Referer
                client.DefaultRequestHeaders.Add("X-Forwarded-For", "6.3.55.123,123.123.123.123,123:0:0:0");  // 添加 X-Forwarded-For (可以模拟 IP)
                client.DefaultRequestHeaders.Add("X-Real-IP", "6.3.55.123");  // 添加 X-Forwarded-For (可以模拟 IP)
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "eyJhbGciOiJSUzI1NiIsImtpZCI6IjQ4OTM1MEE0NjE4QTUxQ0EzMUVDOUYwODI4RjIzM0YwQTg2QzBDNUFSUzI1NiIsInR5cCI6ImF0K2p3dCIsIng1dCI6IlNKTlFwR0dLVWNveDdKOElLUEl6OEtoc0RGbyJ9.eyJuYmYiOjE3MzM3MTQ2NTQsImV4cCI6MTczMzcxODI1NCwiaXNzIjoiaHR0cDovL3Bhc3Nwb3J0LmFsbGlub25lLm91Yy1vbmxpbmUuY29tLmNuIiwiYXVkIjpbIklkZW50aXR5U2VydmVyQXBpIiwicGFzc3BvcnRtYW5hZ2VhcGkiLCJwYXNzcG9ydGFwaSIsImFsbGlub25lbWFuYWdlYXBpIiwiaHR0cDovL3Bhc3Nwb3J0LmFsbGlub25lLm91Yy1vbmxpbmUuY29tLmNuL3Jlc291cmNlcyJdLCJjbGllbnRfaWQiOiJBbGxJbk9uZU1hbmFnZSIsInN1YiI6IjgwMjAyMDAiLCJhdXRoX3RpbWUiOjE3MzM3MDk5MDksImlkcCI6ImxvY2FsIiwiVXNlcklkIjoiMDhkOGY5NmYtMzg2MC00MTc0LTgyMWMtMjgyMGU5YmZkNDdiIiwiVXNlck5hbWUiOiI4MDIwMjAwIiwiTmlja25hbWUiOiLmnKrloavlhpkiLCJBdmF0YXIiOiJodHRwOi8vYWxsaW5vbmVzeS5vYnMuY24tbm9ydGgtNC5teWh1YXdlaWNsb3VkLmNvbS9QYXNzcG9ydEF2YXRhci8yMDIxLzA3LzAxLzIwMjEwNzAxMDkxNjE5Mjc5LmpwZyIsIlVzZXJUeXBlIjoiMyIsIk9yZ2FuaXphdGlvbkNvZGUiOiI4MDIwMjAwIiwiTmFtZSI6IuWunumqjOWtpumZoijlhajnvZEpMzMzIiwiUGhvbmVOdW1iZXIiOiIxMzU4MTg1ODM0NiIsImp0aSI6IkM4NDQ2MkU2QjdDMDdGMEMyNjkyMDA5MTU3QTY5ODk5Iiwic2lkIjoiODQwRUE1QkMwNzJFM0VDMUUwMDEzMDRDMzRDQUQwMzciLCJpYXQiOjE3MzM3MTQ2NTQsInNjb3BlIjpbIklkZW50aXR5U2VydmVyQXBpIiwicGFzc3BvcnRtYW5hZ2VhcGkiLCJwYXNzcG9ydGFwaSIsIm91Y29ubGluZSIsImFsbGlub25lbWFuYWdlYXBpIiwicHJvZmlsZSIsIm9wZW5pZCIsIm9mZmxpbmVfYWNjZXNzIl0sImFtciI6WyJwd2QiXX0.R9kY6LW1CQ9eO8v4xtDpQ75POj2BN-4RvtzBBi4xOzN5e2TIX0Z3222f3cLoE5A6Rw66h6uoa9hq4GYxr33H9R9MuEE3Uxwt30jf2sa0elak5BJd3sJROzw8sp1pcL6QdqMQvFmVOKwKWT0ql8sYIrR9NC9YvA0lUhOzXSgE2pL99J7sKFk6SuiVM8oIklVRY2XahjXhvyNkAfNa1hgkK0GLFE8hsg0oZBprdS9oBuTs3no-QXc1hThCB2eS2aHDrh5MM5_OkQQyNdrlujR09ZY8FDipvobQfuwrjChLwdWWAcOtI0rf22IQFtRKNkhLRSJ0TjVAMqZtPbE5Dy0wDw");
                client.DefaultRequestHeaders.AcceptEncoding.ParseAdd("gzip, deflate");


                // 发送 GET 请求
                try
                {
                    var response = await client.GetAsync(url);

                    // 确保响应成功
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Response successful.");
                        string content = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(content);  // 输出返回的 HTML 或 JSON
                    }
                    else
                    {
                        Console.WriteLine($"Failed to get response. Status Code: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }

            }




        }
        private static string HmacSha256Encode(string value, string key)
        {
            using HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            byte[] data = hmac.ComputeHash(Encoding.UTF8.GetBytes(value));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }
}

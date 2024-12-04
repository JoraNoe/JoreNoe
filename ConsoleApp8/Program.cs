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
                var url = "https://jorenoe.top/dogegg/api/File/IpAddress";  // 替换为目标 URL

                // 修改 User-Agent
                client.DefaultRequestHeaders.UserAgent.ParseAdd("1234/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");  // 更改 User-Agent
                client.DefaultRequestHeaders.Referrer = new Uri("https://1234.example.com");  // 更改 Referer
                client.DefaultRequestHeaders.Add("X-Forwarded-For", "127.0.0.1,123.123.123.123,123:0:0:0");  // 添加 X-Forwarded-For (可以模拟 IP)

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
    }
}

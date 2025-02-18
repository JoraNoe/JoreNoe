namespace ConsoleApp6
{
    using System;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Linq;

    class Program
    {
        private static int successCount = 0;
        private static int failureCount = 0;
        private static readonly HttpClient client = new HttpClient(); // 静态HttpClient实例，复用连接

        static async Task Main(string[] args)
        {
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;

            while (true)
            {
                successCount = 0;
                failureCount = 0;

                Console.WriteLine("请输入请求的URL (输入 'exit' 退出):");
                string url = Console.ReadLine();
                if (url.ToLower() == "exit")
                {
                    break;
                }

                Console.WriteLine("请输入并发请求数:");
                if (!int.TryParse(Console.ReadLine(), out int concurrentRequests))
                {
                    Console.WriteLine("无效的并发请求数，默认设置为80");
                    concurrentRequests = 80;
                }

                Console.WriteLine("请输入总请求数:");
                if (!int.TryParse(Console.ReadLine(), out int totalRequests))
                {
                    Console.WriteLine("无效的总请求数，默认设置为1000");
                    totalRequests = 1000;
                }

                Console.WriteLine("请输入每次请求间隔时间（毫秒）:");
                if (!int.TryParse(Console.ReadLine(), out int requestDelay))
                {
                    Console.WriteLine("无效的请求间隔时间，默认设置为100毫秒");
                    requestDelay = 100;
                }

                Console.WriteLine("执行中...");
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                await RunLoadTest(url, concurrentRequests, totalRequests, requestDelay);

                stopwatch.Stop();
                Console.WriteLine($"测试完成，共耗时: {stopwatch.Elapsed.TotalSeconds} 秒");
                Console.WriteLine($"成功: {successCount}, 失败: {failureCount}");

                Console.WriteLine("测试结束，按任意键继续，输入 'exit' 退出...");
                string continueTest = Console.ReadLine();
                if (continueTest.ToLower() == "exit")
                {
                    break;
                }
            }
        }

        private static async Task RunLoadTest(string requestUrl, int maxConcurrentRequests, int totalReqs, int delayBetweenRequests)
        {
            SemaphoreSlim semaphore = new SemaphoreSlim(maxConcurrentRequests);
            Task[] tasks = new Task[totalReqs];

            for (int i = 0; i < totalReqs; i++)
            {
                await semaphore.WaitAsync();
                tasks[i] = Task.Run(async () =>
                {
                    try
                    {
                        await Task.Delay(delayBetweenRequests); // 每次请求之间增加延迟，防止系统过载

                        var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUrl);

                        // 设置随机IP地址和浏览器User-Agent
                        requestMessage.Headers.Add("X-Forwarded-For", GenerateRandomIP());
                        requestMessage.Headers.Add("User-Agent", GenerateRandomUserAgent());

                        HttpResponseMessage response = await client.SendAsync(requestMessage);
                        if (response.IsSuccessStatusCode)
                        {
                            Interlocked.Increment(ref successCount);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("请求成功");
                        }
                        else
                        {
                            Interlocked.Increment(ref failureCount);
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"请求失败，状态码: {response.StatusCode}");
                        }
                    }
                    catch (HttpRequestException httpRequestEx)
                    {
                        Interlocked.Increment(ref failureCount);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"请求失败: {httpRequestEx.Message}");
                    }
                    catch (TaskCanceledException taskCanceledEx)
                    {
                        Interlocked.Increment(ref failureCount);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"请求超时: {taskCanceledEx.Message}");
                    }
                    catch (Exception ex)
                    {
                        Interlocked.Increment(ref failureCount);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"请求失败: {ex.Message}");
                    }
                    finally
                    {
                        Console.ResetColor();
                        semaphore.Release();
                    }
                });
            }

            await Task.WhenAll(tasks);
        }

        // 生成随机IP地址
        private static string GenerateRandomIP()
        {
            Random random = new Random();
            return $"{random.Next(1, 255)}.{random.Next(1, 255)}.{random.Next(1, 255)}.{random.Next(1, 255)}";
        }

        // 生成随机的浏览器 User-Agent
        private static string GenerateRandomUserAgent()
        {
            Random random = new Random();
            var userAgents = new[]
            {
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Firefox/89.0",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Edge/91.0.864.59 Safari/537.36",
                "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.114 Safari/537.36",
                "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:47.0) Gecko/20100101 Firefox/47.0"
            };

            return userAgents[random.Next(userAgents.Length)];
        }
    }
}

namespace ConsoleApp6
{
    using System;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    class Program
    {
        private static int successCount = 0;
        private static int failureCount = 0;
        private static readonly HttpClient client = new HttpClient(); // 静态HttpClient实例，复用连接

        static async Task Main(string[] args)
        {
            // 提高默认连接池的并发限制
            ServicePointManager.DefaultConnectionLimit = int.MaxValue; // 根据实际需求设置最大连接数

            Console.WriteLine("请输入请求的URL:");
            string url = Console.ReadLine();

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
                requestDelay = 1;
            }

            Console.WriteLine("开始并发压力测试...");
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            await RunLoadTest(url, concurrentRequests, totalRequests, requestDelay);

            stopwatch.Stop();
            Console.WriteLine($"并发测试完成，共耗时: {stopwatch.Elapsed.TotalSeconds} 秒");
            Console.WriteLine($"成功请求数: {successCount}, 失败请求数: {failureCount}");

            Console.WriteLine("测试结束，按任意键退出...");
            Console.ReadKey();
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

                        HttpResponseMessage response = await client.GetAsync(requestUrl);
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
    }
}

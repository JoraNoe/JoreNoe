using JoreNoe.Middleware;

namespace TestNET6Project
{
    public class TestMiddleWare : IJorenoeRuningRequestLogging
    {
        public async Task RunningRequestLogging(JorenoeRuningRequestLoggingModel info)
        {
            Console.WriteLine("方法" + info.Method);
            Console.WriteLine("路径" + info.Path);
            Console.WriteLine("开始时间" + info.StartTime);
            Console.WriteLine("总时长" + info.Duration);
            Console.WriteLine("Get请求参数" + info.QueryString);
            Console.WriteLine("BODY请求参数" + info.RequestBody);
            Console.WriteLine("完整路径" + info.FullPathUrl);
            Console.WriteLine("Headers" + info.Headers);
        }
    }
}

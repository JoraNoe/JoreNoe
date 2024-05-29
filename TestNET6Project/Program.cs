namespace TestNET6Project
{
    using JoreNoe.Cache.Redis;
    using JoreNoe.DB.Dapper;
    using JoreNoe.Middleware;
    using Newtonsoft.Json;

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddJoreNoeRequestLoggingMiddleware<TestMiddleWare>();

            //builder.Logging.AddConsole();


            builder.Services.AddJoreNoeGlobalErrorHandlingMiddleware<TestErrorMiddleWare>();
            builder.Services.AddJoreNoeRedis("43.136.101.66:6379,Password=JoreNoe123", "Dogegg", 1);
            var app = builder.Build();

            


            app.UseJoreNoeGlobalErrorHandlingMiddleware();
            app.UseJoreNoeRequestLoggingMiddleware();

            // webapi 全局错误日志中间件  直接使用方法回调方式
            //app.UseJoreNoeGlobalErrorHandlingMiddleware(async (ex, context) =>
            //{
            //    var x = JoreNoeRequestCommonTools.FormatError(ex,ex.Message);
            //    await Console.Out.WriteLineAsync(x);
            //    // 返回错误信息 // 处理自己的数据
            //    await Console.Out.WriteLineAsync(JsonConvert.SerializeObject(ex));

            //});

            // webapi 全局运行日志中间件 直接使用方法回调方式
            //app.UseJoreNoeRequestLoggingMiddleware(info => {
            //    Console.WriteLine("方法"+info.Method);
            //    Console.WriteLine("路径" + info.Path);
            //    Console.WriteLine("开始时间" + info.StartTime);
            //    Console.WriteLine("总时长" + info.Duration);
            //    Console.WriteLine("Get请求参数" + info.QueryString);
            //    Console.WriteLine("BODY请求参数" + info.RequestBody);
            //    Console.WriteLine("完整路径" + info.FullPathUrl);
            //    Console.WriteLine("Headers" + info.Headers);
            //});


            // webapi 全局运行日志中间件 接口继承 方式
            // app.UseJoreNoeRequestLoggingMiddleware();


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}

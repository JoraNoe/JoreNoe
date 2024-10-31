namespace TestNET6Project
{
    using JoreNoe.Cache.Redis;
    using JoreNoe.DB.Dapper;
    using JoreNoe.Middleware;
    using Newtonsoft.Json;
    using JoreNoe.Queue.RBMQ;
    using System.Runtime.CompilerServices;

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.WebHost.ConfigureKestrel(serverOptions =>
            {
                serverOptions.Limits.MaxConcurrentConnections = int.MaxValue;
            });
            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

           // builder.Services.AddJoreNoeRequestLoggingMiddleware();
            

            //builder.Logging.AddConsole();

            //builder.Services.AddJoreNoeRequestVisitRecordIpAddressMiddleware<ip>();

            //builder.Services.AddJoreNoeGlobalErrorHandlingMiddleware<TestErrorMiddleWare>();
            builder.Services.AddJoreNoeRedis("jorenoe-redis.redis.rds.aliyuncs.com,Password=jiatianhao123$%^,connectTimeout=10000,syncTimeout=10000, asyncTimeout=10000,abortConnect=false", 1);
            builder.Services.AddJoreNoeSystemIPBlackListMiddleware(100000000, TimeSpan.FromMinutes(1), true);
            //builder.Services.AddJoreNoeJoreNoeIntefaceAccessMiddleware();

            builder.Services.AddJoreNoeGlobalErrorHandlingMiddleware<TestErrorMiddleWare>(EnableReturnRecordErrorMessage:true);

            // 使用RabbitMQ
            builder.Services.AddJoreNoeRabbitMQ("amqp://jorenoe:jorenoe@124.70.12.71:5672/Jorenoe-Monitoring");

            builder.Services.AddResponseCaching();
            var app = builder.Build();

            app.UseJoreNoeGlobalErrorHandlingMiddleware();
            //app.UseJoreNoeRequestVisitRecordIpAddressMiddleware();
            //app.UseJoreNoeIntefaceAccessMiddleware();
            //app.UseJoreNoeSystemIPBlackListMiddleware();
            //app.UseJoreNoeRequestVisitRecordIpAddressMiddleware(e => {
            //    Console.WriteLine("方法" + e.IpAddress);
            //});

            app.UseResponseCaching();
            app.usexxmiddle();

            app.UseJoreNoeRequestLoggingMiddleware(ConnectionInfo =>
            {
                Console.WriteLine(ConnectionInfo.ResponseBody);
            });

            //app.UseJoreNoeGlobalErrorHandlingMiddleware();
            //app.UseJoreNoeRequestLoggingMiddleware();

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

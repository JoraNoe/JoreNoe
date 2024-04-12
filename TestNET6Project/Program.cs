namespace TestNET6Project
{
    using JoreNoe.DB.Dapper;
    using JoreNoe.Middleware;
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

            //builder.Logging.AddConsole();


            var app = builder.Build();

            // webapi 全局错误日志中间件
            app.UseJoreNoeGlobalErrorHandlingMiddleware(async (ex, context) =>
            {
                // 返回错误信息 // 处理自己的数据
                await Console.Out.WriteLineAsync(ex.Message);
            });

            // webapi 全局运行日志中间件
            app.UseJoreNoeRequestLoggingMiddleware(info => {
                Console.WriteLine("方法"+info.Method);
                Console.WriteLine("路径" + info.Path);
                Console.WriteLine("开始时间" + info.StartTime);
                Console.WriteLine("总时长" + info.Duration);
                Console.WriteLine("Get请求参数" + info.QueryString);
                Console.WriteLine("BODY请求参数" + info.RequestBody);
                Console.WriteLine("完整路径" + info.FullPathUrl);
                Console.WriteLine("Headers" + info.Headers);
            });


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

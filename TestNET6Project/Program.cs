namespace TestNET6Project
{
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
            var app = builder.Build();



            app.UseJoreNoeGlobalErrorHandlingMiddleware();

            // webapi ȫ�ִ�����־�м��  ֱ��ʹ�÷����ص���ʽ
            //app.UseJoreNoeGlobalErrorHandlingMiddleware(async (ex, context) =>
            //{
            //    var x = JoreNoeRequestCommonTools.FormatError(ex,ex.Message);
            //    await Console.Out.WriteLineAsync(x);
            //    // ���ش�����Ϣ // �����Լ�������
            //    await Console.Out.WriteLineAsync(JsonConvert.SerializeObject(ex));

            //});

            // webapi ȫ��������־�м�� ֱ��ʹ�÷����ص���ʽ
            //app.UseJoreNoeRequestLoggingMiddleware(info => {
            //    Console.WriteLine("����"+info.Method);
            //    Console.WriteLine("·��" + info.Path);
            //    Console.WriteLine("��ʼʱ��" + info.StartTime);
            //    Console.WriteLine("��ʱ��" + info.Duration);
            //    Console.WriteLine("Get�������" + info.QueryString);
            //    Console.WriteLine("BODY�������" + info.RequestBody);
            //    Console.WriteLine("����·��" + info.FullPathUrl);
            //    Console.WriteLine("Headers" + info.Headers);
            //});


            // webapi ȫ��������־�м�� �ӿڼ̳� ��ʽ
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

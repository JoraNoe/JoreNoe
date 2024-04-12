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

            // webapi ȫ�ִ�����־�м��
            app.UseJoreNoeGlobalErrorHandlingMiddleware(async (ex, context) =>
            {
                // ���ش�����Ϣ // �����Լ�������
                await Console.Out.WriteLineAsync(ex.Message);
            });

            // webapi ȫ��������־�м��
            app.UseJoreNoeRequestLoggingMiddleware(info => {
                Console.WriteLine("����"+info.Method);
                Console.WriteLine("·��" + info.Path);
                Console.WriteLine("��ʼʱ��" + info.StartTime);
                Console.WriteLine("��ʱ��" + info.Duration);
                Console.WriteLine("Get�������" + info.QueryString);
                Console.WriteLine("BODY�������" + info.RequestBody);
                Console.WriteLine("����·��" + info.FullPathUrl);
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

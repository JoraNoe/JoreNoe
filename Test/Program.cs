using JoreNoe.Cache.Redis;
using JoreNoe.DB.Dapper;
using JoreNoe.Extend;
using JoreNoe.Middleware;
namespace Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.



            builder.Services.AddControllers().AddJsonOptions(s => s.JsonSerializerOptions.PropertyNamingPolicy = null);

            builder.Services.AddJoreNoeRedis(s => {
                s.ConnectionString = "43.136.101.66:6379,Password=JoreNoe123,connectTimeout=10000,syncTimeout=10000, asyncTimeout=10000,abortConnect=false";
                s.IsEnabledFaieldProjectName = true;
            });

            //builder.Services.AddJoreNoeRequestLoggingMiddleware<WeatherForecast>();
            //builder.Services.AddJoreNoeRequestLoggingMiddleware();
            builder.Services.AddSwaggerGen(option =>
            {
                //option.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                //{
                //    Title = "1",
                //    Version = "v1",

                //});
                //option.SwaggerDoc("v2", new Microsoft.OpenApi.Models.OpenApiInfo
                //{
                //    Title = "2",
                //    Version = "v2",
                //    Contact = new Microsoft.OpenApi.Models.OpenApiContact
                //    {
                //        Email = "jth",
                //        Name = "2",
                //        Url = new Uri("https://jorenoe.top")
                //    },
                //    Description = "测试2",
                //    TermsOfService = new Uri("https://jorenoe.top")
                //});

                //var x = string.Concat(Assembly.GetExecutingAssembly().GetName().Name, ".xml");
                //option.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, x));
            });

            // 单个模式注入
            builder.Services.AddJoreNoeDapper("Server=43.136.101.66;Port=3306;Database=dogegg;Uid=root;Pwd=jorenoe123;Max Pool Size=500;", IDBType.MySql, true);

            // 多个模式注入
            //builder.Services.AddJoreNoeDapper(
            //     new List<DatabaseSettings>
            //        {
            //            new DatabaseSettings("Server=43.136.101.66;Port=3306;Database=dogegg;Uid=root;Pwd=jorenoe123;Max Pool Size=500;",IDBType.MySql,true),
            //            new DatabaseSettings("Server=43.136.101.66;Port=3306;Database=jorenoe;Uid=root;Pwd=jorenoe123;Max Pool Size=500;",IDBType.MySql,true,
            //            AvailableTables:new List<string>{
            //                "test"
            //            }),

            //        }
            //);
            //builder.Services.AddJoreNoeSystemIPBlackListMiddleware(6, TimeSpan.FromSeconds(60), TimeSpan.FromMinutes(6), true);
            builder.Services.AddJoreNoeJoreNoeIntefaceAccessMiddleware(LocalCacheDurationInMinutes: TimeSpan.FromMinutes(5));


            builder.WebHost.UseUrls("http://*:9999");

            var app = builder.Build();


            // Configure the HTTP request pipeline.

            if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
            {
                app.UseSwagger();
                //app.UseJoreNoeSwaggerThemeDark();
                app.UseSwaggerUI(option =>
                {
                    //option.InjectStylesheet(SwaggerThemsExtend.DarkTheme);

                    option.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                    //option.SwaggerEndpoint("/swagger/v2/swagger.json", "v2");

                });
            }

            //app.UseJoreNoeRequestLoggingMiddleware();

            //app.UseJoreNoeRequestLoggingMiddleware(equals =>
            //{
            //    Console.WriteLine(equals.RequestBody);
            //});

            //app.UseJoreNoeSystemIPBlackListMiddleware();
            app.UseJoreNoeIntefaceAccessMiddleware();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}

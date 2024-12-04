using JoreNoe.Cache.Redis;
using JoreNoe.Extend;
using JoreNoe.Middleware;
using SwaggerThemes;
using System.Reflection;
namespace Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddJoreNoeRedis("43.136.101.66:6379,Password=JoreNoe123,connectTimeout=10000,syncTimeout=10000, asyncTimeout=10000,abortConnect=false",6);

            builder.Services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "1",
                    Version = "v1",

                });
                option.SwaggerDoc("v2", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "2",
                    Version = "v2",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact
                    {
                        Email = "jth",
                        Name = "2",
                        Url = new Uri("https://jorenoe.top")
                    },
                    Description = "²âÊÔ2",
                    TermsOfService = new Uri("https://jorenoe.top")
                });

                var x = string.Concat(Assembly.GetExecutingAssembly().GetName().Name, ".xml");
                option.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, x));
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
            {
                app.UseSwagger();
                app.UseJoreNoeSwaggerThemeDark();
                app.UseSwaggerUI(option =>
                {
                    option.InjectStylesheet(SwaggerThemsExtend.DarkTheme);
                    
                    option.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                    option.SwaggerEndpoint("/swagger/v2/swagger.json", "v2");

                });


                

            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}

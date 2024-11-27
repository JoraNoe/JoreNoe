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

                //option.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Test.xml"));
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
            {
                app.UseSwagger();
                app.UseJoreNoeSwaggerThemeDark();
                app.UseSwaggerUI(option =>
                {
                    option.InjectStylesheet(SwaggerThemsExtend.BaseCssPath);
                    option.InjectStylesheet(SwaggerThemsExtend.ThemeCssPath);
                    
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

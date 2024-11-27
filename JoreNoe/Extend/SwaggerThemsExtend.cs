using Google.Protobuf.WellKnownTypes;
using JoreNoe.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JoreNoe.Extend
{
    public static class SwaggerThemsExtend
    {
        public static string BaseCssPath = "/SwaggerStyles/SwaggerBase.css";
        public static string ThemeCssPath = "/SwaggerStyles/SwaggerDark.css";

        /// <summary>
        /// 使用暗黑Swagger模式
        /// </summary>
        /// <param name="App"></param>
        public static void UseJoreNoeSwaggerThemeDark(this IApplicationBuilder App)
        {
            string embeddedResourceText = GetEmbeddedResourceText("SwaggerBase.css");
            string embeddedResourceText2 = GetEmbeddedResourceText("SwaggerDark.css");
            AddGetEndpoint(App, BaseCssPath, embeddedResourceText);
            AddGetEndpoint(App, ThemeCssPath, embeddedResourceText2);
        }

        /// <summary>
        /// 返回内容
        /// </summary>
        /// <param name="app"></param>
        /// <param name="cssPath"></param>
        /// <param name="styleText"></param>
        private static void AddGetEndpoint(IApplicationBuilder app, string cssPath, string styleText)
        {
            string styleText2 = styleText;
            app.Map(cssPath, builder =>
            {
                builder.Run(async context =>
                {
                    context.Response.ContentType = "text/css";
                    await context.Response.WriteAsync(styleText);
                });
            });
        }

        /// <summary>
        /// 读取嵌入内容
        /// </summary>
        /// <param name="embeddedResourcePath"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private static string GetEmbeddedResourceText(string embeddedResourcePath)
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            var Referencing = JoreNoeRequestCommonTools.GetAssemblyName();
            string name = string.Concat(Referencing, ".Extend.SwaggerStyles.", embeddedResourcePath);
            using Stream stream = executingAssembly.GetManifestResourceStream(name);
            if (stream == null)
            {
                throw new ArgumentException("Can't find embedded resource: " + embeddedResourcePath);
            }

            using StreamReader streamReader = new StreamReader(stream);
            return streamReader.ReadToEnd();
        }

    }
}

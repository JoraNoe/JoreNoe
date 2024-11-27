using Google.Protobuf.WellKnownTypes;
using JoreNoe.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using SixLabors.Fonts.Tables.AdvancedTypographic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace JoreNoe.Extend
{
    public static class SwaggerThemsExtend
    {
        public static string DarkTheme = "/SwaggerStyles/SwaggerDark.css";

        /// <summary>
        /// 使用暗黑Swagger模式
        /// </summary>
        /// <param name="App"></param>
        public static void UseJoreNoeSwaggerThemeDark(this IApplicationBuilder App)
        {
            string embeddedResourceText = GetEmbeddedResourceText("SwaggerDark.css");
            AddGetEndpoint(App, DarkTheme, embeddedResourceText);
        }

        /// <summary>
        /// 返回内容
        /// </summary>
        /// <param name="app"></param>
        /// <param name="cssPath"></param>
        /// <param name="styleText"></param>
        private static void AddGetEndpoint(IApplicationBuilder app, string cssPath, string styleText)
        {
            string etag = GenerateETag(styleText);
            string styleText2 = styleText;
            app.Map(cssPath, builder =>
            {
                builder.Run(async context =>
                {
                    context.Response.ContentType = "text/css";
                    context.Response.Headers["Cache-Control"] = "public, max-age=3600"; // 缓存1年
                    context.Response.Headers["ETag"] = etag;
                    context.Response.Headers["Expires"] = DateTime.UtcNow.AddDays(2.0).ToString("R");

                    // 如果请求包含If-None-Match，且匹配ETag，返回304
                    if (context.Request.Headers.TryGetValue("If-None-Match", out var requestEtag) && requestEtag == etag)
                    {
                        context.Response.StatusCode = StatusCodes.Status304NotModified;
                        return;
                    }

                    context.Response.ContentType = "text/css";
                    await context.Response.WriteAsync(styleText);
                });
            });
        }

        /// <summary>
        /// 生成稳定的 ETag
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private static string GenerateETag(string content)
        {
            using var sha256 = SHA256.Create();
            byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(content));
            return Convert.ToBase64String(hash);
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

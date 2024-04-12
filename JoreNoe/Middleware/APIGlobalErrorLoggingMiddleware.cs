using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace JoreNoe.Middleware
{
    public class GlobalErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalErrorHandlingMiddleware> _logger;
        private readonly Action<Exception, HttpContext> _errorHandlingAction;

        public GlobalErrorHandlingMiddleware(RequestDelegate next, ILogger<GlobalErrorHandlingMiddleware> logger, Action<Exception, HttpContext> errorHandlingAction)
        {
            _next = next;
            _logger = logger;
            _errorHandlingAction = errorHandlingAction;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred during request processing.");
                _errorHandlingAction(ex, context);
            }
        }
    }

    public static class JoreNoeGlobalErrorMiddlewareExtensions
    {
        public static IApplicationBuilder UseJoreNoeGlobalErrorHandlingMiddleware(this IApplicationBuilder builder, Action<Exception, HttpContext> errorHandlingAction)
        {
            return builder.UseMiddleware<GlobalErrorHandlingMiddleware>(errorHandlingAction);
        }

    }
}

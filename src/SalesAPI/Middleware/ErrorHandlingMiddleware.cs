using System.Net;
using System.Text.Json;

namespace SalesAPI.Middleware
{
    /// <summary>
    /// Middleware global para tratamento de exceções não tratadas
    /// </summary>
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro não tratado na aplicação: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            
            var (statusCode, message) = exception switch
            {
                InvalidOperationException => ((int)HttpStatusCode.BadRequest, exception.Message),
                KeyNotFoundException => ((int)HttpStatusCode.NotFound, exception.Message),
                _ => ((int)HttpStatusCode.InternalServerError, "Ocorreu um erro interno no servidor")
            };

            context.Response.StatusCode = statusCode;

            var response = new
            {
                error = new
                {
                    message,
                    detail = exception.Message
                }
            };

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions 
            { 
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
            });
            
            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
using LocalShop.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace LocalShop.Shared.Infrastructure.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
                _logger.LogError(ex, "An unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            
            var response = new ApiResponse
            {
                Success = false,
                Message = "An error occurred while processing your request."
            };

            switch (exception)
            {
                case UnauthorizedAccessException:
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    response.Message = "Unauthorized access";
                    break;
                case ArgumentException:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Message = exception.Message;
                    break;
                case InvalidOperationException:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Message = exception.Message;
                    break;
                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response.Message = "Internal server error";
                    break;
            }

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var jsonResponse = JsonSerializer.Serialize(response, options);
            await context.Response.WriteAsync(jsonResponse);
        }
    }
}

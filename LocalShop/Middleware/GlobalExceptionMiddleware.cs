using System.Text.Json;

namespace LocalShop.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger; // Fixed type name  
        private readonly string _logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger) // Fixed constructor parameter type  
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
                _logger.LogError(ex, "Unhandled exception occurred");

                // Ensure directory exists
                if (!Directory.Exists(_logDirectory))
                    Directory.CreateDirectory(_logDirectory);

                // Format: error-20250727.txt
                var fileName = $"error-{DateTime.UtcNow:yyyyMMdd}.txt";
                var filePath = Path.Combine(_logDirectory, fileName);

                var logContent = $@"
[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] Exception:
{ex.Message}
{ex.StackTrace}
--------------------------------------------------------
";

                await File.AppendAllTextAsync(filePath, logContent);

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                var errorResponse = new
                {
                    StatusCode = context.Response.StatusCode,
                    Message = "Something went wrong. Please try again later."
                };

                await context.Response.WriteAsJsonAsync(errorResponse);
            }
        }
    }
}

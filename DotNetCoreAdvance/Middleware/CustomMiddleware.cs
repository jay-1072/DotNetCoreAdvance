using DotNetCoreAdvance.Interfaces;
using DotNetCoreAdvance.Middleware;
using DotNetCoreAdvance.Models;
using Newtonsoft.Json;
using Serilog;
using System.Text;

namespace DotNetCoreAdvance.Middleware
{
    public class CustomMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CustomMiddleware> _logger;

        public CustomMiddleware(RequestDelegate next, ILogger<CustomMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await LogRequestAsync(context.Request);
                var originalBodyStream = context.Response.Body;
                using (var responseBody = new MemoryStream())
                {
                    context.Response.Body = responseBody;
                    await _next(context);
                    LogResponseAsync(context.Response);
                    responseBody.Seek(0, SeekOrigin.Begin);
                    await responseBody.CopyToAsync(originalBodyStream);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(JsonConvert.SerializeObject(ex));
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync("An unexpected error occurred.");
                context.Response.Redirect($"/Home/Error/{context.Response.StatusCode}");
            }
        }

        private async Task LogRequestAsync(HttpRequest request)
        {
            request.EnableBuffering();

            using (StreamReader reader = new(request.Body, Encoding.UTF8, false, 1024, true))
            {
                var requestBody = await reader.ReadToEndAsync();
                request.Body.Seek(0, SeekOrigin.Begin);

                _logger.LogInformation($"Request: {request.Method} {request.Path} {requestBody}");
            }
        }

        private void LogResponseAsync(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var responseBody = new StreamReader(response.Body).ReadToEndAsync();

            _logger.LogInformation($"Response Status: {response.StatusCode} Response Body: {responseBody}");
        }
    }

    public static class CustomMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomMiddleware>();   
        }
    }
}
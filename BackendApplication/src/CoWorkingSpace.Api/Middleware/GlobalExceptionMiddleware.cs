using CoWorkingSpace.Application.Common.Exceptions;

namespace CoWorkingSpace.Api.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger)
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
            catch (BadRequestException ex)
            {
                await WriteResponseAsync(
                    context,
                    StatusCodes.Status400BadRequest,
                    ex.Message);
            }
            catch (NotFoundException ex)
            {
                await WriteResponseAsync(
                    context,
                    StatusCodes.Status404NotFound,
                    ex.Message);
            }
            catch (ConflictException ex)
            {
                await WriteResponseAsync(
                    context,
                    StatusCodes.Status409Conflict,
                    ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unhandled exception occurred.");

                await WriteResponseAsync(
                    context,
                    StatusCodes.Status500InternalServerError,
                    "An unexpected error occurred.");
            }
        }

        private static async Task WriteResponseAsync( HttpContext context, int statusCode, string message)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(new
            {
                message
            });
        }
    }
}

using DigitalArs.Application.DTOs;
using DigitalArs.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using static DigitalArs.Application.Exceptions.AppExceptions;

namespace DigitalArs.API.Middlewares
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly IHostEnvironment _env;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var traceId = httpContext.TraceIdentifier;

            var (statusCode, message) = exception switch
            {
                UnauthorizedAppException or UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, exception.Message),
                ForbiddenException => (StatusCodes.Status403Forbidden, exception.Message),
                NotFoundException or KeyNotFoundException => (StatusCodes.Status404NotFound, exception.Message),
                InvalidOperationException => (StatusCodes.Status400BadRequest, exception.Message),
                DuplicateEmailException or DuplicateAliasException or DuplicateDniException => (StatusCodes.Status409Conflict, exception.Message),
                InvalidRoleException => (StatusCodes.Status400BadRequest, exception.Message),
                _ => (StatusCodes.Status500InternalServerError, "Ocurrió un error interno.")
            };

            _logger.LogError(exception, "Excepción no controlada - TraceId: {TraceId}", traceId);

            var response = new ErrorResponse
            {
                TraceId = traceId,
                Message = message,
                StatusCode = statusCode,
                Errors = statusCode == StatusCodes.Status500InternalServerError
                    ? (_env.IsDevelopment() ? exception.ToString() : null)
                    : null
            };

            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = statusCode;

            await httpContext.Response.WriteAsJsonAsync(response, cancellationToken: cancellationToken);

            return true;
        }
    }
}
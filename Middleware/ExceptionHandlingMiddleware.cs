using AppAPIEmpacadora.Models.Exceptions;
using AppAPIEmpacadora.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace AppAPIEmpacadora.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILoggingService _loggingService;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILoggingService loggingService)
        {
            _next = next;
            _loggingService = loggingService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            var errorResponse = new ErrorResponse();

            switch (exception)
            {
                case EntityNotFoundException:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    errorResponse.Message = exception.Message;
                    errorResponse.Type = "EntityNotFound";
                    _loggingService.LogWarning("Entidad no encontrada: {Message}", exception.Message);
                    break;

                case ValidationException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.Message = exception.Message;
                    errorResponse.Type = "ValidationError";
                    _loggingService.LogWarning("Error de validación: {Message}", exception.Message);
                    break;

                case DatabaseException:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse.Message = "Error interno de base de datos";
                    errorResponse.Type = "DatabaseError";
                    _loggingService.LogError("Error de base de datos: {Message}", exception, exception.Message);
                    break;

                case FileOperationException:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse.Message = "Error en operación de archivo";
                    errorResponse.Type = "FileOperationError";
                    _loggingService.LogError("Error de operación de archivo: {Message}", exception, exception.Message);
                    break;

                case BusinessRuleException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.Message = exception.Message;
                    errorResponse.Type = "BusinessRuleViolation";
                    _loggingService.LogBusinessRuleViolation("Regla de negocio violada", exception.Message);
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse.Message = "Error interno del servidor";
                    errorResponse.Type = "InternalServerError";
                    _loggingService.LogCritical("Error interno no manejado: {Message}", exception, exception.Message);
                    break;
            }

            errorResponse.Timestamp = DateTime.UtcNow;
            errorResponse.Path = context.Request.Path;
            errorResponse.Method = context.Request.Method;

            var result = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await response.WriteAsync(result);
        }
    }

    public class ErrorResponse
    {
        public string Message { get; set; }
        public string Type { get; set; }
        public DateTime Timestamp { get; set; }
        public string Path { get; set; }
        public string Method { get; set; }
    }
}

using AppAPIEmpacadora.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;

namespace AppAPIEmpacadora.Services
{
    public class LoggingService : ILoggingService
    {
        private readonly ILogger<LoggingService> _logger;

        public LoggingService(ILogger<LoggingService> logger)
        {
            _logger = logger;
        }

        public void LogInformation(string message, params object[] args)
        {
            _logger.LogInformation(message, args);
        }

        public void LogWarning(string message, params object[] args)
        {
            _logger.LogWarning(message, args);
        }

        public void LogError(string message, Exception exception = null, params object[] args)
        {
            if (exception != null)
            {
                _logger.LogError(exception, message, args);
            }
            else
            {
                _logger.LogError(message, args);
            }
        }

        public void LogCritical(string message, Exception exception = null, params object[] args)
        {
            if (exception != null)
            {
                _logger.LogCritical(exception, message, args);
            }
            else
            {
                _logger.LogCritical(message, args);
            }
        }

        public void LogDebug(string message, params object[] args)
        {
            _logger.LogDebug(message, args);
        }

        public void LogDatabaseOperation(string operation, string entity, int? entityId = null, string userId = null)
        {
            var message = $"Operación de base de datos: {operation} en entidad {entity}";
            if (entityId.HasValue)
                message += $" (ID: {entityId})";
            if (!string.IsNullOrEmpty(userId))
                message += $" por usuario {userId}";

            _logger.LogInformation(message);
        }

        public void LogFileOperation(string operation, string fileName, string userId = null)
        {
            var message = $"Operación de archivo: {operation} en archivo {fileName}";
            if (!string.IsNullOrEmpty(userId))
                message += $" por usuario {userId}";

            _logger.LogInformation(message);
        }

        public void LogUserAction(string action, string userId, string details = null)
        {
            var message = $"Acción de usuario: {action} por usuario {userId}";
            if (!string.IsNullOrEmpty(details))
                message += $" - Detalles: {details}";

            _logger.LogInformation(message);
        }

        public void LogBusinessRuleViolation(string rule, string details, string userId = null)
        {
            var message = $"Violación de regla de negocio: {rule} - {details}";
            if (!string.IsNullOrEmpty(userId))
                message += $" por usuario {userId}";

            _logger.LogWarning(message);
        }
    }
}

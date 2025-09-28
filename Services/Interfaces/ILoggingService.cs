using Microsoft.Extensions.Logging;

namespace AppAPIEmpacadora.Services.Interfaces
{
    public interface ILoggingService
    {
        void LogInformation(string message, params object[] args);
        void LogWarning(string message, params object[] args);
        void LogError(string message, Exception exception = null, params object[] args);
        void LogCritical(string message, Exception exception = null, params object[] args);
        void LogDebug(string message, params object[] args);
        
        // Métodos específicos para operaciones de negocio
        void LogDatabaseOperation(string operation, string entity, int? entityId = null, string userId = null);
        void LogFileOperation(string operation, string fileName, string userId = null);
        void LogUserAction(string action, string userId, string details = null);
        void LogBusinessRuleViolation(string rule, string details, string userId = null);
    }
}

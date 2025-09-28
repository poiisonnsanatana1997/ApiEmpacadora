using System;

namespace AppAPIEmpacadora.Models.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(string message) : base(message) { }
        public EntityNotFoundException(string entityName, int id) : base($"{entityName} con ID {id} no fue encontrado.") { }
    }

    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message) { }
    }

    public class DatabaseException : Exception
    {
        public DatabaseException(string message) : base(message) { }
        public DatabaseException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class FileOperationException : Exception
    {
        public FileOperationException(string message) : base(message) { }
        public FileOperationException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class BusinessRuleException : Exception
    {
        public BusinessRuleException(string message) : base(message) { }
    }
}

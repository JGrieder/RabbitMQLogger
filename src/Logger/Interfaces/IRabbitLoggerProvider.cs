using System.Runtime.CompilerServices;
using RabbitMQLogger.Concrete;
using RabbitMQLogger.Definitions;

namespace RabbitMQLogger.Interfaces
{
    public interface IRabbitLoggerProvider
    {
        IRabbitLogger CreateRabbitLogger();

        LogEntry CreateLogEntry(LogLevel logLevel, string loggerType, string subject, string messageBody,  
            [CallerMemberName] string callerMemberName = "", [CallerFilePath] string filePathAndLocation = "", 
            [CallerLineNumber] int callerLineNumber = 0);
        
    }
}

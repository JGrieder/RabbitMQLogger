using System;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Options;
using RabbitMQLogger.Definitions;
using RabbitMQLogger.Interfaces;

namespace RabbitMQLogger.Concrete
{
    public class RabbitLoggerProvider : IRabbitLoggerProvider
    {
        private readonly RabbitMqLoggerOptions _rabbitMqLoggerOptions;

        //TODO #ifdef this so that there are two constructors that can take in two different Options objects depending on Compilation  - this might not be needed as the project currently compiles
        public RabbitLoggerProvider(IOptions<RabbitMqLoggerOptions> rabbitMqLoggerOptions)
        {
            _rabbitMqLoggerOptions = rabbitMqLoggerOptions.Value;
        }


        public IRabbitLogger CreateRabbitLogger()
        {
            return new RabbitLogger(_rabbitMqLoggerOptions);
        }

        public LogEntry CreateLogEntry(LogLevel logLevel, string loggerType, string subject, string messageBody, [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string filePathAndLocation = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return new LogEntry
            {
                LogLevel = (int)logLevel,
                LoggerType = loggerType,
                Subject = subject,
                Body = messageBody,
                Timestamp = DateTime.UtcNow.ToString(_rabbitMqLoggerOptions.LogEntrySettings.DateFormat),
                CodeInformation = new Code
                {
                  FunctionName  = callerMemberName,
                  LineNumber = callerLineNumber,
                  FileName = filePathAndLocation,
                },
                Application = new Application
                {
                    Name = _rabbitMqLoggerOptions.LogEntrySettings.ApplicationName,
                    Group = _rabbitMqLoggerOptions.LogEntrySettings.ApplicationGroup,
                    ServerName = !string.IsNullOrEmpty(_rabbitMqLoggerOptions.LogEntrySettings.ServerName) ? _rabbitMqLoggerOptions.LogEntrySettings.ServerName : Environment.MachineName
                },
            };
        }
    }
}

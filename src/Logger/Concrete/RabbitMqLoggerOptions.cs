namespace RabbitMQLogger.Concrete
{
    public class RabbitMqLoggerOptions
    {
        public string Environment { get; set; }
        public LoggerConfiguration LoggerConfiguration { get; set; }
        public LogEntrySettings LogEntrySettings { get; set; }

    }

    public class LoggerConfiguration
    {
        public string ConnectionString { get; set; }
        public string Queue { get; set; }
    }

    public class LogEntrySettings
    {
        public string DateFormat { get; set; }
        public string ServerName { get; set; }
        public string ApplicationName { get; set; }
        public string ApplicationGroup { get; set; }
    }
}

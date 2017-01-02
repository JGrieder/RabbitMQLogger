using System;
using System.Threading.Tasks;
using RabbitMQLogger.Concrete;

namespace RabbitMQLogger.Interfaces
{
    public interface IRabbitLogger : IDisposable
    {
        
        void Log(LogEntry logEntry);

        Task LogAsync(LogEntry logEntry);

    }
}
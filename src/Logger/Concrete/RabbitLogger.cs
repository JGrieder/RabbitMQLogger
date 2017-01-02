using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQLogger.Interfaces;

namespace RabbitMQLogger.Concrete
{
    //TODO Evaluate Confirmation Strategy Class from rabbit mq async library - 
    //If I understand correctly this will become an event handler that will ack back to Rabbit indicating that it has completed the send
    //Since the client is send only I think it is unneccessary
    public class RabbitLogger : IRabbitLogger
    {
        private readonly BlockingCollection<EnqueuedMessage> _queue;
        private Thread _thread;
        private readonly RabbitMqLoggerOptions _rabbitMqLoggerSettings;

        internal RabbitLogger(RabbitMqLoggerOptions rabbitMqLoggerSettings)
        {
            _rabbitMqLoggerSettings = rabbitMqLoggerSettings;
            _queue = new BlockingCollection<EnqueuedMessage>();
        }
        
        public void Log(LogEntry logEntry)  
        {
            string serialized = JsonConvert.SerializeObject(logEntry); 
            var body = Encoding.UTF8.GetBytes(serialized);

            using (
                var rabbitMqConnection =
                    new RabbitMqConnection(_rabbitMqLoggerSettings.LoggerConfiguration.ConnectionString)
                        .InitializeConnection())
            {
                rabbitMqConnection.Try(rabbit =>
                {
                    rabbit.QueueDeclare(_rabbitMqLoggerSettings.LoggerConfiguration.Queue, true, false, false, null);

                    var basicProperties = rabbit.CreateBasicProperties();
                    basicProperties.ContentType = "text/plain";
                    basicProperties.DeliveryMode = 2;
                    basicProperties.Type = "RabbitMQLogger";
                    basicProperties.Persistent = true;

                    rabbit.BasicPublish(string.Empty, _rabbitMqLoggerSettings.LoggerConfiguration.Queue, basicProperties, body);

                }, exception =>
                {
                    throw exception;
                });
            }
        }

        public Task LogAsync(LogEntry logEntry)
        {
            if (_thread == null)
            {
                //Consider Thread.Name a GUID in case having multiple threads of the same name blows up
                //Find a means to pass the thread name up
                _thread = new Thread(ThreadLoop) { Name = new Guid().ToString() };
                _thread.Start();
            }

            string serialized = JsonConvert.SerializeObject(logEntry); 
            var body = Encoding.UTF8.GetBytes(serialized);

            var taskCompletionSource = new TaskCompletionSource<object>();
            _queue.Add(new EnqueuedMessage
            {
                Exchange = string.Empty,
                Body = body,
                Queue = _rabbitMqLoggerSettings.LoggerConfiguration.Queue,
                TaskCompletionSource = taskCompletionSource
            });

            return taskCompletionSource.Task;
        }
        
        private void ThreadLoop()
        {
            foreach (var message in _queue.GetConsumingEnumerable())
            {
                if (_queue.IsAddingCompleted)
                {
                    message.TaskCompletionSource.TrySetCanceled();
                    continue;
                }

                using (
                    var rabbitMqConnection =
                        new RabbitMqConnection(_rabbitMqLoggerSettings.LoggerConfiguration.ConnectionString)
                            .InitializeConnection())
                {
                    rabbitMqConnection.Try(rabbit =>
                    {
                        rabbit.QueueDeclare(message.Queue, true, false, false, null);

                        var basicProperties = rabbit.CreateBasicProperties();
                        basicProperties.ContentType = "text/plain";
                        basicProperties.DeliveryMode = 2;
                        basicProperties.Type = "RabbitMQLogger";
                        basicProperties.Persistent = true;

                        //RoutingKey = Queue?
                        rabbit.BasicPublish(message.Exchange, message.Queue, basicProperties, message.Body);

                        //If implementing the confirmation strategy from the other library this will be swapped out with the event handler
                        message.TaskCompletionSource.SetResult(null);

                    }, exception =>
                    {
                        message.TaskCompletionSource.TrySetException(exception);
                    });
                }
            }
        }
        

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _queue.CompleteAdding();

                _thread?.Join();

                _queue.Dispose();
            }

            _disposed = true;
        }
        
    }
}

using System;
using System.Data.Common;
using RabbitMQ.Client;

namespace RabbitMQLogger.Concrete
{
    public class RabbitMqConnection : IDisposable
    {
        private IConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _channel;
        private readonly string _connectionString;

        internal RabbitMqConnection(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RabbitMqConnection InitializeConnection()
        {
            var builder = new DbConnectionStringBuilder { ConnectionString = _connectionString };

            _connectionFactory = new ConnectionFactory
            {
                HostName = builder["HostName"] as string,
                UserName = builder["Username"] as string,
                Password = builder["Password"] as string
            };
            
            return this;
        }

        public void Try(Action<IModel> action, Action<Exception> catchAction)
        {
            try
            {
                if (_connection == null || _channel == null)
                {
                    Dispose();
                    
                    _connection = _connectionFactory.CreateConnection();
                    _channel = _connection.CreateModel();
                }
                action.Invoke(_channel);
            }
            catch (Exception exception)
            {
                Dispose();
                catchAction.Invoke(exception);
            }
        }

        public void Dispose()
        {
            if (_channel != null)
            {
                try
                {
                    _channel.Dispose();
                }
                catch
                {
                    /*Munch Error*/
                }

                _channel = null;
            }

            if (_connection == null) return;
            try
            {
                _connection.Dispose();
            }
            catch
            {
                /*Munch Error*/
            }
            _connection = null;
        }


    }
}

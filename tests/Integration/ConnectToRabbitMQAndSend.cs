using System;
using Microsoft.Extensions.Options;
using RabbitMQLogger.Concrete;
using RabbitMQLogger.Definitions;
using Xunit;

namespace Integration
{
    public class ConnectToRabbitMqAndSend
    {
        [Fact]
        public void ShouldConnectToRabbitMqAndSend()
        {
            var someOptions = Options.Create(new RabbitMqLoggerOptions
            {
                Environment = "Dev",
                LoggerConfiguration = new LoggerConfiguration
                {
                    ConnectionString = "RabbitMQ Connectstring Goes Here",
                    Queue = "TestLogging"
                },
                LogEntrySettings = new LogEntrySettings
                {
                    ApplicationName = "UnitTest",
                    ApplicationGroup = "Test",
                    DateFormat = "yyyy-MM-ddTHH:mm:ss.fffZ",
                    ServerName = Environment.MachineName

                }
            });
            var loggerProvider = new RabbitLoggerProvider(someOptions);

            using (var logger = loggerProvider.CreateRabbitLogger())
            {
                try
                {
                    var entry = loggerProvider.CreateLogEntry(LogLevel.Info, "UnitTest", "Testing First Attempt",
                    "See Subject");

                    logger.Log(entry);
                }
                catch (Exception)
                {
                    Assert.True(1 == 2); //If Exception is throw this should fail the test
                }

            }

            Assert.True(true);
        }
    }
}

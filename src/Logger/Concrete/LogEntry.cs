using System;
using System.Collections.Generic;

namespace RabbitMQLogger.Concrete
{
    public class LogEntry
    {
        public string LoggerType { get; set; }
        public int LogLevel { get; set; }
        public string Timestamp { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }    
        public Code CodeInformation { get; set; }
        public Application Application { get; set; }
        public Dictionary<string, string> LoggingDictionary { get; protected set; }

        //TODO Create a common set of formatters that can be retrieved as if they were constants
        public LogEntry AddItemToDictionary<TObject>(string keyName, TObject item, Func<TObject, string> formatter)
        {
            if (LoggingDictionary == null)
                LoggingDictionary = new Dictionary<string, string>();

            if (formatter != null)
            {
                string value = formatter(item);
                LoggingDictionary.Add(keyName, value);
            }
            else
            {
                //Override ToString of TObject to inject a blob into the logging dictionary
                string value = item.ToString();
                LoggingDictionary.Add(keyName, value);
            }

            return this;
        }

    }

    public class Code
    {
        public string Source { get; set; }
        public string FunctionName { get; set; }
        public string FileName { get; set; } //This Could Include The Full Path of the File
        public int LineNumber { get; set; }
    }

    public class Application
    {
        public string Name { get; set; }
        public string Group { get; set; }
        public string ServerName { get; set; }
        public string VersionInfo { get; set; }
    }

}

using System;
using System.Threading;

namespace Logging.Core
{
    public class LogRecord
    {
        public string LoggerName { get; }
        public LogLevel LogLevel { get; }
        public string Message { get; }
        public object Details { get; }
        public DateTime Timestamp { get; } = DateTime.Now;
        public int ThreadId { get; } = Thread.CurrentThread.ManagedThreadId;
        public string ThreadName { get; } = Thread.CurrentThread.Name;

        public LogRecord(string loggerName, LogLevel logLevel, string message, object details = null) {
            LoggerName = loggerName;
            LogLevel = logLevel;
            Message = message;
            Details = details;
        }
    }
}
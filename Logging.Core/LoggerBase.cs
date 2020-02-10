namespace Logging.Core
{
    public abstract class LoggerBase : ILogger
    {
        public string Name { get; }

        public LogLevel MinLevel { get; }

        protected LoggerBase(string name, LogLevel minLevel) {
            Name = name;
            MinLevel = minLevel;
        }

        public void Log(LogLevel level, string message, object details = null) {
            if (level >= MinLevel) {
                Log(new LogRecord(Name, level, message, details));
            }
        }

        protected abstract void Log(LogRecord logRecord);
    }
}
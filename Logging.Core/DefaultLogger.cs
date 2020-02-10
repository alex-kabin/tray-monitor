using System;

namespace Logging.Core
{
    public class DefaultLogger : LoggerBase
    {
        private readonly ILog[] _logs;

        public DefaultLogger(string name, LogLevel minLevel, params ILog[] logs) : base(name, minLevel) {
            _logs = logs;
        }

        protected override void Log(LogRecord logRecord) {
            foreach (var log in _logs) {
                try {
                    log.Write(logRecord);
                }
                catch (Exception ex) {
                    System.Diagnostics.Debug.WriteLine($"Writing to {log.GetType()} failed with error: {ex}");
                }
            }
        }
    }
}

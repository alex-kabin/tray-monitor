using System;
using System.Linq;
using System.Security.Cryptography;

namespace Logging.Core
{
    public class CompositeLogger : ILogger
    {
        private readonly ILogger[] _loggers;
        
        public string Name { get; }

        public CompositeLogger(string name, params Func<string, ILogger>[] builders) {
            _loggers = builders.Select(b => b(name)).ToArray();
            Name = name;
        }

        public void Log(LogLevel level, string message, object details = null) {
            foreach (var logger in _loggers) {
                logger.Log(level, message, details);
            }
        }
    }
}
using System;

namespace Logging.Core
{
    public interface ILogger
    {
        string Name { get; }
        
        void Log(LogLevel level, string message, object details = null);
    }
}

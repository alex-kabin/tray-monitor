using System;

namespace Logging.Core 
{
    public static class LoggerFactory
    {
        public static LogLevel MinLevel { get; set; } = LogLevel.Debug;

        public static ILog[] Logs { get; set; } = { new DiagnosticsLog(DefaultLogRecordFormatter.Instance) };
        
        public static Func<string, ILogger> Builder = name => new DefaultLogger(name, MinLevel, Logs);
        
        public static ILogger GetLogger(string name) => Builder(name);

        public static void Loggers(params Func<string, ILogger>[] loggers) {
            Builder = name => new CompositeLogger(name, loggers);
        }
    }
    
}
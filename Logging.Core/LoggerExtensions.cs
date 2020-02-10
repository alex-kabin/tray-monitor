using System;

namespace Logging.Core
{
    public static class LoggerExtensions
    {
        public static void Debug(this ILogger logger, object message) {
            logger.Log(LogLevel.Debug, message?.ToString());
        }
        
        public static void Info(this ILogger logger, object message) {
            logger.Log(LogLevel.Info, message?.ToString());
        }
        
        public static void Warn(this ILogger logger, object message) {
            logger.Log(LogLevel.Warn, message?.ToString());
        }
        
        public static void Warn(this ILogger logger, object message, object details) {
            logger.Log(LogLevel.Warn, message?.ToString(), details);
        }
        
        public static void Error(this ILogger logger, object message, object details) {
            logger.Log(LogLevel.Error, message?.ToString(), details);
        }
    }
}
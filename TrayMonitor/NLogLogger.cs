using System;
using Logging.Core;
using NLog;
using NLog.Fluent;
using ILogger = Logging.Core.ILogger;
using LogLevel = Logging.Core.LogLevel;

namespace TrayMonitor
{
    public class NLogLogger : ILogger
    {
        private readonly Logger _nlog;
        
        public string Name { get; }

        public NLogLogger(string name) {
            Name = name;
            _nlog = LogManager.GetLogger(name);
        }

        public void Log(LogLevel level, string message, object details = null) {
            switch (level) {
                case LogLevel.Debug:
                    _nlog.Debug(message, details);
                    break;
                case LogLevel.Info:
                    _nlog.Info(message, details);
                    break;
                case LogLevel.Warn:
                    if (details is Exception) {
                        _nlog.Warn((Exception) details, message);
                    } else {
                        _nlog.Warn(message, details);
                    }
                    break;
                case LogLevel.Error:
                    if (details is Exception) {
                        _nlog.Error((Exception) details, message);
                    } else {
                        _nlog.Error(message, details);
                    }
                    break;
            }
        }
    }
}
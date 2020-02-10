using System;

namespace Logging.Core
{
    public class DefaultLogRecordFormatter : ILogRecordFormatter
    {
        public static ILogRecordFormatter Instance { get; } = new DefaultLogRecordFormatter();

        public string FormatLogRecord(LogRecord record) {
            return $"{record.Timestamp:O} [{record.LogLevel}] [{record.ThreadId}] {{{record.LoggerName}}} {record.Message}" 
                   + (record.Details == null ? String.Empty : $" : {record.Details}");
        }
    }
}
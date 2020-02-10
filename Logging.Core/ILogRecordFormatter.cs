namespace Logging.Core
{
    public interface ILogRecordFormatter
    {
        string FormatLogRecord(LogRecord record);
    }
}
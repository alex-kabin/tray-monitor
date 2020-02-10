namespace Logging.Core
{
    public abstract class TextLog : ILog
    {
        private readonly ILogRecordFormatter _formatter;

        protected TextLog(ILogRecordFormatter formatter) {
            _formatter = formatter;
        }

        public void Write(LogRecord record) {
            Write(_formatter.FormatLogRecord(record));
        }

        protected abstract void Write(string text);
    }
}

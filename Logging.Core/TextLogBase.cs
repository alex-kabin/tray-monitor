namespace Logging.Core
{
    public abstract class TextLogBase : ILog
    {
        private readonly ILogRecordFormatter _formatter;

        protected TextLogBase(ILogRecordFormatter formatter) {
            _formatter = formatter;
        }

        public void Write(LogRecord record) {
            Write(_formatter.FormatLogRecord(record));
        }

        protected abstract void Write(string text);
    }
}

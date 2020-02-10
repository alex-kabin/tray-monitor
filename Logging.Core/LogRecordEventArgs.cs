using System;

namespace Logging.Core
{
    public class LogRecordEventArgs : EventArgs
    {
        public LogRecord Record { get; }

        public LogRecordEventArgs(LogRecord record) {
            Record = record;
        }
    }
}

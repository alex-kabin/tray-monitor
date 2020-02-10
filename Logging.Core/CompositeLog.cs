namespace Logging.Core
{
    public class CompositeLog : ILog
    {
        private readonly ILog[] _logs;

        public CompositeLog(params ILog[] logs) {
            _logs = logs;
        }

        public void Write(LogRecord record) {
            foreach (var log in _logs) {
                log.Write(record);
            }
        }
    }
}
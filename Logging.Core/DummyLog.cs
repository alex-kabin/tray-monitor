namespace Logging.Core
{
    public class DummyLog : ILog
    {
        public static DummyLog Instance { get; } = new DummyLog();
        
        public void Write(LogRecord record) { }
    }
}
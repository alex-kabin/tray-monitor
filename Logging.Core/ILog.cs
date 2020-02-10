namespace Logging.Core
{
    public interface ILog
    {
        void Write(LogRecord record);
    }
}

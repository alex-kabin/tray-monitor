using System;

namespace Logging.Core 
{
    public class ConsoleLog : TextLogBase
    {
        public ConsoleLog(ILogRecordFormatter logRecordFormatter) 
                : base(logRecordFormatter) {
        }
        
        protected override void Write(string text) {
            Console.WriteLine(text);
        }
    }
}
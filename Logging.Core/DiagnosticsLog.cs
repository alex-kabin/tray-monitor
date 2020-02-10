using System;

namespace Logging.Core 
{
    public class DiagnosticsLog : TextLog
    {
        public DiagnosticsLog(ILogRecordFormatter logRecordFormatter) 
                : base(logRecordFormatter) {
        }
        
        protected override void Write(string text) {
            System.Diagnostics.Debug.WriteLine(text);
        }
    }
}
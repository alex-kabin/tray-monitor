using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading;
using Logging.Core;

namespace TrayMonitor
{
    public class JournalLog : ILog, INotifyCollectionChanged
    {
        private readonly int _capacity;
        private readonly SynchronizationContext _sync;
        private readonly LinkedList<LogRecord> _records;
        
        public JournalLog(int capacity) {
            _capacity = capacity > 0 ? capacity : throw new ArgumentOutOfRangeException(nameof(capacity), "must be > 0");
            _records = new LinkedList<LogRecord>();
            _sync = SynchronizationContext.Current;
        }
        
        public void Write(LogRecord record) {
            _sync.Post(_ => AddRecord(record), null);
        }

        private void RemoveOld() {
            var old = _records.First.Value;
            _records.RemoveFirst();
            CollectionChanged?.Invoke(
                this,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new[] { old }, 0)
            );
        }

        private void Append(LogRecord record) {
            _records.AddLast(record);
            CollectionChanged?.Invoke(
                this,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new[] { record }, _records.Count-1)
            );
        }

        private void AddRecord(LogRecord record) {
            if (_records.Count > _capacity) {
                RemoveOld();
            }
            Append(record);
        }

        public IEnumerable<LogRecord> Records => _records;

        public event NotifyCollectionChangedEventHandler CollectionChanged;
    }
}

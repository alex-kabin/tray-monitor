using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;

namespace Logging.Core
{
    public class JournalLog : ILog, ICollection<LogRecord>, INotifyCollectionChanged
    {
        private readonly int _capacity;
        private readonly LinkedList<LogRecord> _journal;
        
        public JournalLog(int capacity) {
            _capacity = capacity > 0 ? capacity : throw new ArgumentOutOfRangeException(nameof(capacity), "capacity must be > 0");
            _journal = new LinkedList<LogRecord>();
        }
        
        public void Write(LogRecord record) {
            if (record == null) {
                return;
            }

            lock (_journal) {
                if (_journal.Count == _capacity) {
                    var old = _journal.First.Value;
                    _journal.RemoveFirst();
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, old, 0));
                }
                _journal.AddLast(record);
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, record, _journal.Count - 1));
            }
        }

        public LogRecord[] Export() {
            lock (_journal) {
                return _journal.ToArray();
            }
        }
        
        public int Count {
            get {
                lock (_journal) {
                    return _journal.Count;
                }
            }
        }

        public void Clear() {
            lock (_journal) {
                if (_journal.Count > 0) {
                    _journal.Clear();
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }
            }
        }
        
        #region ICollection<LogRecord> implementation
        void ICollection<LogRecord>.Add(LogRecord item) {
            Write(item);
        }

        bool ICollection<LogRecord>.Contains(LogRecord item) {
            lock (_journal) {
                return _journal.Contains(item);
            }
        }

        void ICollection<LogRecord>.CopyTo(LogRecord[] array, int arrayIndex) {
            lock (_journal) {
                _journal.CopyTo(array, arrayIndex);                
            }
        }

        bool ICollection<LogRecord>.Remove(LogRecord item) {
            lock (_journal) {
                return _journal.Remove(item);
            }
        }

        bool ICollection<LogRecord>.IsReadOnly => false;
        
        public IEnumerator<LogRecord> GetEnumerator() {
           return ((IEnumerable<LogRecord>)Export()).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return Export().GetEnumerator();
        }
        #endregion // ICollection<LogRecord> implementation
        
        public event NotifyCollectionChangedEventHandler CollectionChanged;
    }
}
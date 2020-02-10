using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Logging.Core
{
    public class AsyncLogDecorator : ILog, IDisposable
    {
        private readonly ILog _log;
        private readonly int _capacity;
        private readonly ILog _overflowLog;
        private volatile int _count = 0;
        private readonly Thread _thread;
        private ConcurrentQueue<LogRecord> _queue;
        private AutoResetEvent  _signal;
        private CancellationTokenSource _cancellation;

        public AsyncLogDecorator(ILog log, int capacity, ILog overflowLog = null) {
            _log = log;
            _capacity = capacity;
            _overflowLog = overflowLog;

            _queue = new ConcurrentQueue<LogRecord>();
            _signal = new AutoResetEvent(false);

            _cancellation = new CancellationTokenSource();
            
            _thread = new Thread(WriteWorker) { IsBackground = true, Priority = ThreadPriority.BelowNormal, Name = $"Async-{log.GetType()}" };
            _thread.Start();
        }

        private void WriteWorker(object obj) {
            var cancellationToken = _cancellation.Token;
            try {
                while (!cancellationToken.IsCancellationRequested) {
                    if (_queue.TryDequeue(out var record)) {
                        Interlocked.Decrement(ref _count);
                        try {
                            _log.Write(record);
                        }
                        catch (Exception ex) {
                            System.Diagnostics.Debug.WriteLine($"Writing to {_log.GetType()} failed with error: {ex}");
                        }
                    }
                    else {
                        //WaitHandle.WaitAny(new[] { _signal, _cancellationToken.WaitHandle });
                        while (!_signal.WaitOne(200)) {
                            cancellationToken.ThrowIfCancellationRequested();
                        }
                    }
                }
            }
            catch (OperationCanceledException) {
                // ignore
            }
            catch (ObjectDisposedException) {
                // ignore
            }
        }

        public void Write(LogRecord record) {
            if (_count > _capacity) {
                if (_overflowLog != null) {
                    try {
                        _overflowLog.Write(record);
                    }
                    catch (Exception ex) {
                        System.Diagnostics.Debug.WriteLine($"Writing to {_overflowLog.GetType()} failed with error: {ex}");
                    }
                }
                else {
                    System.Diagnostics.Debug.WriteLine($"Logging error: log buffer is full");
                }
            }
            else {
                _queue.Enqueue(record);
                Interlocked.Increment(ref _count);
                _signal.Set();
            }
        }

        public void Dispose() {
            if (_cancellation != null) {
                _cancellation.Cancel();
                _cancellation.Dispose();
                _cancellation = null;
            }
            _signal.Dispose();
        }
    }
}

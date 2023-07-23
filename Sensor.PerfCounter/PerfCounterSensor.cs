using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Logging.Core;
using Sensor.Core;

namespace Sensor.PerfCounter
{
    public class PerfCounterSensor : SensorBase, IConfigurable, IDisposable    {
        private static readonly ILogger Log = LoggerFactory.GetLogger(nameof(PerfCounterSensor));

        private String _categoryName;
        private String _counterName;
        private String _instanceName = String.Empty;
        private String _machineName = ".";
        private PerformanceCounter _counter;
        private CancellationTokenSource _cancellation;
        private TimeSpan _interval = TimeSpan.FromMilliseconds(1000);

        public PerfCounterSensor() {
        }

        public void Configure(IEnumerable<(string, string)> parameters) {
            foreach ((string key, string value) in parameters) {
                if (string.Equals(key, "Interval", StringComparison.OrdinalIgnoreCase)) {
                    _interval = TimeSpan.FromMilliseconds(int.Parse(value));
                }
                else if (string.Equals(key, "Category", StringComparison.OrdinalIgnoreCase)) {
                    _categoryName = value;
                }
                else if (string.Equals(key, "Counter", StringComparison.OrdinalIgnoreCase)) {
                    _counterName = value;
                }
                else if (string.Equals(key, "Instance", StringComparison.OrdinalIgnoreCase)) {
                    _instanceName = value;
                }
                else if (string.Equals(key, "Machine", StringComparison.OrdinalIgnoreCase)) {
                    _machineName = value;
                }
            }
        }

        private void CreateCounter() {
            if (String.IsNullOrEmpty(_categoryName) || String.IsNullOrEmpty(_counterName)) {
                throw new SensorException("Sensor should be properly configured: Category and Counter parameters are mandatory");
            }
            
            Log.Debug($"Creating PerformanceCounter {{CategoryName={_categoryName}, CounterName={_counterName}, InstanceName={_instanceName}, MachineName={_machineName}}}");

            _counter = new PerformanceCounter(_categoryName, _counterName, _instanceName, _machineName);
            //"Processor"
            // "% Processor Time",
            //"_Total",
            
            Title = _counterName;
            
            Log.Info("Created PerformanceCounter", $"CategoryName={_categoryName}, CounterName={_counterName}, InstanceName={_instanceName}, MachineName={_machineName}");
        }

        public override async Task Connect(CancellationToken cancellationToken) {
            if (Status != SensorStatus.Offline) {
                return;
            }
            
            Status = SensorStatus.Connecting;
            Error = null;
            
            Disposable.Destroy(ref _counter);
            try {
                CreateCounter();
            }
            catch (Exception ex) {
                Log.Error("Failed creating PerformanceCounter", ex);
                Status = SensorStatus.Offline;
                Error = ex as SensorException ?? new SensorException("Failed creating PerformanceCounter", ex);
                throw;
            }

            _cancellation?.Dispose();
            _cancellation = new CancellationTokenSource();

            Task.Factory.StartNew(
                () => MonitorPerfCounter(_cancellation.Token), 
                _cancellation.Token, 
                TaskCreationOptions.LongRunning, 
                TaskScheduler.Default
            );

            Status = SensorStatus.Online;
        }

        private async void MonitorPerfCounter(CancellationToken cancellationToken) {
            try {
                while (true) {
                    cancellationToken.ThrowIfCancellationRequested();
                    Value = _counter?.NextValue();
                    await Task.Delay(_interval, cancellationToken);
                }
            }
            catch (OperationCanceledException) {
                Log.Debug("Monitoring canceled");
                Error = null;
                Status = SensorStatus.Offline;
            }
            catch (Exception ex) {
                Log.Error("Monitoring failure", ex);
                Status = SensorStatus.Offline;
                Error = new SensorException("Monitoring failure", ex);
            }
        }

        public override async Task Disconnect(CancellationToken cancellationToken) {
            if (Status != SensorStatus.Online) {
                return;
            }

            Log.Debug("Disconnecting...");
            Status = SensorStatus.Disconnecting;

            if (_cancellation != null) {
                _cancellation.Cancel();
                _cancellation.Dispose();
                _cancellation = null;
                await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken);
            }

            Disposable.Destroy(ref _counter);
            
            Log.Info("Disconnected");
        }

        public void Dispose() {
            if (_cancellation != null) {
                _cancellation.Cancel();
                _cancellation.Dispose();
                _cancellation = null;
            }
            Disposable.Destroy(ref _counter);
        }
    }
}

using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Threading;
using Logging.Core;
using NLog.Fluent;
using Sensor.Core;
using TrayMonitor.Commands;
using TrayMonitor.Core.Indicators;
using TrayMonitor.Indicators;

namespace TrayMonitor
{
    public class Monitor : IComponent
    {
        private static readonly ILogger Log = LoggerFactory.GetLogger(nameof(Monitor));
        
        private IIndicator _indicator;
        private ISensor _sensor;
        private IDisposable _sensorSubscription;
        private CancellationTokenSource _cts;

        public Monitor(ISensor sensor, IIndicator indicator) {
            _sensor = sensor ?? throw new ArgumentNullException(nameof(sensor));
            _indicator = indicator ?? throw new ArgumentNullException(nameof(indicator));
           
            _sensorSubscription = _sensor.AsObservable().ObserveOn(SynchronizationContext.Current).Subscribe(Update);
            
            ControlCommand = new ActionCommand("", Control);
            
            Update(_sensor.GetState());
        }

        public ActionCommand ControlCommand { get; }
        
        private void Control() {
            var sensorStatus = _sensor.Status;

            if (sensorStatus == SensorStatus.Offline) {
                _cts = new CancellationTokenSource();
                _sensor.Connect(_cts.Token);
            }
            else if (sensorStatus == SensorStatus.Online) {
                _sensor.Disconnect(CancellationToken.None);
                if (_cts != null) {
                    _cts.Dispose();
                    _cts = null;
                }
            }
            else if (_cts != null) {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }
        }

        private void Update(SensorState sensorState) {
            UpdateIndicator(sensorState);
            UpdateControlCommand(sensorState);
        }

        private void UpdateIndicator(SensorState sensorState) {
            _indicator?.Update(sensorState);
        }

        private void UpdateControlCommand(SensorState sensorState) {
            switch (sensorState.Status) {
                case SensorStatus.Offline:
                    ControlCommand.Name = "Connect";
                    ControlCommand.CanExecute = true;
                    break;
                case SensorStatus.Online:
                    ControlCommand.Name = "Disconnect";
                    ControlCommand.CanExecute = true;
                    break;
                case SensorStatus.Connecting:
                    ControlCommand.Name = "Cancel";
                    ControlCommand.CanExecute = true;
                    break;
                case SensorStatus.Disconnecting:
                    ControlCommand.Name = "Disconnecting...";
                    ControlCommand.CanExecute = false;
                    break;
                default:
                    ControlCommand.Name = "Wait...";
                    ControlCommand.CanExecute = false;
                    break;
            }
        }

        public ISite Site { get; set; }
        
        public void Dispose() {
            Disposable.Destroy(ref _sensorSubscription);
            Disposable.Destroy(ref _sensor);
            Disposable.Destroy(ref _indicator);

            if (_cts != null) {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }
            
            Disposed?.Invoke(this, EventArgs.Empty);
        }
        
        public event EventHandler Disposed;
    }
}
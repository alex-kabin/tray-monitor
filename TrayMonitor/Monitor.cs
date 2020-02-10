using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Threading;
using Sensor.Core;
using TrayMonitor.Commands;
using TrayMonitor.Core.Indicators;
using TrayMonitor.Indicators;

namespace TrayMonitor
{
    public class Monitor : IComponent
    {
        private IIndicator _indicator;
        private ISensor _sensor;
        private IDisposable _sensorSubscription;
        private CancellationTokenSource _cts;
        private SynchronizationContext _sync;

        public Monitor(ISensor sensor, IIndicator indicator) {
            _sensor = sensor ?? throw new ArgumentNullException(nameof(sensor));
            _indicator = indicator ?? throw new ArgumentNullException(nameof(indicator));
            _sync = SynchronizationContext.Current;
            _sensorSubscription = _sensor.AsObservable().Subscribe(
                sd => _sync.Post(_ => Update(sd), null)
            );
            
            ControlCommand = new ActionCommand("", Control);
            
            Update(_sensor.GetData());
        }

        public ActionCommand ControlCommand { get; }
        
        private void Control() {
            var state = _sensor.State;

            if (state == SensorState.Offline) {
                _cts = new CancellationTokenSource();
                _sensor.Connect(_cts.Token);
            }
            else if (state == SensorState.Online) {
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

        private void Update(SensorData sensorData = null) {
            UpdateIndicator(sensorData);
            UpdateControlCommand(sensorData);
        }

        private void UpdateIndicator(SensorData sensorData) {
            _indicator?.Update(sensorData);
        }

        private void UpdateControlCommand(SensorData sensorData) {
            switch (sensorData?.State) {
                case SensorState.Offline:
                    ControlCommand.Name = "Connect";
                    ControlCommand.CanExecute = true;
                    break;
                case SensorState.Online:
                    ControlCommand.Name = "Disconnect";
                    ControlCommand.CanExecute = true;
                    break;
                case SensorState.Connecting:
                    ControlCommand.Name = "Cancel";
                    ControlCommand.CanExecute = true;
                    break;
                case SensorState.Disconnecting:
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
            if (_sensorSubscription != null) {
                _sensorSubscription.Dispose();
                _sensorSubscription = null;
            }

            if (_sensor != null) {
                (_sensor as IDisposable)?.Dispose();
                _sensor = null;
            }

            if (_indicator != null) {
                (_indicator as IDisposable)?.Dispose();
                _indicator = null;
            }

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
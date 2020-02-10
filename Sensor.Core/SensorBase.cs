using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sensor.Core 
{
    public abstract class SensorBase : ISensor
    {
        public abstract Task Connect(CancellationToken cancellationToken);
        public abstract Task Disconnect(CancellationToken cancellationToken);
        public virtual void Configure(IEnumerable<(string, string)> parameters) { }

        protected SensorState _state;
        public SensorState State {
            get => _state;
            protected set {
                if (_state != value) {
                    _state = value;
                    StateChanged?.Invoke(this, new SensorStateEventArgs(_state));
                } 
            }
        }
        public event EventHandler<SensorStateEventArgs> StateChanged;

        protected object _value;
        public object Value {
            get => _value;
            protected set {
                if (!Equals(_value, value)) {
                    _value = value;
                    if (_value != null) {
                        _error = null;
                    }
                    ValueChanged?.Invoke(this, new SensorValueEventArgs(_value));
                }
            }
        }
        public event EventHandler<SensorValueEventArgs> ValueChanged;

        protected SensorException _error;
        public SensorException Error {
            get => _error;
            protected set {
                if (!Equals(_error, value)) {
                    _error = value;
                    if (_error != null) {
                        _value = null;
                        ErrorOccured?.Invoke(this, EventArgs.Empty);
                    }
                }
            }
        }
        public event EventHandler ErrorOccured;

        protected string _title;
        public string Title {
            get => _title;
            protected set {
                if (!string.Equals(_title, value)) {
                    _title = value;
                    TitleChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }
        public event EventHandler TitleChanged;
    }
}
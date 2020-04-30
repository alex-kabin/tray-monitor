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
        
        protected SensorStatus _status = SensorStatus.Offline;
        public SensorStatus Status {
            get => _status;
            protected set {
                if (_status != value) {
                    _status = value;
                    _value = null;
                    StatusChanged?.Invoke(this, new SensorStatusEventArgs(_status));
                } 
            }
        }
        public event EventHandler<SensorStatusEventArgs> StatusChanged;

        protected object _value;
        public object Value {
            get => _value;
            protected set {
                _value = value;
                if (_value != null) {
                    _error = null;
                }
                ValueReady?.Invoke(this, new SensorValueEventArgs(_value));
            }
        }
        public event EventHandler<SensorValueEventArgs> ValueReady;

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
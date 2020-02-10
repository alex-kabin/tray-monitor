using System;

namespace Sensor.Core 
{
    public class SensorValueEventArgs : EventArgs
    {
        public object Value { get; }

        public SensorValueEventArgs(object value) {
            Value = value;
        }
    }
}
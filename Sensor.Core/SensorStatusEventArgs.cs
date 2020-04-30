using System;

namespace Sensor.Core 
{
    public class SensorStatusEventArgs : EventArgs
    {
        public SensorStatus Status { get; }
        
        public SensorStatusEventArgs(SensorStatus status) {
            Status = status;
        }
    }
}
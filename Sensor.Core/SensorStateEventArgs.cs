using System;

namespace Sensor.Core 
{
    public class SensorStateEventArgs : EventArgs
    {
        public SensorState State { get; }
        
        public SensorStateEventArgs(SensorState state) {
            State = state;
        }
    }
}
using System;

namespace Sensor.Core
{
    public class SensorData
    {
        public string Title { get; set; }
        public SensorException Error { get; set; }
        public SensorState State { get; set; }
        public object Value { get; set; }
        public DateTime Timestamp { get; } = DateTime.Now;
    }
}
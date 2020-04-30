using System;

namespace Sensor.Core
{
    public class SensorState
    {
        public string Title { get; set; }
        public SensorException Error { get; set; }
        public SensorStatus Status { get; set; }
        public object Value { get; set; }
        public DateTime Timestamp { get; } = DateTime.Now;

        public static bool IsUpdated(SensorState newSensorState, SensorState oldSensorState) {
            if (newSensorState == null && oldSensorState == null) {
                return false;
            }
            if (newSensorState == null || oldSensorState == null) {
                return true;
            }
            return newSensorState.Timestamp >= oldSensorState.Timestamp && (
                       !Equals(newSensorState.Value, oldSensorState.Value)
                       || newSensorState.Status != oldSensorState.Status
                       || !ReferenceEquals(newSensorState.Error, oldSensorState.Error)
                       || !string.Equals(newSensorState.Title, oldSensorState.Title)
                   );
        }
    }
}
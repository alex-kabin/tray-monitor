using System;
using System.Runtime.Serialization;

namespace Sensor.Core
{
    public class SensorException : Exception
    {
        protected SensorException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public SensorException(string message) : base(message) { }
        public SensorException(string message, Exception innerException) : base(message, innerException) { }
    }
}
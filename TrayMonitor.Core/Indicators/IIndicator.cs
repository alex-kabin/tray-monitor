using System.Collections.Generic;
using Sensor.Core;

namespace TrayMonitor.Core.Indicators
{
    public interface IIndicator
    {
        void Configure(IEnumerable<(string, string)> properties);
        
        void Update(SensorData data);
    }
}
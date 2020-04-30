using System.Collections.Generic;
using Sensor.Core;

namespace TrayMonitor.Core.Indicators
{
    public interface IIndicator
    {
        void Update(SensorState state);
    }
}
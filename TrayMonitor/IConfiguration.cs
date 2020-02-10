using System.Collections.Generic;

namespace TrayMonitor
{
    public interface IConfiguration
    {
        bool AutoConnect { get; }
        string SensorTypeName { get; }
        List<(string, string)> SensorProperties { get; }
        string IndicatorTypeName { get; }
        List<(string, string)> IndicatorProperties { get; }
    }
}

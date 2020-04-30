using System.Collections.Generic;

namespace Sensor.Core
{
    public interface IConfigurable
    {
        void Configure(IEnumerable<(string, string)> properties);
    }
}

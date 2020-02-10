using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sensor.Core
{
    public interface ISensor
    {
        void Configure(IEnumerable<(string, string)> parameters);

        Task Connect(CancellationToken ct);
        
        Task Disconnect(CancellationToken ct);
        
        string Title { get; }
        
        event EventHandler TitleChanged;
        
        SensorException Error { get; }
        
        event EventHandler ErrorOccured;
        
        SensorState State { get; }
        
        event EventHandler<SensorStateEventArgs> StateChanged;
        
        object Value { get; }

        event EventHandler<SensorValueEventArgs> ValueChanged;
    }
}

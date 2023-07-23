using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sensor.Core
{
    public interface ISensor
    {
        Task Connect(CancellationToken ct);
        
        Task Disconnect(CancellationToken ct);
        
        string Title { get; }
        
        event EventHandler TitleChanged;
        
        SensorException Error { get; }
        
        event EventHandler ErrorOccurred;
        
        SensorStatus Status { get; }
        
        event EventHandler<SensorStatusEventArgs> StatusChanged;
        
        object Value { get; }

        event EventHandler<SensorValueEventArgs> ValueReady;
    }
}

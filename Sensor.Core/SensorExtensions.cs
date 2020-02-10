using System;
using System.Reactive.Linq;

namespace Sensor.Core
{
    public static class SensorExtensions
    {
        public static SensorData GetData(this ISensor sensor) {
            return new SensorData {
                    Title = sensor.Title,
                    Error = sensor.Error,
                    State = sensor.State,
                    Value = sensor.Value
            };
        }
        
        public static IObservable<SensorData> AsObservable(this ISensor sensor) {
            return Observable.Create<SensorData>(
                observer => {
                    void OnSensorChange(object s, object a) {
                        observer.OnNext(GetData(sensor));
                    }
                    sensor.ValueChanged += OnSensorChange;
                    sensor.StateChanged += OnSensorChange;
                    sensor.ErrorOccured += OnSensorChange;
                    sensor.TitleChanged += OnSensorChange;

                    return new Disposable(() => {
                                              sensor.ValueChanged -= OnSensorChange;
                                              sensor.StateChanged -= OnSensorChange;
                                              sensor.ErrorOccured -= OnSensorChange;
                                              sensor.TitleChanged -= OnSensorChange;
                    });

                }
            );
        }
    }
}
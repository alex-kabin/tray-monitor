using System;
using System.Reactive.Linq;

namespace Sensor.Core
{
    public static class SensorExtensions
    {
        public static SensorState GetState(this ISensor sensor) {
            return new SensorState {
                    Title = sensor.Title,
                    Error = sensor.Error,
                    Status = sensor.Status,
                    Value = sensor.Value
            };
        }

        public static IObservable<SensorState> AsObservable(this ISensor sensor) {
            return Observable.Create<SensorState>(
                observer => {
                    void OnSensorChange(object s, object a) {
                        observer.OnNext(GetState(sensor));
                    }

                    sensor.ValueReady += OnSensorChange;
                    sensor.StatusChanged += OnSensorChange;
                    sensor.ErrorOccured += OnSensorChange;
                    sensor.TitleChanged += OnSensorChange;

                    return new Disposable(
                        () => {
                            sensor.ValueReady -= OnSensorChange;
                            sensor.StatusChanged -= OnSensorChange;
                            sensor.ErrorOccured -= OnSensorChange;
                            sensor.TitleChanged -= OnSensorChange;
                        }
                    );
                }
            );
        }
    }
}
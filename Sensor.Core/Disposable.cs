using System;

namespace Sensor.Core
{
    public class Disposable : IDisposable
    {
        public static void Destroy<T>(ref T obj) where T : class {
            if (obj is IDisposable disposable) {
                disposable.Dispose();
            }
            obj = null;
        }
        
        private Action _dispose;

        public Disposable(Action dispose) {
            _dispose = dispose;
        }

        public void Dispose() {
            if (_dispose != null) {
                _dispose.Invoke();
                _dispose = null;
            }
        }
    }
}

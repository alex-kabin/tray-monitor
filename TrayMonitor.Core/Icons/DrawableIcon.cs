using System;
using System.Drawing;

namespace TrayMonitor.Core.Icons 
{
    public abstract class DrawableIcon : IDisposable
    {
        private readonly Action<Icon> _consumer;
        private readonly Bitmap _bitmap;
        private bool _disposed = false;

        public Size Size { get; }

        protected DrawableIcon(Size size, Action<Icon> consumer) {
            _consumer = consumer;
            _bitmap = new Bitmap(size.Width, size.Height);
            Size = size;
        }

        protected abstract void Draw(Graphics gfx);

        public void Update() {
            if (_disposed) {
                throw new ObjectDisposedException(GetType().Name);
            }
            using (var gfx = Graphics.FromImage(_bitmap)) { 
                Draw(gfx);
            }
            var icon = IconHelper.BitmapToIcon(_bitmap);
            try {
                _consumer(icon);
            }
            finally {
                icon.Destroy();
            }
        }

        public virtual void Dispose() {
            if (!_disposed) {
                _bitmap?.Dispose();
                _disposed = true;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Sensor.Core;
using TrayMonitor.Core.Icons;

namespace TrayMonitor.Core.Indicators
{
    public abstract class TrayIndicator : IIndicator, IDisposable
    {
        class DrawableTrayIcon : DrawableIcon
        {
            private readonly Action<Graphics> _draw;

            public DrawableTrayIcon(NotifyIcon notifyIcon, Action<Graphics> draw)
                    : base(new Size(16, 16), icon => notifyIcon.Icon = icon) {
                _draw = draw;
            }

            protected override void Draw(Graphics gfx) {
                _draw(gfx);
            }
        }
        
        private readonly NotifyIcon _notifyIcon;
        private DrawableTrayIcon _trayIcon;
        private SensorData _data;
        
        protected Size Size { get; }
        

        protected TrayIndicator(NotifyIcon notifyIcon) {
            _notifyIcon = notifyIcon;
            _trayIcon = new DrawableTrayIcon(notifyIcon, gfx => Draw(gfx, _data));
            Size = new Size(_trayIcon.Size.Width - 1, _trayIcon.Size.Height - 1);
        }

        protected virtual void SetTitle(string text) {
            _notifyIcon.Text = text.Length > 63 ? text.Substring(0, 62) + "…" : text;
        }

        public virtual void Configure(IEnumerable<(string, string)> properties) {
        }

        public void Update(SensorData data) {
            _data = data;
            _trayIcon?.Update();
        }

        protected void DrawWaitOverlay(Graphics gfx) {
            int r = 4, cx = Size.Width - r, cy = Size.Height - r;
            gfx.FillPolygon(Brushes.White, new [] { new Point(cx-r, cy), new Point(cx, cy - r), new Point(cx+r, cy), new Point(cx, cy + r) });
            //gfx.FillEllipse(Brushes.White, cx - r, cy - r, r + r, r + r);
            gfx.FillRectangle(Brushes.Black, cx - r/2, cy, 1, 1);
            gfx.FillRectangle(Brushes.Black, cx, cy, 1, 1);
            gfx.FillRectangle(Brushes.Black, cx + r/2, cy, 1, 1);
        }
        
        protected void DrawErrorOverlay(Graphics gfx) {
            int r = 4, cx = Size.Width - r - 1, cy = Size.Height - r - 1;
            gfx.FillEllipse(Brushes.White, cx - r, cy - r, r + r, r + r);
            gfx.FillRectangle(Brushes.Red, cx - r/2, cy - 1, r, 2);
        }

        protected abstract void Draw(Graphics gfx, SensorData data);

        public void Dispose() {
            if (_trayIcon != null) {
                _trayIcon.Dispose();
                _trayIcon = null;
            }
        }
    }
}
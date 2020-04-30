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
        private const int MAX_TITLE_LENGTH = 63;
        
        protected static readonly Size Size = new Size(16, 16);
        protected static readonly int MaxWidth = Size.Width - 1;
        protected static readonly int MaxHeight = Size.Height - 1;
        
        class DrawableTrayIcon : DrawableIcon
        {
            private readonly Action<Graphics> _draw;

            public DrawableTrayIcon(NotifyIcon notifyIcon, Action<Graphics> draw)
                    : base(TrayIndicator.Size, icon => notifyIcon.Icon = icon) {
                _draw = draw;
            }

            protected override void Draw(Graphics gfx) {
                _draw(gfx);
            }
        }
        
        private readonly NotifyIcon _notifyIcon;
        private DrawableTrayIcon _trayIcon;

        protected SensorState CurrentState { get; private set; }
        
        protected TrayIndicator(NotifyIcon notifyIcon) {
            _notifyIcon = notifyIcon;
            _trayIcon = new DrawableTrayIcon(notifyIcon, Draw);
        }

        protected virtual void SetTitle(string text) {
            _notifyIcon.Text = text.Length > MAX_TITLE_LENGTH ? text.Substring(0, MAX_TITLE_LENGTH-1) + "…" : text;
        }

        protected virtual bool ShouldRefresh(SensorState newState, SensorState currentState) {
            return SensorState.IsUpdated(newState, currentState);
        }

        public void Update(SensorState state) {
            bool shouldRefresh = ShouldRefresh(state, CurrentState); 
            CurrentState = state;
            if (shouldRefresh) {
                Refresh();
            }
        }

        protected virtual void Refresh() {
            _trayIcon?.Update();
        }

        protected void DrawWaitOverlay(Graphics gfx) {
            int r = 4, cx = MaxWidth - r, cy = MaxHeight - r;
            gfx.FillPolygon(Brushes.White, new[] {
                    new Point(cx - r, cy),
                    new Point(cx, cy - r),
                    new Point(cx + r, cy),
                    new Point(cx, cy + r)
                }
            );
            //gfx.FillEllipse(Brushes.White, cx - r, cy - r, r + r, r + r);
            gfx.FillRectangle(Brushes.Black, cx - r / 2, cy, 1, 1);
            gfx.FillRectangle(Brushes.Black, cx, cy, 1, 1);
            gfx.FillRectangle(Brushes.Black, cx + r / 2, cy, 1, 1);
        }

        protected void DrawErrorOverlay(Graphics gfx) {
            int r = 4, cx = MaxWidth - r - 1, cy = MaxHeight - r - 1;
            gfx.FillEllipse(Brushes.White, cx - r, cy - r, r + r, r + r);
            gfx.FillRectangle(Brushes.Red, cx - r/2, cy - 1, r, 2);
        }

        protected abstract void Draw(Graphics gfx);

        public void Dispose() {
            Disposable.Destroy(ref _trayIcon);
        }
    }
}
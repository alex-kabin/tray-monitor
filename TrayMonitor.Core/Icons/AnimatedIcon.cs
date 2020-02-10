using System;
using System.Drawing;
using System.Windows.Forms;

namespace TrayMonitor.Core.Icons 
{
    public abstract class AnimatedIcon : DrawableIcon
    {
        private Timer _timer;

        protected AnimatedIcon(Size size, TimeSpan interval, Action<Icon> consumer): base(size, consumer) {
            _timer = new Timer() { Interval = (int)interval.TotalMilliseconds };
            _timer.Tick += OnTimerTick;
            _timer.Start();
        }

        private void OnTimerTick(object sender, EventArgs e) {
            Update();
        }
        
        public override void Dispose() {
            if (_timer != null) {
                _timer.Stop();
                _timer.Dispose();
                _timer = null;
            }
            base.Dispose();
        }
    }
}

using System;
using System.Drawing;
using TrayMonitor.Core.Icons;

namespace TrayMonitor.Icons
{
    class TestIcon : AnimatedIcon
    {
        private int _counter;

        public TestIcon(Action<Icon> consumer) : base(new Size(16, 16), TimeSpan.FromSeconds(1), consumer) {
            _counter = 0;
        }

        protected override void Draw(Graphics gfx) {
            _counter = (_counter + 1) % 10;
            gfx.Clear(Color.Transparent);
            gfx.FillEllipse(Brushes.Red, 8-_counter/2, 8-_counter/2, _counter, _counter);
        }
    }
}

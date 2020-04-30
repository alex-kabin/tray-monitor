using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Sensor.Core;
using TrayMonitor.Core.Indicators;

namespace TrayMonitor.Indicators
{
    public class GraphTrayIndicator : TrayIndicator, IConfigurable
    {
        class History
        {
            private readonly LinkedList<float?> _values = new LinkedList<float?>();
            private readonly int _capacity;

            public History(int capacity) {
                _capacity = capacity;
            }

            public void Add(float? value) {
                if (_values.Count == _capacity) {
                    _values.RemoveFirst();
                }
                _values.AddLast(value);
            }

            public void Clear() {
                _values.Clear();
            }

            public IEnumerable<(float?, int)> Enumerate() {
                var node = _values.Last;
                int i = 0;
                while (node != null) {
                    yield return (node.Value, i);
                    node = node.Previous;
                    i++;
                }
            }
        }

        private const int RED_LEVEL = 11;
        private const int YELLOW_LEVEL = 7;
        
        private readonly History _history;
        private float _min = 0;
        private float _max = 100;

        public GraphTrayIndicator(NotifyIcon notifyIcon) : base(notifyIcon) {
            _history = new History(Size.Width);
            Update(null);
        }
        
        public void Configure(IEnumerable<(string, string)> properties) {
            foreach ((string key, string value) in properties) {
                if (string.Equals(key, "Min", StringComparison.OrdinalIgnoreCase)) {
                    _min = float.Parse(value);
                }
                else if (string.Equals(key, "Max", StringComparison.OrdinalIgnoreCase)) {
                    _max = float.Parse(value);
                }
            }
            if (_min > _max) {
                throw new ApplicationException("Bad configuration: Min should be < Max");
            }
        }

        protected override bool ShouldRefresh(SensorState newState, SensorState currentState) {
            if (newState?.Value != null) {
                _history.Add(Convert.ToSingle(newState.Value));
            }
            else {
                _history.Clear();
            }
            return true;
        }

        protected override void Draw(Graphics gfx) {
            SensorState state = CurrentState;
            
            gfx.SmoothingMode = SmoothingMode.None;
            gfx.Clear(Color.Black);
            //gfx.DrawRectangle(new Pen(Pens.Gray, (Size.Width-w)/2, 0, w, h);            
            
            string title = string.IsNullOrEmpty(state?.Title) ? string.Empty : $"{state.Title}: ";
            
            if (state != null) {
                if (state.Error != null) {
                    //gfx.FillEllipse(Brushes.DimGray, ix, iy, iw, ih);
                    gfx.DrawRectangle(new Pen(Color.Red), 0, 0, MaxWidth, MaxHeight);
                    title += state.Error.Message;
                }
                else {
                    if (state.Status == SensorStatus.Connecting) {
                        //gfx.FillEllipse(Brushes.DimGray, ix, iy, iw, ih);
                        gfx.DrawRectangle(new Pen(Color.Cyan), 0, 0, MaxWidth, MaxHeight);
                        title += "Connecting...";
                    }
                    else if (state.Status == SensorStatus.Disconnecting) {
                        //gfx.FillEllipse(Brushes.DimGray, ix, iy, iw, ih);
                        gfx.DrawRectangle(new Pen(Color.Fuchsia), 0, 0, MaxWidth, MaxHeight);
                        title += "Disconnecting...";
                    }
                    else if (state.Status == SensorStatus.Offline) {
                        gfx.DrawRectangle(new Pen(Color.DarkGray), 0, 0, MaxWidth, MaxHeight);
                        title += "Offline";
                    }
                    else if (state.Status == SensorStatus.Online) {
                        float f = MaxHeight / (_max - _min);
                        foreach (var (value, index) in _history.Enumerate()) {
                            if (value != null) {
                                float v = f * (value.Value - _min);
                                v = v < 0 ? 0 : v > MaxHeight ? MaxHeight : v;
                                
                                Color color = Color.LimeGreen;
                                if (v >= RED_LEVEL) {
                                    color = Color.Red;
                                }
                                else if (v >= YELLOW_LEVEL) {
                                    color = Color.Yellow;
                                }
                                float x = MaxWidth - index;
                                gfx.DrawLine(new Pen(color), x, MaxHeight, x, MaxHeight - v);
                            }
                        }
                        if (state.Value != null) {
                            try {
                                var value = Convert.ToDouble(state.Value);
                                title += $"{value:F2}";
                            }
                            catch (SystemException) {
                                title += state.Value;
                            }
                        }
                        else {
                            title += "???";
                        }
                    }
                }
            }
            else {
                gfx.DrawRectangle(new Pen(Color.DarkGray) { DashStyle = DashStyle.Dash }, 0, 0, MaxWidth, MaxHeight);
                title += "???";
            }
            
            SetTitle(title);
        }
    }
}
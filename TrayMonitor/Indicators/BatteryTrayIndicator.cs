using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Sensor.Core;
using TrayMonitor.Core.Indicators;

namespace TrayMonitor.Indicators
{
    public class BatteryTrayIndicator : TrayIndicator
    {
        private const int GREEN_LEVEL = 50;
        private const int YELLOW_LEVEL = 15;
                
        public BatteryTrayIndicator(NotifyIcon notifyIcon) : base(notifyIcon) {
            Update(null);
        }
        
        protected override void Draw(Graphics gfx) {
            const int w = 8;
            int h = MaxHeight;

            SensorState state = CurrentState;

            gfx.Clear(Color.Transparent);
            //gfx.DrawRectangle(new Pen(Pens.Gray, (Size.Width-w)/2, 0, w, h);            
            
            int ix = (MaxWidth - w)/2 + 1;
            int iy = 1;
            int iw = w - 1;
            int ih = h - 1;
            string title = string.IsNullOrEmpty(state?.Title) ? string.Empty : $"{state.Title}: ";
            Pen pen = Pens.Red;

            if (state != null) {
                if (state.Error != null) {
                    gfx.FillRectangle(Brushes.DimGray, ix, iy, iw, ih);
                    pen = new Pen(Color.Salmon, 1) { DashStyle = DashStyle.Solid };
                    title += state.Error.Message;
                }
                else {
                    if (state.Status == SensorStatus.Connecting) {
                        gfx.FillRectangle(Brushes.DimGray, ix, iy, iw, ih);
                        pen = new Pen(Color.Cyan, 1) { DashStyle = DashStyle.Dot };
                        title += "Connecting...";
                    }
                    else if (state.Status == SensorStatus.Disconnecting) {
                        gfx.FillRectangle(Brushes.DimGray, ix, iy, iw, ih);
                        pen = new Pen(Color.Fuchsia, 1) { DashStyle = DashStyle.Dot };
                        title += "Disconnecting...";
                    }
                    else if (state.Status == SensorStatus.Offline) {
                        gfx.FillRectangle(Brushes.Black, ix, iy, iw, ih);
                        pen = new Pen(Color.Gray, 1) { DashStyle = DashStyle.Dot };
                        title += "Offline";
                    }
                    else if (state.Status == SensorStatus.Online) {
                        double value = -1;
                        if (state.Value != null) {
                            try {
                                value = Convert.ToDouble(state.Value);
                                title += $"{value:F0}%";
                            }
                            catch (SystemException) {
                                title += state.Value;
                            }
                        }
                        else {
                            title += "???";
                        }

                        if (value < 0) {
                            gfx.FillRectangle(Brushes.LightGray, ix, iy, iw, ih);
                        }
                        else {
                            int nh = (int) (value * ih / 100.0);
                            int ny = iy + ih - nh;
                            
                            Brush brush = Brushes.Red;
                            if (value > GREEN_LEVEL) {
                                brush = Brushes.LimeGreen;
                            }
                            else if (value > YELLOW_LEVEL) {
                                brush = Brushes.Yellow;
                            }
                            // charge level
                            gfx.FillRectangle(brush, ix, ny, iw, nh);
                        }
                        pen = new Pen(Color.Gray, 1) { DashStyle = DashStyle.Solid };
                    }
                }
            }
            else {
                gfx.FillRectangle(Brushes.DarkGray, ix, iy, iw, ih);
                title += "???";
                pen = new Pen(Color.Gray, 1) { DashStyle = DashStyle.Dot };
            }

            // battery contour
            gfx.DrawRectangle(pen, (MaxWidth - w) / 2, 0, w, h);
            SetTitle(title);
        }
    }
}
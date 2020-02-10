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
        public BatteryTrayIndicator(NotifyIcon notifyIcon) : base(notifyIcon) {
            Update(null);
        }
        
        protected override void Draw(Graphics gfx, SensorData data) {
            const int w = 8;
            int h = Size.Height;

            gfx.Clear(Color.Transparent);
            //gfx.DrawRectangle(new Pen(Pens.Gray, (Size.Width-w)/2, 0, w, h);            
            
            int ix = (Size.Width - w)/2 + 1;
            int iy = 1;
            int iw = w - 1;
            int ih = h - 1;
            string title = string.IsNullOrEmpty(data?.Title) ? string.Empty : $"{data.Title}: ";
            Pen pen = Pens.Red;

            if (data != null) {
                if (data.Error != null) {
                    gfx.FillRectangle(Brushes.DimGray, ix, iy, iw, ih);
                    pen = new Pen(Color.Salmon, 1) { DashStyle = DashStyle.Solid };
                    title += data.Error.Message;
                }
                else {
                    if (data.State == SensorState.Connecting) {
                        gfx.FillRectangle(Brushes.DimGray, ix, iy, iw, ih);
                        pen = new Pen(Color.Cyan, 1) { DashStyle = DashStyle.Dot };
                        title += "Connecting...";
                    }
                    else if (data.State == SensorState.Disconnecting) {
                        gfx.FillRectangle(Brushes.DimGray, ix, iy, iw, ih);
                        pen = new Pen(Color.Fuchsia, 1) { DashStyle = DashStyle.Dot };
                        title += "Disconnecting...";
                    }
                    else if (data.State == SensorState.Offline) {
                        gfx.FillRectangle(Brushes.Black, ix, iy, iw, ih);
                        pen = new Pen(Color.Gray, 1) { DashStyle = DashStyle.Dot };
                        title += "Offline";
                    }
                    else if (data.State == SensorState.Online) {
                        double value = -1;
                        if (data.Value != null) {
                            try {
                                value = Convert.ToDouble(data.Value);
                                title += $"{value:F0}%";
                            }
                            catch (SystemException) {
                                title += data.Value;
                            }
                        }
                        else {
                            title += "???";
                        }

                        if (value < 0) {
                            gfx.FillRectangle(Brushes.LightGray, ix, iy, iw, ih);
                        }
                        else {
                            int nh = (int) (ih / 100.0 * value);
                            int ny = iy + ih - nh;
                            Brush brush = Brushes.Red;
                            if (value > 50) {
                                brush = Brushes.LimeGreen;
                            }
                            else if (value > 15) {
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
            gfx.DrawRectangle(pen, (Size.Width - w) / 2, 0, w, h);
            SetTitle(title);
        }
    }
}
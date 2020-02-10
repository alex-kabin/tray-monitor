using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Sensor.Core;
using TrayMonitor.Core.Indicators;

namespace TrayMonitor.Indicators
{
    public class LampTrayIndicator : TrayIndicator
    {
        public LampTrayIndicator(NotifyIcon notifyIcon) : base(notifyIcon) {
            Update(null);
        }
        
        protected override void Draw(Graphics gfx, SensorData data) {
            const float r = 7.1f;
            
            gfx.Clear(Color.Transparent);
            //gfx.DrawRectangle(new Pen(Pens.Gray, (Size.Width-w)/2, 0, w, h);            
            
            float ix = Size.Width/2f - r;
            float iy = Size.Height/2f - r;
            float iw = r * 2;
            float ih = r * 2;
            string title = string.IsNullOrEmpty(data?.Title) ? string.Empty : $"{data.Title}: ";
            
            gfx.SmoothingMode = SmoothingMode.AntiAlias;
            if (data != null) {
                if (data.Error != null) {
                    //gfx.FillEllipse(Brushes.DimGray, ix, iy, iw, ih);
                    gfx.DrawEllipse(new Pen(Color.Red, 2f), ix, iy,iw, ih);
                    title += data.Error.Message;
                }
                else {
                    if (data.State == SensorState.Connecting) {
                        //gfx.FillEllipse(Brushes.DimGray, ix, iy, iw, ih);
                        gfx.DrawEllipse(new Pen(Color.Cyan, 2), ix, iy,iw, ih);
                        title += "Connecting...";
                    }
                    else if (data.State == SensorState.Disconnecting) {
                        //gfx.FillEllipse(Brushes.DimGray, ix, iy, iw, ih);
                        gfx.DrawEllipse(new Pen(Color.Fuchsia, 2), ix, iy,iw, ih);
                        title += "Disconnecting...";
                    }
                    else if (data.State == SensorState.Offline) {
                        gfx.FillEllipse(Brushes.Black, ix, iy, iw, ih);
                        gfx.DrawEllipse(new Pen(Color.DarkGray, 2), ix, iy,iw, ih);
                        title += "Offline";
                    }
                    else if (data.State == SensorState.Online) {
                        bool? value = null;
                        if (data.Value != null) {
                            try {
                                value = Convert.ToBoolean(data.Value);
                            }
                            catch (SystemException) {
                                title += data.Value;
                            }
                        }
                        else {
                            title += "???";
                        }

                        Brush brush = Brushes.Gray;
                        if (value != null) {
                            brush = true == value ? Brushes.LimeGreen : Brushes.Red;
                        }
                        gfx.FillEllipse(brush, ix, iy, iw, ih);
                    }
                }
            }
            else {
                gfx.FillEllipse(Brushes.DarkGray, ix, iy, iw, ih);
                title += "???";
            }
            
            SetTitle(title);
        }
    }
}
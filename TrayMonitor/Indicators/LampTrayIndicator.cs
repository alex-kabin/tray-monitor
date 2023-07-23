using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Sensor.Core;
using TrayMonitor.Core.Indicators;

namespace TrayMonitor.Indicators
{
    public class LampTrayIndicator : TrayIndicatorBase
    {
        public LampTrayIndicator(NotifyIcon notifyIcon) : base(notifyIcon) {
            Update(null);
        }
        
        protected override void Draw(Graphics gfx) {
            const float r = 7.15f;

            SensorState state = CurrentState;
            
            gfx.Clear(Color.Transparent);
            //gfx.DrawRectangle(new Pen(Pens.Gray, (Size.Width-w)/2, 0, w, h);            
            
            float ix = MaxWidth/2f - r;
            float iy = MaxHeight/2f - r;
            float iw = r * 2;
            float ih = r * 2;
            string title = string.IsNullOrEmpty(state?.Title) ? string.Empty : $"{state.Title}: ";
            
            gfx.SmoothingMode = SmoothingMode.AntiAlias;
            if (state != null) {
                if (state.Error != null) {
                    //gfx.FillEllipse(Brushes.DimGray, ix, iy, iw, ih);
                    gfx.DrawEllipse(new Pen(Color.Red, 2f), ix, iy,iw, ih);
                    title += state.Error.Message;
                }
                else {
                    if (state.Status == SensorStatus.Connecting) {
                        //gfx.FillEllipse(Brushes.DimGray, ix, iy, iw, ih);
                        gfx.DrawEllipse(new Pen(Color.Cyan, 2), ix, iy,iw, ih);
                        title += "Connecting...";
                    }
                    else if (state.Status == SensorStatus.Disconnecting) {
                        //gfx.FillEllipse(Brushes.DimGray, ix, iy, iw, ih);
                        gfx.DrawEllipse(new Pen(Color.Fuchsia, 2), ix, iy,iw, ih);
                        title += "Disconnecting...";
                    }
                    else if (state.Status == SensorStatus.Offline) {
                        gfx.FillEllipse(Brushes.Black, ix, iy, iw, ih);
                        gfx.DrawEllipse(new Pen(Color.DarkGray, 2), ix, iy,iw, ih);
                        title += "Offline";
                    }
                    else if (state.Status == SensorStatus.Online) {
                        bool? value = null;
                        if (state.Value != null) {
                            try {
                                value = Convert.ToBoolean(state.Value);
                                title += value == true ? "ON" : "OFF";
                            }
                            catch (SystemException) {
                                title += state.Value;
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
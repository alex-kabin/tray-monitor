using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace TrayMonitor.Core.Icons
{
    static class IconHelper
    {
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = CharSet.Auto)]
        extern static bool DestroyIcon(IntPtr handle);

        public static Icon BitmapToIcon(Bitmap bitmap) {
            return Icon.FromHandle(bitmap.GetHicon());
        }

        public static bool Destroy(this Icon icon) {
            if (icon == null) {
                return false;
            }
            if (icon.Handle != IntPtr.Zero) {
                DestroyIcon(icon.Handle);
            }
            icon.Dispose();
            return true;
        }
    }
}

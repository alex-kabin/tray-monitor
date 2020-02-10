using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace TrayMonitor.Utils
{
    public class TextBoxHideCaretBehavior : IDisposable
    {
        [DllImport("user32.dll", EntryPoint = "HideCaret")]
        private static extern int HideCaret(IntPtr hwnd);
        
        private RichTextBox _rtb;

        public TextBoxHideCaretBehavior(RichTextBox rtb) {
            _rtb = rtb;
            _rtb.GotFocus += RichTextBoxEvent;
            _rtb.Enter += RichTextBoxEvent;
            //_rtb.Resize += RichTextBoxEvent;
            _rtb.MouseDown += RichTextBoxEvent;
            _rtb.MouseUp += RichTextBoxEvent;
            //_rtb.ContentsResized += RichTextBoxEvent;
            //_rtb.TextChanged += RichTextBoxEvent;
        }
        
        // private void RichTextBoxGotFocusEvent(object sender, EventArgs e) {
        //     var ctrl = (Control) sender;
        //     HideCaret(ctrl.Handle);
        //     ctrl.Parent?.Focus();
        // }
        
        private void RichTextBoxEvent(object sender, EventArgs e) {
            HideCaret(((Control)sender).Handle);
        }

        public void Dispose() {
            if (_rtb != null) {
                _rtb.GotFocus -= RichTextBoxEvent;
                _rtb.Enter -= RichTextBoxEvent;
                //_rtb.Resize -= RichTextBoxEvent;
                _rtb.MouseDown -= RichTextBoxEvent;
                _rtb.MouseUp -= RichTextBoxEvent;
                //_rtb.ContentsResized -= RichTextBoxEvent;
                //_rtb.TextChanged -= RichTextBoxEvent;
                _rtb = null;
            }
        }
    }
}
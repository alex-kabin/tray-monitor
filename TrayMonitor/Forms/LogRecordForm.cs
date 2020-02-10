using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Logging.Core;
using TrayMonitor.Utils;

namespace TrayMonitor
{
    public partial class LogRecordForm : Form
    {
        public LogRecordForm() {
            InitializeComponent();
            new TextBoxHideCaretBehavior(messageRichTextBox);
            new TextBoxHideCaretBehavior(detailsRichTextBox);
        }

        public void Bind(LogRecord record) {
            var time = record.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var level = record.LogLevel.ToString().ToUpper();
            Text = $"{time} {level}";
            levelLabel.Text = level;
            switch (record.LogLevel) {
                case LogLevel.Debug:
                    levelLabel.ForeColor = Color.DarkGray;
                    break;
                case LogLevel.Info:
                    levelLabel.ForeColor = Color.Blue;
                    break;
                case LogLevel.Warn:
                    levelLabel.ForeColor = Color.Yellow;
                    break;
                case LogLevel.Error:
                    levelLabel.ForeColor = Color.Red;
                    break;
            }
            timeLabel.Text = time;
            threadLabel.Text = string.IsNullOrEmpty(record.ThreadName) ? record.ThreadId.ToString() : $"[{record.ThreadId}] {record.ThreadName}";
            loggerLabel.Text = record.LoggerName;
            messageRichTextBox.Text = record.Message;
            detailsRichTextBox.Text = record.Details?.ToString() ?? string.Empty;
        }

        protected override void OnShown(EventArgs e) {
            base.OnShown(e);
            ActiveControl = null;
        }

        protected override void OnKeyUp(KeyEventArgs e) {
            base.OnKeyUp(e);
            if (e.KeyCode == Keys.Escape) {
                Close();
            }
        }
    }
}
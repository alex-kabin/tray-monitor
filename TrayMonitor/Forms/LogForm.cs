using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Logging.Core;

namespace TrayMonitor
{
    public partial class LogForm : Form
    {
        private ICollection<LogRecord> _log;

        public LogForm() {
            InitializeComponent();
            Text = $"{AppInfo.Title}: Log";
            listView1.MouseDoubleClick += ListView1OnMouseDoubleClick;
        }

        private void ListView1OnMouseDoubleClick(object sender, MouseEventArgs e) {
            var item = listView1.GetItemAt(e.X, e.Y);
            if (item?.Tag is LogRecord logRecord) {
                var logRecordForm = new LogRecordForm();
                logRecordForm.Bind(logRecord);
                logRecordForm.ShowDialog();
            }
        }

        public void Bind(ICollection<LogRecord> log) {
            if (_log is INotifyCollectionChanged) {
                ((INotifyCollectionChanged)_log).CollectionChanged -= OnLogCollectionChanged;
            }

            _log = log;
            
            listView1.BeginUpdate();
            listView1.Items.Clear();
            if (_log != null) {
                foreach (var record in _log) {
                    listView1.Items.Add(BuildLogRecordListItem(record));
                }
            }
            listView1.EndUpdate();
            ScrollToLastItem();

            if (_log is INotifyCollectionChanged) {
                ((INotifyCollectionChanged)_log).CollectionChanged += OnLogCollectionChanged;
            }
        }

        private void OnLogCollectionChanged(object sender, NotifyCollectionChangedEventArgs ea) {
            if (InvokeRequired) {
                BeginInvoke(new NotifyCollectionChangedEventHandler(this.OnLogCollectionChanged), sender, ea);
                return;
            }

            listView1.BeginUpdate();
            if (ea.Action == NotifyCollectionChangedAction.Reset) {
                listView1.Items.Clear();
                foreach (var record in (ICollection<LogRecord>)sender) {
                    listView1.Items.Add(BuildLogRecordListItem(record));
                }
            }
            else if (ea.Action == NotifyCollectionChangedAction.Add) {
                if (ea.NewItems.Count > 0) {
                    if (ea.NewStartingIndex >= listView1.Items.Count - 1) {
                        foreach (var record in ea.NewItems.OfType<LogRecord>()) {
                            listView1.Items.Add(BuildLogRecordListItem(record));
                        }
                    }
                    else {
                        for (int i = 0; i < ea.NewItems.Count; i++) {
                            listView1.Items.Insert(
                                    ea.NewStartingIndex + i,
                                    BuildLogRecordListItem((LogRecord) ea.OldItems[i])
                            );
                        }
                    }
                }
            }
            else if (ea.Action == NotifyCollectionChangedAction.Remove) {
                if (ea.OldItems.Count > 0) {
                    var offset = ea.OldItems.Count - 1;
                    do {
                        listView1.Items.RemoveAt(ea.OldStartingIndex + offset);
                    } while (--offset >= 0);
                }
            }
            listView1.EndUpdate();
            ScrollToLastItem();
        }
       
        private void ScrollToLastItem() {
            var count = listView1.Items.Count;
            if (count > 0) {
                listView1.EnsureVisible(count - 1);
            }
        }

        private ListViewItem BuildLogRecordListItem(LogRecord logRecord) {
            var listViewItem = new ListViewItem {
                Tag = logRecord,
                Text = logRecord.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                Name = logRecord.Timestamp.Ticks.ToString()
            };

            ListViewItem.ListViewSubItem si;

            si = listViewItem.SubItems.Add(logRecord.LogLevel.ToString().ToUpperInvariant());

            si = listViewItem.SubItems.Add(logRecord.ThreadId.ToString());
            
            si = listViewItem.SubItems.Add(logRecord.LoggerName);
            
            si = listViewItem.SubItems.Add(logRecord.Message);
            
            if (logRecord.Details != null) {
                var details = logRecord.Details.ToString();
                si = listViewItem.SubItems.Add(details);
            }
            
            Color foreColor = Color.Black;
            Color backColor = Color.White;
            switch (logRecord.LogLevel) {
                case LogLevel.Debug: {
                    foreColor = Color.Gray;
                    break;
                }
                case LogLevel.Info: {
                    foreColor = Color.DarkBlue;
                    break;
                }
                case LogLevel.Warn: {
                    foreColor = Color.Black;
                    backColor = Color.LightYellow;
                    break;
                }
                case LogLevel.Error: {
                    foreColor = Color.Black;
                    backColor = Color.LightPink;
                    break;
                }
            }

            listViewItem.BackColor = backColor;
            listViewItem.ForeColor = foreColor;
            foreach (ListViewItem.ListViewSubItem subItem in listViewItem.SubItems) {
                subItem.BackColor = backColor;
                subItem.ForeColor = foreColor;
            }

            return listViewItem;
        }
        
        protected override void OnShown(EventArgs e) {
            base.OnShown(e);
            ScrollToLastItem();
        }

        protected override void OnClosed(EventArgs e) {
            base.OnClosed(e);
            Bind(null);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Logging.Core;
using Sensor.Core;
using TrayMonitor.Commands;
using TrayMonitor.Core.Indicators;
using TrayMonitor.Indicators;

namespace TrayMonitor
{
    /// <summary>
    /// This class should be created and passed into Application.Run( ... )
    /// </summary>
    public class App : ApplicationContext
    {
        private readonly IConfiguration _configuration;
        private System.ComponentModel.IContainer components;	// a list of components to dispose when the context is disposed
        private NotifyIcon _notifyIcon;				            // the icon that sits in the system tray
        private Monitor _monitor;
        private LogForm _logForm;
        private JournalLog _journalLog;
        private ToggleCommand _logCommand;

        public App(IConfiguration configuration) {
            _configuration = configuration;
            InitLogger();
            InitializeContext();
        }

        private void InitLogger() {
            var logs = new ILog[] {
                    new DiagnosticsLog(DefaultLogRecordFormatter.Instance),
                    _journalLog = new JournalLog(2000)
            };
            LoggerFactory.Loggers(
                    name => new DefaultLogger(name, LogLevel.Debug, logs),
                    name => new NLogLogger(name)
            );
        }

        private void InitializeContext() {
            components = new System.ComponentModel.Container();
            
            CreateNotifyIcon();
            CreateMonitor();

            _logCommand = new ToggleCommand(
                new ActionCommand("Show Log", () => {
                                                  _logForm = new LogForm();
                                                  _logForm.Bind(_journalLog);
                                                  _logForm.Show();
                                                  _logForm.Closed += (s, ea) => _logCommand.Reset();
                                                  _logCommand.Toggle();
                                              }
                ),
                new ActionCommand("Hide Log", () => {
                                                  _logForm?.Close();
                                              }
                ),
                false
            );

            if (_configuration.AutoConnect) {
                SynchronizationContext.Current.Post(_ => _monitor.ControlCommand.Execute(), null);
            }
        }

        private void CreateNotifyIcon() {
            _notifyIcon = new NotifyIcon(components) {
                ContextMenuStrip = new ContextMenuStrip(),
                Visible = true
            };
            _notifyIcon.ContextMenuStrip.Opening += ContextMenuStrip_Opening;
            _notifyIcon.DoubleClick += (sender, ea) => {
                                           _monitor?.ControlCommand.Execute();
                                       };

            var showContextMenu =  typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
            _notifyIcon.MouseUp += (sender, ea) => {
                                       if (ea.Button == MouseButtons.Right) {
                                           showContextMenu?.Invoke(_notifyIcon, null);
                                       }
                                   };
        }

        private void CreateMonitor() {
            var objectBuilder = new ObjectBuilder();

            var sensor = objectBuilder.Create<ISensor>(_configuration.SensorTypeName);
            (sensor as IConfigurable)?.Configure(_configuration.SensorProperties);
            
            var indicator = objectBuilder.Create<IIndicator>(_configuration.IndicatorTypeName, _notifyIcon);
            (indicator as IConfigurable)?.Configure(_configuration.IndicatorProperties);
            
            _monitor = new Monitor(sensor, indicator);
            components.Add(_monitor);
        }

        private bool _contextMenuReady = false;

        private void ContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e) {
            e.Cancel = false;

            if (_contextMenuReady) {
                return;
            }
            _contextMenuReady = true;

            new ToolStripItemCommandBinding(_notifyIcon.ContextMenuStrip.Items.Add("1"), _monitor.ControlCommand);
            _notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            new ToolStripItemCommandBinding(_notifyIcon.ContextMenuStrip.Items.Add("1"), _logCommand);
            _notifyIcon.ContextMenuStrip.Items.Add("About", null, about_Click);
            _notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            _notifyIcon.ContextMenuStrip.Items.Add("Exit", null, exit_Click);
        }
        
        private void about_Click(object sender, EventArgs e) {
            new AboutForm().ShowDialog();
        }
        
        /// <summary>
		/// When the application context is disposed, dispose things like the notify icon.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing) {
            if (disposing) { components?.Dispose(); }
        }

        /// <summary>
        /// When the exit menu item is clicked, make a call to terminate the ApplicationContext.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exit_Click(object sender, EventArgs e) {
            ExitThread();
        }

        /// <summary>
        /// If we are presently showing a form, clean it up.
        /// </summary>
        protected override void ExitThreadCore()
        {
            _notifyIcon.Visible = false; // should remove lingering tray icon
            base.ExitThreadCore();
        }
        
    }
}
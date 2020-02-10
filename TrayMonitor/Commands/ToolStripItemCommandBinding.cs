using System;
using System.Windows.Forms;

namespace TrayMonitor.Commands
{
    public class ToolStripItemCommandBinding : IDisposable
    {
        private ToolStripItem _toolStripItem;
        private ICommand _command;

        public ToolStripItemCommandBinding(ToolStripItem toolStripItem, ICommand command) {
            _toolStripItem = toolStripItem ?? throw new ArgumentNullException(nameof(toolStripItem));
            _command = command ?? throw new ArgumentNullException(nameof(command));
            
            _command.CanExecuteChanged += OnCommandCanExecuteChanged;
            _command.NameChanged += OnCommandNameChanged;
            
            _toolStripItem.Click += OnToolStripItemClicked;
            
            _toolStripItem.Enabled = _command.CanExecute;
            _toolStripItem.Text = _command.Name;
        }

        private void OnCommandNameChanged(object sender, EventArgs e) {
            _toolStripItem.Text = _command.Name;
        }

        private void OnToolStripItemClicked(object sender, EventArgs e) {
            _command.Execute();
        }

        private void OnCommandCanExecuteChanged(object sender, EventArgs e) {
            _toolStripItem.Enabled = _command.CanExecute;
        }

        public void Dispose() {
            if (_command != null) {
                _command.CanExecuteChanged -= OnCommandCanExecuteChanged;
                _command.NameChanged -= OnCommandNameChanged;
                _command = null;
            }

            if (_toolStripItem != null) {
                _toolStripItem.Click -= OnToolStripItemClicked;
                _toolStripItem = null;
            }
        }
    }
}
using System;

namespace TrayMonitor.Commands
{
    public class ToggleCommand : ICommand, IDisposable
    {
        private readonly ICommand _command1;
        private readonly ICommand _command2;
        private readonly bool _autoToggle;
        private ICommand _command;

        public ToggleCommand(ICommand command1, ICommand command2, bool autoToggle = true) {
            _command1 = command1;
            _command2 = command2;
            _autoToggle = autoToggle;
            Reset();
        }

        public void Toggle() {
            SetCommand(ReferenceEquals(_command, _command1) ? _command2 : _command1);
        }

        private void SetCommand(ICommand command) {
            if (ReferenceEquals(_command, command)) {
                return;
            }
            if (_command != null) {
                _command.NameChanged -= OnCommandNameChanged;
                _command.CanExecuteChanged -= OnCommandCanExecuteChanged;
            }
            _command = command;
            _command.NameChanged += OnCommandNameChanged;
            _command.CanExecuteChanged += OnCommandCanExecuteChanged;
            NameChanged?.Invoke(this, EventArgs.Empty);
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Reset() {
            SetCommand(_command1);
        }
        
        private void OnCommandCanExecuteChanged(object sender, EventArgs e) {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnCommandNameChanged(object sender, EventArgs e) {
            NameChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Execute() {
            if (_command == null) {
                return;
            }
            if (_command.CanExecute) {
                _command.Execute();
                if (_autoToggle) {
                    Toggle();
                }
            }
        }

        public string Name => _command?.Name;

        public event EventHandler NameChanged;

        public bool CanExecute => _command?.CanExecute ?? false;

        public event EventHandler CanExecuteChanged;
        
        public void Dispose() {
            if (_command != null) {
                _command.NameChanged -= OnCommandNameChanged;
                _command.CanExecuteChanged -= OnCommandCanExecuteChanged;
                _command = null;
            }
        }
    }
}
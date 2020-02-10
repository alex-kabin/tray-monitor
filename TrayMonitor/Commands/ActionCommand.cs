using System;

namespace TrayMonitor.Commands
{
    public class ActionCommand : ICommand
    {
        private readonly Action _action;
        
        public ActionCommand(string name, Action action) {
            _name = name;
            _canExecute = true;
            _action = action;
        }

        public void Execute() {
            if (CanExecute) {
                _action?.Invoke();
            }
        }

        private string _name;
        public string Name {
            get => _name;
            set {
                _name = value; 
                NameChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler NameChanged;

        private bool _canExecute;
        public bool CanExecute {
            get => _canExecute;
            set {
                if (value != _canExecute) {
                    _canExecute = value;
                    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                }
            } 
        }

        public event EventHandler CanExecuteChanged;
    }
}
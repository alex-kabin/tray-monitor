using System;

namespace TrayMonitor.Commands
{
    public interface ICommand
    {
        void Execute();
        
        string Name { get; }
        event EventHandler NameChanged;
        
        bool CanExecute { get; }
        event EventHandler CanExecuteChanged;
    }
}
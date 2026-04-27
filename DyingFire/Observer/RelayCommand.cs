using System;
using System.Windows.Input;

namespace DyingFire.ViewModels
{
    // A generic command that delegates execution and can-execute checks to the provided delegates.
    // The view model supplies the logic to run and optionally a predicate that enables/disables the command.
    public class RelayCommand<T> : ICommand
    {
        // The action to run when Execute is called.
        private readonly Action<T> _execute;
        // The predicate used to determine if the command can execute right now.
        private readonly Predicate<T> _canExecute;

        // The constructor requires an execute delegate and accepts an optional canExecute predicate.
        // View models pass methods or lambdas here to define what the command does.
        public RelayCommand(Action<T> execute, Predicate<T> canExecute = null)
        {
            // Validate that an execute delegate is provided because commands without execution are not useful.
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        // Called by WPF to ask if the command can run.
        // The parameter comes from the UI. It is cast to T here.
        // If no predicate was supplied we always allow execution.
        public bool CanExecute(object parameter) => _canExecute == null || _canExecute((T)parameter);

        // Called by WPF when the user triggers the command.
        // The parameter comes from the UI. It is cast to T and passed to the stored execute delegate.
        public void Execute(object parameter) => _execute((T)parameter);

        // WPF listens to this event to know when it should re-query CanExecute for controls bound to this command.
        // This implementation hooks into CommandManager.RequerySuggested so WPF will automatically re-check
        // command availability on common UI events (focus change, keyboard activity, timers).
        // View models can also raise this event indirectly by toggling state that causes CommandManager to requery.
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
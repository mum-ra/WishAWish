using System;
using System.Windows.Input;

namespace WishAWish.ViewModels
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _exec;
        private readonly Func<object, bool> _can;
        public RelayCommand(Action exec, Func<bool> can = null)
            : this(_ => exec(), can == null ? (Func<object, bool>)null : _ => can()) { }
        public RelayCommand(Action<object> exec, Func<object, bool> can = null) { _exec = exec; _can = can; }
        public bool CanExecute(object p) => _can == null || _can(p);
        public void Execute(object p) => _exec(p);
        public event EventHandler CanExecuteChanged { add { CommandManager.RequerySuggested += value; } remove { CommandManager.RequerySuggested -= value; } }
    }
}

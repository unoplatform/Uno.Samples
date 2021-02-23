using System;
using System.Windows.Input;

namespace TimeEntryRia.MVVM
{
    /// <summary>
    /// This class was presented in the book:
    /// Pro Business Applications with Silverlight 5 by Chris Anderson (Apress, 2012).
    /// https://www.apress.com/us/book/9781430235002
    /// </summary>
    public class DelegateCommand : ICommand
    {
        private Func<object, bool> canExecute;
        private Action<object> executeAction;
        private bool canExecuteCache;

        public DelegateCommand(Action<object> executeAction, Func<object, bool> canExecute)
        {
            this.executeAction = executeAction;
            this.canExecute = canExecute;
        }

        #region ICommand Members
        public bool CanExecute(object parameter)
        {
            bool temp = canExecute(parameter);

            if (canExecuteCache != temp)
            {
                canExecuteCache = temp;
                if (CanExecuteChanged != null)
                {
                    CanExecuteChanged(this, new EventArgs());
                }
            }

            return canExecuteCache;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            executeAction(parameter);
        }
        #endregion
    }
}
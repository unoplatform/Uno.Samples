using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChatUI.Commands;

public class RelayCommand : ICommand
{
	private Action<string> _execute;

	public RelayCommand(Action<string> execute)
	{
		_execute = execute;
	}

	public event EventHandler CanExecuteChanged;

	public bool CanExecute(object parameter)
	{
		return true;
	}

	public void Execute(object parameter)
	{
		_execute((string)parameter);
	}
}
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Binding;

public class MainViewModel : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler? PropertyChanged;

	private ObservableCollection<string>? _items;
	public ObservableCollection<string>? Items
	{
		get { return _items; }
		set
		{
			_items = value;
			OnPropertyChanged(nameof(Items));
		}
	}

	public ICommand RemoveItemCommand { get; }

	public MainViewModel()
	{
		Items =
		[
			"Item 1",
			"Item 2",
			"Item 3",
			"Item 4"
		];

		RemoveItemCommand = new RelayCommand(RemoveItem);
	}

	private void RemoveItem(object? obj)
	{
		if (obj is string itemToRemove && Items is { Count: > 0})
		{
			Items.Remove(itemToRemove);
		}
	}

	protected virtual void OnPropertyChanged(string propertyName)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}

public class RelayCommand : ICommand
{
	private readonly Action<object?> _execute;
	public RelayCommand(Action<object?> execute) => _execute = execute;
	public static ICommand Create(Action<object?> execute) => new RelayCommand(execute);
	public event EventHandler? CanExecuteChanged;
	public bool CanExecute(object? parameter) => true;
	public void Execute(object? parameter) => _execute?.Invoke(parameter);
}
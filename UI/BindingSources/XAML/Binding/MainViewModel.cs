using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Binding;

public class MainViewModel : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler? PropertyChanged;

	private ObservableCollection<Item>? _items;
	public ObservableCollection<Item>? Items
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
		Items = new ObservableCollection<Item>
		{
			new Item { Text = "Item 1" },
			new Item { Text = "Item 2" },
			new Item { Text = "Item 3" }
		};

		RemoveItemCommand = new RelayCommand(RemoveItem);
	}

	private void RemoveItem(object? obj)
	{
		if (obj is Item itemToRemove && Items is { Count: > 0 })
		{
			Items.Remove(itemToRemove);
		}
	}

	protected virtual void OnPropertyChanged(string propertyName)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}

public class Item
{
	public string? Text { get; set; }
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
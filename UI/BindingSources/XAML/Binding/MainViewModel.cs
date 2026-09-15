namespace Binding;

public partial class MainViewModel : ObservableObject
{
	[ObservableProperty]
	private ObservableCollection<Item> items;

	public ICommand RemoveItemCommand { get; }

	public MainViewModel()
	{
		Items =
		[
			new Item { Text = "Item 1" },
			new Item { Text = "Item 2" },
			new Item { Text = "Item 3" }
		];

		RemoveItemCommand = new RelayCommand<Item>(RemoveItem, i => i is not null && Items.Contains(i));
	}

	private void RemoveItem(Item? item)
	{
		Items.Remove(item!);
	}
}

public class Item
{
	public string? Text { get; set; }
}
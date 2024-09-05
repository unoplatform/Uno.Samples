namespace MVUX.Presentation.FeedViewCommandSample;

public partial class FeedViewCommandModel
{
    public IListState<Item> Items => ListState.Value(this, () =>
    {
        return ImmutableList.Create(
            new Item { Text = "Item 1" },
            new Item { Text = "Item 2" },
            new Item { Text = "Item 3" }
            );
    });

    public async ValueTask RemoveItem(Item item)
    {
        if (item != null)
        {
            await Items.UpdateAsync(currentItems =>
            {
                var updatedItems = currentItems.Remove(item); 
                return updatedItems;
            });
        }
    }
}

public class Item
{
    public string Text { get; set; }
}

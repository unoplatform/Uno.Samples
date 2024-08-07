namespace Navigation.Presentation;

public class BreadcrumbTemplateSelector : DataTemplateSelector
{
	public DataTemplate DefaultTemplate { get; set; }
	public DataTemplate LastItemTemplate { get; set; }

	protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
	{
		var listViewItem = container as FrameworkElement;
		if (listViewItem != null && item is string breadcrumb)
		{
			var itemsControl = ItemsControl.ItemsControlFromItemContainer(listViewItem);
			var index = itemsControl.Items.IndexOf(item);

			if (index == itemsControl.Items.Count - 1)
			{
				return LastItemTemplate;
			}
		}
		return DefaultTemplate;
	}
}


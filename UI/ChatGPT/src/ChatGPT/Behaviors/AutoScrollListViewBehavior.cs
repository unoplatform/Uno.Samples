using System.Collections.Specialized;
using Uno.UI.Extensions;

namespace ChatGPT;
public partial class AutoScrollListViewBehavior : DependencyObject
{
	public static readonly DependencyProperty AutoScrollProperty =
		DependencyProperty.RegisterAttached("AutoScroll", typeof(bool), typeof(AutoScrollListViewBehavior), new PropertyMetadata(false, OnAutoScrollPropertyChanged));

	public static bool GetAutoScroll(ListView listView)
	{
		return (bool)listView.GetValue(AutoScrollProperty);
	}

	public static void SetAutoScroll(ListView listView, bool value)
	{
		listView.SetValue(AutoScrollProperty, value);
	}

	private static void OnAutoScrollPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is ListView listView && e.NewValue is bool isEnabled && isEnabled)
		{
			// TODO: Support unregister
			listView.RegisterPropertyChangedCallback(ListView.ItemsSourceProperty, OnItemSourceChanged);
		}
	}

	private static void OnItemSourceChanged(DependencyObject sender, DependencyProperty dp)
	{
		if (sender is not ListView listView)
		{
			return;
		}

		// TODO: Unsubscribe from previous collection
		if (listView.ItemsSource is INotifyCollectionChanged items)
		{
			items.CollectionChanged += (snd, e) =>
			{
				if (e.Action is NotifyCollectionChangedAction.Add)
				{
					ScrollToBottom(listView);
				}
				else if (e.Action is NotifyCollectionChangedAction.Replace
					&& e.NewStartingIndex == listView.Items.Count - 1
					&& listView.FindFirstDescendant<ScrollViewer>() is { } scroller
					&& (scroller.ScrollableHeight - scroller.VerticalOffset) < Math.Min(300, scroller.ViewportHeight / 4))
				{
					ScrollToBottom(listView);
				}
			};
		}
	}

	private static void ScrollToBottom(ListView listView)
	{
		listView.DispatcherQueue.TryEnqueue(() =>
		{
			var scroller = listView.FindFirstDescendant<ScrollViewer>();
			scroller?.ChangeView(null, scroller.ExtentHeight, null);
		});
	}
}
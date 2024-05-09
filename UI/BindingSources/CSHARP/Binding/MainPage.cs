namespace Binding;

public sealed partial class MainPage : Page
{
	public MainPage()
	{
		this.DataContext(new MainViewModel(), (page, vm)
			=> page
			.Background(Theme.Brushes.Background.Default)
			.Content(
				new StackPanel()
					.VerticalAlignment(VerticalAlignment.Center)
					.HorizontalAlignment(HorizontalAlignment.Center)
					.Children(
						new ListView()
							.ItemsSource(() => vm.Items)
							.ItemTemplate<string>(item =>
								new StackPanel()
									.Spacing(8)
									.Orientation(Orientation.Horizontal)
									.Children(
										new TextBlock()
											.Text(() => item)
											.VerticalAlignment(VerticalAlignment.Center),
										new Button()
											.Content("Delete")
											.CommandParameter(() => item)
											.Command(x => x.Source(page)
															.DataContext()
															.Binding(() => vm.RemoveItemCommand))
									)
									.ContextFlyout(
										MenuFlyout(item)
									)
							)
					)
			)
		);
	}

	private MenuFlyout MenuFlyout(string item)
		=> new MenuFlyout()
				.Items(
					new MenuFlyoutItem()
						.Text("Delete")
						.CommandParameter(() => item)
						.Command(x => x.Source(this)
										.DataContext<MainViewModel>()
										.Binding(vm => vm.RemoveItemCommand))
				);
}
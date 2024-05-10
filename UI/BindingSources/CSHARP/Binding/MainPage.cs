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
							.ItemTemplate<Item>(item =>
								new StackPanel()
									.Spacing(8)
									.Orientation(Orientation.Horizontal)
									.Children(
										new TextBlock()
											.Text(() => item.Text)
											.VerticalAlignment(VerticalAlignment.Center),
										new Button()
											.Content("Delete")
											.CommandParameter(() => item)
											.Command(x => x.Source(page)
															.DataContext()
															.Binding(() => vm.RemoveItemCommand)
											)
										// Alternatively we could extract the button instance to a helper method
										// And take advantage of the RelativeSource method to provide the CommandParameter
										//, CreateButton()
									)
									.ContextFlyout(
										MenuFlyout()
									)
							)
					)
			)
		);
	}

	private Button CreateButton()
		=> new Button()
			   .Content("Delete")
			   // RelativeSource as TemplatedParent works on Windows but not on Uno
			   // RelativeSource as Self works on Uno but not on Windows
			   .CommandParameter(x => x.RelativeSource<Button>(RelativeSourceMode.Self)
									   .Binding(y => y.DataContext)
			   )
			   .Command(x => x.Source(this)
								.DataContext<MainViewModel>()
								.Binding(vm => vm.RemoveItemCommand)
			   );

	private MenuFlyout MenuFlyout()
		=> new MenuFlyout()
				.Items(
					new MenuFlyoutItem()
						.Text("Delete")
						// Fix on Windows on the way
						.CommandParameter(x => x.Binding())
						.Command(x => x.Source(this)
										.DataContext<MainViewModel>()
										.Binding(vm => vm.RemoveItemCommand)
						)
				);
}
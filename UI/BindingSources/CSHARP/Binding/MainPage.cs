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
											.Command(x => x
														   // Since we have access to the `page` and `vm` alias from the DataContext method
														   // We can take advantage of them and use them on our binding expression
														   .Source(page)
														   .DataContext()
														   .Binding(() => vm.RemoveItemCommand)
											),

										// Alternatively we could extract the button instance to a helper method
										// And take advantage of the RelativeSource method to provide the CommandParameter
										CreateButton()
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
			   // Should be fixed by https://github.com/unoplatform/uno/pull/12732
			   .CommandParameter(x => x.RelativeSource<Button>(RelativeSourceMode.TemplatedParent)
									   .Binding(btn => btn.DataContext)
			   )
			   .Command(x => x
							  // Since we we don't have access to the page alias here (as we have on line #30)
							  // We need to set `this` as the source
							  .Source(this)

							  // Here we specify the DataContext type so that we can access the ViewModel alias in the Binding method
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
						.Command(x => x
									   // Since we we don't have access to the page alias here (as we have on line #30)
									   // We need to set `this` as the source
									   .Source(this)

									   // Here we specify the DataContext type so that we can access the ViewModel alias in the Binding method
									   .DataContext<MainViewModel>()
									   .Binding(vm => vm.RemoveItemCommand)
						)
				);
}
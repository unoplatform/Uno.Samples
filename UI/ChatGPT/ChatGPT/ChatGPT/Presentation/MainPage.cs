namespace ChatGPT.Presentation;

public sealed partial class MainPage : Page
{
	private const string PowerIcon = "M8.66667 2H7.33333V8.66667H8.66667V2ZM11.8867 3.44667L10.94 4.39333C11.9933 5.24 12.6667 6.54 12.6667 8C12.6667 10.58 10.58 12.6667 8 12.6667C5.42 12.6667 3.33333 10.58 3.33333 8C3.33333 6.54 4.00667 5.24 5.05333 4.38667L4.11333 3.44667C2.82 4.54667 2 6.17333 2 8C2 11.3133 4.68667 14 8 14C11.3133 14 14 11.3133 14 8C14 6.17333 13.18 4.54667 11.8867 3.44667Z";

	public MainPage()
	{
		this.Background(ThemeResource.Get<Brush>("ApplicationPageBackgroundThemeBrush"))
			.DataContext<BindableMainModel>((page, vm) => page
				.Content(
					new Grid()
						.BorderBrush(Colors.Gray)
						.BorderThickness(1)
						.CornerRadius(10)
						.MaxWidth(500)
						.Padding(10)
						.Margin(10)
						.RowDefinitions<Grid>("Auto,*,Auto")
						.Children(
							Header(vm),
							Messages(vm),
							Prompt(vm)
					)
				)
			);
	}

	private ToggleButton Header(BindableMainModel vm)
		=> new ToggleButton()
			.HorizontalAlignment(HorizontalAlignment.Center)
			.Visibility(x => x.Bind(() => vm.CanStream))
			.IsChecked(x => x.Bind(() => vm.UseStream).TwoWay())
			.IsThreeState(false)
			.Content(
				new StackPanel()
					.Spacing(4)
					.Orientation(Orientation.Horizontal)
					.Children(
						new PathIcon()
							.Foreground(Colors.Red)
							.Data(PowerIcon),
						new TextBlock()
							.Foreground(Colors.Red)
							.Text("Stream Off")
					)
			)
			.ControlExtensions(
				alternateContent:
					new StackPanel()
						.Spacing(4)
						.Orientation(Orientation.Horizontal)
						.Children(
							new PathIcon()
								.Foreground(Colors.Green)
								.Data(PowerIcon),
							new TextBlock()
								.Foreground(Colors.Green)
								.Text("Stream On")
						)
			);
	

	private UIElement Messages(BindableMainModel vm)
		=> new ListView()
			.Grid(row: 1)
			.VerticalAlignment(VerticalAlignment.Bottom)
			.SelectionMode(ListViewSelectionMode.None)
			.ItemsSource(() => vm.Messages)
			.AutoScrollListViewBehavior(autoScroll: true)
			.ItemTemplateSelector<Message>((message, selector) => selector
				.Default(() => new TextBlock().Text(() => message.Content))
				.Case(
					m => m.Source == Source.User,
					() => UserMessage(message)
				)
				.Case(
					m => m.Source == Source.AI && m.Status == Status.Value,
					() => AIMessage(message)
				)
				.Case(
					m => m.Source == Source.AI && m.Status == Status.Error,
					() => ErrorMessage(message)
				)
				.Case(
					m => m.Source == Source.AI && m.Status == Status.Loading,
					() => LoadingMessage()
				)
			);

	private StackPanel UserMessage(Message message)
		=> new StackPanel()
			.Margin(8)
			.HorizontalAlignment(HorizontalAlignment.Right)
			.Children(
				new Border()
					.Background(Theme.Brushes.Primary.Default)
					.CornerRadius(10)
					.MinWidth(70)
					.MaxWidth(350)
					.Padding(10)
					.Child(
						new TextBlock()
							.Foreground(Theme.Brushes.OnPrimary.Default)
							.HorizontalAlignment(HorizontalAlignment.Center)
							.TextWrapping(TextWrapping.Wrap)
							.Text(() => message.Content))
			);

	private StackPanel AIMessage(Message message)
		=> new StackPanel()
			.Margin(8)
			.HorizontalAlignment(HorizontalAlignment.Left)
			.Children(
				new Border()
					.Background(Theme.Brushes.Secondary.Default)
					.CornerRadius(10)
					.MinWidth(70)
					.MaxWidth(350)
					.Padding(10)
					.Child(
						new TextBlock()
							.Foreground(Theme.Brushes.OnSecondary.Default)
							.HorizontalAlignment(HorizontalAlignment.Center)
							.TextWrapping(TextWrapping.Wrap)
							.Text(() => message.Content)
					)
			);

	private StackPanel ErrorMessage(Message message)
		=> new StackPanel()
			.Margin(8)
			.HorizontalAlignment(HorizontalAlignment.Left)
			.Children(
				new Border()
					.Background(Theme.Brushes.Error.Default)
					.CornerRadius(10)
					.MinWidth(70)
					.MaxWidth(350)
					.Padding(10)
					.Child(
						new TextBlock()
							.Foreground(Theme.Brushes.OnError.Default)
							.HorizontalAlignment(HorizontalAlignment.Center)
							.TextWrapping(TextWrapping.Wrap)
							.Text(() => message.Content)
					)
			);

	private StackPanel LoadingMessage()
		=> new StackPanel()
			.Margin(8)
			.HorizontalAlignment(HorizontalAlignment.Left)
			.Children(
				new Border()
					.Background(Theme.Brushes.Secondary.Default)
					.CornerRadius(10)
					.MinWidth(70)
					.MaxWidth(350)
					.Padding(10)
					.Child(
						new TextBlock()
							.Foreground(Theme.Brushes.OnSecondary.Default)
							.HorizontalAlignment(HorizontalAlignment.Center)
							.TextWrapping(TextWrapping.Wrap)
							.Text("...")
					)
			);

	private Grid Prompt(BindableMainModel vm)
		=> new Grid()
			.Grid(row: 2)
			.ColumnDefinitions("*, Auto")
			.VerticalAlignment(VerticalAlignment.Bottom)
			.HorizontalAlignment(HorizontalAlignment.Stretch)
			.Children(
				new TextBox()
					.PlaceholderText("Message ChatGPT")
					.CommandExtensions(x => x.Command(() => vm.AskMessage))
					.Text(x => x.Bind(() => vm.Prompt).TwoWay().UpdateSourceTrigger(UpdateSourceTrigger.PropertyChanged)),
				new Button()
					.Grid(column: 1)
					.Style(Theme.Button.Styles.Icon)
					.Command(() => vm.AskMessage)
					.Content(
						new PathIcon()
							.Data("M2.01 21L23 12L2.01 3L2 10L17 12L2 14L2.01 21Z")
					)
		);
}

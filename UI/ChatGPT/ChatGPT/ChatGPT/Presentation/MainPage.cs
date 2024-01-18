using ChatGPT.Business;

namespace ChatGPT.Presentation;

public sealed partial class MainPage : Page
{
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
						.Padding(10).Margin(0, 10)
						.RowDefinitions<Grid>("Auto,*,Auto")
						.Children(
							Header(vm),
							Messages(vm),
							Prompt(vm)
					)
				)
			);
	}

	private StackPanel Header(BindableMainModel vm)
		=> new StackPanel()
			.HorizontalAlignment(HorizontalAlignment.Center)
			.Visibility(x => x.Bind(() => vm.IsStreamEnabled)
						.Convert(isStreamEnabled => isStreamEnabled ? Visibility.Visible : Visibility.Collapsed))
			.Children(
				new TextBlock()
					.Text("Message Stream"),
				new ToggleSwitch()
					.HorizontalAlignment(HorizontalAlignment.Center)
					.IsOn(x => x.Bind(() => vm.IsMessageStream).TwoWay())
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
					.Background(Colors.DarkSlateGray)
					.CornerRadius(10)
					.MinWidth(70)
					.MaxWidth(350)
					.Padding(10)
					.Child(
						new TextBlock()
							.Foreground(Colors.Yellow)
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
					.Background(Colors.DarkGray)
					.CornerRadius(10)
					.MinWidth(70)
					.MaxWidth(350)
					.Padding(10)
					.Child(
						new TextBlock()
							.Foreground(Colors.Black)
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
					.Background(Colors.DarkGray)
					.CornerRadius(10)
					.MinWidth(70)
					.MaxWidth(350)
					.Padding(10)
					.Child(
						new TextBlock()
							.Foreground(Colors.Red)
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
					.Background(Colors.DarkGray)
					.CornerRadius(10)
					.MinWidth(70)
					.MaxWidth(350)
					.Padding(10)
					.Child(
						new TextBlock()
							.Foreground(Colors.Black)
							.HorizontalAlignment(HorizontalAlignment.Center)
							.TextWrapping(TextWrapping.Wrap)
							.Text("...")
					)
			);

	private StackPanel Prompt(BindableMainModel vm)
		=> new StackPanel()
			.Grid(row: 2)
			.VerticalAlignment(VerticalAlignment.Bottom)
			.HorizontalAlignment(HorizontalAlignment.Center)
			.Orientation(Orientation.Horizontal)
			.Spacing(10)
			.Children(
				new TextBox()
					.Width(300)
					.PlaceholderText("Message ChatGPT")
					.CommandExtensions(x => x.Command(() => vm.AskMessage))
					.Text(x => x.Bind(() => vm.Prompt).TwoWay().UpdateSourceTrigger(UpdateSourceTrigger.PropertyChanged)),
				new Button()
					.Command(() => vm.AskMessage)
					.Content("Send")
		);
}

namespace Navigation.Presentation;

public sealed partial class CControlNavigationPage : Page
{
	public Entity[] Items { get; } =
		[
			new Entity("First"),
			new Entity("Second"),
			new Entity("Third"),
			new Entity("Fourth"),
		];

	public CControlNavigationPage()
	{
		this.InitializeComponent();
	}
}

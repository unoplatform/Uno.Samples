namespace ToDo.Views;

public sealed partial class HomePage : Page
{
	public HomePage()
	{
		this.InitializeComponent();

		NavView.SizeChanged += HomePage_SizeChanged;
	}

	private bool? LastIsWide { get; set; }
	private void HomePage_SizeChanged(object sender, SizeChangedEventArgs args)
	{
		var isWide = (App.Current as App)?.Window?.Content?.ActualSize.X > (double)App.Current.Resources[ResourceKeys.WideMinWindowWidth];
		if (!LastIsWide.HasValue || LastIsWide.Value != isWide)
		{
			LastIsWide = isWide;
			if (isWide)
			{
				var binding = new Binding() { Path = new PropertyPath("SelectedList.Value") };
				NavView.SetBinding(NavigationView.SelectedItemProperty, binding);
			}
			else
			{
				NavView.ClearValue(NavigationView.SelectedItemProperty);
			}
		}
	}
}

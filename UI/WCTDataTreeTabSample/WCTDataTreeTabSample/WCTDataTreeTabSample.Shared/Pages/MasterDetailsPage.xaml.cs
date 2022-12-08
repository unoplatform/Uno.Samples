

namespace WCTDataTreeTabSample;

public sealed partial class MasterDetailsPage : Page
{
	private MountainDataSource MountainData = new MountainDataSource();

	public MountainDataItem SelectedMountain
	{
		get => (MountainDataItem)GetValue(SelectedMountainProperty);
		set => SetValue(SelectedMountainProperty, value);
	}

	public static readonly DependencyProperty SelectedMountainProperty =
		DependencyProperty.Register(
			nameof(SelectedMountain),
			typeof(MountainDataItem),
			typeof(MasterDetailsPage),
			new PropertyMetadata(null));

	public MasterDetailsPage()
	{
		this.InitializeComponent();

		Loaded += MasterDetailsPage_Loaded;
	}

	private async void MasterDetailsPage_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
	{
		var mountains = await MountainData.GetDataAsync();
		this.MoutainList.ItemsSource = mountains.ToList();

		SelectedMountain = mountains.FirstOrDefault();
	}
}

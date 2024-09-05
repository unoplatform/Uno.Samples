namespace MVUX.Presentation.SelectionSample
{

	public sealed partial class SelectionPage : Page
	{
		public SelectionPage()
		{
			this.InitializeComponent();
            this.DataContext = new BindableSelectionModel();
        }
	}
}

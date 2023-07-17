namespace SkeletonLoaderAnimation.Presentation
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void itemsListView_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var listView = sender as ListView;
            var viewModel = listView?.DataContext as BindableMainModel;

            if (viewModel != null)
            {

                if (viewModel.IsLoading)
                {
                    listView.ItemTemplate = (DataTemplate)Resources["LoadingTemplate"];
                }
                else
                {
                    listView.ItemTemplate = (DataTemplate)Resources["LoadedTemplate"];
                }

                viewModel.IsLoading = !viewModel.IsLoading;
            }
        }
    }
}
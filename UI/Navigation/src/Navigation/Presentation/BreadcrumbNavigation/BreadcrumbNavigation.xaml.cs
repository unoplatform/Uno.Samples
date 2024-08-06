namespace Navigation.Presentation;

public sealed partial class BreadcrumbNavigation : Page
{
	public BreadcrumbNavigation()
	{
		this.InitializeComponent();
	}

	//private void OnNavigateToPage(string pageName)
	//{
	//	// Navigate to the desired page
	//	// Frame.Navigate(typeof(DesiredPage));

	//	// Update breadcrumbs
	//	var breadcrumbVM = (BreadcrumbViewModel)this.DataContext;
	//	breadcrumbVM.AddBreadcrumb(pageName);
	//}

	//private void OnNavigateBack()
	//{
	//	// Navigate back to the previous page
	//	if (Frame.CanGoBack)
	//	{
	//		Frame.GoBack();
	//		var breadcrumbVM = (BreadcrumbViewModel)this.DataContext;
	//		breadcrumbVM.RemoveLastBreadcrumb();
	//	}
	//}
}

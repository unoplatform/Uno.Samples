namespace Navigation.Presentation
{
	public partial class BreadcrumbViewModel : BaseBreadcrumbViewModel
	{
		private readonly INavigator _navigator;
		public BreadcrumbViewModel(IRouteNotifier notifier, INavigator navigator)
			: base(notifier, navigator)
		{
			_navigator = navigator;
		}

		[RelayCommand]
		public async Task NavigateToFirst()
		{
			await _navigator.NavigateViewAsync<FirstBreadcrumbPage>(this);
		}
	}
}

namespace UnoChatGPT.Presentation
{
	public class ShellModel
	{
		private INavigator Navigator { get; }

		public ShellModel(INavigator navigator)
		{
			Navigator = navigator;

			_ = Start();
		}

		public async Task Start()
		{
			await Navigator.NavigateViewModelAsync<MainModel>(this);
		}
	}
}
using Microsoft.UI.Xaml.Controls;
using System;
using Microsoft.UI.Xaml;
using ChatUI.Entities;
using Windows.System;
using Microsoft.UI.Xaml.Input;

namespace ChatUI;

public sealed partial class MainPage : Page
{
	private MainPageViewModel _viewModel;

	public MainPage()
	{
		this.InitializeComponent();
		_viewModel = new MainPageViewModel();
		this.DataContext = _viewModel;
		this.Loaded += MainPage_Loaded;
	}

	private void MainPage_Loaded(object sender, RoutedEventArgs e)
	{
		scrollViewer.ChangeView(0, scrollViewer.ScrollableHeight, 1);
	}
}

public class MessageTemplateSelector : DataTemplateSelector
{
	public DataTemplate MyMessageTemplate { get; set; }
	public DataTemplate OtherMessageTemplate { get; set; }

	protected override DataTemplate SelectTemplateCore(object item)
	{
		if(item != null)
		{
			return item switch
			{
				Message { IsMyMessage: true } => MyMessageTemplate,
				Message { IsMyMessage: false } => OtherMessageTemplate,
				_ => throw new InvalidOperationException(),
			};
		}

		return MyMessageTemplate;
	}
}
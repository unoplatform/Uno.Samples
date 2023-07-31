using ChatUI.Models;
using ChatUI.Services;
using Microsoft.UI.Xaml.Controls;

namespace ChatUI;

public sealed partial class MainPage : Page
{
	public MainPage()
	{
		this.InitializeComponent();
		this.DataContext = new BindableMessageModel(new MessageService());
	}
}
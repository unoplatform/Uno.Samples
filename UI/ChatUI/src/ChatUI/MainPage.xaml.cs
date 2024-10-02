using ChatUI.Models;
using ChatUI.Services;

namespace ChatUI;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();

        this.DataContext = new BindableMessageModel(new MessageService());
    }
}
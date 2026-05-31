using Microsoft.UI.Xaml.Controls;
using Uno.Extensions.Navigation.UI;

namespace EnterpriseDashboard.Views;

public sealed partial class Shell : UserControl, IContentControlProvider
{
    public Shell()
    {
        this.InitializeComponent();
    }

    // Tells the navigation system which ContentControl hosts navigated content.
    public ContentControl ContentControl => ShellContent;
}

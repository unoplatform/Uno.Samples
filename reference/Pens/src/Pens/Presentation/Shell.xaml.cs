using Microsoft.UI.Xaml.Controls;
using Uno.Extensions.Navigation.UI;

namespace Pens.Presentation;

public sealed partial class Shell : UserControl, IContentControlProvider
{
    public Shell()
    {
        this.InitializeComponent();
    }

    // Tells the navigation system which ContentControl hosts navigated content.
    public ContentControl ContentControl => ShellContent;
}

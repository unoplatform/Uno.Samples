using Uno.Extensions.Navigation.UI;

namespace Nexus.Presentation;

public sealed partial class Shell : UserControl, IContentControlProvider
{
    public Shell()
    {
        this.InitializeComponent();
    }

    // Tells the navigation system which ContentControl hosts navigated content.
    public ContentControl ContentControl => ShellContent;
}

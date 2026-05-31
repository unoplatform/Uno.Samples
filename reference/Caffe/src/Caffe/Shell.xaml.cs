using Uno.Extensions.Navigation.UI;

namespace Caffe;

public sealed partial class Shell : UserControl, IContentControlProvider
{
    public Shell()
    {
        this.InitializeComponent();
    }

    // Tells the navigation system which ContentControl hosts navigated content.
    public ContentControl ContentControl => ShellContent;
}

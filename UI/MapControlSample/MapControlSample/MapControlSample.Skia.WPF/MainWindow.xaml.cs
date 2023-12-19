using System.Windows;
using Uno.UI.Runtime.Skia.Wpf;

namespace MapControlSample.WPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        root.Content = new WpfHost(Dispatcher, () => new MapControlSample.App());
    }
}

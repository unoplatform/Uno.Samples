using Window = System.Windows.Window;

namespace Commerce.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            root.Content = new global::Uno.UI.Runtime.Skia.Wpf.WpfHost(Dispatcher, () => new AppHead());
        }
    }
}

using Microsoft.UI.Xaml;

namespace XamlBrewerUnoApp
{
    public sealed partial class Shell : UserControl
    {
        public Shell()
        {
            InitializeComponent();

            (Application.Current as App)?.EnsureSettings();
            Loaded += Shell_Loaded;
        }

        private void Shell_Loaded(object sender, RoutedEventArgs e)
        {
            ApplyTheme();
        }

        internal UIElement AppRoot => Root;

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if ((Application.Current as App)?.Settings is { } settings)
            {
                settings.IsLightTheme = !settings.IsLightTheme;
                (Application.Current as App)?.SaveSettings();
                Root.ActualThemeChanged += Root_ActualThemeChanged;
                ApplyTheme();
            }
        }

        private void ApplyTheme()
        {
            var settings = (Application.Current as App)?.Settings;
            if (Root.XamlRoot?.Content is FrameworkElement root &&
                settings is not null)
            {
                root.RequestedTheme = settings.IsLightTheme ? ElementTheme.Light : ElementTheme.Dark;
            };
        }
        private void Root_ActualThemeChanged(FrameworkElement sender, object args)
        {
            // Theme change refinements (e.g. content dialogs and title bar).
        }
    }
}

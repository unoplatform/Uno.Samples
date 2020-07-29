using System;
using System.Reactive;
using System.Reactive.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UnoChat.Client
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly ViewModel _viewModel;
        private IDisposable _behaviours;

        public MainPage()
        {
            this.InitializeComponent();

            _viewModel = new ViewModel();
            DataContext = _viewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var messageToSendReturn = Observable
                .FromEvent<KeyEventHandler, KeyRoutedEventArgs>(
                    handler => (s, k) => handler(k),
                    handler => MessageToSendTextBox.KeyUp += handler,
                    handler => MessageToSendTextBox.KeyUp -= handler)
                .Where(k => k.Key == Windows.System.VirtualKey.Enter);

            var themeChanger = Observer.Create<string>(
                value =>
                {
                    if (Enum.TryParse<ElementTheme>(value, out ElementTheme theme) && Window.Current.Content is FrameworkElement frameworkElement)
                    {
                        frameworkElement.RequestedTheme = theme;
                    }
                }
            );

            _behaviours = _viewModel.Activate(messageToSendReturn, themeChanger);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            if (_behaviours != null)
            {
                _behaviours.Dispose();
                _behaviours = null;
            }
        }

        private void LightButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (Window.Current.Content is FrameworkElement frameworkElement)
            {
                frameworkElement.RequestedTheme = ElementTheme.Light;
            }
        }

        private void DarkButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (Window.Current.Content is FrameworkElement frameworkElement)
            {
                frameworkElement.RequestedTheme = ElementTheme.Dark;
            }
        }
    }
}

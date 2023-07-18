using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Reactive;
using System.Reactive.Linq;
using UnoChat.Client;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UnoChat
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
                    if (Enum.TryParse<ElementTheme>(value, out ElementTheme theme) && 
                        this.XamlRoot.Content is FrameworkElement frameworkElement)
                    {
                        frameworkElement.RequestedTheme = theme;
                    }
                }
            );

            var scroller = Observer.Create<UnoChat.Client.Message.Model>(model => MessagesListView.ScrollIntoView(model));

            _behaviours = _viewModel.Activate(messageToSendReturn, themeChanger, scroller);
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
    }
}

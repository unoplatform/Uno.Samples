namespace TimeEntryRia
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Navigation;
    using TimeEntryRia.LoginUI;
    using TimeEntryRia.Web;

    /// <summary>
    /// <see cref="UserControl"/> class providing the main UI for the application.
    /// </summary>
    public partial class MainPage : UserControl
    {
        private bool _rollBackNav = false;
        private bool _errorWhileRollback = false;

        /// <summary>
        /// Creates a new <see cref="MainPage"/> instance.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
            this.loginContainer.Child = new LoginStatus();

            WebContext.Current.Authentication.LoggedOut += (s, e) =>
            {
                // this works around an issue wher the CanGoBack flag is not updated
                // quickly enough so another GoBack is issued, causing exceptions.
                try
                {
                    _rollBackNav = true;
                    while (ContentFrame.CanGoBack && !_errorWhileRollback)
                    {
                        ContentFrame.GoBack();
                    }
                }
                finally
                {
                    _rollBackNav = false;
                    _errorWhileRollback = false;
                }
            };

        }

        /// <summary>
        /// After the Frame navigates, ensure the <see cref="HyperlinkButton"/> representing the current page is selected
        /// </summary>
        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            foreach (UIElement child in LinksStackPanel.Children)
            {
                HyperlinkButton hb = child as HyperlinkButton;
                if (hb != null && hb.NavigateUri != null)
                {
                    if (hb.NavigateUri.ToString().Equals(e.Uri.ToString()))
                    {
                        VisualStateManager.GoToState(hb, "ActiveLink", true);
                    }
                    else
                    {
                        VisualStateManager.GoToState(hb, "InactiveLink", true);
                    }
                }
            }
        }

        /// <summary>
        /// If an error occurs during navigation, show an error window
        /// </summary>
        private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            e.Handled = true;

            if (_rollBackNav)
            {
                _errorWhileRollback = true;
            }
            else
            {
                ErrorWindow.CreateNew(e.Exception);
            }
        }

        private Dictionary<string, string> _secureViews = new Dictionary<string, string> { 
            {"/TimeEntryPage", TimeEntryRoles.Consultant},
            { "/NewTimeEntryPage", TimeEntryRoles.Consultant}, 
            {"/ReportsPage",  TimeEntryRoles.ReportViewer}, 
            {"/AdminPage", TimeEntryRoles.Admin }
        };

        private void ContentFrame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (_secureViews.ContainsKey(e.Uri.OriginalString))
            {
                var requiredRole = _secureViews[e.Uri.OriginalString];

                if (e.NavigationMode != NavigationMode.Back && !WebContext.Current.User.IsAuthenticated)
                {
                    ErrorWindow.CreateNew(ApplicationStrings.Main_LoginRequired, StackTracePolicy.Never);
                    e.Cancel = true;
                }
                else if (e.NavigationMode != NavigationMode.Back && 
                    WebContext.Current.User.IsAuthenticated && 
                    !WebContext.Current.User.IsInRole(requiredRole))
                {
                    ErrorWindow.CreateNew(ApplicationStrings.Main_RoleRequired + requiredRole, StackTracePolicy.Never);
                    e.Cancel = true;
                }
            }
        }
    }
}
namespace TimeEntryRia
{
    using System.Windows.Controls;
    using System.Windows.Navigation;
    using System.Reflection;
    using System;

    /// <summary>
    /// <see cref="Page"/> class to present information about the current application.
    /// </summary>
    public partial class About : Page
    {
        /// <summary>
        /// Creates a new instance of the <see cref="About"/> class.
        /// </summary>
        public About()
        {
            InitializeComponent();

            this.Title = ApplicationStrings.AboutPageTitle;
        }

        /// <summary>
        /// Executes when the user navigates to this page.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string name = Assembly.GetExecutingAssembly().FullName;
            AssemblyName asmName = new AssemblyName(name);

            VersionInfo.Text = "Version: " + asmName.Version.Major + "." + asmName.Version.Minor + "." + asmName.Version.Build + "." + asmName.Version.Revision;
        }

        private void HyperlinkButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            System.Windows.Browser.HtmlPage.Window.Navigate(new Uri("https://icons8.com"), "_blank");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Navigation;
using TimeEntryRia.Web;

namespace TimeEntryRia.Views
{
    public partial class AdminPage : Page
    {
        public AdminPage()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void projectDomainDataSource_LoadedData(object sender, LoadedDataEventArgs e)
        {

            if (e.HasError)
            {
                System.Windows.MessageBox.Show(e.Error.ToString(), "Load Error", System.Windows.MessageBoxButton.OK);
                e.MarkErrorAsHandled();
            }
        }

        private void AddRow_Click(object sender, RoutedEventArgs e)
        {
            projectPager.PageIndex = 0;

            var view = projectDataGrid.ItemsSource as DomainDataSourceView;
            var newProject = new Project();
            view.Add(newProject);

            projectDataGrid.Focus();
            projectDataGrid.SelectedItem = newProject;
            projectDataGrid.CurrentColumn = projectDataGrid.Columns[1]; // no point selecting the ID column
            projectDataGrid.ScrollIntoView(projectDataGrid.SelectedItem, projectDataGrid.CurrentColumn);
            projectDataGrid.BeginEdit();
        }

    }
}

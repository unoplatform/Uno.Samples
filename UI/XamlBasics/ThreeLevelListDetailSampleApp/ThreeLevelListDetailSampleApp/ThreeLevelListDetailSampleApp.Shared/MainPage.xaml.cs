using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using ThreeLevelListDetailsSample.Data;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ThreeLevelListDetailsSample
{
    public sealed partial class MainPage : Page
    {
        public ObservableCollection<WorkRequestItem> Items = new ObservableCollection<WorkRequestItem>(ItemsDataSource.GetAllItems());

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void MainPage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (WorkRequestItemsListView.SelectedIndex < Items.Count && WorkRequestItemsListView.SelectedIndex > -1)
            {
                ListViewPage.AttachmentViewerPage.Item = new AttachmentItem();
                ListViewPage.Items = Items[WorkRequestItemsListView.SelectedIndex].AttachmentItems;
            }
        }
    }
}

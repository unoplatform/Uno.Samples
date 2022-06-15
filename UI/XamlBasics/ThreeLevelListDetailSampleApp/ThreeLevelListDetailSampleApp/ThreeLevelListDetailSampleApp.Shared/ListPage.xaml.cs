using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using ThreeLevelListDetailsSample.Data;

namespace ThreeLevelListDetailsSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ListPage : Page
    {
        public ListPage()
        {
            this.InitializeComponent();
        }

        public IList<AttachmentItem> Items
        {
            get { return (IList<AttachmentItem>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(IList<AttachmentItem>), typeof(ListPage), new PropertyMetadata(new List<AttachmentItem>()));

        private void ListPage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AttachmentsListView.SelectedIndex < Items.Count && AttachmentsListView.SelectedIndex > -1)
            {
                AttachmentViewerPage.DetailsImage.Visibility = Visibility.Visible;
                AttachmentViewerPage.Item = Items[AttachmentsListView.SelectedIndex];
            }
            else
            {
                AttachmentViewerPage.DetailsImage.Visibility = Visibility.Collapsed;
            }
        }
    }
}

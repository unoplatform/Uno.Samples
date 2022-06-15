using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ThreeLevelListDetailsSample.Data;

namespace ThreeLevelListDetailsSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DetailsPage : Page
    {
        public DetailsPage()
        {
            this.InitializeComponent();
        }

        public AttachmentItem Item
        {
            get { return (AttachmentItem)GetValue(ItemProperty); }
            set { SetValue(ItemProperty, value); }
        }

        public static readonly DependencyProperty ItemProperty =
            DependencyProperty.Register("Item", typeof(AttachmentItem), typeof(DetailsPage), new PropertyMetadata(null));
    }
}

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ListViewSample
{
    public sealed partial class NewConnectionDialog : ContentDialog
	{
        public string ConnectionOccupation
        {
            get { return (string)GetValue(ConnectionOccupationProperty); }
            set { SetValue(ConnectionOccupationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ConnectionOccupation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ConnectionOccupationProperty =
            DependencyProperty.Register("ConnectionOccupation", typeof(string), typeof(NewConnectionDialog), new PropertyMetadata(null));

        public string ConnectionName
        {
            get { return (string)GetValue(ConnectionNameProperty); }
            set { SetValue(ConnectionNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ConnectionName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ConnectionNameProperty =
            DependencyProperty.Register("ConnectionName", typeof(string), typeof(NewConnectionDialog), new PropertyMetadata(null));


        public NewConnectionDialog()
		{
			this.InitializeComponent();
		}

		private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
            if (string.IsNullOrWhiteSpace(ConnectionName) || string.IsNullOrWhiteSpace(ConnectionOccupation))
            {
                args.Cancel = true;
            }
		}
	}
}

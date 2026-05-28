using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FieldOpsPro.Presentation.Controls;

public sealed partial class ChatPreviewCard : UserControl
{
    public ChatPreviewCard()
    {
        this.InitializeComponent();
    }

    public static readonly DependencyProperty UnreadCountProperty =
        DependencyProperty.Register(nameof(UnreadCount), typeof(int), typeof(ChatPreviewCard),
            new PropertyMetadata(4, OnUnreadCountChanged));

    public int UnreadCount
    {
        get => (int)GetValue(UnreadCountProperty);
        set => SetValue(UnreadCountProperty, value);
    }

    private static void OnUnreadCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ChatPreviewCard card && card.UnreadCountText != null)
        {
            card.UnreadCountText.Text = card.UnreadCount.ToString();
        }
    }

    // Raised when the user sends a quick reply from the preview
    public event EventHandler<string>? MessageSent;

    private void OnSendButtonClick(object sender, RoutedEventArgs e)
    {
        SendReply();
    }

    private void OnReplyTextBoxKeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        // Send on Enter (avoid sending when Shift+Enter is used, but AcceptsReturn is false)
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            SendReply();
            e.Handled = true;
        }
    }

    private void SendReply()
    {
        if (ReplyTextBox == null) return;

        var text = ReplyTextBox.Text?.Trim();
        if (string.IsNullOrEmpty(text)) return;

        // Raise the event for the host to handle
        MessageSent?.Invoke(this, text);

        // Clear the box and keep focus so mobile keyboard stays open
        ReplyTextBox.Text = string.Empty;
        ReplyTextBox.Focus(Microsoft.UI.Xaml.FocusState.Programmatic);
    }
}

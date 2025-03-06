using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

#if HAS_TELERIK
using Telerik.Maui.Controls.Compatibility.Primitives;
#endif
using AppTheme = Microsoft.Maui.ApplicationModel.AppTheme;
using MauiApplication = Microsoft.Maui.Controls.Application;

namespace TelerikApp.Business.Models;

public partial class User : ObservableObject
{
    //private string unreadMessagesText;
    private Color highLightedTextColor = MauiApplication.Current?.RequestedTheme == AppTheme.Light ? Color.FromArgb("#0E88F2") : Color.FromArgb("#42A5F5");
    private Color defaultTextColor = MauiApplication.Current?.RequestedTheme == AppTheme.Light ? Color.FromArgb("#99000000") : Color.FromArgb("#99FFFFFF");

    [ObservableProperty]
    private string? name;

    [ObservableProperty]
    private string? lastMessageReceived;

    [ObservableProperty]
    private string? imageSourcePath;

#if HAS_TELERIK
	[ObservableProperty]
	private BadgeType activityStatus;
#endif

	[ObservableProperty]
    private string? unreadMessagesText;

    [ObservableProperty]
    private string? lastMessageReceivedDate;

    [ObservableProperty]
    private Color lastMessageReceivedDateColor = Colors.Blue;

    [ObservableProperty]
    private FontAttributes messageFontAttributes;

    [ObservableProperty]
    private bool isVisibleUnreadMessages;

    partial void OnUnreadMessagesTextChanged(string? value)
    {
        if(value is not null)
        {
            LastMessageReceivedDateColor = highLightedTextColor;
            MessageFontAttributes = FontAttributes.Bold;
            IsVisibleUnreadMessages = true;
        }
        else
        {
            LastMessageReceivedDateColor = defaultTextColor;
            MessageFontAttributes = FontAttributes.None;
            IsVisibleUnreadMessages = false;
        }
    }
}

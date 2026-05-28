using Microsoft.UI.Xaml.Media;

namespace VTrack.Controls;

public sealed partial class AppHeader : UserControl
{
    public static readonly DependencyProperty ActiveStepProperty = DependencyProperty.Register(
        nameof(ActiveStep), typeof(int), typeof(AppHeader),
        new PropertyMetadata(1, (d, _) => ((AppHeader)d).UpdateActiveTab()));

    public static readonly DependencyProperty StatusTextProperty = DependencyProperty.Register(
        nameof(StatusText), typeof(string), typeof(AppHeader),
        new PropertyMetadata("no recording loaded", (d, e) => ((AppHeader)d).UpdateStatus()));

    public static readonly DependencyProperty IsRecordingLoadedProperty = DependencyProperty.Register(
        nameof(IsRecordingLoaded), typeof(bool), typeof(AppHeader),
        new PropertyMetadata(false, (d, _) => ((AppHeader)d).UpdateStatus()));

    public int ActiveStep
    {
        get => (int)GetValue(ActiveStepProperty);
        set => SetValue(ActiveStepProperty, value);
    }

    public string StatusText
    {
        get => (string)GetValue(StatusTextProperty);
        set => SetValue(StatusTextProperty, value);
    }

    public bool IsRecordingLoaded
    {
        get => (bool)GetValue(IsRecordingLoadedProperty);
        set => SetValue(IsRecordingLoadedProperty, value);
    }

    public AppHeader()
    {
        this.InitializeComponent();
        UpdateActiveTab();
        UpdateStatus();
    }

    private void UpdateActiveTab()
    {
        if (Tab1 is null) return; // pre-XAML init

        SetTabState(Tab1, Tab1Label, ActiveStep == 1);
        SetTabState(Tab2, Tab2Label, ActiveStep == 2);
        SetTabState(Tab3, Tab3Label, ActiveStep == 3);
    }

    private static void SetTabState(Border border, TextBlock label, bool active)
    {
        if (active)
        {
            border.Background = (Brush)Application.Current.Resources["PrimaryBrush"];
            label.Foreground = (Brush)Application.Current.Resources["OnPrimaryBrush"];
        }
        else
        {
            border.Background = null;
            label.Foreground = (Brush)Application.Current.Resources["OnSurfaceVariantBrush"];
        }
    }

    private void UpdateStatus()
    {
        if (StatusLabel is null) return;

        StatusLabel.Text = StatusText;
        StatusDot.Fill = (Brush)Application.Current.Resources[
            IsRecordingLoaded ? "PrimaryBrush" : "OutlineVariantBrush"];
    }
}

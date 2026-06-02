using System.ComponentModel;

namespace DenimOverallsApp.Presentation;

/// <summary>
/// Reactive, single-coordinate ("SVG") denim overall figure, shared between the configurator
/// and the order summary. The host binds the raw configuration values; the figure inside binds
/// to the computed visual-state properties below (its DataContext is the control itself).
/// </summary>
public sealed partial class OverallPreview : UserControl, INotifyPropertyChanged
{
    public OverallPreview()
    {
        this.InitializeComponent();
        // The figure subtree binds to the computed properties on this control; the control's
        // own DataContext stays inherited from the host so the dependency-property bindings resolve.
        Figure.DataContext = this;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    // ── Inputs (set by the host page) ───────────────────────────────────────
    public static readonly DependencyProperty ColorHexProperty = DependencyProperty.Register(
        nameof(ColorHex), typeof(string), typeof(OverallPreview), new PropertyMetadata(null, OnVisualChanged));

    public string ColorHex
    {
        get => (string)GetValue(ColorHexProperty);
        set => SetValue(ColorHexProperty, value);
    }

    public static readonly DependencyProperty LengthOptionProperty = DependencyProperty.Register(
        nameof(LengthOption), typeof(string), typeof(OverallPreview), new PropertyMetadata(null, OnVisualChanged));

    public string LengthOption
    {
        get => (string)GetValue(LengthOptionProperty);
        set => SetValue(LengthOptionProperty, value);
    }

    public static readonly DependencyProperty BibTypeProperty = DependencyProperty.Register(
        nameof(BibType), typeof(string), typeof(OverallPreview), new PropertyMetadata(null, OnVisualChanged));

    public string BibType
    {
        get => (string)GetValue(BibTypeProperty);
        set => SetValue(BibTypeProperty, value);
    }

    public static readonly DependencyProperty PocketTypeProperty = DependencyProperty.Register(
        nameof(PocketType), typeof(string), typeof(OverallPreview), new PropertyMetadata(null, OnVisualChanged));

    public string PocketType
    {
        get => (string)GetValue(PocketTypeProperty);
        set => SetValue(PocketTypeProperty, value);
    }

    public static readonly DependencyProperty HasLogoProperty = DependencyProperty.Register(
        nameof(HasLogo), typeof(bool), typeof(OverallPreview), new PropertyMetadata(false, OnVisualChanged));

    public bool HasLogo
    {
        get => (bool)GetValue(HasLogoProperty);
        set => SetValue(HasLogoProperty, value);
    }

    public static readonly DependencyProperty EmbroideryTextProperty = DependencyProperty.Register(
        nameof(EmbroideryText), typeof(string), typeof(OverallPreview), new PropertyMetadata(string.Empty));

    public string EmbroideryText
    {
        get => (string)GetValue(EmbroideryTextProperty);
        set => SetValue(EmbroideryTextProperty, value);
    }

    // ── Computed visual state (bound by the figure) ─────────────────────────
    public string SelectedColorHex => ColorHex;
    public bool IsLongLength  => LengthOption == "long";
    public bool IsShortLength => LengthOption == "short";
    public bool ShowThighPockets => PocketType is "patch" or "cargo";
    public bool ShowBibPanel          => BibType != "crossback";
    public bool ShowSingleChestPocket => BibType == "classic";
    public bool ShowDoubleChestPocket => BibType == "wide";
    public bool ShowCrossStraps       => BibType == "crossback";
    public bool IsClassicBib => BibType == "classic";
    public bool IsWideBib    => BibType == "wide";
    public bool IsScoopBib   => BibType == "scoop";
    public bool ShowLogo => HasLogo;

    private static void OnVisualChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        => ((OverallPreview)d).RaiseVisualStateChanged();

    private void RaiseVisualStateChanged()
    {
        foreach (var name in ComputedProperties)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    private static readonly string[] ComputedProperties =
    {
        nameof(SelectedColorHex),
        nameof(IsLongLength), nameof(IsShortLength),
        nameof(ShowThighPockets),
        nameof(ShowBibPanel), nameof(ShowSingleChestPocket), nameof(ShowDoubleChestPocket), nameof(ShowCrossStraps),
        nameof(IsClassicBib), nameof(IsWideBib), nameof(IsScoopBib),
        nameof(ShowLogo),
    };
}

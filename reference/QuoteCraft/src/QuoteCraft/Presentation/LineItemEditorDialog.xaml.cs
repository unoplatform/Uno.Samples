using Microsoft.UI.Xaml.Media;

namespace QuoteCraft.Presentation;

public sealed partial class LineItemEditorDialog : ContentDialog
{
    private string? _editingItemId;

    public LineItemEditorDialog()
    {
        this.InitializeComponent();
        ApplyGradientBrushes();
        this.PrimaryButtonClick += OnPrimaryButtonClick;
        this.SecondaryButtonClick += OnSecondaryButtonClick;
    }

    /// <summary>
    /// The saved line item after the user clicks Save. Null if cancelled.
    /// </summary>
    public LineItemEntity? Result { get; private set; }

    /// <summary>
    /// True if the user clicked Delete in edit mode.
    /// </summary>
    public bool WasDeleted { get; private set; }

    /// <summary>
    /// The ID of the line item being edited, if in edit mode.
    /// </summary>
    public string? EditingItemId => _editingItemId;

    public void SetAddMode()
    {
        _editingItemId = null;
        SecondaryButtonText = "";
        DescriptionBox.Text = string.Empty;
        UnitPriceBox.Text = string.Empty;
        QuantityBox.Text = "1";
        Result = null;
        WasDeleted = false;
        UpdateLineTotal();
    }

    public void SetEditMode(LineItemEntity item)
    {
        _editingItemId = item.Id;
        SecondaryButtonText = "Delete";
        DescriptionBox.Text = item.Description;
        UnitPriceBox.Text = item.UnitPrice.ToString("F2");
        QuantityBox.Text = item.Quantity.ToString();
        Result = null;
        WasDeleted = false;
        UpdateLineTotal();
    }

    private void OnPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        var desc = DescriptionBox.Text?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(desc))
        {
            args.Cancel = true;
            return;
        }

        _ = decimal.TryParse(UnitPriceBox.Text, out var unitPrice);
        if (!int.TryParse(QuantityBox.Text, out var quantity) || quantity < 1)
            quantity = 1;

        Result = new LineItemEntity
        {
            Description = desc,
            UnitPrice = unitPrice,
            Quantity = quantity,
        };

        if (!string.IsNullOrEmpty(_editingItemId))
            Result.Id = _editingItemId;
    }

    private void OnSecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        WasDeleted = true;
    }

    private void MinusQty_Click(object sender, RoutedEventArgs e)
    {
        if (int.TryParse(QuantityBox.Text, out var qty) && qty > 1)
            QuantityBox.Text = (qty - 1).ToString();
        UpdateLineTotal();
    }

    private void PlusQty_Click(object sender, RoutedEventArgs e)
    {
        if (int.TryParse(QuantityBox.Text, out var qty))
            QuantityBox.Text = (qty + 1).ToString();
        else
            QuantityBox.Text = "1";
        UpdateLineTotal();
    }

    private void PriceOrQty_Changed(object sender, TextChangedEventArgs e)
    {
        UpdateLineTotal();
    }

    private void UpdateLineTotal()
    {
        _ = double.TryParse(UnitPriceBox.Text, out var price);
        if (!int.TryParse(QuantityBox.Text, out var qty) || qty < 1)
            qty = 1;
        LineTotalText.Text = $"${price * qty:F2}";
    }

    private void ApplyGradientBrushes()
    {
        HeaderBanner.Background = DialogHelper.BuildGradientBrush();
        TotalBorder.Background = DialogHelper.BuildGradientBrush();
        HeaderIcon.Foreground = (Brush)Application.Current.Resources["AppBarAccentBrush"];
        HeaderTitle.Foreground = (Brush)Application.Current.Resources["AppBarForegroundBrush"];
        HeaderSubtitle.Foreground = (Brush)Application.Current.Resources["UpgradeBannerSubtitleBrush"];
        TotalLabel.Foreground = (Brush)Application.Current.Resources["UpgradeBannerSubtitleBrush"];
        LineTotalText.Foreground = (Brush)Application.Current.Resources["AppBarForegroundBrush"];
        LineTotalText.FontFamily = Application.Current.Resources["MonospaceFontFamily"] as FontFamily;
    }
}

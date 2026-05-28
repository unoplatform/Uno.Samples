using Microsoft.UI.Xaml.Media;

namespace QuoteCraft.Presentation;

public sealed partial class CatalogBrowserDialog : ContentDialog
{
    private List<CatalogItemEntity> _allItems = [];
    private string _searchText = string.Empty;
    private string _selectedCategory = "All";

    public CatalogBrowserDialog()
    {
        this.InitializeComponent();
        ApplyGradientBrushes();
    }

    /// <summary>
    /// Fired when the user clicks the Add button on a catalog item.
    /// </summary>
    public event Action<CatalogItemEntity>? ItemAdded;

    public void LoadItems(List<CatalogItemEntity> items)
    {
        _allItems = items;
        _searchText = string.Empty;
        _selectedCategory = "All";
        SearchBox.Text = string.Empty;
        BuildCategoryChips();
        FilterItems();
    }

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        _searchText = SearchBox.Text ?? string.Empty;
        FilterItems();
    }

    private void BuildCategoryChips()
    {
        CategoryChips.Children.Clear();
        var categories = new List<string> { "All" };
        categories.AddRange(_allItems.Select(i => i.Category).Distinct().OrderBy(c => c));

        foreach (var cat in categories)
        {
            var chip = new Button
            {
                Content = cat,
                Padding = new Thickness(12, 4, 12, 4),
                CornerRadius = new CornerRadius(16),
                MinHeight = 28,
                BorderThickness = new Thickness(1),
                Tag = cat,
            };
            UpdateChipStyle(chip, cat == _selectedCategory);
            chip.Click += CategoryChip_Click;
            CategoryChips.Children.Add(chip);
        }
    }

    private void UpdateChipStyle(Button chip, bool isSelected)
    {
        if (isSelected)
        {
            chip.Background = (Brush)Application.Current.Resources["PrimaryBrush"];
            chip.Foreground = (Brush)Application.Current.Resources["OnPrimaryBrush"];
            chip.BorderBrush = (Brush)Application.Current.Resources["PrimaryBrush"];
        }
        else
        {
            chip.Background = null;
            chip.Foreground = (Brush)Application.Current.Resources["OnSurfaceBrush"];
            chip.BorderBrush = (Brush)Application.Current.Resources["OutlineVariantBrush"];
        }
    }

    private void CategoryChip_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is string cat)
        {
            _selectedCategory = cat;
            foreach (var child in CategoryChips.Children)
            {
                if (child is Button chipBtn)
                    UpdateChipStyle(chipBtn, chipBtn.Tag as string == _selectedCategory);
            }
            FilterItems();
        }
    }

    private void FilterItems()
    {
        IEnumerable<CatalogItemEntity> filtered = _allItems;

        if (_selectedCategory != "All")
            filtered = filtered.Where(i => i.Category.Equals(_selectedCategory, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(_searchText))
            filtered = filtered.Where(i =>
                i.Description.Contains(_searchText, StringComparison.OrdinalIgnoreCase) ||
                i.Category.Contains(_searchText, StringComparison.OrdinalIgnoreCase));

        CatalogItemsList.ItemsSource = filtered.ToList();
    }

    private void AddItem_Click(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: CatalogItemEntity item })
            ItemAdded?.Invoke(item);
    }

    private void ApplyGradientBrushes()
    {
        HeaderBanner.Background = DialogHelper.BuildGradientBrush();
        HeaderIcon.Foreground = (Brush)Application.Current.Resources["AppBarAccentBrush"];
        HeaderTitle.Foreground = (Brush)Application.Current.Resources["AppBarForegroundBrush"];
        HeaderSubtitle.Foreground = (Brush)Application.Current.Resources["UpgradeBannerSubtitleBrush"];
    }
}

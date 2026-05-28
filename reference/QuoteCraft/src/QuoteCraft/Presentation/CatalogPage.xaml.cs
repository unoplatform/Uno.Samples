using Microsoft.UI.Xaml.Input;

namespace QuoteCraft.Presentation;

public sealed partial class CatalogPage : Page
{
    /// <summary>Access the underlying MVUX model from the generated ViewModel DataContext.</summary>
    private CatalogModel? Model => MvuxHelper.GetModel<CatalogModel>(DataContext);

    public CatalogPage()
    {
        this.InitializeComponent();
    }

    // -- Category Card Selection ------------------------------------------------

    private async void CategoryCard_Tapped(object sender, TappedRoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: CatalogCategoryCard card })
        {
            var model = Model;
            if (model is not null)
                await model.SelectCategoryCard(card, CancellationToken.None);
        }
    }

    // -- Item Editing -----------------------------------------------------------

    private async void CatalogItem_Tapped(object sender, TappedRoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: CatalogDisplayItem item })
        {
            var model = Model;
            if (model is null) return;
            var entity = await model.GetItemByIdAsync(item.Id);
            if (entity is not null)
                await ShowCatalogEditorDialog(entity, isNew: false);
        }
    }

    private async void AddItem_Click(object sender, RoutedEventArgs e)
    {
        await ShowCatalogEditorDialog(new CatalogItemEntity(), isNew: true);
    }

    private async void AddItemToCategory_Click(object sender, RoutedEventArgs e)
    {
        // Pre-fill category with the currently selected category
        var entity = new CatalogItemEntity();
        var catalogModel = Model;
        if (catalogModel is not null)
        {
            var category = await catalogModel.SelectedCategory;
            if (!string.IsNullOrEmpty(category))
                entity.Category = category;
        }
        await ShowCatalogEditorDialog(entity, isNew: true);
    }

    private async Task ShowCatalogEditorDialog(CatalogItemEntity entity, bool isNew = false)
    {
        var model = Model;
        if (model is null) return;

        var descBox = new TextBox
        {
            Header = "Description",
            Text = entity.Description ?? string.Empty,
            PlaceholderText = "Item description",
        };
        var priceBox = new TextBox
        {
            Header = "Unit Price ($)",
            Text = isNew ? string.Empty : entity.UnitPrice.ToString("F2"),
            PlaceholderText = "0.00",
            InputScope = new InputScope { Names = { new InputScopeName(InputScopeNameValue.Number) } },
        };

        // Category ComboBox with existing categories
        var categories = await model.GetCategoriesAsync();
        var categoryBox = new ComboBox
        {
            Header = "Category",
            IsEditable = true,
            PlaceholderText = "e.g. Plumbing, Electrical, Painting",
            ItemsSource = categories,
            HorizontalAlignment = HorizontalAlignment.Stretch,
        };

        // Set current category value
        var currentCategory = entity.Category ?? string.Empty;
        if (!string.IsNullOrEmpty(currentCategory))
        {
            if (categories.Contains(currentCategory))
                categoryBox.SelectedItem = currentCategory;
            else
                categoryBox.Text = currentCategory;
        }

        var fieldsPanel = new StackPanel { Spacing = 16 };
        fieldsPanel.Children.Add(descBox);
        fieldsPanel.Children.Add(priceBox);
        fieldsPanel.Children.Add(categoryBox);

        if (!isNew)
        {
            var deleteBtn = new Button
            {
                Content = "Delete Item",
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Foreground = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["ErrorBrush"],
                Style = (Style)Application.Current.Resources["MaterialOutlinedButtonStyle"],
                Margin = new Thickness(0, 8, 0, 0),
            };
            deleteBtn.Click += async (s, args) =>
            {
                var confirmDialog = Helpers.DialogHelper.BuildBannerDialog(
                    this.XamlRoot,
                    "\uE74D",
                    "Delete Catalog Item?",
                    "This item will be permanently removed from your catalog.",
                    closeButtonText: "Cancel",
                    primaryButtonText: "Delete");

                var confirmResult = await confirmDialog.ShowAsync();
                if (confirmResult == ContentDialogResult.Primary)
                {
                    await model.DeleteItemAsync(entity.Id, CancellationToken.None);
                    RefreshCatalog();
                }
            };
            fieldsPanel.Children.Add(deleteBtn);
        }

        var dialog = Helpers.DialogHelper.BuildBannerDialogWithContent(
            this.XamlRoot,
            "\uE82D",
            isNew ? "Add Catalog Item" : "Edit Catalog Item",
            isNew ? "Add a new item to your pricing catalog" : "Update item details and pricing",
            fieldsPanel,
            primaryButtonText: "Save",
            closeButtonText: "Cancel");

        dialog.PrimaryButtonClick += async (s, args) =>
        {
            var desc = descBox.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(desc))
            {
                args.Cancel = true;
                return;
            }

            entity.Description = desc;

            // Read category from ComboBox (selected item or typed text)
            var selectedCategory = categoryBox.SelectedItem as string;
            var typedCategory = categoryBox.Text?.Trim();
            entity.Category = !string.IsNullOrEmpty(selectedCategory) ? selectedCategory
                            : !string.IsNullOrEmpty(typedCategory) ? typedCategory
                            : string.Empty;

            var priceStr = priceBox.Text ?? "0";
            if (decimal.TryParse(priceStr, out var price))
                entity.UnitPrice = price;

            await model.SaveItemAsync(entity, CancellationToken.None);
        };

        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            RefreshCatalog();
        }
    }

    private void RefreshCatalog()
    {
        var model = Model;
        if (model is not null)
            _ = model.RefreshList(CancellationToken.None);
    }
}

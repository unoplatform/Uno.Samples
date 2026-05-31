using Microsoft.UI.Xaml.Input;

namespace QuoteCraft.Presentation;

public sealed partial class QuoteEditorPage : Page
{
    /// <summary>Access the underlying MVUX model from the generated ViewModel DataContext.</summary>
    private QuoteEditorModel? Model => MvuxHelper.GetModel<QuoteEditorModel>(DataContext);

    public QuoteEditorPage()
    {
        this.InitializeComponent();
    }

    // -- Back Navigation with Unsaved Changes Warning --------------------------

    private async void BackButton_Click(object sender, RoutedEventArgs e)
    {
        bool hasChanges = false;
        var model = Model;
        if (model is not null)
            hasChanges = await model.HasUnsavedChangesAsync();

        if (hasChanges)
        {
            var dialog = DialogHelper.BuildBannerDialog(
                this.XamlRoot,
                "\uE7BA",
                "Unsaved Changes",
                "You have unsaved changes. Are you sure you want to leave?",
                closeButtonText: "Stay",
                primaryButtonText: "Discard");
            var result = await dialog.ShowAsync();
            if (result != ContentDialogResult.Primary)
                return;
        }

        model = Model;
        if (model is not null)
            await model.GoBack(CancellationToken.None);
    }

    // -- Line Item Dialog -------------------------------------------------------

    private async void OpenAddLineItem_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new LineItemEditorDialog { XamlRoot = this.XamlRoot };
        dialog.SetAddMode();
        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary && dialog.Result is not null)
        {
            var model = Model;
            if (model is not null)
                await model.SaveLineItem(dialog.Result, CancellationToken.None);
        }
    }

    private async void LineItemCard_Tapped(object sender, TappedRoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: LineItemEntity item })
        {
            var dialog = new LineItemEditorDialog { XamlRoot = this.XamlRoot };
            dialog.SetEditMode(item);
            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary && dialog.Result is not null)
            {
                var model = Model;
                if (model is not null)
                    await model.SaveLineItem(dialog.Result, CancellationToken.None);
            }
            else if (result == ContentDialogResult.Secondary && dialog.WasDeleted)
            {
                var model = Model;
                if (model is not null)
                    await model.DeleteLineItem(new LineItemEntity { Id = dialog.EditingItemId ?? string.Empty }, CancellationToken.None);
            }
        }
    }

    // -- Catalog Browser Dialog -------------------------------------------------

    private async void OpenCatalogBrowser_Click(object sender, RoutedEventArgs e)
    {
        List<CatalogItemEntity>? items = null;
        var model = Model;
        if (model is not null)
            items = await model.GetCatalogItemsAsync();

        if (items is null || items.Count == 0) return;

        var dialog = new CatalogBrowserDialog { XamlRoot = this.XamlRoot };
        dialog.LoadItems(items);
        dialog.ItemAdded += async catalogItem =>
        {
            var m = Model;
            if (m is not null)
                await m.AddFromCatalog(catalogItem, CancellationToken.None);
        };
        await dialog.ShowAsync();
    }

    // -- Photo Picker -----------------------------------------------------------

    private async void AddPhoto_Click(object sender, RoutedEventArgs e)
    {
        var model = Model;
        if (model is null) return;

        if (model.CurrentPhotoCount >= model.MaxPhotos)
        {
            var limitDialog = DialogHelper.BuildBannerDialog(
                this.XamlRoot,
                "\uE722",
                "Photo Limit Reached",
                $"You can attach up to {model.MaxPhotos} photos per quote.",
                closeButtonText: "OK");
            await limitDialog.ShowAsync();
            return;
        }

        var picker = new Windows.Storage.Pickers.FileOpenPicker();
        picker.FileTypeFilter.Add(".jpg");
        picker.FileTypeFilter.Add(".jpeg");
        picker.FileTypeFilter.Add(".png");
        picker.FileTypeFilter.Add(".bmp");
        picker.FileTypeFilter.Add(".webp");

#if !__BROWSERWASM__
        WinRT.Interop.InitializeWithWindow.Initialize(picker,
            WinRT.Interop.WindowNative.GetWindowHandle(App.MainAppWindow));
#endif

        var file = await picker.PickSingleFileAsync();
        if (file is not null)
        {
            await model.AddPhotoFromPath(file.Path, CancellationToken.None);
        }
    }

    // -- Client AutoSuggest -----------------------------------------------------

    private async void ClientAutoSuggest_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        {
            var query = sender.Text?.Trim() ?? string.Empty;
            if (query.Length < 1)
            {
                sender.ItemsSource = null;
                return;
            }

            List<string>? suggestions = null;
            var model = Model;
            if (model is not null)
                suggestions = await model.SearchClientNamesAsync(query);

            sender.ItemsSource = suggestions;
        }
    }

    private void ClientAutoSuggest_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        if (args.SelectedItem is string clientName)
            sender.Text = clientName;
    }
}

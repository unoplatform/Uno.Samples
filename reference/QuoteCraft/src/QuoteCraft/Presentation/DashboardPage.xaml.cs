using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;

namespace QuoteCraft.Presentation;

public sealed partial class DashboardPage : Page
{
    private Border? _selectedQuoteCard;
    private Uno.Toolkit.UI.Chip[] _filterChips = [];

    private static SolidColorBrush AlternateRowBrush =>
        (SolidColorBrush)Application.Current.Resources["AlternateRowBrush"];

    public static readonly DependencyProperty IsEditModeProperty =
        DependencyProperty.Register(
            nameof(IsEditMode), typeof(bool), typeof(DashboardPage),
            new PropertyMetadata(false));

    public bool IsEditMode
    {
        get => (bool)GetValue(IsEditModeProperty);
        set => SetValue(IsEditModeProperty, value);
    }

    public static readonly DependencyProperty EditingNotesProperty =
        DependencyProperty.Register(
            nameof(EditingNotes), typeof(string), typeof(DashboardPage),
            new PropertyMetadata(string.Empty));

    public string EditingNotes
    {
        get => (string)GetValue(EditingNotesProperty);
        set => SetValue(EditingNotesProperty, value);
    }

    /// <summary>Access the underlying MVUX model from the generated ViewModel DataContext.</summary>
    private DashboardModel? Model => MvuxHelper.GetModel<DashboardModel>(DataContext);

    public DashboardPage()
    {
        this.InitializeComponent();
        this.Loaded += DashboardPage_Loaded;

        _filterChips = [FilterAll, FilterDraft, FilterSent, FilterViewed, FilterAccepted, FilterDeclined, FilterExpired];
    }

    private async void FilterChip_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Uno.Toolkit.UI.Chip clickedChip) return;

        // Enforce single selection: uncheck all others
        foreach (var chip in _filterChips)
            chip.IsChecked = chip == clickedChip;

        var filter = clickedChip.Content as string;
        if (string.IsNullOrEmpty(filter)) return;

        var model = Model;
        if (model is not null)
            await model.SelectedFilter.UpdateAsync(_ => filter, CancellationToken.None);
    }

    private async void DashboardPage_Loaded(object sender, RoutedEventArgs e)
    {
        // Auto-expire overdue quotes on dashboard load
        var model = Model;
        if (model is not null)
            await model.ExpireOverdueQuotes(CancellationToken.None);
    }

    // -- Quote Card Selection ---------------------------------------------------

    private void QuoteCard_Tapped(object sender, TappedRoutedEventArgs e)
    {
        if (sender is Border tappedCard)
        {
            IsEditMode = false;

            // Clear previous selection visual
            if (_selectedQuoteCard is not null)
            {
                _selectedQuoteCard.Background = (SolidColorBrush)Application.Current.Resources["SurfaceBrush"];
                _selectedQuoteCard.BorderBrush = (SolidColorBrush)Application.Current.Resources["OutlineVariantBrush"];
            }

            // Highlight new selection
            tappedCard.Background = (SolidColorBrush)Application.Current.Resources["PrimaryContainerBrush"];
            tappedCard.BorderBrush = (SolidColorBrush)Application.Current.Resources["PrimaryBrush"];
            _selectedQuoteCard = tappedCard;

            // Model command is invoked via CommandExtensions.Command on the ItemsRepeater
        }
    }

    // -- Inline Edit ------------------------------------------------------------

    private async void EditQuote_Click(object sender, RoutedEventArgs e)
    {
        IsEditMode = true;
        var model = Model;
        if (model is not null)
            EditingNotes = await model.GetSelectedQuoteNotesAsync();
    }

    private async void DoneEditing_Click(object sender, RoutedEventArgs e)
    {
        IsEditMode = false;
        var model = Model;
        if (model is not null)
            await model.SaveInlineNotes(EditingNotes, CancellationToken.None);
    }

    private async void DeleteQuote_Click(object sender, RoutedEventArgs e)
    {
        var dialog = DialogHelper.BuildBannerDialog(
            this.XamlRoot,
            "\uE74D",
            "Delete Quote?",
            "This quote and all its line items will be permanently removed.",
            closeButtonText: "Cancel",
            primaryButtonText: "Delete");

        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            var model = Model;
            if (model is not null)
                await model.DeleteQuote(CancellationToken.None);
        }
    }

    // -- Alternate Row Styling --------------------------------------------------

    private void LineItems_ElementPrepared(Microsoft.UI.Xaml.Controls.ItemsRepeater sender, Microsoft.UI.Xaml.Controls.ItemsRepeaterElementPreparedEventArgs args)
    {
        if (args.Element is FrameworkElement element)
        {
            if (args.Index % 2 == 1)
            {
                if (element is Panel panel) panel.Background = AlternateRowBrush;
                else if (element is Border border) border.Background = AlternateRowBrush;
            }
            else
            {
                if (element is Panel panel) panel.Background = null;
                else if (element is Border border) border.Background = null;
            }
        }
    }

    // -- Inline Line Item Dialog ------------------------------------------------

    private async void InlineAddLineItem_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new LineItemEditorDialog { XamlRoot = this.XamlRoot };
        dialog.SetAddMode();
        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary && dialog.Result is not null)
        {
            var model = Model;
            if (model is not null)
                await model.SaveInlineLineItem(dialog.Result, CancellationToken.None);
        }
    }

    private async void InlineLineItem_Tapped(object sender, TappedRoutedEventArgs e)
    {
        if (!IsEditMode) return;
        if (sender is FrameworkElement { DataContext: LineItemEntity item })
        {
            var dialog = new LineItemEditorDialog { XamlRoot = this.XamlRoot };
            dialog.SetEditMode(item);
            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary && dialog.Result is not null)
            {
                var model = Model;
                if (model is not null)
                    await model.SaveInlineLineItem(dialog.Result, CancellationToken.None);
            }
            else if (result == ContentDialogResult.Secondary && dialog.WasDeleted)
            {
                var model = Model;
                if (model is not null)
                    await model.DeleteInlineLineItem(dialog.EditingItemId ?? string.Empty, CancellationToken.None);
            }
        }
    }

    // -- Create Quote Dialog ----------------------------------------------------

    private async void CreateQuote_Click(object sender, RoutedEventArgs e)
    {
        var model = Model;
        if (model is null) return;

        // Feature gate check
        if (!await model.CanCreateQuoteAsync())
        {
            var limitDialog = Helpers.DialogHelper.BuildBannerDialog(
                this.XamlRoot,
                "\uE1D0",
                "Quote Limit Reached",
                model.GetQuoteLimitMessage(),
                closeButtonText: "OK",
                primaryButtonText: "Upgrade");
            await limitDialog.ShowAsync();
            return;
        }

        var titleBox = new TextBox
        {
            Header = "Quote Title",
            Text = string.Empty,
            PlaceholderText = "e.g. Kitchen Renovation",
        };

        var allClients = await model.GetAllClientsAsync();
        var clientNames = allClients.Select(c => c.Name).ToList();

        var clientBox = new AutoSuggestBox
        {
            Header = "Client Name (optional)",
            PlaceholderText = "Start typing a client name...",
        };
        clientBox.TextChanged += (s, args) =>
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var query = s.Text?.Trim() ?? string.Empty;
                s.ItemsSource = string.IsNullOrEmpty(query)
                    ? null
                    : clientNames.Where(n => n.Contains(query, StringComparison.OrdinalIgnoreCase)).Take(8).ToList();
            }
        };
        clientBox.SuggestionChosen += (s, args) =>
        {
            s.Text = args.SelectedItem as string ?? string.Empty;
        };

        var fieldsPanel = new StackPanel { Spacing = 16 };
        fieldsPanel.Children.Add(titleBox);
        fieldsPanel.Children.Add(clientBox);

        var dialog = Helpers.DialogHelper.BuildBannerDialogWithContent(
            this.XamlRoot,
            "\uE1D0",
            "New Quote",
            "Create a new quote for a client",
            fieldsPanel,
            primaryButtonText: "Create",
            closeButtonText: "Cancel");

        dialog.PrimaryButtonClick += (s, args) =>
        {
            var title = titleBox.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(title))
            {
                args.Cancel = true;
                return;
            }
        };

        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            var title = titleBox.Text?.Trim() ?? "New Quote";
            var clientName = clientBox.Text?.Trim() ?? string.Empty;
            await model.CreateQuoteAsync(title, clientName, CancellationToken.None);
        }
    }

    // -- Send Quote via Email ---------------------------------------------------

    private async void SendQuote_Click(object sender, RoutedEventArgs e)
    {
        var model = Model;
        if (model is null) return;

        var quote = await model.SelectedQuote;
        if (quote is null) return;

        var freshQuote = await model.GetFreshQuoteAsync(quote.Id);
        if (freshQuote is null) return;

        // Resolve client email
        var clientEmail = string.Empty;
        if (!string.IsNullOrEmpty(freshQuote.ClientId))
        {
            var client = await model.GetClientByIdAsync(freshQuote.ClientId);
            clientEmail = client?.Email ?? string.Empty;
        }

        // Resolve business profile for email template
        var profile = await model.GetProfileAsync();
        var businessName = profile.BusinessName ?? "Our Company";

        // Build email fields
        var recipientBox = new TextBox
        {
            Header = "Recipient Email",
            Text = clientEmail,
            PlaceholderText = "client@example.com",
        };
        var subjectBox = new TextBox
        {
            Header = "Subject",
            Text = $"Quote {freshQuote.QuoteNumber} - {freshQuote.Title}",
        };
        var bodyBox = new TextBox
        {
            Header = "Message",
            Text = $"Hi {freshQuote.ClientName ?? "there"},\n\nPlease find attached quote {freshQuote.QuoteNumber} for \"{freshQuote.Title}\".\n\nThe quote is valid until {freshQuote.ValidUntil?.ToString("MMMM d, yyyy") ?? "further notice"}.\n\nPlease don't hesitate to reach out with any questions.\n\nBest regards,\n{businessName}",
            AcceptsReturn = true,
            TextWrapping = TextWrapping.Wrap,
            MinHeight = 140,
        };
        var infoBanner = new Border
        {
            Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["TertiaryContainerBrush"],
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(12, 8, 12, 8),
            Child = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Spacing = 8,
                Children =
                {
                    new FontIcon
                    {
                        Glyph = "\uE946",
                        FontSize = 14,
                        Foreground = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["OnTertiaryContainerBrush"],
                        VerticalAlignment = VerticalAlignment.Center,
                    },
                    new TextBlock
                    {
                        Text = "The PDF will be downloaded. Please attach it to the email.",
                        Style = (Style)Application.Current.Resources["BodySmall"],
                        Foreground = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["OnTertiaryContainerBrush"],
                        TextWrapping = TextWrapping.Wrap,
                        VerticalAlignment = VerticalAlignment.Center,
                    },
                },
            },
        };

        var fieldsPanel = new StackPanel { Spacing = 12 };
        fieldsPanel.Children.Add(recipientBox);
        fieldsPanel.Children.Add(subjectBox);
        fieldsPanel.Children.Add(bodyBox);
        fieldsPanel.Children.Add(infoBanner);

        var dialog = Helpers.DialogHelper.BuildBannerDialogWithContent(
            this.XamlRoot,
            "\uE724",
            "Send Quote",
            $"Send {freshQuote.QuoteNumber} to {freshQuote.ClientName ?? "client"}",
            fieldsPanel,
            primaryButtonText: "Send via Email",
            closeButtonText: "Cancel");

        dialog.SecondaryButtonText = "Download PDF Only";

        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
            // Download PDF + launch email client + mark as sent
            await model.GenerateAndDownloadPdfAsync(freshQuote);
            await model.ComposeEmailAsync(
                recipientBox.Text?.Trim() ?? string.Empty,
                subjectBox.Text?.Trim() ?? string.Empty,
                bodyBox.Text?.Trim() ?? string.Empty);
            await model.MarkQuoteAsSentAsync(freshQuote);
            await model.RefreshDetail(CancellationToken.None);
        }
        else if (result == ContentDialogResult.Secondary)
        {
            // Download PDF only
            await model.GenerateAndDownloadPdfAsync(freshQuote);
        }
    }

    // -- Inline Catalog Browser -------------------------------------------------

    private async void InlineCatalogBrowser_Click(object sender, RoutedEventArgs e)
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
                await m.AddInlineFromCatalog(catalogItem, CancellationToken.None);
        };
        await dialog.ShowAsync();
    }
}

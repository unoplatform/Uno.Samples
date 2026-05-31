using Microsoft.UI.Xaml.Input;

namespace QuoteCraft.Presentation;

public sealed partial class ClientsPage : Page
{
    private ClientEntity? _selectedClientEntity;

    /// <summary>Access the underlying MVUX model from the generated ViewModel DataContext.</summary>
    private ClientsModel? Model => MvuxHelper.GetModel<ClientsModel>(DataContext);

    public ClientsPage()
    {
        this.InitializeComponent();
    }

    private async void ClientCard_Tapped(object sender, TappedRoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: ClientDisplayItem client })
        {
            _selectedClientEntity = client.Entity;
            var model = Model;
            if (model is not null)
                await model.SelectClient(client, CancellationToken.None);
        }
    }

    private async void AddClient_Click(object sender, RoutedEventArgs e)
    {
        var model = Model;
        if (model is null) return;

        if (!await model.CanAddClientAsync())
        {
            var limitDialog = Helpers.DialogHelper.BuildBannerDialog(
                this.XamlRoot,
                "\uE77B",
                "Client Limit Reached",
                model.GetClientUpgradeMessage(),
                closeButtonText: "OK",
                primaryButtonText: "Upgrade");
            await limitDialog.ShowAsync();
            return;
        }

        await ShowClientEditorDialog(new ClientEntity(), isNew: true);
    }

    private async void EditClient_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedClientEntity is null) return;
        var model = Model;
        if (model is null) return;

        // Re-fetch to get latest data
        var fresh = await model.GetClientByIdAsync(_selectedClientEntity.Id);
        if (fresh is not null)
            await ShowClientEditorDialog(fresh, isNew: false);
    }

    private async Task ShowClientEditorDialog(ClientEntity entity, bool isNew = false)
    {
        var model = Model;
        if (model is null) return;

        var nameBox = new TextBox
        {
            Header = "Name",
            Text = entity.Name ?? string.Empty,
            PlaceholderText = "Client or business name",
        };
        var emailBox = new TextBox
        {
            Header = "Email",
            Text = entity.Email ?? string.Empty,
            PlaceholderText = "email@example.com",
            InputScope = new InputScope { Names = { new InputScopeName(InputScopeNameValue.EmailSmtpAddress) } },
        };
        var phoneBox = new TextBox
        {
            Header = "Phone",
            Text = entity.Phone ?? string.Empty,
            PlaceholderText = "(555) 123-4567",
            InputScope = new InputScope { Names = { new InputScopeName(InputScopeNameValue.TelephoneNumber) } },
        };
        var addressBox = new TextBox
        {
            Header = "Address",
            Text = entity.Address ?? string.Empty,
            PlaceholderText = "Street, City, State/Province",
            AcceptsReturn = true,
            TextWrapping = TextWrapping.Wrap,
            MinHeight = 80,
        };

        var fieldsPanel = new StackPanel { Spacing = 16 };
        fieldsPanel.Children.Add(nameBox);
        fieldsPanel.Children.Add(emailBox);
        fieldsPanel.Children.Add(phoneBox);
        fieldsPanel.Children.Add(addressBox);

        var dialog = Helpers.DialogHelper.BuildBannerDialogWithContent(
            this.XamlRoot,
            "\uE77B",
            isNew ? "Add Client" : "Edit Client",
            isNew ? "Add a new client to your contact list" : "Update client details",
            fieldsPanel,
            primaryButtonText: "Save",
            closeButtonText: "Cancel");

        dialog.PrimaryButtonClick += async (s, args) =>
        {
            var name = nameBox.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(name))
            {
                args.Cancel = true;
                return;
            }

            entity.Name = name;
            entity.Email = emailBox.Text;
            entity.Phone = phoneBox.Text;
            entity.Address = addressBox.Text;
            entity.UpdatedAt = DateTimeOffset.UtcNow;
            await model.SaveClientAsync(entity, CancellationToken.None);
        };

        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            RefreshClients();
        }
    }

    private void RefreshClients()
    {
        var model = Model;
        if (model is not null)
            _ = model.RefreshList(CancellationToken.None);
    }

    private async void DeleteClient_Click(object sender, RoutedEventArgs e)
    {
        var model = Model;
        if (model is null) return;

        var dialog = Helpers.DialogHelper.BuildBannerDialog(
            this.XamlRoot,
            "\uE74D",
            "Delete Client?",
            "This client will be permanently removed from your contact list.",
            closeButtonText: "Cancel",
            primaryButtonText: "Delete");

        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            await model.DeleteClient(CancellationToken.None);
            _selectedClientEntity = null;
        }
    }

    private async void NewQuoteForClient_Click(object sender, RoutedEventArgs e)
    {
        var model = Model;
        if (model is not null)
            await model.CreateQuoteForClient(CancellationToken.None);
    }

    private async void QuoteRow_Tapped(object sender, TappedRoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: QuoteEntity quote })
        {
            var model = Model;
            if (model is not null)
                await model.OpenQuoteAsync(quote, CancellationToken.None);
        }
    }
}

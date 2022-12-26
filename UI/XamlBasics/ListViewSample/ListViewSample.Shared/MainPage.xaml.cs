using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using ListViewSample.Models;
using ListViewSample.ViewModels;
using ListViewSample.Dialogs;
using CommunityToolkit.Mvvm.Input;

namespace ListViewSample
{
    public sealed partial class MainPage : Page
    {
        private MainPageViewModel viewModel { get; }
        private ICommand addFriendCommand;

        public MainPage()
        {
            this.InitializeComponent();
            viewModel = DataContext as MainPageViewModel;
            addFriendCommand = new AsyncRelayCommand(PromptForFriendAsync);
        }

        private async Task PromptForFriendAsync()
        {
            NewFriendDialog dialog = new NewFriendDialog();
            ContentDialogResult result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                var friend = new WorkplaceFriend
                {
                    Name = dialog.ConnectionName,
                    Occupation = dialog.ConnectionOccupation
                };
                viewModel.AddFriend(friend);
                await viewModel.SaveFriendsAsync();
            }
        }

        private void ListView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            AlternateRowBackgroundColor((ListViewItem)args.ItemContainer);
        }

        private void AlternateRowBackgroundColor(ListViewItem itemContainer)
        {
            var itemIndex = friendsList.IndexFromContainer(itemContainer);
            // Alternate color between each even and odd ListViewItem
            if (itemIndex % 2 == 0)
            {
                itemContainer.Background = App.Current.Resources["LayerFillColorDefaultBrush"] as SolidColorBrush;
            }
        }
    }
}

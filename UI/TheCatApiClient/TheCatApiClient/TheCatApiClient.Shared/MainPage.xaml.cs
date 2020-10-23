using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TheCatApiClient.Shared.Models.DataModels;
using TheCatApiClient.Shared.Models.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TheCatApiClient
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += async (s,e) => await ViewModel.LoadFavorites();
        }

        private async void BreedSearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            await ViewModel.SearchBreeds();
        }

        // insert favorites below here
        private async void Favorites_ItemClick(object sender, ItemClickEventArgs e)
        {
            await ViewModel.DeleteFavorite((Favorite)e.ClickedItem);
        }

        private async void SearchResults_ItemClick(object sender, ItemClickEventArgs e)
        {
            await ViewModel.AddFavorite((Breed)e.ClickedItem);
        }

    }
}

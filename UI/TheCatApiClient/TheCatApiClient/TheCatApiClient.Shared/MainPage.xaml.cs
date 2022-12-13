using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TheCatApiClient.Shared.Models.DataModels;
using Windows.Foundation;
using Windows.Foundation.Collections;

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
            this.Loaded += async (s, e) => await ViewModel.LoadFavorites();
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

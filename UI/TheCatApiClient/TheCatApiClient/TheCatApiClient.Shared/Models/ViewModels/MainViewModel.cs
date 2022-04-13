using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using TheCatApiClient.Shared.Models.DataModels;
using TheCatApiClient.Shared.WebServices; 

namespace TheCatApiClient.Shared.Models.ViewModels
{
    public class MainViewModel : DispatchedBindableBase
    {
        // Insert member variables below here
        private bool _isBusy;
        private string _searchTerm;
        private ObservableCollection<Breed> _searchResults = new ObservableCollection<Breed>();
        private BreedSearchApi _breedSearchApi = new BreedSearchApi();

        // Insert properties below here
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public string SearchTerm
        {
            get => _searchTerm;
            set => SetProperty(ref _searchTerm, value);
        }

        public ObservableCollection<Breed> SearchResults
        {
            get => _searchResults;
            set => SetProperty(ref _searchResults, value);
        }

        // Insert constructor below here
        public MainViewModel()
        {
        }

        // Insert SearchBreeds below here
        public async Task SearchBreeds()
        {
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                try
                {
                    IsBusy = true;
                    var result = await _breedSearchApi.Search(SearchTerm).ConfigureAwait(false);
                    if (result.Any())
                    {
                        SearchResults = new ObservableCollection<Breed>(result);
                    }
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        // Insert Favorites below here
        private ImageApi _imageApi = new ImageApi();
        private FavoritesApi _favoritesApi = new FavoritesApi();
        private ObservableCollection<Favorite> _favorites = new ObservableCollection<Favorite>();

        public ObservableCollection<Favorite> Favorites
        {
            get => _favorites;
            set => SetProperty(ref _favorites, value);
        }

        // Insert LoadFavorites below here
        public async Task LoadFavorites()
        {
            try
            {
                IsBusy = true;
                var result = await _favoritesApi.GetAll().ConfigureAwait(false);
                if (result.Any())
                {
                    Favorites = new ObservableCollection<Favorite>(result);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        // Insert AddFavorite below here
        public async Task AddFavorite(Breed selectedBreed)
        {
            if (selectedBreed != null)
            {
                try
                {
                    IsBusy = true;
                    var result = await _imageApi.GetByBreed(selectedBreed.Id).ConfigureAwait(false);
                    if (result.Any())
                    {
                        var image = result.First();
                        var response = await _favoritesApi.Add(image).ConfigureAwait(false);

                        if (response != null && response.Message == "SUCCESS")
                        {
                            var favoriteResult = await _favoritesApi.Get(response.Id).ConfigureAwait(false);
                            if (favoriteResult != null)
                            {
                                await DispatchAsync(() => Favorites.Insert(0, favoriteResult)).ConfigureAwait(false);
                            }
                        }
                    }
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        // Insert DeleteFavorite below here
        public async Task DeleteFavorite(Favorite selectedFavorite)
        {
            if (selectedFavorite != null)
            {
                try
                {
                    IsBusy = true;
                    var result = await _favoritesApi.Delete(selectedFavorite).ConfigureAwait(false);
                    if (result.Message != null)
                    {
                        await DispatchAsync(() => Favorites.Remove(selectedFavorite)).ConfigureAwait(false);
                    }
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }
    }
}

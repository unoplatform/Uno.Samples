using System.Collections.ObjectModel;

namespace SkeletonLoaderAnimation.Presentation
{
    public class Item
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string Description { get; set; }
    }
    public partial record MainModel
    {
        private INavigator _navigator;

        public MainModel(
            IStringLocalizer localizer,
            IOptions<AppConfig> appInfo,
            INavigator navigator)
        {
            _navigator = navigator;
            Title = "Main";
            Title += $" - {localizer["ApplicationName"]}";
            Title += $" - {appInfo?.Value?.Environment}";

            MainItems = new ObservableCollection<Item>()
            {
                new Item { Id = Guid.NewGuid().ToString(), Text = "First item", Description="This is an item description." },
                new Item { Id = Guid.NewGuid().ToString(), Text = "Second item", Description="This is an item description." },
                new Item { Id = Guid.NewGuid().ToString(), Text = "Third item", Description = "This is an item description." },
                new Item { Id = Guid.NewGuid().ToString(), Text = "Fourth item", Description = "This is an item description." },
                new Item { Id = Guid.NewGuid().ToString(), Text = "Fifth item", Description = "This is an item description." },
                new Item { Id = Guid.NewGuid().ToString(), Text = "Sixth item", Description = "This is an item description." }
            };
        }

        public string? Title { get; }

        public IState<string> Name => State<string>.Value(this, () => string.Empty);

        public ObservableCollection<Item> MainItems { get; set; }

        public bool IsLoading { get; set; }

        public async Task GoToSecond()
        {
            var name = await Name;
            await _navigator.NavigateViewModelAsync<SecondModel>(this, data: new Entity(name!));
        }

    }
}
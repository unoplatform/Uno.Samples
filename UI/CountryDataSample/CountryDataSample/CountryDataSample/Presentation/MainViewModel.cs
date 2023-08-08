namespace CountryDataSample.Presentation
{
    public partial class MainViewModel : ObservableObject
    {
        private INavigator _navigator;

        [ObservableProperty]
        private string? name;

        [ObservableProperty]
        private Address? address;

        public MainViewModel(
            IStringLocalizer localizer,
            IOptions<AppConfig> appInfo,
            INavigator navigator)
        {
            _navigator = navigator;
            Title = "Main";
            Title += $" - {localizer["ApplicationName"]}";
            Title += $" - {appInfo?.Value?.Environment}";
            GoToSecond = new AsyncRelayCommand(GoToSecondView);

            Address = new Address
            {
                AddressOne = string.Empty,
                AddressTwo = string.Empty,
                State = string.Empty,
                Country = string.Empty,
                City = string.Empty,
                PostalCode = string.Empty
            };
        }
        public string? Title { get; }

        public ICommand GoToSecond { get; }


        private async Task GoToSecondView()
        {
            await _navigator.NavigateViewModelAsync<SecondViewModel>(this, data: Address);
        }

    }
}
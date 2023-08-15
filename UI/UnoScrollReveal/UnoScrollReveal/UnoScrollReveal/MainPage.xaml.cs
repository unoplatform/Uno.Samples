using System.Collections.ObjectModel;

namespace UnoScrollReveal
{
    public sealed partial class MainPage : Page
    {
        public ObservableCollection<Item> Items { get; set; } = new ObservableCollection<Item>();
        List<Item> items = new List<Item>()
            {
                new Item
                {
                    Title = "Tiger Lily - George Ezra",
                    IsEmpty = false,
                    Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec auctor, libero sed aliquam lacinia, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl.",

                },
                new Item
                {
                    Title = "Last Last- Burna Boy",
                    IsEmpty = false,
                    Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec auctor, libero sed aliquam lacinia, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl.",

                },
                new Item
                {
                    Title = "Unknown to you - Jacob Banks",
                    IsEmpty = false,
                    Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec auctor, libero sed aliquam lacinia, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl.",

                },
                new Item
                {
                    Title = "Caroline - Jacob Banks",
                    IsEmpty = false,
                    Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec auctor, libero sed aliquam lacinia, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl.",

                },
                new Item
                {
                    Title = "Okay - Adekunle Gold",
                    IsEmpty = false,
                    Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec auctor, libero sed aliquam lacinia, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl.",

                },
                new Item
                {
                    Title = "It is what it is - Adekunle Gold",
                    IsEmpty = false,
                    Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec auctor, libero sed aliquam lacinia, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl.",

                },
                new Item
                {
                    Title = "American Pie - Don McLean",
                    IsEmpty = false,
                    Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec auctor, libero sed aliquam lacinia, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl.",

                },
                new Item
                {
                    Title = "Anybody - Burna Boy",
                    IsEmpty = false,
                    Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec auctor, libero sed aliquam lacinia, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl.",

                },
                new Item
                {
                    Title = "Ye - Burna Boy",
                    IsEmpty = false,
                    Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec auctor, libero sed aliquam lacinia, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl. Sed euismod, nisl eget ultrices aliquam, nisl nunc faucibus arcu, nec aliquam elit nunc quis nisl.",

                },

            };
        Item emptyItem = new Item { IsEmpty = true };

        public MainPage()
        {
            this.InitializeComponent();
#if ANDROID || IOS
            backgroundImg.Height = 900;
#endif

            Items.AddRange(items);

        }

        private void OnPullToRefresh(RefreshContainer sender, RefreshRequestedEventArgs args)
        {
            Items.Add(emptyItem);
            Items.AddRange(items);
            Items.AddRange(items);

        }


    }
}
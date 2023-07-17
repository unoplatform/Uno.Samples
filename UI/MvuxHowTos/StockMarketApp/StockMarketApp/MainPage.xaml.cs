namespace StockMarketApp
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            this.DataContext = new BindableStockMarketModel(new StockMarketService());
        }
    }
}
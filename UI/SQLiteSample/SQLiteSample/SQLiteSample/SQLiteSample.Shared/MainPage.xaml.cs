using Microsoft.UI.Xaml.Controls;
using SQLiteSample.Data;
using System;
using System.Linq;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SQLiteSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            InitializeDatabase();
        }

        private async void InitializeDatabase()
        {
#if __WASM__
            SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_sqlite3());
#endif
            // Ensure that the storage subsystem is initialized on webassembly
            await Windows.Storage.StorageFolder.GetFolderFromPathAsync(Windows.Storage.ApplicationData.Current.LocalFolder.Path);

            UpdateList();
        }

        public void OnClickMe()
        {
            AddStock(stockSymbol.Text);

            UpdateList();
        }

        private void UpdateList()
        {
            if (SqlLiteContext.Stocks.Count() > 0)
            {
                symbolsList.ItemsSource = SqlLiteContext.Stocks.Select(s => s.Symbol).ToList();
            }
        }

        public static void AddStock(string symbol)
        {
            var stock = new Stock()
            {
                Symbol = symbol
            };

            SqlLiteContext.Stocks.Add(stock);
            SqlLiteContext.SaveChanges();
            Console.WriteLine("{0} == {1}", stock.Symbol, stock.Id);
        }

        private static DataContext SqlLiteContext => sqlLiteContext ??= new();
        private static DataContext sqlLiteContext;
    }

    public record Stock
    {
        public Stock()
        {
        }

        public Stock(int id, string symbol)
        {
        }

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string Symbol { get; set; }
    }

    public record Valuation
    {
        public Valuation()
        {
        }

        public Valuation(int id, int stockId, DateTime time, decimal price)
        {
        }

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public int StockId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public DateTime Time { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public decimal Price { get; set; }
    }
}
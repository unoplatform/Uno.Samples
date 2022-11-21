using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

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
            // Ensure that the storage subsystem is initialized on webassembly
            await Windows.Storage.StorageFolder.GetFolderFromPathAsync(Windows.Storage.ApplicationData.Current.LocalFolder.Path);

            using (var db = TryCreateDatabase())
            {
                UpdateList(db);
            }
        }

        public void OnClickMe()
        {
            using (var db = TryCreateDatabase())
            {
                AddStock(db, stockSymbol.Text);

                UpdateList(db);
            }
        }

        private static SQLiteConnection TryCreateDatabase()
        {
            // Get an absolute path to the database file
            var databasePath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "MyData.db");

            var exists = File.Exists(databasePath);

            var db = new SQLiteConnection(databasePath);

            if (!exists)
            {
                db.CreateTable<Stock>();
                db.CreateTable<Valuation>();
            }

            return db;
        }

        private void UpdateList(SQLiteConnection db)
        {
            symbolsList.ItemsSource = db.Table<Stock>().Select(s => s.Symbol).ToList();
        }

        public static void AddStock(SQLiteConnection db, string symbol)
        {
            var stock = new Stock()
            {
                Symbol = symbol
            };
            db.Insert(stock);
            Console.WriteLine("{0} == {1}", stock.Symbol, stock.Id);
        }
    }

    public class Stock
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Symbol { get; set; }
    }

    public class Valuation
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Indexed]
        public int StockId { get; set; }
        public DateTime Time { get; set; }
        public decimal Price { get; set; }
    }
}

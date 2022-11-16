using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using LiteDB;

namespace LiteDBSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly ObservableCollection<TodoItem> _todoItems;

        public MainPage()
        {
            InitializeComponent();
            _todoItems = new ObservableCollection<TodoItem>();
        }

        private static string DbPath => System.IO.Path.Combine(ApplicationData.Current.LocalFolder.Path, "save.db");

        private void LoadFromDatabase(object sender, RoutedEventArgs args)
        {
            var liteDatabase = new LiteDatabase(DbPath);
            var liteCollection = liteDatabase.GetCollection<TodoItem>();
            var todoItems = liteCollection
                .FindAll()
                .ToList();
            _todoItems.Clear();
            todoItems.ForEach(t => _todoItems.Add(t));
        }

        private void DeleteFromDatabase(object sender, RoutedEventArgs args)
        {
            var liteDatabase = new LiteDatabase(DbPath);
            liteDatabase.GetCollection<TodoItem>().DeleteAll();
            _todoItems.Clear();
        }

        private void AddTodoItem(object sender, RoutedEventArgs args)
        {
            var todoItem = new TodoItem
            {
                Text = todoText.Text
            };

            todoText.Text = string.Empty;
            var liteDatabase = new LiteDatabase(DbPath);
            var liteCollection = liteDatabase.GetCollection<TodoItem>();
            liteCollection.Insert(todoItem);
            liteDatabase.Commit();
            _todoItems.Add(todoItem);
        }
    }
}

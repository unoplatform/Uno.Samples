using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Storage;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using LiteDB;
using Microsoft.UI.Xaml.Navigation;

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

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Before the database is accessed for the first time on WASM, the filesystem needs to be
            // initialized, otherwise data might be lost. Ensure that any asynchronous operation is
			// awaited before creating/accessing the database, as await will wait for the initialization to complete.
            // See https://platform.uno/docs/articles/features/file-management.html#webassembly-file-system for more information.
            var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
			var data = await localFolder.CreateFolderAsync("Data", CreationCollisionOption.OpenIfExists);
            LoadFromDatabase(this, null);
        }

        private LiteDatabase _liteDatabase;

		private LiteDatabase LiteDatabase
		{
			get
			{
				// If the database is created in the constructor, it stops working on WASM.
				if (_liteDatabase == null)
				{
					_liteDatabase = new LiteDatabase(DbPath);
				}
				return _liteDatabase;
			}
		}

		private static string DbPath => System.IO.Path.Combine(ApplicationData.Current.LocalFolder.Path, "Data", "save.db");

		private void LoadFromDatabase(object sender, RoutedEventArgs args)
		{
			var liteCollection = LiteDatabase.GetCollection<TodoItem>();
			var todoItems = liteCollection
				.FindAll()
				.ToList();
			_todoItems.Clear();
			todoItems.ForEach(t => _todoItems.Add(t));
		}

		private void DeleteFromDatabase(object sender, RoutedEventArgs args)
		{
			LiteDatabase.GetCollection<TodoItem>().DeleteAll();
			_todoItems.Clear();
		}

		private void AddTodoItem(object sender, RoutedEventArgs args)
		{
			var todoItem = new TodoItem
			{
				Text = todoText.Text
			};

			todoText.Text = string.Empty;
			var liteCollection = LiteDatabase.GetCollection<TodoItem>();
			liteCollection.Insert(todoItem);
			LiteDatabase.Commit();
			_todoItems.Add(todoItem);
		}
	}
}

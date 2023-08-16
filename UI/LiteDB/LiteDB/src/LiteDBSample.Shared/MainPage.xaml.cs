using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Storage;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
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

		private static string DbPath => System.IO.Path.Combine(ApplicationData.Current.LocalFolder.Path, "save.db");

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

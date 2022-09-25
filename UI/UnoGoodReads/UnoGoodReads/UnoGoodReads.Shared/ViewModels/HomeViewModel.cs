using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

using UnoGoodReads.Models;

namespace UnoGoodReads.ViewModels
{
    public class HomeViewModel
    {
        public ObservableCollection<Book> Books { get; set; }
        public SeedData DataSeeder { get; set; }
        public List<string> States { get; set; }

        public HomeViewModel()
        {
            Books = new ObservableCollection<Book>(App.DataSeeder.FakeBooks);
            States = new List<string> { 
                State.Read.ToStringFormat(),
                State.CurrentlyReading.ToStringFormat(),
                State.WantToRead.ToStringFormat()
            };
            
        }
    }
}

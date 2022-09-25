using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnoGoodReads.Models;

namespace UnoGoodReads.ViewModels
{
    public class BookViewModel
    {
        public Book Book { get; set; }

        public BookViewModel()
        {
            Book = App.DataSeeder.FakeBooks.First();
        }
    }
}

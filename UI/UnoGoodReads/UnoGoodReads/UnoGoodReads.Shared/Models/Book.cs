using System;
using System.Collections.Generic;
using System.Text;

namespace UnoGoodReads.Models
{    
    public class Book
    {
        public Guid Id { get; set; }
        public string ISBN { get; set; }
        public Uri Url { get; set; }
        public int Pages { get; set; }
        public string Publisher { get; set; }
        public DateTime Published { get; set; }
        public string Title { get; set; }
        public Author Author { get; set; }
        public string Description { get; set; }
        public Genre Genre { get; set; }
        public decimal Price { get; set; }
        public Rating AverageRating { get; set; }
        public int RatingsCount { get; set; }
        public State State { get; set; }
    }
}

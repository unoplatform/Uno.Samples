using System;
using System.Collections.Generic;
using System.Text;

namespace UnoGoodReads.Models
{
    public class Author
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Rating AverageRatings { get; set; }
        public int RatingsCount { get; set; }
        public Uri ImageUrl { get; set; }
    }
}

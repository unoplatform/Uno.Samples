using Bogus;
using Bogus.DataSets;

using System;
using System.Collections.Generic;
using System.Text;

namespace UnoGoodReads.Models
{
    public class SeedData
    {
        Faker<Author> authorFaker = new Faker<Author>()
            .RuleFor(a => a.Id, f => Guid.NewGuid())
            .RuleFor(a => a.Name, f => f.Name.FullName())
            .RuleFor(a => a.AverageRatings, f => f.PickRandom<Rating>())
            .RuleFor(a => a.RatingsCount, f => f.Random.Number(1, 1000))
            .RuleFor(a => a.ImageUrl, f => new Uri(f.Image.PlaceImgUrl(width: 200, height: 300, category: "people")));

        Faker<Book> bookFaker = new Faker<Book>()
            .RuleFor(b => b.Id, f => Guid.NewGuid())
            .RuleFor(b => b.ISBN, f => f.Commerce.Ean13())
            .RuleFor(b => b.Url, f => new Uri(f.Image.PicsumUrl(width: 200, height: 300)))
            .RuleFor(b => b.Pages, f => f.Random.Number(1, 1000))
            .RuleFor(b => b.Publisher, f => f.Company.CompanyName())
            .RuleFor(b => b.Published, f => f.Date.Past())
            .RuleFor(b => b.Title, f => f.Lorem.Sentence(null,3))
            .RuleFor(b => b.State, f => f.PickRandom<State>())
            .RuleFor(b => b.Description, f => f.Lorem.Paragraph())
            .RuleFor(b => b.Genre, f => f.PickRandom<Genre>())
            .RuleFor(b => b.Price, f => f.Random.Number(1, 100))
            .RuleFor(b => b.AverageRating, f => f.PickRandom<Rating>())
            .RuleFor(b => b.RatingsCount, f => f.Random.Number(1, 1000));

        public List<Book> FakeBooks { get; set; }
        public List<Author> FakeAuthors { get; set; }
        public SeedData()
        {
            FakeAuthors = authorFaker.Generate(10);
            FakeBooks = bookFaker.Generate(100);

            foreach(Book book in FakeBooks)
            {
                var author = FakeAuthors[new Random().Next(FakeAuthors.Count)];
                book.Author = author;
            }
            
        }
    }
}

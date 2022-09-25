using System;
using System.Collections.Generic;
using System.Text;

using UnoGoodReads.Models;

using Windows.UI.Xaml.Data;

namespace UnoGoodReads.Converters
{
    public class GenreConverters : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var genre = (Genre)value;
            return $"Popular on Goodreads in {genre.ToStringFormat()}";

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }

    public class AuthorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var author = (Author)value;
            return $"by {author.Name}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return "Unknown Author";
        }
    }
    
    public class StateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var state = (State)value;
            return $"{state.ToStringFormat()}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return State.WantToRead;
        }
    }

    public class BookRatingsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var rating = (int)value;
            return $"{rating} ratings. {new Random().Next(1000)} reviewes";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return "";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace UnoGoodReads.Models
{
    public enum State
    {
        WantToRead,
        Read,
        CurrentlyReading
    }
    
    public enum Rating
    {
        One,
        Two,
        Three,
        Four,
        Five
    }

    public enum Genre
    {
        Action,
        Adventure,
        Comedy,
        Drama,
        Horror,
        Mystery,
        Romance,
        SciFi,
        Thriller,
        Western,
        Fiction,
        NonFiction,
        Biography,
    }
    public static class EnumExtensions
    {
        public static string ToStringFormat(this Genre genre)
        {
            switch(genre)
            {
                case Genre.Action:
                    return "Action";
                case Genre.Adventure:
                    return "Adventure";
                case Genre.Comedy:
                    return "Comedy";
                case Genre.Drama:
                    return "Drama";
                case Genre.Horror:
                    return "Horror";
                case Genre.Mystery:
                    return "Mystery";
                case Genre.Romance:
                    return "Romance";
                case Genre.SciFi:
                    return "Sci-Fi";
                case Genre.Thriller:
                    return "Thriller";
                case Genre.Western:
                    return "Western";
                case Genre.Fiction:
                    return "Fiction";
                case Genre.NonFiction:
                    return "Non-Fiction";
                case Genre.Biography:
                    return "Biography";
                default:
                    return "";
            }
        }
        
        public static string ToStringFormat(this State state)
        {
            switch(state)
            {
                case State.WantToRead:
                    return "Want to Read";
                case State.Read:
                    return "Read";
                case State.CurrentlyReading:
                    return "Currently Reading";
                default:
                    return "";
            }
        }
    }
}

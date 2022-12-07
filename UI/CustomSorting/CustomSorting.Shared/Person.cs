using System.Collections.ObjectModel;

namespace CustomSorting
{
    public class Person
    {
        public Person(string displayName, string occupation)
        {
            DisplayName = displayName;
            Occupation = occupation;
        }

        public string DisplayName { get; set; }

        public string Occupation { get; set; }
    }

    // Names provided by https://goodbyejohndoe.com
    public class People : ObservableCollection<Person>
    {
        public People()
        {
            Add(new Person("Evolent, Ben", "Developer"));
            Add(new Person("Ruüd van Driver", "Facilities"));
            Add(new Person("Eleanor Fant", "Facilities"));
            Add(new Person("Weary, Jake", "Developer"));
            Add(new Person("Ruby Von Rails", "Developer"));
            Add(new Person("Hugh Millie-Yate", "Marketing"));
            Add(new Person("Schpellchek, Lurch", "Marketing"));
            Add(new Person("Ravi O'Leigh", "Catering"));
            Add(new Person("Hanson Deck", "Developer"));
            Add(new Person("Hugh Saturation", "Designer"));
            Add(new Person("Justin Case", "Legal"));
            Add(new Person("Serif, Sam", "Designer"));
            Add(new Person("Phillip Anthropy", "Legal"));
            Add(new Person("Max Conversion", "Marketing"));
            Add(new Person("Jarvis Pepperspray", "Designer"));
        }
    }
}

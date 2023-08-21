using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace GrapeCityApp.MauiControls;

public partial class EmbeddedControl : ContentView
{
	public EmbeddedControl()
	{
		Application.Current.MainPage = new Page();

		InitializeComponent();

        var data = Customer.GetCustomerList(20);
        grid.ItemsSource = data;
    }
}



public class Customer : INotifyPropertyChanged, IEditableObject
{
    //fields

    int _id, _countryId, _orderCount;
    string _first, _last;
    string _address, _city, _postalCode, _email;
    bool _active;
    DateTime _lastOrderDate;
    double _orderTotal;

    static Random _rnd = new Random();
    static string[] _firstNames = "Andy|Ben|Charlie|Dan|Ed|Fred|Gil|Herb|Jack|Karl|Larry|Mark|Noah|Oprah|Paul|Quince|Rich|Steve|Ted|Ulrich|Vic|Xavier|Zeb".Split('|');
    static string[] _lastNames = "Ambers|Bishop|Cole|Danson|Evers|Frommer|Griswold|Heath|Jammers|Krause|Lehman|Myers|Neiman|Orsted|Paulson|Quaid|Richards|Stevens|Trask|Ulam".Split('|');
    static KeyValuePair<string, string[]>[] _countries = "China-Beijing,Chongqing,Shanghai,Tianjin,Hong Kong,Macau,Anqing,Bengbu,Bozhou,Chaohu|India-New Delhi,Mumbai,Delhi,Bangalore,Hyderabad,Ahmedabad,Chennai,Kolkata,Surat,Pune|United States-Washington,New York,Los Angeles,Chicago,Houston,Philadelphia,Phoenix,San Antonio,San Diego,Dallas|Indonesia-Jakarta,Surabaya,Bandung,Bekasi,Medan,Tangerang,Depok,Semarang,Palembang,South Tangerang|Brazil-Brasilia,San Pablo,Rio de Janeiro,Salvador,Fortaleza,Belo Horizonte,Manaus,Curitiba,Recife,Porto Alegre|Pakistan-Islamabad,Karachi,Lahore,Faisalabad,Rawalpindi,Gujranwala,Multan,Hyderabad,Peshawar,Quetta|Russia-Moscow,Saint Petersburg,Novosibirsk,Yekaterinburg,Nizhny Novgorod,Kazan,Chelyabinsk,Samara,Omsk,Rostov-na-Donu|Japan-Tokio,Yokohama,Ōsaka,Nagoya,Sapporo,Kōbe,Kyōto,Fukuoka,Kawasaki,Saitama|Mexico-Mexico City,Guadalajara,Monterrey,Puebla,Toluca,Tijuana,León,Juárez,Torreón,Querétaro".Split('|').Select(str => new KeyValuePair<string, string[]>(str.Split('-').First(), str.Split('-').Skip(1).First().Split(','))).ToArray();
    static string[] _emailServers = "gmail|yahoo|outlook|aol".Split('|');
    static string[] _streetNames = "Main|Broad|Grand|Panoramic|Green|Golden|Park|Fake".Split('|');
    static string[] _streetTypes = "ST|AVE|BLVD".Split('|');
    static string[] _streetOrientation = "S|N|W|E|SE|SW|NE|NW".Split('|');

    //initialization

    public Customer()
    {
    }

    public Customer(int id)
    {
        Id = id;
        FirstName = GetRandomString(_firstNames);
        LastName = GetRandomString(_lastNames);
        Address = GetRandomAddress();
        CountryId = _rnd.Next() % _countries.Length;
        var cities = _countries[CountryId].Value;
        City = GetRandomString(cities);
        PostalCode = _rnd.Next(10000, 99999).ToString();
        Email = string.Format("{0}@{1}.com", (FirstName + LastName.Substring(0, 1)).ToLower(), GetRandomString(_emailServers));
        LastOrderDate = DateTime.Today.AddDays(-_rnd.Next(1, 365)).AddHours(_rnd.Next(0, 24)).AddMinutes(_rnd.Next(0, 60));
        OrderCount = _rnd.Next(0, 100);
        OrderTotal = Math.Round(_rnd.NextDouble() * 10000.00, 2);
        Active = _rnd.NextDouble() >= .5;
    }

    //object model

    [Display(AutoGenerateField = false)]
    [JsonIgnore]
    public int Id
    {
        get { return _id; }
        set
        {
            if (value != _id)
            {
                _id = value;
                OnPropertyChanged();
            }
        }
    }

    public string FirstName
    {
        get { return _first; }
        set
        {
            if (value != _first)
            {
                _first = value;
                OnPropertyChanged();
                OnPropertyChanged("Name");
            }
        }
    }

    public string LastName
    {
        get { return _last; }
        set
        {
            if (value != _last)
            {
                _last = value;
                OnPropertyChanged();
                OnPropertyChanged("Name");
            }
        }
    }

    public string Address
    {
        get { return _address; }
        set
        {
            if (value != _address)
            {
                _address = value;
                OnPropertyChanged();
            }
        }
    }

    public string City
    {
        get { return _city; }
        set
        {
            if (value != _city)
            {
                _city = value;
                OnPropertyChanged();
            }
        }
    }

    [Display(AutoGenerateField = false)]
    [JsonIgnore]
    public int CountryId
    {
        get { return _countryId; }
        set
        {
            if (value != _countryId && value > -1 && value < _countries.Length)
            {
                _countryId = value;
                //_city = _countries[_countryId].Value.First();
                OnPropertyChanged();
                OnPropertyChanged("Country");
                OnPropertyChanged("City");
            }
        }
    }

    public string PostalCode
    {
        get { return _postalCode; }
        set
        {
            if (value != _postalCode)
            {
                _postalCode = value;
                OnPropertyChanged();
            }
        }
    }

    [Display(Name = "e-mail")]
    public string Email
    {
        get { return _email; }
        set
        {
            if (value != _email)
            {
                _email = value;
                OnPropertyChanged();
            }
        }
    }

    [Display(AutoGenerateField = false)]
    [JsonIgnore]
    public DateTime LastOrderDate
    {
        get { return _lastOrderDate; }
        set
        {
            if (value != _lastOrderDate)
            {
                _lastOrderDate = value;
                OnPropertyChanged();
            }
        }
    }

    [Display(AutoGenerateField = false)]
    [JsonIgnore]
    public TimeSpan LastOrderTime
    {
        get
        {
            return LastOrderDate.TimeOfDay;
        }
    }

    [Display(AutoGenerateField = false)]
    [JsonIgnore]
    public int OrderCount
    {
        get { return _orderCount; }
        set
        {
            if (value != _orderCount)
            {
                _orderCount = value;
                OnPropertyChanged();
            }
        }
    }

    [Display(AutoGenerateField = false)]
    [JsonIgnore]
    public double OrderTotal
    {
        get { return _orderTotal; }
        set
        {
            if (value != _orderTotal)
            {
                _orderTotal = value;
                OnPropertyChanged();
            }
        }
    }

    [Display(AutoGenerateField = false)]
    [JsonIgnore]
    public bool Active
    {
        get { return _active; }
        set
        {
            if (value != _active)
            {
                _active = value;
                OnPropertyChanged();
            }
        }
    }

    [Display(AutoGenerateField = false)]
    [JsonIgnore]
    public string Name
    {
        get { return string.Format("{0} {1}", FirstName, LastName); }
    }

    [Display(AutoGenerateField = false)]
    [JsonIgnore]
    public string Country
    {
        get { return _countries[_countryId].Key; }
    }

    [Display(AutoGenerateField = false)]
    [JsonIgnore]
    public double OrderAverage
    {
        get { return OrderTotal / (double)OrderCount; }
    }

    //implementation

    // ** utilities
    static string GetRandomString(string[] arr)
    {
        return arr[_rnd.Next(arr.Length)];
    }
    static string GetName()
    {
        return string.Format("{0} {1}", GetRandomString(_firstNames), GetRandomString(_lastNames));
    }

    // ** static list provider
    public static ObservableCollection<Customer> GetCustomerList(int count)
    {
        var list = new ObservableCollection<Customer>();
        for (int i = 0; i < count; i++)
        {
            list.Add(new Customer(i));
        }
        return list;
    }

    private static string GetRandomAddress()
    {
        if (_rnd.NextDouble() > 0.9)
            return string.Format("{0} {1} {2} {3}", _rnd.Next(1, 999), GetRandomString(_streetNames), GetRandomString(_streetTypes), GetRandomString(_streetOrientation));
        else
            return string.Format("{0} {1} {2}", _rnd.Next(1, 999), GetRandomString(_streetNames), GetRandomString(_streetTypes));
    }

    // ** static value providers
    public static KeyValuePair<int, string>[] GetCountries() { return _countries.Select((p, index) => new KeyValuePair<int, string>(index, p.Key)).ToArray(); }
    public static string[] GetFirstNames() { return _firstNames; }
    public static string[] GetLastNames() { return _lastNames; }

    //INotifyPropertyChanged Members

    // this interface allows bounds controls to react to changes in the data objects.
    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
    }

    protected void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        if (PropertyChanged != null)
            PropertyChanged(this, e);
    }

    //IEditableObject Members

    // this interface allows transacted edits (user can press escape to restore previous values).

    Customer _clone;
    public void BeginEdit()
    {
        _clone = (Customer)MemberwiseClone();
    }

    public void EndEdit()
    {
        _clone = null;
    }

    public void CancelEdit()
    {
        if (_clone != null)
        {
            foreach (var p in GetType().GetProperties())
            {
                if (p.CanRead && p.CanWrite)
                {
                    p.SetValue(this, p.GetValue(_clone, null), null);
                }
            }
        }
    }

    public static IEnumerable<City> GetCities()
    {
        return _countries.SelectMany(country => country.Value, (pair, city) => new City() { Name = city, Country = pair.Key });
    }

}

public class City
{
    [Display(AutoGenerateField = false)]
    public bool Selected { get; set; }
    public string Name { get; set; }
    public string Country { get; set; }
}


#nullable disable

using System.Collections.ObjectModel;
using System.ComponentModel;

namespace DevExpressApp.MauiControls;

public partial class DataGridControl : ScrollView
{
    public DataGridControl()
    {
        InitializeComponent();
    }
}

public class EmployeeDataViewModel : INotifyPropertyChanged
{
    readonly EmployeeData data;

#pragma warning disable CS8612 // Nullability of reference types in type doesn't match implicitly implemented member.
    public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS8612 // Nullability of reference types in type doesn't match implicitly implemented member.
    public IReadOnlyList<Employee> Employees { get => data.Employees; }

    public EmployeeDataViewModel()
    {
        data = new EmployeeData();
    }

    protected void RaisePropertyChanged(string name)
    {
        if (PropertyChanged != null)
            PropertyChanged(this, new PropertyChangedEventArgs(name));
    }
}

public enum AccessLevel
{
    Admin,
    User
}

public class Employee
{
    string name;
    string resourceName;

    public string Name
    {
        get { return name; }
        set
        {
            name = value;
            if (Photo == null)
            {
                resourceName = "DataGridExample.Images." + value.Replace(" ", "_") + ".jpg";
                if (!String.IsNullOrEmpty(resourceName))
                    Photo = ImageSource.FromResource(resourceName);
            }
        }
    }

    public Employee(string name)
    {
        this.Name = name;
    }
    public ImageSource Photo { get; set; }
    public DateTime BirthDate { get; set; }
    public DateTime HireDate { get; set; }
    public string Position { get; set; }
    public string Address { get; set; }
    public string Phone { get; set; }
    public AccessLevel Access { get; set; }
    public bool OnVacation { get; set; }
}

public class EmployeeData
{
    void GenerateEmployees()
    {
        ObservableCollection<Employee> result = new ObservableCollection<Employee>();
        result.Add(
            new Employee("Nancy Davolio")
            {
                BirthDate = new DateTime(1978, 12, 8),
                HireDate = new DateTime(2005, 5, 1),
                Position = "Sales Representative",
                Address = "98122, 507 - 20th Ave. E. Apt. 2A, Seattle WA, USA",
                Phone = "(206) 555-9857",
                Access = AccessLevel.User,
                OnVacation = false
            }
        );
        result.Add(
            new Employee("Andrew Fuller")
            {
                BirthDate = new DateTime(1965, 2, 19),
                HireDate = new DateTime(1992, 8, 14),
                Position = "Vice President, Sales",
                Address = "98401, 908 W. Capital Way, Tacoma WA, USA",
                Phone = "(206) 555-9482",
                Access = AccessLevel.Admin,
                OnVacation = false
            }
        );
        result.Add(
            new Employee("Janet Leverling")
            {
                BirthDate = new DateTime(1985, 8, 30),
                HireDate = new DateTime(2002, 4, 1),
                Position = "Sales Representative",
                Address = "98033, 722 Moss Bay Blvd., Kirkland WA, USA",
                Phone = "(206) 555-3412",
                Access = AccessLevel.User,
                OnVacation = false
            }
        );
        result.Add(
            new Employee("Margaret Peacock")
            {
                BirthDate = new DateTime(1973, 9, 19),
                HireDate = new DateTime(1993, 5, 3),
                Position = "Sales Representative",
                Address = "98052, 4110 Old Redmond Rd., Redmond WA, USA",
                Phone = "(206) 555-8122",
                Access = AccessLevel.User,
                OnVacation = false
            }
        );
        result.Add(
            new Employee("Steven Buchanan")
            {
                BirthDate = new DateTime(1955, 3, 4),
                HireDate = new DateTime(1993, 10, 17),
                Position = "Sales Manager",
                Address = "SW1 8JR, 14 Garrett Hill, London, UK",
                Phone = "(71) 555-4848",
                Access = AccessLevel.User,
                OnVacation = true
            }
        );
        result.Add(
            new Employee("Michael Suyama")
            {
                BirthDate = new DateTime(1981, 7, 2),
                HireDate = new DateTime(1999, 10, 17),
                Position = "Sales Representative",
                Address = "EC2 7JR, Coventry House Miner Rd., London, UK",
                Phone = "(71) 555-7773",
                Access = AccessLevel.User,
                OnVacation = false
            }
        );
        result.Add(
            new Employee("Robert King")
            {
                BirthDate = new DateTime(1960, 5, 29),
                HireDate = new DateTime(1994, 1, 2),
                Position = "Sales Representative",
                Address = "RG1 9SP, Edgeham Hollow Winchester Way, London, UK",
                Phone = "(71) 555-5598",
                Access = AccessLevel.User,
                OnVacation = false
            }
        );
        result.Add(
            new Employee("Laura Callahan")
            {
                BirthDate = new DateTime(1985, 1, 9),
                HireDate = new DateTime(2004, 3, 5),
                Position = "Inside Sales Coordinator",
                Address = "98105, 4726 - 11th Ave. N.E., Seattle WA, USA",
                Phone = "(206) 555-1189",
                Access = AccessLevel.User,
                OnVacation = true
            }
        );
        result.Add(
            new Employee("Anne Dodsworth")
            {
                BirthDate = new DateTime(1980, 1, 27),
                HireDate = new DateTime(2004, 11, 15),
                Position = "Sales Representative",
                Address = "WG2 7LT, 7 Houndstooth Rd., London, UK",
                Phone = "(71) 555-4444",
                Access = AccessLevel.User,
                OnVacation = false
            }
        );
        Employees = result;
    }

    public ObservableCollection<Employee> Employees { get; private set; }

    public EmployeeData()
    {
        GenerateEmployees();
    }
}

using DevExpressApp.Data;
using ImageSource = Microsoft.Maui.Controls.ImageSource;

namespace DevExpressApp.Models;
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
                resourceName = "Assets/Images/Photos/" + value.ToLower().Replace(" ", "_") + ".jpg";
#if ANDROID
                resourceName =  (string)new UnoImageConverter().Convert(resourceName, typeof(string), null, null);
#endif
                if (!String.IsNullOrEmpty(resourceName))
                    Photo = ImageSource.FromFile(resourceName);
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

using System.ComponentModel.DataAnnotations;

namespace GrapeCityApp.MauiControls;

public class City
{
    [Display(AutoGenerateField = false)]
    public bool Selected { get; set; }
    public string Name { get; set; }
    public string Country { get; set; }
}


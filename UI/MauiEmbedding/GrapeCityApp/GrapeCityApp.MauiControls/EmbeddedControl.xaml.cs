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

		// TODO: Uncomment code for loading Customers
        //var data = Customer.GetCustomerList(20);
        //grid.ItemsSource = data;
    }
}


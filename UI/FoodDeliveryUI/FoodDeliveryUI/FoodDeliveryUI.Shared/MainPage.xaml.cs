using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace FoodDeliveryUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public readonly ObservableCollection<Food> food;
        public readonly ObservableCollection<Recommendation> recommendations;

        public MainPage()
        {
            this.InitializeComponent();

            GenerateInfo();
        }

        public void GenerateInfo()
        {
            // Food list
            food.Add(new Food() { Picture = "food1", Name = "Sushi", Description = "25+ Restaurants" });
            food.Add(new Food() { Picture = "food2", Name = "Sushi", Description = "25+ Restaurants" });
            // Recomendatiosn list
            recommendations.Add(new Recommendation { Picture = "pizza", Restaurant = "PIZZA HUT", Title = "Get some pizza!", Description = "Pizza . 10%" });
            recommendations.Add(new Recommendation { Picture = "pizza", Restaurant = "PALA PIZZA", Title = "Best PIZZA Today!", Description = "Pizza . 20%" });
            recommendations.Add(new Recommendation { Picture = "pizza", Restaurant = "DOMINOS PIZZA", Title = "Get some pizza!!", Description = "Pizza . 15%" });
        }
    }
}

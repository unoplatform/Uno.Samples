using Java.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static Java.Util.Jar.Attributes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace FoodDeliveryUI
{
    public sealed partial class MainPage : Page
    {
        public readonly ObservableCollection<Food> food;
        public readonly ObservableCollection<Recommendation> recommendations;

        public MainPage()
        {
            this.InitializeComponent();
            food = new ObservableCollection<Food>();
            recommendations = new ObservableCollection<Recommendation>();
            GenerateInfo();
        }

        public void GenerateInfo()
        {
            // Food list
            food.Add(new Food() {  Picture ="food1", Name="Sushi", Description="25+ Restaurants" });
            food.Add(new Food() { Picture = "food2", Name = "Sushi", Description = "25+ Restaurants" });
            // Recomendatiosn list
            recommendations.Add(new Recommendation { Picture = "pizza", Restaurant = "PIZZA HUT", Title = "Get some pizza!", Description = "Pizza . 10%" });
            recommendations.Add(new Recommendation { Picture = "pizza", Restaurant = "PALA PIZZA", Title = "Best PIZZA Today!", Description = "Pizza . 20%" });
            recommendations.Add(new Recommendation { Picture = "pizza", Restaurant = "DOMINOS PIZZA", Title = "Get some pizza!!", Description = "Pizza . 15%" });
        }
    }
}

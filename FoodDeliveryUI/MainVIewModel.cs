using System;

namespace FoodDeliveryUI
{
    public class MainViewModel
    {
        List<Food> food;

        public List<Food> Food
        {
            get { return food; }
            set { this.food = value; }
        }

        public MainViewModel()
        {
            GenerateInfo();
        }

        void GenerateInfo()
        {
            food = new List<Food>();
            food.Add(new Food { }Picture = "", Name = "lala", Description = "lele");
        }
    }
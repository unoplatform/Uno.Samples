using System;

namespace FoodDeliveryUI
{
    public class Food
    {
        public string picture { get; set; }
        public string name { get; set; }
        public string description { get; set; }

        public string Picture
        {
            get { return picture; }
            set { picture = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }
    }
}
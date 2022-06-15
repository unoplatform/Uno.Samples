using System;
using System.Collections.Generic;

namespace ThreeLevelListDetailsSample.Data
{
    public class ItemsDataSource
    {
        private static List<WorkRequestItem> _items = new List<WorkRequestItem>()
        {
            new WorkRequestItem()
            {
                Id = 0,
                DateAdded = DateTime.Now.AddDays(new Random().Next(1, 31)),
                Title = "My shower makes an awful loud noise",
                Description = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer id facilisis lectus. Cras nec convallis ante, quis pulvinar tellus. Integer dictum accumsan pulvinar. Pellentesque eget enim sodales sapien vestibulum consequat.

Maecenas eu sapien ac urna aliquam dictum.

Nullam eget mattis metus. Donec pharetra, tellus in mattis tincidunt, magna ipsum gravida nibh, vitae lobortis ante odio vel quam.",
                AttachmentItems = new List<AttachmentItem>()
                {
                    new AttachmentItem()
                    {
                        Title = "ORDER PRODUCT: Modern rain shower faucet",
                        DateAdded = DateTime.Now.AddDays(new Random().Next(1, 31)),
                        Photo = new Microsoft.UI.Xaml.Media.Imaging.BitmapImage(new Uri(@"ms-appx:///Assets/showerhe.jpg"))
                    },
                    new AttachmentItem()
                    {
                        Title = "PHOTO: Damaged faucet",
                        DateAdded = DateTime.Now.AddDays(new Random().Next(1, 31)),
                        Photo = new Microsoft.UI.Xaml.Media.Imaging.BitmapImage(new Uri(@"ms-appx:///Assets/pipesinsp.jpg"))
                    },
                    new AttachmentItem()
                    {
                        Title = "PHOTO: Water heater inspection",
                        DateAdded = DateTime.Now.AddDays(new Random().Next(1, 31)),
                        Photo = new Microsoft.UI.Xaml.Media.Imaging.BitmapImage(new Uri(@"ms-appx:///Assets/waterheat.jpg"))
                    },
                }
            },
            new WorkRequestItem()
            {
                Id = 1,
                DateAdded = DateTime.Now.AddDays(new Random().Next(1, 31)),
                Title = "AC Stuck at 79 degrees F",
                Description = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer id facilisis lectus. Cras nec convallis ante, quis pulvinar tellus. Integer dictum accumsan pulvinar. Pellentesque eget enim sodales sapien vestibulum consequat.

Maecenas eu sapien ac urna aliquam dictum.

Nullam eget mattis metus. Donec pharetra, tellus in mattis tincidunt, magna ipsum gravida nibh, vitae lobortis ante odio vel quam.",
                AttachmentItems = new List<AttachmentItem>()
                {
                    new AttachmentItem()
                    {
                        Title = "ORDER PRODUCT: Smart Thermostat",
                        DateAdded = DateTime.Now.AddDays(new Random().Next(1, 31)),
                        Photo = new Microsoft.UI.Xaml.Media.Imaging.BitmapImage(new Uri(@"ms-appx:///Assets/thermo.jpg"))
                    },
                    new AttachmentItem()
                    {
                        Title = "PHOTO: Stuck thermostat Value",
                        DateAdded = DateTime.Now.AddDays(new Random().Next(1, 31))
                    }
                }
            },
            new WorkRequestItem()
            {
                Id = 2,
                DateAdded = DateTime.Now.AddDays(new Random().Next(1, 31)),
                Title = "Window bug screen fell out",
                Description = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer id facilisis lectus. Cras nec convallis ante, quis pulvinar tellus. Integer dictum accumsan pulvinar. Pellentesque eget enim sodales sapien vestibulum consequat.

Maecenas eu sapien ac urna aliquam dictum.

Nullam eget mattis metus. Donec pharetra, tellus in mattis tincidunt, magna ipsum gravida nibh, vitae lobortis ante odio vel quam.",
                AttachmentItems = new List<AttachmentItem>()
                {
                    new AttachmentItem()
                    {
                        Title = "ORDER PRODUCT: Standard window bug screen",
                        DateAdded = DateTime.Now.AddDays(new Random().Next(1, 31))
                    },
                    new AttachmentItem()
                    {
                        Title = "PHOTO: Bugs in apartment",
                        DateAdded = DateTime.Now.AddDays(new Random().Next(1, 31))
                    },
                    new AttachmentItem()
                    {
                        Title = "PHOTO: Pest control business card",
                        DateAdded = DateTime.Now.AddDays(new Random().Next(1, 31))
                    },
                }
            },
        };

        public static List<WorkRequestItem> GetAllItems()
        {
            return _items;
        }

        public static WorkRequestItem GetItemById(int id)
        {
            return _items[id];
        }
    }
}

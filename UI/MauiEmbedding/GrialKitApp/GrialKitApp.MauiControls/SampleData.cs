using System;
namespace GrialKitApp.MauiControls
{
	public static class SampleData
	{
		public static object[] Ring()
		{
			return new[]
            {
              new {
                Value = 521.02,
              },
              new {
                Value = 62.56,
              },
              new {
                Value = 245.52,
              },
              new {
                Value = 33.26,
              },
              new {
                Value = 33.26,
              },
              new {
                Value = 78.95,
              }
           };
        }

        public static object[] Line()
        {
            return new[]
            {
              new {
                Value = 100,
                Label = "12h",
              },
              new {
                Value = 10,
                Label = "11h",
              },
              new {
                Value = 90,
                Label = "10h",
              },
              new {
                Value = 8,
                Label = "9h",
              },
              new {
                Value = 70,
                Label = "8h",
              },
              new {
                Value = 100,
                Label = "7h",
              },
              new {
                Value = 10,
                Label = "6h",
              },
              new {
                Value = 100,
                Label = "5h",
              },
              new {
                Value = 50,
                Label = "4h",
              },
              new {
                Value = 40,
                Label = "3h",
              },
              new {
                Value = 10,
                Label = "2h",
              },
              new {
                Value = 90,
                Label = "1h",
              }
            };
        }

        public static object[] Bar()
        {
            return new[]
            {
                new {
                    Label = "Cooling",
                    Value = 300
                },
                new {
                    Label = "Lighting",
                    Value = 250
                },
                new {
                    Label = "Heating",
                    Value = 190
                },
                new {
                    Label = "Washing",
                    Value = 150
                },
                new {
                    Label = "Cooking",
                    Value = 80
                }
            };
        }

        public static object MultiSeriesBar()
        {
            return new
            {
                FirstSeries = new[]
                {
                    new {
                        Label = "Jul 21",
                        Value = 25
                    },
                    new {
                        Label = "Jul 23",
                        Value = 45
                    },
                    new {
                        Label = "Jul 25",
                        Value = 120
                    },
                    new {
                        Label = "Jul 27",
                        Value = 105
                    },
                    new {
                        Label = "Jul 29",
                        Value = 225
                    }
                },
                SecondSeries = new []
                {
                    new {
                        Label = "Jul 21",
                        Value = 50
                    },
                    new {
                        Label = "Jul 23",
                        Value = 75
                    },
                    new {
                        Label = "Jul 25",
                        Value = 60
                    },
                    new {
                        Label = "Jul 27",
                        Value = 150
                    },
                    new {
                        Label = "Jul 29",
                        Value = 160
                    }
                }
            };
        }

        public static object MultiseriesWithLabels()
        {
            return new
            {
                Labels = new[]
                {
                    "12 AM",
                    "4 AM",
                    "8 AM",
                    "12 PM",
                    "4 PM",
                    "8 PM"
                },
                MaxValue = 15,
                FirstSeries = new []
                {
                    0,
                    2,
                    7,
                    2,
                    4,
                    3
                },
                SecondSeries = new[]
                {
                    0,
                    4,
                    3,
                    5,
                    1,
                    4
                }
            };
        }
    }
}


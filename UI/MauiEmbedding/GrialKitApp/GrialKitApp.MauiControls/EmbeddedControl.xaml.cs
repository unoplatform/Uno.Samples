namespace GrialKitApp.MauiControls;

public partial class EmbeddedControl : ContentView
{
    public EmbeddedControl()
    {
        InitializeComponent();

        chart.BindingContext = new[]
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
}

#if MVU
using Model = SimpleCalculator.Presentation.BindableMainModel;
#else
using Model = SimpleCalculator.Presentation.MainViewModel;
#endif
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SimpleCalculator;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();
        DataContext = new Model(App.ThemeService!);
    }
}

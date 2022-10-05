using AdvancedXBind.ViewModel;
using Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking.Logging;
using Microsoft.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AdvancedXBind
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        public PlanetViewModel ViewModel { get; } = new PlanetViewModel("Jupiter");

        // Wrap an async method
        public bool DisplayPlanetSync()
        {
            return AsyncHelpers.RunSync(ViewModel.DisplayPlanetAsync);
        }

        public string GetResultOfAsyncMethod()
        {
            return AsyncHelpers.RunSync(ViewModel.SimulateLongRunningMethodAsync);
        }
    }

}

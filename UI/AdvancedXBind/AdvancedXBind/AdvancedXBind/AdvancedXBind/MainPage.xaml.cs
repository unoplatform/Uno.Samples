using AdvancedXBind.ViewModel;
using Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking.Logging;

namespace AdvancedXBind;

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

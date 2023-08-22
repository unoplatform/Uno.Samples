namespace SyncfusionApp.MauiControls;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

        SyncfusionApp.MauiControls.Samples.Base.BaseConfig.IsIndividualSB = true;
    }
}

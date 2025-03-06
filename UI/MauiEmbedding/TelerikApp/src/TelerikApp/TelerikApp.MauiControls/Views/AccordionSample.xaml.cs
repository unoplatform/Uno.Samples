namespace TelerikApp.MauiControls;

public partial class AccordionSample : ContentView
{
#if HAS_TELERIK
	static void Preserve()
    {
        _ = new Telerik.Maui.Controls.AccordionItem();
    }
#endif

    public AccordionSample()
    {
        InitializeComponent();
    }
}

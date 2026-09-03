namespace FitBeginnerApp.Presentation;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();

        // Hot Design renders this page without running Navigation, so seed a design-time DataContext
        // for the preview. Set it on the *page* (this.DataContext), never on a child element: at
        // runtime Navigation injects the MainModel as the page's DataContext, and a child carrying its
        // own explicit DataContext would shadow it, leaving every binding stuck on the inert seed.
        this.DataContext = new MainModel();
        // (A plain [ReactiveBindable(false)] Model IS the design-time data here — it projects fixed
        //  values, so no separate mock is needed and the auto-Default preview renders populated.)
    }
}

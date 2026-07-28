namespace MovieStreamApp.Presentation;

public sealed partial class BrowsePage : Page
{
    public BrowsePage()
    {
        this.InitializeComponent();

        // Hot Design / the Previews gallery renders the page WITHOUT running Navigation, so no model
        // is injected and the preview shows empty lists + blank images. Seed a representative sample
        // DataContext (the MVUX bindable proxy over a real model) so the preview populates. Set it on
        // the PAGE, unconditionally — Navigation overrides this.DataContext at runtime, and it must not
        // be gated on DesignMode.DesignModeEnabled (which is false in Hot Design).
        this.DataContext = new BrowseViewModel(new WatchlistService());
    }
}

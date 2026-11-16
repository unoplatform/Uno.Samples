namespace EmbeddedResourcesSample;

public sealed partial class MainPage : Page
{
    private string currentFile;

    public MainPage()
    {
        this.InitializeComponent();
    }

    public string[] EmbeddedResources { get; } = typeof(MainPage).Assembly.GetManifestResourceNames();

    public string CurrentFile
    {
        get => currentFile;
        set
        {
            currentFile = value;

            using (var s = typeof(MainPage).Assembly.GetManifestResourceStream(value))
            {
                var r = new StreamReader(s);
                content.Text = r.ReadToEnd();
            }
        }
    }
}

using System.Reflection;

namespace TelerikApp.MauiControls;

public partial class PdfViewSample : ContentView
{
	public PdfViewSample()
	{
		InitializeComponent();
	}

    // TODO: Uncomment
    //protected override void OnBindingContextChanged()
    //{
    //    if (BindingContext is null)
    //        return;

    //    Func<CancellationToken, Task<Stream>> streamFunc = ct => Task.Run(() =>
    //    {
    //        var assembly = BindingContext.GetType().Assembly;
    //        var fileName = assembly.GetManifestResourceNames().FirstOrDefault(n => n.EndsWith("pdfviewer-firstlook.pdf"));
    //        if (string.IsNullOrEmpty(fileName))
    //        {
    //            return Stream.Null;
    //        }
    //        return assembly.GetManifestResourceStream(fileName) ?? Stream.Null;
    //    });
    //    this.pdfViewer.Source = streamFunc;
    //}
}
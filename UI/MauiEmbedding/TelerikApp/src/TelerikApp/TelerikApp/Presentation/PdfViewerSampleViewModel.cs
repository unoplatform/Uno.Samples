#if HAS_TELERIK
using Telerik.Maui.Controls.PdfViewer;
#endif

namespace TelerikApp.Presentation;

internal partial class PdfViewerSampleViewModel : ObservableObject
{
#if HAS_TELERIK
	public PdfViewerSampleViewModel()
    {
       var assembly = typeof(PdfViewerSampleViewModel).Assembly;
       var resourceName = assembly.GetManifestResourceNames().FirstOrDefault(x => x.EndsWith("pdfviewer-firstlook.pdf"));
       if(!string.IsNullOrEmpty(resourceName))
       {
           using var stream = assembly.GetManifestResourceStream(resourceName);
           var data = stream.ReadBytes();
           Source = new ByteArrayDocumentSource(data);
       }
    }

    [ObservableProperty]
    private DocumentSource? source;
#endif
}

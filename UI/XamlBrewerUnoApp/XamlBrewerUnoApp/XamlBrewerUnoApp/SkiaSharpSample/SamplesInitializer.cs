
namespace Xamarin.Essentials
{
    // dummy placeholder
}

namespace SkiaSharpSample
{
    public static class SamplesInitializer
	{
		public static void Init()
		{
			var fontName = "content-font.ttf";

			var pkg = Package.Current.InstalledLocation.Path;
			var path = Path.Combine(pkg, "Assets", "Media", fontName);
			var localStorage = ApplicationData.Current.LocalFolder.Path;

			SamplesManager.ContentFontPath = path;
			SamplesManager.TempDataPath = Path.Combine(localStorage, "SkiaSharpSample", "TemporaryData");
			if (!Directory.Exists(SamplesManager.TempDataPath))
			{
				Directory.CreateDirectory(SamplesManager.TempDataPath);
			}
		}
    }
}

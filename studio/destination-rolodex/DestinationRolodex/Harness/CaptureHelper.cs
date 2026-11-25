#if DEBUG && !__WASM__
using Microsoft.UI.Xaml.Media.Imaging;
using SkiaSharp;

namespace DestinationRolodex.Harness;

/// <summary>Captures a XAML element through RenderTargetBitmap to a PNG, for harness runs without a window capture tool.</summary>
internal static class CaptureHelper
{
    public static async Task<string> CaptureAsync(UIElement element, string path)
    {
        var rtb = new RenderTargetBitmap();
        await rtb.RenderAsync(element);
        var buffer = await rtb.GetPixelsAsync();
        var bytes = System.Runtime.InteropServices.WindowsRuntime.WindowsRuntimeBufferExtensions.ToArray(buffer);
        var info = new SKImageInfo(rtb.PixelWidth, rtb.PixelHeight, SKColorType.Bgra8888, SKAlphaType.Premul);
        using var bitmap = new SKBitmap(info);
        System.Runtime.InteropServices.Marshal.Copy(bytes, 0, bitmap.GetPixels(), Math.Min(bytes.Length, info.BytesSize));
        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path))!);
        await using var file = File.Create(path);
        data.SaveTo(file);
        return $"{rtb.PixelWidth}x{rtb.PixelHeight}";
    }
}
#endif

// TODO: Uncomment
using Telerik.Maui.Controls.Compatibility;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace TelerikApp.MauiControls;

public static class AppBuilderExtensions
{
    public static MauiAppBuilder UseMauiControls(this MauiAppBuilder maui)
    {
        maui
            // TODO: Uncomment
            .UseTelerik()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("TelerikApp/Assets/Fonts/OpenSansRegular.ttf", "OpenSansRegular");
                fonts.AddFont("TelerikApp/Assets/Fonts/telerikfont.ttf", "TelerikFont");
            });

#if __IOS__
        // TODO: When https://github.com/dotnet/maui/pull/11569 is merged and released
        // with the upcoming versions, remove the next line and the respective method.
        // Currently, setting TextType of a Label to Html results to the font properties being ignored.
        Label.ControlsLabelMapper.AppendToMapping(nameof(ILabel.Font), UpdateFontProperties);
        Label.ControlsLabelMapper.AppendToMapping(nameof(ILabel.Text), UpdateFontProperties);
#endif

#if __ANDROID__ || __IOS__
        // TODO: When https://github.com/dotnet/maui/issues/5835 is really fixed, remove the following lines and the respective methods.
        // Currently, setting MaxLines of a Label to more than one for Android or iOS and LineBreakMode to TailTruncation
        // results to a single-line Label with truncation. The MaxLines is ignored.
        Label.ControlsLabelMapper.AppendToMapping(nameof(Label.LineBreakMode), UpdateMaxLines);
        Label.ControlsLabelMapper.AppendToMapping(nameof(Label.MaxLines), UpdateMaxLines);
#endif

        return maui;
    }

#if __ANDROID__
    private static void UpdateMaxLines(Microsoft.Maui.Handlers.LabelHandler handler, ILabel label)
    {
        var textView = handler.PlatformView;
        if (label is Label controlsLabel && textView.Ellipsize == Android.Text.TextUtils.TruncateAt.End)
        {
            textView.SetMaxLines(controlsLabel.MaxLines);
        }
    }
#elif __IOS__
    private static void UpdateMaxLines(Microsoft.Maui.Handlers.LabelHandler handler, ILabel label)
    {
        var labelView = handler.PlatformView;
        if (label is Label controlsLabel && labelView.LineBreakMode == UIKit.UILineBreakMode.TailTruncation)
        {
            labelView.Lines = controlsLabel.MaxLines;
        }
    }

    private static void UpdateFontProperties(Microsoft.Maui.Handlers.LabelHandler handler, ILabel label)
    {
        if (label is Label controlsLabel && controlsLabel.TextType == TextType.Html)
        {
            var hasSpans = controlsLabel.FormattedText != null && controlsLabel.FormattedText.Spans?.Count > 0;
            if (hasSpans)
            {
                return;
            }

            if (!controlsLabel.IsSet(Label.FontAttributesProperty) || controlsLabel.IsSet(Label.FontFamilyProperty) || controlsLabel.IsSet(Label.FontSizeProperty))
            {
                Microsoft.Maui.Handlers.LabelHandler.MapFont(handler, label);
            }
        }
    }
#endif
}

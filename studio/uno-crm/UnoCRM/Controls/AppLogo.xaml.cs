namespace UnoCRM.Controls;

/// <summary>
/// The CRM brand mark (bar-chart glyph on a dark tile), drawn with native XAML shapes so it
/// renders reliably everywhere it's shown in-app (sidebar logo, extended splash) without
/// depending on runtime SVG/gradient rendering.
/// </summary>
public sealed partial class AppLogo : UserControl
{
    public AppLogo()
    {
        this.InitializeComponent();
    }
}

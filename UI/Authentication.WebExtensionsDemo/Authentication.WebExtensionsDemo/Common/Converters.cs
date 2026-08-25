using Authentication.WebExtensionsDemo.AuthFlow;
using Microsoft.UI.Xaml.Data;

namespace Authentication.WebExtensionsDemo.Common;

/// <summary>
/// Maps a <see cref="bool"/> onto <see cref="Visibility"/>, optionally inverted.
/// </summary>
public sealed partial class BoolToVisibilityConverter : IValueConverter
{
    /// <summary>When true, <c>false</c> becomes <see cref="Visibility.Visible"/>.</summary>
    public bool Invert { get; set; }

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var flag = value is bool b && b;

        if (Invert)
        {
            flag = !flag;
        }

        return flag ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) =>
        throw new NotSupportedException();
}

/// <summary>
/// Colours a flow log entry by its <see cref="FlowStepKind"/>.
/// </summary>
public sealed partial class FlowStepKindToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var key = value switch
        {
            FlowStepKind.Call => "FlowCallBrush",
            FlowStepKind.Success => "FlowSuccessBrush",
            FlowStepKind.Warning => "FlowWarningBrush",
            FlowStepKind.Error => "FlowErrorBrush",
            _ => "FlowInfoBrush"
        };

        return Application.Current.Resources[key];
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) =>
        throw new NotSupportedException();
}

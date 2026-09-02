using System.Collections;
using Microsoft.UI.Xaml.Data;

namespace UnoCRM.Converters;

/// <summary>
/// <see cref="Visibility.Visible"/> when the bound collection is null, not enumerable, or has no
/// items; <see cref="Visibility.Collapsed"/> once it holds something.
/// </summary>
// Used to show a loading shape inside a FeedView's VALUE template. That reads like a contradiction —
// the value template means data arrived — but for a list feed it is the one honest reading: an empty
// list reports "none" and selects the empty-state template instead, so a value state whose collection
// is empty cannot be a genuinely empty result. It is a collection that has not filled yet. MVUX hands
// the value template an observable collection view whose contents are pushed in separately, one
// dispatcher hop later, and a host that measures before that hop sees the view empty.
//
// Returning a Visibility rather than a Brush is why this converter is safe where a colour converter
// would not be: it never touches the resource dictionary, so it cannot resolve against the wrong
// theme or come back empty in a design-time host.
public sealed partial class EmptyToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        // Bind this to the collection's COUNT, not to the collection: MVUX hands the value template
        // the same collection-view instance every time and fills it in place, so a binding on the
        // collection itself never re-evaluates and the loading shape would sit behind the real rows
        // for good. A count changes, and raises notification when it does.
        if (value is int count)
        {
            return count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        if (value is not IEnumerable items)
        {
            return Visibility.Visible;
        }

        foreach (var _ in items)
        {
            return Visibility.Collapsed;
        }

        return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}

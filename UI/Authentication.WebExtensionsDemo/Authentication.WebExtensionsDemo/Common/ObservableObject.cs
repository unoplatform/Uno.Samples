using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Authentication.WebExtensionsDemo.Common;

/// <summary>
/// Minimal <see cref="INotifyPropertyChanged"/> base for the demo's view state.
/// </summary>
public abstract class ObservableObject : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void Set<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return;
        }

        field = value;
        Raise(propertyName);
    }

    protected void Raise([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

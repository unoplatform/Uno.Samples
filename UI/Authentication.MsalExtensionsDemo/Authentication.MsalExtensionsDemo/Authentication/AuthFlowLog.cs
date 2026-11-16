using System.Collections.ObjectModel;
using System.ComponentModel;
using Authentication.MsalExtensionsDemo.Common;
using Microsoft.UI.Dispatching;

namespace Authentication.MsalExtensionsDemo.Authentication;

/// <summary>
/// Severity/role of a step in the authentication flow, used to colour the log.
/// </summary>
public enum FlowStepKind
{
    /// <summary>Contextual information.</summary>
    Info,

    /// <summary>An Uno.Extensions authentication API call is about to be made.</summary>
    Call,

    /// <summary>A call succeeded.</summary>
    Success,

    /// <summary>An expected, recoverable outcome (for example, interaction is required).</summary>
    Warning,

    /// <summary>A call failed.</summary>
    Error
}

/// <summary>
/// A single entry in the authentication flow log.
/// </summary>
/// <remarks>
/// The entry keeps what was actually logged and exposes it through
/// <see cref="DisplayTitle"/> / <see cref="DisplayDetail"/>, which run it past the
/// <see cref="SecretRedactor"/> on the way to the screen. That is what lets recording mode be
/// switched on mid-demo and still cover the steps already on screen.
/// </remarks>
public sealed class FlowStep : INotifyPropertyChanged
{
    private readonly SecretRedactor _redactor;

    public FlowStep(FlowStepKind kind, string title, string? detail, SecretRedactor redactor)
    {
        Kind = kind;
        Title = title;
        Detail = detail;
        _redactor = redactor;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public FlowStepKind Kind { get; }

    public string Title { get; }

    public string? Detail { get; }

    public string DisplayTitle => _redactor.Apply(Title) ?? Title;

    public string? DisplayDetail => _redactor.Apply(Detail);

    public bool HasDetail => !string.IsNullOrWhiteSpace(Detail);

    /// <summary>Re-reads the display properties, after redaction was turned on or off.</summary>
    internal void RefreshDisplay()
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DisplayTitle)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DisplayDetail)));
    }

    public DateTimeOffset Timestamp { get; } = DateTimeOffset.Now;

    public string Time => Timestamp.ToString("HH:mm:ss.fff");

    public string Glyph => Kind switch
    {
        FlowStepKind.Call => "→",     // →
        FlowStepKind.Success => "✓",  // ✓
        FlowStepKind.Warning => "⚠",  // ⚠
        FlowStepKind.Error => "✕",    // ✕
        _ => "·"                      // ·
    };
}

/// <summary>
/// Ordered, observable narration of what the authentication stack is doing. Bound directly to a
/// <see cref="Microsoft.UI.Xaml.Controls.ItemsControl"/> so the sign-in flow is visible
/// step by step on every platform.
/// </summary>
public sealed class AuthFlowLog : ObservableCollection<FlowStep>
{
    private readonly SecretRedactor _redactor;

    private DispatcherQueue? _dispatcher;

    public AuthFlowLog(SecretRedactor redactor)
    {
        _redactor = redactor;
        _redactor.Changed += (_, _) =>
        {
            foreach (var step in this)
            {
                step.RefreshDisplay();
            }
        };
    }

    /// <summary>
    /// Lets the log marshal additions onto the UI thread. MSAL continuations normally resume on
    /// the calling (UI) context, but broker and browser callbacks can arrive on a background
    /// thread, and mutating a bound collection from there would throw.
    /// </summary>
    public void AttachDispatcher(DispatcherQueue dispatcher) => _dispatcher = dispatcher;

    public void Info(string title, string? detail = null) => Append(FlowStepKind.Info, title, detail);

    public void Call(string title, string? detail = null) => Append(FlowStepKind.Call, title, detail);

    public void Success(string title, string? detail = null) => Append(FlowStepKind.Success, title, detail);

    public void Warning(string title, string? detail = null) => Append(FlowStepKind.Warning, title, detail);

    public void Error(string title, string? detail = null) => Append(FlowStepKind.Error, title, detail);

    private void Append(FlowStepKind kind, string title, string? detail)
    {
        var step = new FlowStep(kind, title, detail, _redactor);

        if (_dispatcher is { } dispatcher && !dispatcher.HasThreadAccess)
        {
            dispatcher.TryEnqueue(() => Add(step));
        }
        else
        {
            Add(step);
        }
    }
}

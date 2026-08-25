using System.Collections.ObjectModel;
using Microsoft.UI.Dispatching;

namespace Authentication.OidcExtensionsDemo.AuthFlow;

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
public sealed class FlowStep
{
    public FlowStep(FlowStepKind kind, string title, string? detail)
    {
        Kind = kind;
        Title = title;
        Detail = detail;
    }

    public FlowStepKind Kind { get; }

    public string Title { get; }

    public string? Detail { get; }

    public bool HasDetail => !string.IsNullOrWhiteSpace(Detail);

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
    private DispatcherQueue? _dispatcher;

    /// <summary>
    /// Lets the log marshal additions onto the UI thread. Authentication continuations normally resume on
    /// the calling (UI) context, but browser callbacks can arrive on a background
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
        var step = new FlowStep(kind, title, detail);

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

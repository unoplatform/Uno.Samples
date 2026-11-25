using System.Runtime.CompilerServices;

namespace QuoteCraft.Services;

/// <summary>
/// What the Quotes list and the open quote share. They are two models in two regions (DashboardModel on the page,
/// QuoteDetailModel in its detail region), so neither reaches into the other: the list opens a quote and highlights
/// it, the detail closes itself, and anything that changes quotes (the detail's actions, the editor's Save, the
/// status buttons) says so here, so both read the quotes again.
/// </summary>
public sealed class QuoteBoard
{
    private readonly object _gate = new();
    private string? _openQuoteId;
    private int _version;

    public event Action? Changed;

    /// <summary>The open quote and the change count, read together.</summary>
    public QuoteBoardState Current
    {
        get
        {
            lock (_gate)
            {
                return new QuoteBoardState(_openQuoteId, _version);
            }
        }
    }

    public void Open(string quoteId) => Set(() => _openQuoteId = quoteId);

    public void Close() => Set(() => _openQuoteId = null);

    /// <summary>Quotes were saved, deleted or changed status: lists and details read them again.</summary>
    public void QuotesChanged() => Set(() => _version++);

    /// <summary>The current state, then every change, for a model's <c>Feed.AsyncEnumerable</c>.</summary>
    public async IAsyncEnumerable<QuoteBoardState> WatchAsync([EnumeratorCancellation] CancellationToken ct)
    {
        var changed = new ChangeSignal();
        Action handler = changed.Raise;
        Changed += handler;
        try
        {
            while (true)
            {
                yield return Current;
                await changed.WaitAsync(ct);
            }
        }
        finally
        {
            Changed -= handler;
        }
    }

    private void Set(Action change)
    {
        lock (_gate)
        {
            change();
        }
        Changed?.Invoke();
    }
}

/// <summary>Which quote is open (null for none), and how many times quotes changed.</summary>
public record QuoteBoardState(string? OpenQuoteId, int Version);

using System.Threading.Channels;

namespace QuoteCraft.Helpers;

/// <summary>
/// "Something changed" from a service event, for a model's <c>Feed.AsyncEnumerable</c> to wait on. Signals that
/// arrive while nobody waits collapse into one, so a burst of events reads the service once.
/// </summary>
public sealed class ChangeSignal
{
    private readonly Channel<bool> _channel = Channel.CreateBounded<bool>(
        new BoundedChannelOptions(1) { FullMode = BoundedChannelFullMode.DropWrite });

    public void Raise() => _channel.Writer.TryWrite(true);

    public async ValueTask WaitAsync(CancellationToken ct) => await _channel.Reader.ReadAsync(ct);
}

using DestinationRolodex.Models;
using DestinationRolodex.Rendering;
using SkiaSharp;

namespace DestinationRolodex.Services;

/// <summary>
/// Async decode with a bounded byte budget, in-flight de-duplication, pinning and deferred disposal.
/// Decoding happens on the thread pool, never in a render pass. The cache has no say in which
/// destination is selected; the card asks for photos and decides what is displayed.
/// </summary>
public sealed class DestinationMediaCache : IDisposable
{
    private readonly object _gate = new();
    private readonly Dictionary<string, Entry> _entries = new();
    private readonly List<Retired> _retired = new();
    private readonly Func<Uri, CancellationToken, Task<Stream>> _open;
    private readonly int _maxSide;
    private long _bytes;
    private long _tick;
    private bool _disposed;

    private sealed class Entry
    {
        public required string Id;
        public required Task<DecodedPhoto> Task;
        public DecodedPhoto? Ready;
        public long LastUse;
    }

    private readonly record struct Retired(DecodedPhoto Photo, long FrameBarrier, DateTime RetiredAtUtc);

    public DestinationMediaCache(Func<Uri, CancellationToken, Task<Stream>>? opener = null, long byteBudget = 96L * 1024 * 1024, int maxLongestSide = PhotoRenderer.MaxTextureSide)
    {
        _open = opener ?? OpenPackagedAsync;
        ByteBudget = byteBudget;
        _maxSide = maxLongestSide;
    }

    public long ByteBudget { get; }
    public long Bytes { get { lock (_gate) { return _bytes; } } }
    public int Count { get { lock (_gate) { return _entries.Count; } } }
    public int RetiredCount { get { lock (_gate) { return _retired.Count; } } }

    /// <summary>Ids that eviction must never touch (displayed, transitioning, queued). Replaced wholesale by the card.</summary>
    public HashSet<string> Pinned { get; } = new();

    /// <summary>Test/harness hooks: artificial decode delay and ids that fail to decode.</summary>
    public TimeSpan ArtificialDelay { get; set; }
    public HashSet<string> FailingIds { get; } = new();

    /// <summary>Returns a ready photo without starting a load, or null.</summary>
    public DecodedPhoto? Peek(string id)
    {
        lock (_gate)
        {
            if (_entries.TryGetValue(id, out var e) && e.Ready is { IsDisposed: false })
            {
                e.LastUse = ++_tick;
                return e.Ready;
            }

            return null;
        }
    }

    public Task<DecodedPhoto> GetAsync(Destination destination, CancellationToken ct = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        lock (_gate)
        {
            if (_entries.TryGetValue(destination.Id, out var existing))
            {
                existing.LastUse = ++_tick;
                return existing.Task;
            }

            var entry = new Entry { Id = destination.Id, Task = null!, LastUse = ++_tick };
            entry.Task = DecodeAsync(entry, destination, ct);
            _entries[destination.Id] = entry;
            return entry.Task;
        }
    }

    /// <summary>Forgets a failed (or stale) entry so the next request decodes again. Used by Retry.</summary>
    public void Invalidate(string id)
    {
        lock (_gate)
        {
            if (_entries.Remove(id, out var e) && e.Ready is not null)
            {
                RetireLocked(e.Ready);
            }
        }
    }

    /// <summary>Removes every entry (data replacement). Photos are retired, not disposed, until the renderer has moved on.</summary>
    public void Clear()
    {
        lock (_gate)
        {
            foreach (var e in _entries.Values)
            {
                if (e.Ready is not null)
                {
                    RetireLocked(e.Ready);
                }
            }

            _entries.Clear();
        }
    }

    /// <summary>
    /// Disposes retired photos once the renderer has produced at least two frames since retirement and half a second passed,
    /// so a render pass that captured the old frame on the render thread can never touch a disposed image.
    /// </summary>
    public void Collect(long framesRendered)
    {
        List<DecodedPhoto>? toDispose = null;
        lock (_gate)
        {
            var now = DateTime.UtcNow;
            for (var i = _retired.Count - 1; i >= 0; i--)
            {
                var r = _retired[i];
                if (framesRendered >= r.FrameBarrier && now - r.RetiredAtUtc > TimeSpan.FromMilliseconds(500))
                {
                    (toDispose ??= new()).Add(r.Photo);
                    _retired.RemoveAt(i);
                }
            }
        }

        if (toDispose is not null)
        {
            foreach (var p in toDispose)
            {
                p.Dispose();
            }
        }
    }

    private long _lastKnownFrames;

    /// <summary>Lets Collect know the renderer's frame counter at retirement time.</summary>
    public void ObserveFrames(long framesRendered) => Interlocked.Exchange(ref _lastKnownFrames, framesRendered);

    private void RetireLocked(DecodedPhoto photo)
    {
        _bytes -= photo.Bytes;
        _retired.Add(new Retired(photo, Interlocked.Read(ref _lastKnownFrames) + 2, DateTime.UtcNow));
    }

    private void EvictLocked()
    {
        while (_bytes > ByteBudget)
        {
            Entry? victim = null;
            foreach (var e in _entries.Values)
            {
                if (e.Ready is null || Pinned.Contains(e.Id))
                {
                    continue;
                }

                if (victim is null || e.LastUse < victim.LastUse)
                {
                    victim = e;
                }
            }

            if (victim is null)
            {
                return;
            }

            _entries.Remove(victim.Id);
            RetireLocked(victim.Ready!);
        }
    }

    private async Task<DecodedPhoto> DecodeAsync(Entry entry, Destination destination, CancellationToken ct)
    {
        try
        {
            if (ArtificialDelay > TimeSpan.Zero)
            {
                await Task.Delay(ArtificialDelay, ct).ConfigureAwait(false);
            }

            if (FailingIds.Contains(destination.Id))
            {
                throw new IOException($"Photo unavailable for '{destination.Id}'.");
            }

            await using var stream = await _open(destination.ImageUri, ct).ConfigureAwait(false);
            var photo = await Task.Run(() => Decode(destination.Id, stream, _maxSide), ct).ConfigureAwait(false);

            lock (_gate)
            {
                if (_disposed || !ReferenceEquals(_entries.GetValueOrDefault(entry.Id), entry))
                {
                    // Invalidated or cleared while decoding: nobody owns this photo, dispose it later.
                    _retired.Add(new Retired(photo, Interlocked.Read(ref _lastKnownFrames) + 2, DateTime.UtcNow));
                    return photo;
                }

                entry.Ready = photo;
                _bytes += photo.Bytes;
                EvictLocked();
            }

            return photo;
        }
        catch
        {
            lock (_gate)
            {
                if (ReferenceEquals(_entries.GetValueOrDefault(entry.Id), entry))
                {
                    _entries.Remove(entry.Id);
                }
            }

            throw;
        }
    }

    /// <summary>Decodes, applies EXIF orientation via the codec, and limits the longest side (reference guard: 1600 px).</summary>
    public static DecodedPhoto Decode(string id, Stream stream, int maxLongestSide)
    {
        using var data = SKData.Create(stream);
        using var codec = SKCodec.Create(data) ?? throw new InvalidDataException($"'{id}' is not a decodable image.");
        var info = new SKImageInfo(codec.Info.Width, codec.Info.Height, SKColorType.Rgba8888, SKAlphaType.Premul);
        using var bitmap = SKBitmap.Decode(codec, info) ?? throw new InvalidDataException($"'{id}' failed to decode.");

        var longest = Math.Max(bitmap.Width, bitmap.Height);
        if (longest > maxLongestSide)
        {
            var scale = (float)maxLongestSide / longest;
            var target = new SKImageInfo(Math.Max(1, (int)Math.Round(bitmap.Width * scale)), Math.Max(1, (int)Math.Round(bitmap.Height * scale)), SKColorType.Rgba8888, SKAlphaType.Premul);
            using var resized = bitmap.Resize(target, new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear))
                ?? throw new InvalidDataException($"'{id}' could not be resized.");
            return new DecodedPhoto(id, SKImage.FromBitmap(resized));
        }

        return new DecodedPhoto(id, SKImage.FromBitmap(bitmap));
    }

    private static async Task<Stream> OpenPackagedAsync(Uri uri, CancellationToken ct)
    {
        var file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(uri).AsTask(ct).ConfigureAwait(false);
        return await file.OpenStreamForReadAsync().ConfigureAwait(false);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        List<DecodedPhoto> all;
        lock (_gate)
        {
            all = _entries.Values.Where(e => e.Ready is not null).Select(e => e.Ready!).ToList();
            all.AddRange(_retired.Select(r => r.Photo));
            _entries.Clear();
            _retired.Clear();
            _bytes = 0;
        }

        foreach (var p in all)
        {
            p.Dispose();
        }
    }
}

using DestinationRolodex.Models;

namespace DestinationRolodex.Rendering;

/// <summary>
/// Owns which photo is displayed, the active transition's clock and the latest-target queue.
/// Policy (decision D17): a running transition always finishes (bounded by its duration); if newer targets arrived
/// meanwhile only the newest plays next. Progress is elapsed active time, so a paused loop pauses the effect.
/// </summary>
public sealed class TransitionCoordinator
{
    public sealed class Transition
    {
        public required Destination FromDestination { get; init; }
        public required DecodedPhoto FromPhoto { get; init; }
        public required Destination ToDestination { get; init; }
        public required DecodedPhoto ToPhoto { get; init; }
        public required TransitionSettings Settings { get; init; }
        public required int Direction { get; init; }
        public bool IsPreview { get; init; }
        public bool IsPaused { get; set; }
        public double ElapsedMs { get; set; }
        public float Progress => (float)Math.Clamp(ElapsedMs / Math.Max(1, Settings.Duration.TotalMilliseconds), 0, 1);
    }

    private readonly record struct Pending(Destination Destination, DecodedPhoto Photo, int Direction, TransitionSettings Settings);
    private Pending? _queued;

    public Destination? DisplayedDestination { get; private set; }
    public DecodedPhoto? DisplayedPhoto { get; private set; }
    public Transition? Active { get; private set; }
    public bool HasQueuedTarget => _queued is not null;

    public event Action<Transition>? Started;
    public event Action<Transition>? Completed;

    /// <summary>Shows a photo immediately with no effect (first mount, reduced motion, same photo, fallback renderer).</summary>
    public void ShowImmediately(Destination destination, DecodedPhoto photo)
    {
        Active = null;
        _queued = null;
        DisplayedDestination = destination;
        DisplayedPhoto = photo;
    }

    /// <summary>Requests the configured effect from the displayed photo to <paramref name="photo"/>, or queues it behind a running one.</summary>
    public void Present(Destination destination, DecodedPhoto photo, int direction, TransitionSettings settings)
    {
        if (Active is { IsPreview: false } running)
        {
            // Bounded latest-target: the running effect finishes; only the newest request plays after it.
            _queued = ReferenceEquals(running.ToPhoto, photo) ? null : new Pending(destination, photo, direction, settings);
            return;
        }

        if (Active is { IsPreview: true })
        {
            CancelPreview();
        }

        if (DisplayedPhoto is null || DisplayedDestination is null || ReferenceEquals(DisplayedPhoto, photo))
        {
            ShowImmediately(destination, photo);
            return;
        }

        Start(DisplayedDestination, DisplayedPhoto, destination, photo, direction, settings, preview: false);
    }

    /// <summary>Debug replay: plays a pair from the start as a preview; the displayed identity is restored by the caller on exit.</summary>
    public void Replay(Destination from, DecodedPhoto fromPhoto, Destination to, DecodedPhoto toPhoto, int direction, TransitionSettings settings)
    {
        _queued = null;
        Start(from, fromPhoto, to, toPhoto, direction, settings, preview: true);
    }

    /// <summary>Debug scrub: holds a pair at a fixed progress.</summary>
    public void Seek(Destination from, DecodedPhoto fromPhoto, Destination to, DecodedPhoto toPhoto, int direction, TransitionSettings settings, double progress)
    {
        _queued = null;
        if (Active is not { IsPreview: true } active || !ReferenceEquals(active.FromPhoto, fromPhoto) || !ReferenceEquals(active.ToPhoto, toPhoto) || active.Settings.Kind != settings.Kind)
        {
            Start(from, fromPhoto, to, toPhoto, direction, settings, preview: true);
            active = Active!;
        }

        active.IsPaused = true;
        active.ElapsedMs = Math.Clamp(progress, 0, 1) * settings.Duration.TotalMilliseconds;
    }

    /// <summary>Drops a preview without touching the displayed identity.</summary>
    public void CancelPreview()
    {
        if (Active is { IsPreview: true })
        {
            Active = null;
        }
    }

    /// <summary>Cancels everything; the displayed identity jumps to the active transition's target so the photo never lags a cancelled effect.</summary>
    public void CancelAll()
    {
        if (Active is { IsPreview: false } active)
        {
            DisplayedDestination = active.ToDestination;
            DisplayedPhoto = active.ToPhoto;
        }

        Active = null;
        _queued = null;
    }

    public void Clear()
    {
        Active = null;
        _queued = null;
        DisplayedDestination = null;
        DisplayedPhoto = null;
    }

    /// <summary>Advances the active transition. Returns true while something is animating.</summary>
    public bool Tick(double dtMilliseconds)
    {
        var active = Active;
        if (active is null)
        {
            return false;
        }

        if (active.FromPhoto.IsDisposed || active.ToPhoto.IsDisposed)
        {
            // A photo was replaced underneath us (data replacement); abandon the effect cleanly.
            Active = null;
            return false;
        }

        if (!active.IsPaused)
        {
            active.ElapsedMs += dtMilliseconds;
        }

        if (active.Progress >= 1 && !active.IsPreview)
        {
            Active = null;
            DisplayedDestination = active.ToDestination;
            DisplayedPhoto = active.ToPhoto;
            Completed?.Invoke(active);
            if (_queued is { } next)
            {
                _queued = null;
                if (!ReferenceEquals(next.Photo, DisplayedPhoto))
                {
                    Start(DisplayedDestination, DisplayedPhoto, next.Destination, next.Photo, next.Direction, next.Settings, preview: false);
                    return true;
                }

                DisplayedDestination = next.Destination;
            }

            return false;
        }

        if (active.Progress >= 1 && active.IsPreview)
        {
            Active = null;
            Completed?.Invoke(active);
            return false;
        }

        return !active.IsPaused;
    }

    /// <summary>Photo inputs for the current frame.</summary>
    public (DecodedPhoto? From, DecodedPhoto? To, FocalPoint FocalA, FocalPoint FocalB, float Progress, PhotoTransitionKind Kind, float Strength, int Direction) FrameInputs()
    {
        if (Active is { } a)
        {
            return (a.FromPhoto, a.ToPhoto, a.FromDestination.FocalPoint, a.ToDestination.FocalPoint, a.Progress, a.Settings.Kind, (float)a.Settings.Strength, a.Direction);
        }

        var focal = DisplayedDestination?.FocalPoint ?? FocalPoint.Center;
        return (DisplayedPhoto, DisplayedPhoto, focal, focal, 0, PhotoTransitionKind.MorphBlend, .6f, 1);
    }

    private void Start(Destination from, DecodedPhoto fromPhoto, Destination to, DecodedPhoto toPhoto, int direction, TransitionSettings settings, bool preview)
    {
        var t = new Transition
        {
            FromDestination = from,
            FromPhoto = fromPhoto,
            ToDestination = to,
            ToPhoto = toPhoto,
            Settings = settings,
            Direction = direction < 0 ? -1 : 1,
            IsPreview = preview,
        };
        Active = t;
        Started?.Invoke(t);
    }
}

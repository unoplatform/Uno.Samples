using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Windows.Media.Core;
using Windows.Media.Playback;
// On iOS "MediaPlayer" is also an Apple framework namespace, so the simple name is ambiguous —
// alias the WinRT type this page uses (a distinct alias name to avoid clashing with the namespace).
using WinMediaPlayer = Windows.Media.Playback.MediaPlayer;

namespace MovieStreamApp.Presentation;

/// <summary>
/// Hosts a real <see cref="MediaPlayerElement"/>. Its built-in transport is disabled; the custom
/// Liquid-Glass controls in the XAML drive our own <see cref="MediaPlayer"/> directly, and the
/// scrubber + time labels + play/pause glyphs are kept in sync from the player's playback events.
/// Media events arrive off the UI thread, so every handler marshals back via <c>DispatcherQueue</c>.
/// </summary>
public sealed partial class PlaybackPage : Page
{
    private WinMediaPlayer? _player;
    // True only while WE are writing the slider from the player (so ValueChanged doesn't seek), and
    // while the user is dragging the thumb (so a position tick doesn't yank it back).
    private bool _suppressSliderUpdate;
    private bool _userScrubbing;

    public PlaybackPage()
    {
        this.InitializeComponent();

        // Seed a sample DataContext so the Hot Design Previews gallery (which renders without
        // Navigation) populates; the DataViewMap injects the playing movie at runtime, overriding this.
        this.DataContext = new PlaybackModel(MovieData.Featured);

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        // Give the page keyboard focus so the Esc accelerator fires without the user clicking first
        // (Programmatic focus draws no focus ring); Space/Enter on this button then toggles play/pause.
        PlayPauseButton.Focus(FocusState.Programmatic);

        if (_player is not null)
        {
            return;
        }

        var url = (DataContext as PlaybackModel)?.VideoUrl ?? MovieData.SampleVideo;

        // Own the MediaPlayer (rather than reading MediaPlayerElement.MediaPlayer, which is created
        // lazily) so we can wire events immediately. ORDER MATTERS: attach the player to the element
        // FIRST, then assign the Source, then Play — a Source set before SetMediaPlayer never reaches
        // the native player. Only MediaSource.CreateFromUri sources are supported when set from code.
        _player = new WinMediaPlayer { AutoPlay = true };
        _player.MediaOpened += OnMediaOpened;
        _player.MediaFailed += OnMediaFailed;
        _player.PlaybackSession.PositionChanged += OnPositionChanged;
        _player.PlaybackSession.PlaybackStateChanged += OnPlaybackStateChanged;
        Player.SetMediaPlayer(_player);
        _player.Source = MediaSource.CreateFromUri(new Uri(url));
        _player.Play();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        // Pause + tear down when leaving the page, so audio doesn't keep playing after Back and the
        // player's events don't hold this page alive.
        if (_player is null)
        {
            return;
        }

        _player.MediaOpened -= OnMediaOpened;
        _player.MediaFailed -= OnMediaFailed;
        _player.PlaybackSession.PositionChanged -= OnPositionChanged;
        _player.PlaybackSession.PlaybackStateChanged -= OnPlaybackStateChanged;
        _player.Pause();
        Player.SetMediaPlayer(null);

        // Defer Dispose one dispatcher turn instead of disposing inline. On the macOS desktop head the
        // native player keeps posting periodic position callbacks, and disposing mid-teardown pulls the
        // player out from under a callback that's already queued on the UI thread — it then dereferences
        // a null and throws deep in the media runtime. Pausing quiesces those callbacks and deferring the
        // dispose lets any queued one drain against a live player first, so teardown is race-free.
        var player = _player;
        _player = null;
        DispatcherQueue.TryEnqueue(() => player.Dispose());
    }

    // ── Player → UI (events arrive off-thread) ──────────────────────────────────────────────────

    private void OnMediaOpened(WinMediaPlayer sender, object args)
    {
        var duration = sender.PlaybackSession.NaturalDuration;
        DispatcherQueue.TryEnqueue(() => TotalTime.Text = Format(duration));
    }

    // Esc = leave the player (see Page.KeyboardAccelerators). Routes back through the same navigator the
    // on-screen Back button uses (uen:Navigation.Request="-"), so it works even when the native video
    // surface covers that button — a keyboard exit that never depends on a clickable overlay.
    private void OnEscapeInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
    {
        args.Handled = true;
        _ = this.Navigator()?.NavigateBackAsync(this);
    }

    private void OnMediaFailed(WinMediaPlayer sender, MediaPlayerFailedEventArgs args) =>
        System.Diagnostics.Debug.WriteLine($"[Playback] MediaFailed: {args.Error} / {args.ErrorMessage}");

    private void OnPositionChanged(MediaPlaybackSession session, object args)
    {
        var position = session.Position;
        var duration = session.NaturalDuration;
        DispatcherQueue.TryEnqueue(() =>
        {
            CurrentTime.Text = Format(position);
            if (_userScrubbing || duration <= TimeSpan.Zero)
            {
                return;
            }

            _suppressSliderUpdate = true;
            // Scrub position as a 0..1 ratio (the slider's range). Compute over TotalSeconds and clamp,
            // so a position that momentarily runs past the reported duration can't overshoot the track.
            ScrubSlider.Value = Math.Clamp(position.TotalSeconds / duration.TotalSeconds, 0, 1);
            _suppressSliderUpdate = false;
        });
    }

    private void OnPlaybackStateChanged(MediaPlaybackSession session, object args)
    {
        var playing = session.PlaybackState == MediaPlaybackState.Playing;
        DispatcherQueue.TryEnqueue(() =>
        {
            CenterPlayIcon.Visibility = playing ? Visibility.Collapsed : Visibility.Visible;
            CenterPauseIcon.Visibility = playing ? Visibility.Visible : Visibility.Collapsed;
            TransportPlayIcon.Visibility = playing ? Visibility.Collapsed : Visibility.Visible;
            TransportPauseIcon.Visibility = playing ? Visibility.Visible : Visibility.Collapsed;
        });
    }

    // ── UI → player ─────────────────────────────────────────────────────────────────────────────

    private void TogglePlayPause(object sender, RoutedEventArgs e)
    {
        if (_player is null)
        {
            return;
        }

        if (_player.PlaybackSession.PlaybackState == MediaPlaybackState.Playing)
        {
            _player.Pause();
        }
        else
        {
            _player.Play();
        }
    }

    private void SkipBack(object sender, RoutedEventArgs e) => SeekBy(TimeSpan.FromSeconds(-10));

    private void SkipForward(object sender, RoutedEventArgs e) => SeekBy(TimeSpan.FromSeconds(30));

    private void SeekBy(TimeSpan delta)
    {
        if (_player is null)
        {
            return;
        }

        var session = _player.PlaybackSession;
        var target = session.Position + delta;
        if (target < TimeSpan.Zero)
        {
            target = TimeSpan.Zero;
        }
        else if (session.NaturalDuration > TimeSpan.Zero && target > session.NaturalDuration)
        {
            target = session.NaturalDuration;
        }

        session.Position = target;
    }

    private void ScrubSlider_PointerPressed(object sender, PointerRoutedEventArgs e) => _userScrubbing = true;

    private void ScrubSlider_PointerReleased(object sender, PointerRoutedEventArgs e)
    {
        _userScrubbing = false;
        SeekToRatio(ScrubSlider.Value);
    }

    private void ScrubSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
    {
        // Only a user-driven change seeks; our own position-sync writes set _suppressSliderUpdate.
        if (_suppressSliderUpdate)
        {
            return;
        }

        SeekToRatio(e.NewValue);
    }

    private void SeekToRatio(double ratio)
    {
        if (_player is null)
        {
            return;
        }

        var duration = _player.PlaybackSession.NaturalDuration;
        if (duration > TimeSpan.Zero)
        {
            _player.PlaybackSession.Position = duration * ratio;
        }
    }

    private static string Format(TimeSpan time) =>
        time.TotalHours >= 1 ? time.ToString(@"h\:mm\:ss") : time.ToString(@"m\:ss");
}

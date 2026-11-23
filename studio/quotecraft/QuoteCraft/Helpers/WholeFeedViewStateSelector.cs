namespace QuoteCraft.Helpers;

/// <summary>
/// The FeedView visual states for every message, not only for the axes that message changed.
/// </summary>
/// <remarks>
/// MVUX's selector moves a visual state group only when its axis is in the message's changes. The navigator gives a
/// page a new view model each time its tab is opened (<c>MainModel.OpenTab</c> clears the tab's back stack), and the
/// FeedView then subscribes to the new model's feed. That feed's first message has no error, but "no error" is not a
/// change for the new subscription, so a FeedView that showed "could not be loaded" kept that template over the data
/// the new feed loaded, and Retry could not clear it. This selector sets the Data, Error and Progress groups on every
/// message, then adds the states of any other changed axis as MVUX would.
/// </remarks>
public sealed class WholeFeedViewStateSelector : FeedViewVisualStateSelector
{
    private static readonly MessageAxis[] AlwaysSet = [MessageAxis.Data, MessageAxis.Error, MessageAxis.Progress];

    protected override IEnumerable<(string stateName, bool shouldUseTransition)> GetVisualStates(FeedView feedView, IMessage message)
    {
        foreach (var axis in AlwaysSet)
        {
            if (GetVisualState(feedView, message, axis, message.Current[axis]) is { Length: > 0 } state)
            {
                yield return (state, true);
            }
        }

        foreach (var axis in (IEnumerable<MessageAxis>)message.Changes)
        {
            if (!AlwaysSet.Contains(axis)
                && GetVisualState(feedView, message, axis, message.Current[axis]) is { Length: > 0 } state)
            {
                yield return (state, true);
            }
        }
    }

    /// <summary>The visual state names this selector gives a message: the tests' seam onto the protected override.</summary>
    internal IEnumerable<string> StatesFor(FeedView feedView, IMessage message) =>
        GetVisualStates(feedView, message).Select(s => s.stateName);
}

namespace FitBeginnerApp.Presentation;

// The landing page. Everything here is read from IFitnessService — asynchronously, like a real
// endpoint — so the header stats are scalar feeds that bind straight, and the recent-results strip
// is a list feed the page renders through a FeedView (results / nothing yet / failed / loading).
public partial record HomeModel(IFitnessService Fitness)
{
    // One request for the whole header payload, cached so each derived scalar shares it instead of
    // re-fetching. Every scalar below is a projection of it and can never be None, so they bind
    // directly to Text and to the WeeklyRing's dependency properties.
    private IFeed<HomeSummary>? _summary;
    private IFeed<HomeSummary> Summary => _summary ??= Feed.Async(Fitness.GetHomeSummaryAsync);

    public IFeed<string> GreetingMessage => Summary.Select(s => s.Greeting);
    public IFeed<string> MotivationMessage => Summary.Select(s => s.Motivation);
    public IFeed<WorkoutEntry> TodayWorkout => Summary.Select(s => s.TodayWorkout);

    // Bound straight to the WeeklyRing control's Completed/Goal dependency properties; the ring
    // formats the "2/3" readout itself from them.
    public IFeed<int> WeeklyCompletedDays => Summary.Select(s => s.WeeklyCompletedDays);
    public IFeed<int> WeeklyGoalDays => Summary.Select(s => s.WeeklyGoalDays);
    public IFeed<int> TotalMinutesThisWeek => Summary.Select(s => s.TotalMinutesThisWeek);
    public IFeed<int> CaloriesBurnedThisWeek => Summary.Select(s => s.CaloriesBurnedThisWeek);

    // The recent-results strip: its own request, rendered by a FeedView. A brand-new user genuinely
    // has none, so the empty state is real — and it is the UI this page previously had no answer for.
    private IListFeed<WorkoutResult>? _recentResults;
    public IListFeed<WorkoutResult> RecentResults =>
        _recentResults ??= ListFeed.Async(ct => Fitness.GetRecentResultsAsync(3, ct));

    // Beginner tips. Bound directly rather than through a FeedView: this is a decorative content
    // strip whose empty and failed states have no UI worth designing.
    private IListFeed<QuickTip>? _tips;
    public IListFeed<QuickTip> Tips => _tips ??= ListFeed.Async(Fitness.GetTipsAsync);
}

public partial record QuickTip(string Title, string Body);

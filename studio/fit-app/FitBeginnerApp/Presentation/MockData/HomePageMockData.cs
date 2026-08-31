namespace FitBeginnerApp.Presentation.MockData;

// Design-time DataContext for HomePage in Hot Design / Studio. The recent-results strip is a
// FeedView, and a FeedView can only subscribe to a FEED — a plain list would never reach it and the
// preview would sit on the empty state. So this mock is [ReactiveBindable] and its statics return
// the GENERATED ViewModel, whose constructor creates the SourceContext that makes the feed pump.
//
// Expression-bodied (a fresh instance per access), never a cached singleton: a generated ViewModel
// has a view-scoped lifecycle, so a shared instance can be built before Hot Design's dispatcher is
// ready, or be dead from a previous render, leaving feeds that never emit. It is never seeded from
// the page constructor — see that constructor for why.
[Uno.Extensions.Reactive.ReactiveBindable]
public partial record HomePageMockData
{
    // Default design-time state: an active member mid-week.
    public static HomePageMockDataViewModel Data => new();

    // Day one: the ring at zero and no results yet, so the strip falls through to its NoneTemplate.
    public static HomePageMockDataViewModel FirstDay =>
        HomePageMockDataViewModel.ForModel(new()
        {
            MotivationMessage = "Your first week starts today.",
            WeeklyCompletedDays = 0,
            TotalMinutesThisWeek = 0,
            CaloriesBurnedThisWeek = 0,
            RecentResults = ListFeed.Async(_ =>
                ValueTask.FromResult<IImmutableList<WorkoutResult>>(ImmutableList<WorkoutResult>.Empty)),
        });

    // Goal met: the ring completes its sweep.
    public static HomePageMockDataViewModel GoalMet =>
        HomePageMockDataViewModel.ForModel(new()
        {
            MotivationMessage = "Weekly goal complete — outstanding!",
            WeeklyCompletedDays = 3,
            TotalMinutesThisWeek = 65,
            CaloriesBurnedThisWeek = 520,
        });

    public string GreetingMessage { get; init; } = FitData.Home.Greeting;
    public string MotivationMessage { get; init; } = FitData.Home.Motivation;
    public WorkoutEntry TodayWorkout { get; init; } = FitData.Home.TodayWorkout;

    public int WeeklyCompletedDays { get; init; } = FitData.Home.WeeklyCompletedDays;
    public int WeeklyGoalDays { get; init; } = FitData.Home.WeeklyGoalDays;
    public int TotalMinutesThisWeek { get; init; } = FitData.Home.TotalMinutesThisWeek;
    public int CaloriesBurnedThisWeek { get; init; } = FitData.Home.CaloriesBurnedThisWeek;

    public IListFeed<WorkoutResult> RecentResults { get; init; } =
        ListFeed.Async(_ => ValueTask.FromResult<IImmutableList<WorkoutResult>>(
            FitData.WorkoutHistory.Take(3).ToImmutableList()));

    public IListFeed<QuickTip> Tips { get; init; } =
        ListFeed.Async(_ => ValueTask.FromResult(FitData.Tips));
}

// The generator's model-taking ViewModel constructor is protected; this partial reaches it from
// inside the class so the variants above can wrap a customized model.
public partial class HomePageMockDataViewModel
{
    internal static HomePageMockDataViewModel ForModel(HomePageMockData model) => new(model);
}

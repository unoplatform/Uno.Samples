using Uno.Extensions.Reactive;

namespace FieldOpsPro.Presentation;

public partial record DashboardModel
{
    private readonly IFieldOpsService _fieldOpsService;

    public DashboardModel(IFieldOpsService fieldOpsService)
    {
        _fieldOpsService = fieldOpsService;
    }

    public IFeed<DashboardStats> Stats => Feed.Async(_fieldOpsService.GetDashboardStatsAsync);

    public IListFeed<Agent> MapAgents => ListFeed.Async(_fieldOpsService.GetAgentsAsync);

    public IListFeed<TaskItem> PriorityTasks => ListFeed.Async(_fieldOpsService.GetPriorityTasksAsync);

    public IListFeed<TeamMember> TeamMembers => ListFeed.Async(_fieldOpsService.GetTeamMembersAsync);

    public IListFeed<Activity> RecentActivity => ListFeed.Async(_fieldOpsService.GetRecentActivityAsync);

    public IFeed<UserProfile> CurrentUser => Feed.Async(_fieldOpsService.GetCurrentUserAsync);

    public IFeed<CurrentAssignment> CurrentAssignment => Feed.Async(_fieldOpsService.GetCurrentAssignmentAsync);

    public IFeed<TodayProgress> TodayProgress => Feed.Async(_fieldOpsService.GetTodayProgressAsync);

    public IListFeed<TaskItem> TodayTasks => ListFeed.Async(_fieldOpsService.GetTodayTasksAsync);

    public string CurrentDate => DateTime.Now.ToString("dddd, MMMM d, yyyy");
}

using FieldOpsPro.Models;

namespace FieldOpsPro.Services;

public interface IFieldOpsService
{
    ValueTask<DashboardStats> GetDashboardStatsAsync(CancellationToken ct = default);
    ValueTask<IImmutableList<Agent>> GetAgentsAsync(CancellationToken ct = default);
    ValueTask<IImmutableList<TaskItem>> GetPriorityTasksAsync(CancellationToken ct = default);
    ValueTask<IImmutableList<TeamMember>> GetTeamMembersAsync(CancellationToken ct = default);
    ValueTask<IImmutableList<Activity>> GetRecentActivityAsync(CancellationToken ct = default);
    ValueTask<UserProfile> GetCurrentUserAsync(CancellationToken ct = default);
    ValueTask<CurrentAssignment> GetCurrentAssignmentAsync(CancellationToken ct = default);
    ValueTask<TodayProgress> GetTodayProgressAsync(CancellationToken ct = default);
    ValueTask<IImmutableList<TaskItem>> GetTodayTasksAsync(CancellationToken ct = default);
}

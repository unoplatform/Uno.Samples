using FieldOpsPro.Models;
using FieldOpsPro.Models.Enums;
using TaskStatus = FieldOpsPro.Models.Enums.TaskStatus;

namespace FieldOpsPro.Services;

public class MockFieldOpsService : IFieldOpsService
{
    private readonly Random _random = new();

    // Montreal area coordinates for realistic map display
    private const double MontrealLat = 45.5017;
    private const double MontrealLng = -73.5673;

    public async ValueTask<DashboardStats> GetDashboardStatsAsync(CancellationToken ct = default)
    {
        await Task.Delay(100, ct);

        return new DashboardStats(
            ActiveWorkOrders: new StatValue(47, 42, 12),
            OnlineAgents: new StatValue(12, 15),
            AvgResponseTime: new StatValue(24, 21, -12.5),
            CompletionRate: new StatValue(94, 92, 2.2),
            LastUpdated: DateTime.UtcNow
        );
    }

    public async ValueTask<IImmutableList<Agent>> GetAgentsAsync(CancellationToken ct = default)
    {
        await Task.Delay(150, ct);

        var agents = new List<Agent>
        {
            new Agent(
                Id: "agent_001",
                Name: "Jake Donovan",
                Initials: "JD",
                AvatarColor: AvatarColor.Orange,
                Status: AgentStatus.OnRoute,
                Location: new Location(
                    MontrealLat + 0.02,
                    MontrealLng - 0.01,
                    "Downtown Montreal",
                    Distance: "2.3 km"
                ),
                CurrentTaskId: "task_4523",
                CurrentTaskTitle: "Emergency HVAC Repair",
                Eta: "12 min"
            ),
            new Agent(
                Id: "agent_002",
                Name: "Sarah Chen",
                Initials: "SC",
                AvatarColor: AvatarColor.Cyan,
                Status: AgentStatus.OnSite,
                Location: new Location(
                    MontrealLat - 0.01,
                    MontrealLng + 0.02,
                    "Old Montreal",
                    Distance: "1.8 km"
                ),
                CurrentTaskId: "task_4520",
                CurrentTaskTitle: "Electrical Inspection"
            ),
            new Agent(
                Id: "agent_003",
                Name: "Mike Rodriguez",
                Initials: "MR",
                AvatarColor: AvatarColor.Purple,
                Status: AgentStatus.Available,
                Location: new Location(
                    MontrealLat + 0.005,
                    MontrealLng - 0.02,
                    "HQ - Available",
                    Distance: "0.5 km"
                )
            ),
            new Agent(
                Id: "agent_004",
                Name: "Emily Watson",
                Initials: "EW",
                AvatarColor: AvatarColor.Pink,
                Status: AgentStatus.OnRoute,
                Location: new Location(
                    MontrealLat - 0.015,
                    MontrealLng - 0.008,
                    "Plateau Mont-Royal",
                    Distance: "3.1 km"
                ),
                CurrentTaskId: "task_4525",
                CurrentTaskTitle: "Plumbing Repair",
                Eta: "8 min"
            ),
            new Agent(
                Id: "agent_005",
                Name: "Alex Park",
                Initials: "AP",
                AvatarColor: AvatarColor.Green,
                Status: AgentStatus.OnSite,
                Location: new Location(
                    MontrealLat + 0.025,
                    MontrealLng + 0.01,
                    "Westmount",
                    Distance: "4.2 km"
                ),
                CurrentTaskId: "task_4518",
                CurrentTaskTitle: "Security System Install"
            ),
            new Agent(
                Id: "agent_006",
                Name: "Chris Taylor",
                Initials: "CT",
                AvatarColor: AvatarColor.Blue,
                Status: AgentStatus.Available,
                Location: new Location(
                    MontrealLat - 0.008,
                    MontrealLng - 0.015,
                    "Ville-Marie",
                    Distance: "1.2 km"
                )
            )
        };

        // Simulate slight position changes for agents on route
        return agents.Select(a =>
        {
            if (a.Status == AgentStatus.OnRoute)
            {
                var newLat = a.Location.Latitude + (_random.NextDouble() - 0.5) * 0.001;
                var newLng = a.Location.Longitude + (_random.NextDouble() - 0.5) * 0.001;
                return a with
                {
                    Location = a.Location with { Latitude = newLat, Longitude = newLng }
                };
            }
            return a;
        }).ToImmutableList();
    }

    public async ValueTask<IImmutableList<TaskItem>> GetPriorityTasksAsync(CancellationToken ct = default)
    {
        await Task.Delay(100, ct);

        return new List<TaskItem>
        {
            new TaskItem(
                Id: "task_4523",
                OrderNumber: "WO-4523",
                Title: "Emergency HVAC Repair",
                Description: "AC unit on floor 12 not functioning",
                Priority: TaskPriority.High,
                Status: TaskStatus.InProgress,
                Type: TaskType.Urgent,
                Location: new Location(
                    MontrealLat + 0.01,
                    MontrealLng - 0.005,
                    "123 Business Ave, Floor 12",
                    Name: "Meridian Tower",
                    Floor: "12",
                    Distance: "2.3 km"
                ),
                AssigneeId: "agent_001",
                AssigneeName: "Jake Donovan",
                AssigneeInitials: "JD",
                AssigneeAvatarColor: AvatarColor.Orange,
                EstimatedDuration: "2h",
                CreatedAt: DateTime.UtcNow.AddHours(-2)
            ),
            new TaskItem(
                Id: "task_4525",
                OrderNumber: "WO-4525",
                Title: "Plumbing Inspection",
                Description: "Annual plumbing system inspection",
                Priority: TaskPriority.Medium,
                Status: TaskStatus.Pending,
                Type: TaskType.Scheduled,
                Location: new Location(
                    MontrealLat - 0.02,
                    MontrealLng + 0.01,
                    "456 Commerce St, Suite 200",
                    Name: "Harbor View Complex",
                    Distance: "1.8 km"
                ),
                AssigneeId: "agent_004",
                AssigneeName: "Emily Watson",
                AssigneeInitials: "EW",
                AssigneeAvatarColor: AvatarColor.Pink,
                EstimatedDuration: "1.5h",
                ScheduledTime: DateTime.UtcNow.AddHours(2),
                CreatedAt: DateTime.UtcNow.AddDays(-1)
            ),
            new TaskItem(
                Id: "task_4527",
                OrderNumber: "WO-4527",
                Title: "Fire Alarm Testing",
                Description: "Quarterly fire alarm system test",
                Priority: TaskPriority.Medium,
                Status: TaskStatus.Pending,
                Type: TaskType.Scheduled,
                Location: new Location(
                    MontrealLat + 0.005,
                    MontrealLng + 0.02,
                    "789 Tower Rd",
                    Name: "Skyline Towers",
                    Distance: "3.5 km"
                ),
                EstimatedDuration: "45m",
                ScheduledTime: DateTime.UtcNow.AddHours(4),
                CreatedAt: DateTime.UtcNow.AddDays(-2)
            ),
            new TaskItem(
                Id: "task_4528",
                OrderNumber: "WO-4528",
                Title: "Light Fixture Replacement",
                Description: "Replace damaged lobby light fixtures",
                Priority: TaskPriority.Low,
                Status: TaskStatus.Pending,
                Type: TaskType.Routine,
                Location: new Location(
                    MontrealLat - 0.01,
                    MontrealLng - 0.01,
                    "321 Park Ave",
                    Name: "Parkview Center",
                    Distance: "2.1 km"
                ),
                EstimatedDuration: "1h",
                CreatedAt: DateTime.UtcNow.AddDays(-3)
            )
        }.ToImmutableList();
    }

    public async ValueTask<IImmutableList<TeamMember>> GetTeamMembersAsync(CancellationToken ct = default)
    {
        await Task.Delay(80, ct);

        return new List<TeamMember>
        {
            new TeamMember("agent_001", "Jake Donovan", "JD", AvatarColor.Orange, AgentStatus.OnRoute, "En route - Meridian Tower", "2.3 mi"),
            new TeamMember("agent_002", "Sarah Chen", "SC", AvatarColor.Cyan, AgentStatus.OnSite, "On site - Harbor View", "1.8 mi"),
            new TeamMember("agent_003", "Mike Rodriguez", "MR", AvatarColor.Purple, AgentStatus.Available, "Available - HQ", "0.5 mi"),
            new TeamMember("agent_004", "Emily Watson", "EW", AvatarColor.Pink, AgentStatus.OnRoute, "En route - Capitol Hill", "3.1 mi"),
            new TeamMember("agent_005", "Alex Park", "AP", AvatarColor.Green, AgentStatus.OnSite, "On site - Northgate", "4.2 mi"),
            new TeamMember("agent_006", "Chris Taylor", "CT", AvatarColor.Blue, AgentStatus.Available, "Available - SLU", "1.2 mi"),
            new TeamMember("agent_007", "Jordan Lee", "JL", AvatarColor.Orange, AgentStatus.Break, "Break - Returns 2:30 PM"),
            new TeamMember("agent_008", "Morgan Davis", "MD", AvatarColor.Purple, AgentStatus.Offline, "Offline")
        }.ToImmutableList();
    }

    public async ValueTask<IImmutableList<Activity>> GetRecentActivityAsync(CancellationToken ct = default)
    {
        await Task.Delay(100, ct);

        var now = DateTime.UtcNow;

        return new List<Activity>
        {
            new Activity(
                Id: "act_001",
                Type: ActivityType.TaskCompleted,
                ActorId: "agent_005",
                ActorName: "Alex Park",
                Message: "completed work order #4521",
                Timestamp: now.AddMinutes(-5)
            ),
            new Activity(
                Id: "act_002",
                Type: ActivityType.Arrival,
                ActorId: "agent_002",
                ActorName: "Sarah Chen",
                Message: "arrived at Meridian Tower",
                Timestamp: now.AddMinutes(-12)
            ),
            new Activity(
                Id: "act_003",
                Type: ActivityType.Assignment,
                ActorId: "agent_001",
                ActorName: "Jake Donovan",
                Message: "was assigned to WO-4523",
                Timestamp: now.AddMinutes(-25)
            ),
            new Activity(
                Id: "act_004",
                Type: ActivityType.Report,
                ActorId: "agent_004",
                ActorName: "Emily Watson",
                Message: "submitted inspection report",
                Timestamp: now.AddMinutes(-38)
            ),
            new Activity(
                Id: "act_005",
                Type: ActivityType.StatusChange,
                ActorId: "agent_003",
                ActorName: "Mike Rodriguez",
                Message: "changed status to Available",
                Timestamp: now.AddMinutes(-45)
            )
        }.ToImmutableList();
    }

    public async ValueTask<UserProfile> GetCurrentUserAsync(CancellationToken ct = default)
    {
        await Task.Delay(50, ct);

        return new UserProfile(
            Id: "user_001",
            Name: "Marcus Klein",
            Initials: "MK",
            Role: "Ops Manager",
            AvatarColor: AvatarColor.Blue,
            Status: AgentStatus.OnSite
        );
    }

    public async ValueTask<CurrentAssignment> GetCurrentAssignmentAsync(CancellationToken ct = default)
    {
        await Task.Delay(50, ct);

        return new CurrentAssignment(
            Status: AgentStatus.OnRoute,
            Destination: "Meridian Tower",
            Eta: "12 min",
            DestinationLocation: new Location(
                MontrealLat + 0.01,
                MontrealLng - 0.005,
                "123 Business Ave, Floor 12",
                Name: "Meridian Tower"
            )
        );
    }

    public async ValueTask<TodayProgress> GetTodayProgressAsync(CancellationToken ct = default)
    {
        await Task.Delay(50, ct);

        return new TodayProgress(
            Completed: 3,
            Remaining: 2,
            Total: 5
        );
    }

    public async ValueTask<IImmutableList<TaskItem>> GetTodayTasksAsync(CancellationToken ct = default)
    {
        await Task.Delay(100, ct);

        return new List<TaskItem>
        {
            new TaskItem(
                Id: "task_4523",
                OrderNumber: "WO-4523",
                Title: "Emergency HVAC Repair",
                Description: "AC unit on floor 12 not functioning. Client reports no cooling in server room.",
                Priority: TaskPriority.High,
                Status: TaskStatus.InProgress,
                Type: TaskType.Urgent,
                Location: new Location(
                    MontrealLat + 0.01,
                    MontrealLng - 0.005,
                    "123 Business Ave, Floor 12",
                    Name: "Meridian Tower",
                    Floor: "12",
                    Distance: "2.3 km"
                ),
                EstimatedDuration: "2h",
                ScheduledTime: DateTime.Today.AddHours(9),
                CreatedAt: DateTime.UtcNow.AddHours(-2)
            ),
            new TaskItem(
                Id: "task_4530",
                OrderNumber: "WO-4530",
                Title: "Security Panel Update",
                Description: "Update firmware on main security panel",
                Priority: TaskPriority.Medium,
                Status: TaskStatus.Pending,
                Type: TaskType.Scheduled,
                Location: new Location(
                    MontrealLat - 0.015,
                    MontrealLng + 0.008,
                    "555 Commerce Way",
                    Name: "Pacific Plaza",
                    Distance: "1.5 km"
                ),
                EstimatedDuration: "1h",
                ScheduledTime: DateTime.Today.AddHours(14),
                CreatedAt: DateTime.UtcNow.AddDays(-1)
            )
        }.ToImmutableList();
    }
}

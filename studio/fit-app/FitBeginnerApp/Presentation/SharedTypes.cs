namespace FitBeginnerApp.Presentation;

/// <summary>Represents a single exercise within a workout.</summary>
public partial record ExerciseItem(
    string Id,
    string Name,
    string Category,
    int DurationSeconds,
    int Sets,
    int Reps,
    string Difficulty,
    string Description);

/// <summary>A scheduled workout plan session (used on Plan and Progress pages).</summary>
public partial record WorkoutEntry(
    string Id,
    string Title,
    string Type,
    DateOnly ScheduledDate,
    int DurationMinutes,
    bool IsCompleted,
    string Difficulty);

/// <summary>A completed workout result for progress tracking.</summary>
public partial record WorkoutResult(
    string Id,
    string WorkoutTitle,
    DateOnly CompletedDate,
    int DurationMinutes,
    int CaloriesBurned,
    string Feeling);

/// <summary>User profile info used on Home and Profile pages.</summary>
public partial record UserProfile(
    string DisplayName,
    string FitnessLevel,
    string Goal,
    int WeeklyGoalDays,
    int CurrentStreakDays,
    int TotalWorkouts);

/// <summary>The Home page's header and weekly-stats payload, fetched as one request.</summary>
public partial record HomeSummary(
    string Greeting,
    string Motivation,
    WorkoutEntry TodayWorkout,
    int WeeklyCompletedDays,
    int WeeklyGoalDays,
    int TotalMinutesThisWeek,
    int CaloriesBurnedThisWeek);

/// <summary>The Plan page's header counters.</summary>
public partial record PlanSummary(string WeekLabel, int ScheduledCount, int CompletedCount);

/// <summary>The Progress page's all-time stats strip.</summary>
public partial record ProgressStats(
    int TotalWorkouts,
    int TotalMinutes,
    int TotalCalories,
    int CurrentStreak,
    int LongestStreak);

/// <summary>The Profile page's preference rows, beside the shared <see cref="UserProfile"/>.</summary>
public partial record ProfileDetails(
    string AvatarInitials,
    string JoinedDate,
    string PreferredTime,
    string EquipmentAvailable,
    int SessionLengthMinutes);

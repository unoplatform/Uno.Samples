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
    string IconGlyph,
    string Description);

/// <summary>A scheduled workout plan session (used on Plan and Progress pages).</summary>
public partial record WorkoutEntry(
    string Id,
    string Title,
    string Type,
    DateOnly ScheduledDate,
    int DurationMinutes,
    bool IsCompleted,
    string Difficulty,
    string IconGlyph);

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

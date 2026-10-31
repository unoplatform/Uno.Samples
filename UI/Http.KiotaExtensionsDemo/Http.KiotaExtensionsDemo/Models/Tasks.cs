namespace Http.KiotaExtensionsDemo.Models;

// The app's own view of the Tasks API. Presentation code sees only these types; the generated
// Kiota models stay inside Services/TasksService.cs.

public enum TaskPriority { Low, Normal, High }

public enum DoneFilter { All, Open, Done }

/// <summary>A task as the server returned it. <c>Id</c> makes it key-equatable for MVUX lists.</summary>
public partial record TaskItem(
    int Id,
    string Title,
    string? Notes,
    bool Done,
    DateTimeOffset? DueDate,
    TaskPriority Priority,
    IImmutableList<string> Tags,
    DateTimeOffset CreatedAt,
    int AttachmentCount)
{
    public string DueText => DueDate is { } due ? $"Due {due.LocalDateTime:d}" : "No due date";

    public string Details =>
        string.Join(" · ", new[] { Priority.ToString(), DueText, Tags.Count > 0 ? string.Join(", ", Tags) : null,
            AttachmentCount > 0 ? $"{AttachmentCount} attachment(s)" : null }.Where(s => s is not null));
}

/// <summary>The editable form of a task. Tags are edited as comma-separated text.</summary>
public partial record TaskDraft(string Title, string Notes, bool Done, DateTimeOffset? DueDate, TaskPriority Priority, string Tags)
{
    public static TaskDraft Empty { get; } = new(string.Empty, string.Empty, false, null, TaskPriority.Normal, string.Empty);

    public static TaskDraft From(TaskItem task) =>
        new(task.Title, task.Notes ?? string.Empty, task.Done, task.DueDate, task.Priority, string.Join(", ", task.Tags));

    public IImmutableList<string> TagList =>
        Tags.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToImmutableList();
}

/// <summary>Navigation data for the edit page: <c>Task</c> is null when creating.</summary>
public record TaskEditRequest(TaskItem? Task);

public partial record Attachment(int Id, string FileName, string? ContentType, long Size, string? Description)
{
    public string SizeText => Size < 1024 ? $"{Size} B" : $"{Size / 1024.0:0.#} KB";
}

public record TaskSummary(int Total, int Done, int Overdue, IImmutableDictionary<string, int> ByPriority)
{
    public string ByPriorityText => string.Join(" · ", ByPriority.OrderBy(p => p.Key).Select(p => $"{p.Key} {p.Value}"));
}

/// <summary>The list query. Every value is optional; the server's defaults apply when null.</summary>
public record TaskQuery(string? Search = null, bool? Done = null, int? Top = null, int? Skip = null)
{
    public static bool? ToDone(DoneFilter filter) => filter switch
    {
        DoneFilter.Open => false,
        DoneFilter.Done => true,
        _ => null,
    };
}

/// <summary>A file the user picked, read into memory.</summary>
public record PickedFile(string Name, string ContentType, byte[] Content);

/// <summary>The server rejected the input. <see cref="Errors"/> maps a field name (as the API spells it, e.g. "title") to its messages.</summary>
public sealed class TaskValidationException(IImmutableDictionary<string, IImmutableList<string>> errors)
    : Exception(string.Join(" ", errors.SelectMany(e => e.Value)))
{
    public IImmutableDictionary<string, IImmutableList<string>> Errors { get; } = errors;
}

/// <summary>The server has no task with this id (the API's <c>task_not_found</c> error).</summary>
public sealed class TaskNotFoundException(int id, string message) : Exception(message)
{
    public int Id { get; } = id;
}

/// <summary>Per-field messages for the edit form, from a <see cref="TaskValidationException"/>.</summary>
public partial record FieldErrors(string Title, string Notes, string DueDate, string Priority, string Tags, string Form)
{
    public static FieldErrors None { get; } = new(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);

    public static FieldErrors From(IImmutableDictionary<string, IImmutableList<string>> errors)
    {
        string Take(string field) => errors.TryGetValue(field, out var messages) ? string.Join(" ", messages) : string.Empty;

        string[] known = ["title", "notes", "dueDate", "priority", "tags"];
        var other = string.Join(" ", errors.Where(e => !known.Contains(e.Key)).SelectMany(e => e.Value));
        return new(Take("title"), Take("notes"), Take("dueDate"), Take("priority"), Take("tags"), other);
    }
}

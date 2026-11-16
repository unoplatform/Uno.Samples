namespace Http.KiotaExtensionsDemo.Server.Apis;

// The wire shapes of TasksApi.yaml's schemas. Property names serialize camelCase (the ASP.NET Core
// default) and Priority as "low" / "normal" / "high" (see Program.cs).
public enum Priority { Low, Normal, High }

public record TaskInput(string? Title, string? Notes, bool Done, DateTimeOffset? DueDate, Priority? Priority, string[]? Tags);

public record TaskItem(
    int Id, string Title, string? Notes, bool Done, DateTimeOffset? DueDate, Priority Priority, string[] Tags,
    DateTimeOffset CreatedAt, int AttachmentCount);

public record Attachment(int Id, string FileName, string? ContentType, long Size, string? Description);

public record ApiError(string Code, string Message);

public record Summary(int Total, int Done, int Overdue, Dictionary<string, int> ByPriority);

/// <summary>
/// The in-memory store behind the API. Data lives as long as the process; a restart reseeds it.
/// </summary>
// ponytail: one lock for the whole store; a real backend has a database.
public sealed class TaskStore
{
    private readonly Lock _gate = new();
    private readonly List<TaskItem> _tasks = [];
    private readonly Dictionary<int, List<(Attachment Info, byte[] Bytes)>> _attachments = [];
    private int _nextTaskId = 1;
    private int _nextAttachmentId = 1;
    private int _summaryCalls;

    /// <summary>How many calls to the summary answer 503 before it is "warm".</summary>
    public const int WarmUpCalls = 2;

    public TaskStore(TimeProvider clock)
    {
        var now = clock.GetUtcNow();
        Add(new("Write the Kiota sample README", "Setup, versions, tested-on list.", false, now.AddDays(2), Priority.High, ["docs"]), now);
        Add(new("Regenerate the client", "kiota update -o Content/Client/TasksApiClient", true, now.AddDays(-3), Priority.Normal, ["kiota"]), now);
        Add(new("Renew the TLS certificate", null, false, now.AddDays(-1), Priority.High, ["ops"]), now);
        Add(new("Buy milk", null, false, null, Priority.Low, []), now);
        Add(new("Plan the team offsite", "Venue, agenda, budget.", false, now.AddDays(14), Priority.Normal, ["team"]), now);
    }

    public int Count
    {
        get { lock (_gate) { return _tasks.Count; } }
    }

    public IReadOnlyList<TaskItem> List(string? search, bool? done, int top, int skip)
    {
        lock (_gate)
        {
            return _tasks
                .Where(t => string.IsNullOrWhiteSpace(search) || t.Title.Contains(search.Trim(), StringComparison.OrdinalIgnoreCase))
                .Where(t => done is null || t.Done == done)
                .OrderBy(t => t.DueDate is null)   // the contract says "by due date": undated last
                .ThenBy(t => t.DueDate)
                .ThenBy(t => t.Id)
                .Skip(skip)
                .Take(top)
                .ToList();
        }
    }

    public TaskItem? Get(int id)
    {
        lock (_gate) { return _tasks.Find(t => t.Id == id); }
    }

    public TaskItem Add(TaskInput input) => Add(input, DateTimeOffset.UtcNow);

    private TaskItem Add(TaskInput input, DateTimeOffset createdAt)
    {
        lock (_gate)
        {
            var task = ToItem(_nextTaskId++, input, createdAt, 0);
            _tasks.Add(task);
            return task;
        }
    }

    public TaskItem? Replace(int id, TaskInput input)
    {
        lock (_gate)
        {
            var index = _tasks.FindIndex(t => t.Id == id);
            if (index < 0)
            {
                return null;
            }
            var old = _tasks[index];
            return _tasks[index] = ToItem(id, input, old.CreatedAt, old.AttachmentCount);
        }
    }

    public bool Delete(int id)
    {
        lock (_gate)
        {
            _attachments.Remove(id);
            return _tasks.RemoveAll(t => t.Id == id) > 0;
        }
    }

    public IReadOnlyList<Attachment>? Attachments(int id)
    {
        lock (_gate)
        {
            if (!_tasks.Exists(t => t.Id == id))
            {
                return null;
            }
            return _attachments.TryGetValue(id, out var list) ? list.Select(a => a.Info).ToList() : [];
        }
    }

    public Attachment? Attach(int id, string fileName, string? contentType, byte[] bytes, string? description)
    {
        lock (_gate)
        {
            var index = _tasks.FindIndex(t => t.Id == id);
            if (index < 0)
            {
                return null;
            }
            var attachment = new Attachment(_nextAttachmentId++, fileName, contentType, bytes.LongLength, description);
            if (!_attachments.TryGetValue(id, out var list))
            {
                _attachments[id] = list = [];
            }
            list.Add((attachment, bytes));
            _tasks[index] = _tasks[index] with { AttachmentCount = list.Count };
            return attachment;
        }
    }

    /// <summary>True while the report is still warming up (the first <see cref="WarmUpCalls"/> calls).</summary>
    public bool TryWarmUp() => Interlocked.Increment(ref _summaryCalls) <= WarmUpCalls;

    public Summary Summary(DateTimeOffset now)
    {
        lock (_gate)
        {
            return new(
                _tasks.Count,
                _tasks.Count(t => t.Done),
                _tasks.Count(t => !t.Done && t.DueDate < now),
                Enum.GetValues<Priority>().ToDictionary(p => p.ToString().ToLowerInvariant(), p => _tasks.Count(t => t.Priority == p)));
        }
    }

    private static TaskItem ToItem(int id, TaskInput input, DateTimeOffset createdAt, int attachmentCount) =>
        new(id, input.Title!.Trim(), input.Notes, input.Done, input.DueDate, input.Priority ?? Priority.Normal,
            input.Tags ?? [], createdAt, attachmentCount);
}

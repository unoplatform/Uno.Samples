using Http.KiotaExtensionsDemo.Client;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Serialization;
using Api = Http.KiotaExtensionsDemo.Client.Models;

namespace Http.KiotaExtensionsDemo.Services;

/// <summary>
/// The app's only door to the Tasks API. It calls the generated <see cref="TasksApiClient"/>, maps
/// its models to the app's records and its error models to the app's exceptions, so no Model or
/// page ever sees a Kiota type.
/// </summary>
public sealed class TasksService(TasksApiClient client)
{
    /// <summary>Raised after every successful write, so every feed that reads tasks reloads.</summary>
    public Signal Changed { get; } = new();

    public async ValueTask<IImmutableList<TaskItem>> GetTasksAsync(TaskQuery query, CancellationToken ct)
    {
        var tasks = await client.Api.Tasks.GetAsync(config =>
        {
            config.QueryParameters.Search = string.IsNullOrWhiteSpace(query.Search) ? null : query.Search;
            config.QueryParameters.Done = query.Done;
            config.QueryParameters.Top = query.Top;
            config.QueryParameters.Skip = query.Skip;
        }, ct);
        return (tasks ?? []).Select(ToTask).ToImmutableList();
    }

    public async ValueTask<TaskItem> GetTaskAsync(int id, CancellationToken ct) =>
        await Call(id, async () => ToTask((await client.Api.Tasks[id].GetAsync(cancellationToken: ct))!));

    public async ValueTask<TaskItem> CreateAsync(TaskDraft draft, CancellationToken ct)
    {
        var created = await Call(null, async () => ToTask((await client.Api.Tasks.PostAsync(ToInput(draft), cancellationToken: ct))!));
        Changed.Raise();
        return created;
    }

    public async ValueTask<TaskItem> UpdateAsync(int id, TaskDraft draft, CancellationToken ct)
    {
        var updated = await Call(id, async () => ToTask((await client.Api.Tasks[id].PutAsync(ToInput(draft), cancellationToken: ct))!));
        Changed.Raise();
        return updated;
    }

    public async ValueTask DeleteAsync(int id, CancellationToken ct)
    {
        await Call(id, async () => { await client.Api.Tasks[id].DeleteAsync(cancellationToken: ct); return 0; });
        Changed.Raise();
    }

    public async ValueTask<IImmutableList<Attachment>> GetAttachmentsAsync(int id, CancellationToken ct) =>
        await Call(id, async () => ((await client.Api.Tasks[id].Attachments.GetAsync(cancellationToken: ct)) ?? []).Select(ToAttachment).ToImmutableList());

    /// <summary>Uploads a file as the contract's multipart/form-data body: a "file" part with its file name, and an optional "description" part.</summary>
    public async ValueTask<Attachment> UploadAsync(int id, PickedFile file, string? description, CancellationToken ct)
    {
        var body = new MultipartBody();
        body.AddOrReplacePart("file", string.IsNullOrEmpty(file.ContentType) ? "application/octet-stream" : file.ContentType, file.Content, file.Name);
        if (!string.IsNullOrWhiteSpace(description))
        {
            body.AddOrReplacePart("description", "text/plain", description);
        }

        var attachment = await Call(id, async () => ToAttachment((await client.Api.Tasks[id].Attachments.PostAsync(body, cancellationToken: ct))!));
        Changed.Raise();
        return attachment;
    }

    /// <summary>The server's one-line status, a <c>text/plain</c> response.</summary>
    public async ValueTask<string> GetStatusAsync(CancellationToken ct) =>
        await client.Api.Status.GetAsync(cancellationToken: ct) ?? string.Empty;

    /// <summary>
    /// The summary report. The server answers 503 with <c>Retry-After</c> while it warms up; Kiota's
    /// retry handler waits and retries (up to three times), so this call only throws if it is still warming up after that.
    /// </summary>
    public async ValueTask<TaskSummary> GetSummaryAsync(CancellationToken ct)
    {
        var summary = await client.Api.Reports.Summary.GetAsync(cancellationToken: ct);
        return new(
            summary?.Total ?? 0,
            summary?.Done ?? 0,
            summary?.Overdue ?? 0,
            (summary?.ByPriority?.AdditionalData ?? new Dictionary<string, object>())
                .ToImmutableDictionary(p => p.Key, p => ToInt(p.Value)));
    }

    // Turns the contract's error models into the app's exceptions.
    private static async Task<T> Call<T>(int? id, Func<Task<T>> call)
    {
        try
        {
            return await call();
        }
        catch (Api.ValidationProblem problem)
        {
            throw new TaskValidationException((problem.Errors?.AdditionalData ?? new Dictionary<string, object>())
                .ToImmutableDictionary(e => e.Key, e => Messages(e.Value)));
        }
        catch (Api.ApiError error) when (error.ResponseStatusCode == 404)
        {
            // A property named "message" collides with Exception.Message, so Kiota names it
            // MessageEscaped and makes Message return it.
            throw new TaskNotFoundException(id ?? 0, error.Message);
        }
    }

    // A map schema (additionalProperties) has no properties of its own: Kiota puts every entry in
    // AdditionalData as an untyped node.
    private static IImmutableList<string> Messages(object? value) => value switch
    {
        UntypedArray array => array.GetValue().Select(n => n is UntypedString s ? s.GetValue() ?? string.Empty : n.ToString() ?? string.Empty).ToImmutableList(),
        string text => [text],
        _ => [value?.ToString() ?? string.Empty],
    };

    private static int ToInt(object? value) => value switch
    {
        UntypedInteger i => i.GetValue(),
        UntypedLong l => (int)l.GetValue(),
        UntypedDouble d => (int)d.GetValue(),
        IConvertible c => c.ToInt32(null),
        _ => 0,
    };

    private static TaskItem ToTask(Api.TaskItem t) => new(
        t.Id ?? 0,
        t.Title ?? string.Empty,
        t.Notes,
        t.Done ?? false,
        t.DueDate,
        t.Priority switch { Api.Priority.Low => TaskPriority.Low, Api.Priority.High => TaskPriority.High, _ => TaskPriority.Normal },
        (t.Tags ?? []).ToImmutableList(),
        t.CreatedAt ?? default,
        t.AttachmentCount ?? 0);

    private static Api.TaskInput ToInput(TaskDraft d) => new()
    {
        Title = d.Title,
        Notes = string.IsNullOrWhiteSpace(d.Notes) ? null : d.Notes,
        Done = d.Done,
        DueDate = d.DueDate,
        Priority = d.Priority switch { TaskPriority.Low => Api.Priority.Low, TaskPriority.High => Api.Priority.High, _ => Api.Priority.Normal },
        Tags = [.. d.TagList],
    };

    private static Attachment ToAttachment(Api.Attachment a) =>
        new(a.Id ?? 0, a.FileName ?? string.Empty, a.ContentType, a.Size ?? 0, a.Description);
}

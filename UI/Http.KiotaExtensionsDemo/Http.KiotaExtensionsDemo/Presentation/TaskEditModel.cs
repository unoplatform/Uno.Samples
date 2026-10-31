namespace Http.KiotaExtensionsDemo.Presentation;

/// <summary>
/// Create and edit share this page. Validation is the server's: its 400 problem details come back as
/// <see cref="TaskValidationException"/> and each message is shown under its field.
/// </summary>
public partial record TaskEditModel(TasksService Tasks, INavigator Navigator, TaskEditRequest Request)
{
    public string Heading => Request.Task is null ? "New task" : "Edit task";

    public IImmutableList<TaskPriority> Priorities { get; } = [TaskPriority.Low, TaskPriority.Normal, TaskPriority.High];

    public IState<TaskDraft> Draft => State.Value(this, () => Request.Task is { } task ? TaskDraft.From(task) : TaskDraft.Empty);

    public IState<FieldErrors> Errors => State.Value(this, () => FieldErrors.None);

    public async ValueTask Save(TaskDraft draft, CancellationToken ct)
    {
        try
        {
            _ = Request.Task is { } task
                ? await Tasks.UpdateAsync(task.Id, draft, ct)
                : await Tasks.CreateAsync(draft, ct);
            await Navigator.NavigateBackAsync(this, cancellation: ct);
        }
        catch (TaskValidationException ex)
        {
            var errors = FieldErrors.From(ex.Errors);
            await Errors.UpdateAsync(_ => errors, ct);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            var errors = FieldErrors.None with { Form = $"Could not save: {ex.Message}" };
            await Errors.UpdateAsync(_ => errors, ct);
        }
    }
}

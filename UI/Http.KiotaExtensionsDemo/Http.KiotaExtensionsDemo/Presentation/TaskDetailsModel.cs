namespace Http.KiotaExtensionsDemo.Presentation;

/// <summary>
/// The navigation payload only identifies the task: the page shows the server's current copy,
/// reloaded after every write.
/// </summary>
public partial record TaskDetailsModel(TasksService Tasks, FilePicker Picker, INavigator Navigator, TaskItem Entity)
{
    public IFeed<TaskItem> Current => Feed.Async(ct => Tasks.GetTaskAsync(Entity.Id, ct), Tasks.Changed);

    public IListFeed<Attachment> Attachments => ListFeed.Async(ct => Tasks.GetAttachmentsAsync(Entity.Id, ct), Tasks.Changed);

    public IState<string> Description => State.Value(this, () => string.Empty);

    /// <summary>The outcome of the last upload or delete, shown under the buttons.</summary>
    public IState<string> Message => State.Value(this, () => string.Empty);

    public async ValueTask Upload(string description, CancellationToken ct)
    {
        try
        {
            if (await Picker.PickAsync(ct) is not { } file)
            {
                return;
            }
            await Message.SetAsync($"Uploading {file.Name}...", ct);
            var attachment = await Tasks.UploadAsync(Entity.Id, file, description, ct);
            await Description.SetAsync(string.Empty, ct);
            await Message.SetAsync($"Attached {attachment.FileName} ({attachment.SizeText}).", ct);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            await Message.SetAsync($"Upload failed: {ex.Message}", ct);
        }
    }

    public async ValueTask Edit(TaskItem current, CancellationToken ct) =>
        await Navigator.NavigateDataAsync(this, new TaskEditRequest(current), cancellation: ct);

    public async ValueTask Delete(TaskItem current, CancellationToken ct)
    {
        var choice = await Navigator.ShowMessageDialogAsync<string>(this,
            title: "Delete task?",
            content: $"\"{current.Title}\" and its attachments will be deleted.",
            buttons: [new DialogAction("Delete", Id: "Delete"), new DialogAction("Cancel", Id: "Cancel")],
            cancellation: ct);
        if (choice != "Delete")
        {
            return;
        }

        try
        {
            await Tasks.DeleteAsync(current.Id, ct);
            await Navigator.NavigateBackAsync(this, cancellation: ct);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            await Message.SetAsync($"Delete failed: {ex.Message}", ct);
        }
    }
}

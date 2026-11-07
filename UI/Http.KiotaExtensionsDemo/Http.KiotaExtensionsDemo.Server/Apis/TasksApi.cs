namespace Http.KiotaExtensionsDemo.Server.Apis;

/// <summary>
/// Implements every operation of <c>Http.KiotaExtensionsDemo/Specs/TasksApi.yaml</c>. That
/// document is the contract: change it first, then this file, then regenerate the app's client.
/// </summary>
internal static class TasksApi
{
    public const int MaxTitleLength = 200;

    public static WebApplication MapTasksApi(this WebApplication app)
    {
        var tasks = app.MapGroup("/api/tasks");

        tasks.MapGet("/", (TaskStore store, string? search, bool? done, int? top, int? skip) =>
            Results.Ok(store.List(search, done, Math.Clamp(top ?? 50, 1, 100), Math.Max(skip ?? 0, 0))));

        tasks.MapPost("/", (TaskStore store, TaskInput input) =>
        {
            if (Validate(input) is { } problem)
            {
                return problem;
            }
            var task = store.Add(input);
            return Results.Created($"/api/tasks/{task.Id}", task);
        });

        tasks.MapGet("/{id:int}", (TaskStore store, int id) =>
            store.Get(id) is { } task ? Results.Ok(task) : NotFound(id));

        tasks.MapPut("/{id:int}", (TaskStore store, int id, TaskInput input) =>
            Validate(input) is { } problem ? problem
            : store.Replace(id, input) is { } task ? Results.Ok(task)
            : NotFound(id));

        tasks.MapDelete("/{id:int}", (TaskStore store, int id) =>
            store.Delete(id) ? Results.NoContent() : NotFound(id));

        tasks.MapGet("/{id:int}/attachments", (TaskStore store, int id) =>
            store.Attachments(id) is { } list ? Results.Ok(list) : NotFound(id));

        // The contract's multipart/form-data body: a required "file" part and an optional
        // "description" part. Not a browser form, so no antiforgery token.
        tasks.MapPost("/{id:int}/attachments", async (TaskStore store, int id, IFormFile? file, [FromForm] string? description) =>
        {
            if (store.Get(id) is null)
            {
                return NotFound(id);
            }
            if (file is null)
            {
                return Results.ValidationProblem(new Dictionary<string, string[]> { ["file"] = ["A file is required."] });
            }

            using var buffer = new MemoryStream();
            await file.CopyToAsync(buffer);
            var attachment = store.Attach(id, file.FileName, file.ContentType, buffer.ToArray(), description)!;
            return Results.Created($"/api/tasks/{id}/attachments/{attachment.Id}", attachment);
        }).DisableAntiforgery();

        app.MapGet("/api/status", (TaskStore store) =>
            Results.Text($"Tasks API 1.0, {store.Count} tasks", "text/plain"));

        // Warms up on the first two calls after start: 503 with Retry-After, as the contract says.
        app.MapGet("/api/reports/summary", (TaskStore store, HttpResponse response) =>
        {
            if (store.TryWarmUp())
            {
                response.Headers.RetryAfter = "1";
                return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
            }
            return Results.Ok(store.Summary(DateTimeOffset.UtcNow));
        });

        return app;
    }

    private static IResult? Validate(TaskInput input)
    {
        var title = input.Title?.Trim();
        string? error = string.IsNullOrEmpty(title) ? "The title is required."
            : title.Length > MaxTitleLength ? $"The title must be at most {MaxTitleLength} characters."
            : null;

        return error is null ? null : Results.ValidationProblem(new Dictionary<string, string[]> { ["title"] = [error] });
    }

    private static IResult NotFound(int id) =>
        Results.NotFound(new ApiError("task_not_found", $"No task has the id {id}."));
}

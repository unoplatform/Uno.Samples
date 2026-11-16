using System.Diagnostics;
using System.Text;
using Http.KiotaExtensionsDemo.Client;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Serialization;
using Microsoft.Kiota.Http.HttpClientLibrary.Middleware.Options;
using Uno.Extensions;
using Uno.Extensions.Http.Kiota;
using Api = Http.KiotaExtensionsDemo.Client.Models;

namespace Http.KiotaExtensionsDemo.Tests;

/// <summary>
/// Every operation of TasksApi.yaml, from the generated client through the app's registration and
/// <see cref="TasksService"/>, against the real server in-process.
/// </summary>
public class TasksApiTests
{
    private TasksApiHost _api = null!;

    [SetUp]
    public void SetUp() => _api = new TasksApiHost();

    [TearDown]
    public void TearDown() => _api.Dispose();

    private TasksService Tasks => _api.Tasks;

    private TasksApiClient Client => _api.Services.GetRequiredService<TasksApiClient>();

    [Test]
    public async Task List_returns_the_seeded_tasks_by_due_date_with_undated_last()
    {
        var tasks = await Tasks.GetTasksAsync(new TaskQuery(), default);

        tasks.Should().HaveCount(5);
        tasks.Select(t => t.DueDate).Should().BeInAscendingOrder(Comparer<DateTimeOffset?>.Create((a, b) =>
            (a, b) switch { (null, null) => 0, (null, _) => 1, (_, null) => -1, _ => a.Value.CompareTo(b.Value) }));
        tasks.Last().Title.Should().Be("Buy milk");
        tasks.Should().Contain(t => t.Title == "Write the Kiota sample README" && t.Priority == TaskPriority.High && t.Tags.Contains("docs"));
    }

    [Test]
    public async Task List_search_matches_the_title_case_insensitively()
    {
        var tasks = await Tasks.GetTasksAsync(new TaskQuery(Search: "KIOTA"), default);

        tasks.Select(t => t.Title).Should().BeEquivalentTo("Write the Kiota sample README");
    }

    [Test]
    public async Task List_done_filters_by_completion_state()
    {
        var done = await Tasks.GetTasksAsync(new TaskQuery(Done: true), default);
        var open = await Tasks.GetTasksAsync(new TaskQuery(Done: false), default);

        done.Select(t => t.Title).Should().BeEquivalentTo("Regenerate the client");
        open.Should().HaveCount(4).And.OnlyContain(t => !t.Done);
    }

    [Test]
    public async Task List_top_and_skip_page_through_the_results()
    {
        var all = await Tasks.GetTasksAsync(new TaskQuery(), default);

        var page = await Tasks.GetTasksAsync(new TaskQuery(Top: 2, Skip: 1), default);

        page.Select(t => t.Id).Should().Equal(all.Skip(1).Take(2).Select(t => t.Id));
    }

    [Test]
    public async Task Create_returns_the_created_task_and_lists_it()
    {
        var draft = TaskDraft.Empty with { Title = "  Ship it  ", Notes = "today", Priority = TaskPriority.High, Tags = "a, b", DueDate = new DateTimeOffset(2030, 1, 2, 0, 0, 0, TimeSpan.Zero) };

        var created = await Tasks.CreateAsync(draft, default);

        created.Id.Should().BeGreaterThan(0);
        created.Title.Should().Be("Ship it");
        created.Notes.Should().Be("today");
        created.Priority.Should().Be(TaskPriority.High);
        created.Tags.Should().Equal("a", "b");
        created.DueDate.Should().Be(draft.DueDate);
        created.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromMinutes(1));
        (await Tasks.GetTasksAsync(new TaskQuery(Search: "ship"), default)).Should().ContainSingle(t => t.Id == created.Id);
    }

    [Test]
    public async Task Create_with_an_empty_title_throws_the_generated_ValidationProblem_with_its_errors()
    {
        // application/problem+json: with AddKiotaClient this would be ArgumentOutOfRangeException (see AddKiotaClient_limits).
        var act = () => Client.Api.Tasks.PostAsync(new Api.TaskInput { Title = "" });

        var problem = (await act.Should().ThrowAsync<Api.ValidationProblem>()).Which;
        problem.ResponseStatusCode.Should().Be(400);
        problem.Status.Should().Be(400);
        problem.Title.Should().NotBeNullOrEmpty();
        problem.Errors!.AdditionalData.Should().ContainKey("title");
        // A map schema's values arrive as Kiota untyped nodes.
        var messages = problem.Errors.AdditionalData["title"].Should().BeOfType<UntypedArray>().Which.GetValue()
            .Select(n => ((UntypedString)n).GetValue());
        messages.Should().Equal("The title is required.");
    }

    [Test]
    public async Task Create_with_a_too_long_title_surfaces_the_message_per_field_through_the_service()
    {
        var act = async () => await Tasks.CreateAsync(TaskDraft.Empty with { Title = new string('x', 201) }, default);

        var ex = (await act.Should().ThrowAsync<TaskValidationException>()).Which;
        ex.Errors["title"].Should().Equal("The title must be at most 200 characters.");
        FieldErrors.From(ex.Errors).Title.Should().Be("The title must be at most 200 characters.");
    }

    [Test]
    public async Task Get_an_unknown_id_throws_the_generated_ApiError_with_404()
    {
        var act = () => Client.Api.Tasks[999].GetAsync();

        var error = (await act.Should().ThrowAsync<Api.ApiError>()).Which;
        error.ResponseStatusCode.Should().Be(404);
        error.Code.Should().Be("task_not_found");
        // The schema's "message" property is generated as MessageEscaped, and Message returns it.
        error.MessageEscaped.Should().Be("No task has the id 999.");
        error.Message.Should().Be("No task has the id 999.");

        var mapped = async () => await Tasks.GetTaskAsync(999, default);
        (await mapped.Should().ThrowAsync<TaskNotFoundException>()).Which.Id.Should().Be(999);
    }

    [Test]
    public async Task Update_replaces_the_task()
    {
        var task = (await Tasks.GetTasksAsync(new TaskQuery(Search: "milk"), default)).Single();

        var updated = await Tasks.UpdateAsync(task.Id, TaskDraft.From(task) with { Title = "Buy oat milk", Done = true, Tags = "shopping" }, default);

        updated.Id.Should().Be(task.Id);
        updated.Title.Should().Be("Buy oat milk");
        updated.Done.Should().BeTrue();
        updated.Tags.Should().Equal("shopping");
        updated.CreatedAt.Should().Be(task.CreatedAt);
        (await Tasks.GetTaskAsync(task.Id, default)).Should().BeEquivalentTo(updated);
    }

    [Test]
    public async Task Update_with_an_empty_title_and_an_unknown_id_are_typed_errors()
    {
        var invalid = () => Client.Api.Tasks[1].PutAsync(new Api.TaskInput { Title = " " });
        (await invalid.Should().ThrowAsync<Api.ValidationProblem>()).Which.ResponseStatusCode.Should().Be(400);

        var missing = () => Client.Api.Tasks[999].PutAsync(new Api.TaskInput { Title = "x" });
        (await missing.Should().ThrowAsync<Api.ApiError>()).Which.ResponseStatusCode.Should().Be(404);
    }

    [Test]
    public async Task Delete_removes_the_task()
    {
        var task = (await Tasks.GetTasksAsync(new TaskQuery(Search: "offsite"), default)).Single();

        await Tasks.DeleteAsync(task.Id, default);

        (await Tasks.GetTasksAsync(new TaskQuery(), default)).Should().NotContain(t => t.Id == task.Id);
        var again = async () => await Tasks.DeleteAsync(task.Id, default);
        await again.Should().ThrowAsync<TaskNotFoundException>();
    }

    [Test]
    public async Task Upload_sends_multipart_and_the_attachment_is_listed()
    {
        var bytes = Encoding.UTF8.GetBytes("hello, attachment");

        var attachment = await Tasks.UploadAsync(1, new PickedFile("notes.txt", "text/plain", bytes), "first draft", default);

        attachment.Id.Should().BeGreaterThan(0);
        attachment.FileName.Should().Be("notes.txt");
        attachment.ContentType.Should().Be("text/plain");
        attachment.Size.Should().Be(bytes.Length);
        attachment.Description.Should().Be("first draft");

        var listed = await Tasks.GetAttachmentsAsync(1, default);
        listed.Should().ContainSingle().Which.Should().Be(attachment);
        (await Tasks.GetTaskAsync(1, default)).AttachmentCount.Should().Be(1);
    }

    [Test]
    public async Task Upload_without_a_description_and_attachments_of_an_unknown_task()
    {
        var attachment = await Tasks.UploadAsync(2, new PickedFile("image.png", "", [1, 2, 3]), null, default);
        attachment.ContentType.Should().Be("application/octet-stream");
        attachment.Description.Should().BeNull();

        var list = async () => await Tasks.GetAttachmentsAsync(999, default);
        await list.Should().ThrowAsync<TaskNotFoundException>();
        var upload = async () => await Tasks.UploadAsync(999, new PickedFile("x.txt", "text/plain", [1]), null, default);
        await upload.Should().ThrowAsync<TaskNotFoundException>();
    }

    [Test]
    public async Task Status_is_the_plain_text_line()
    {
        (await Tasks.GetStatusAsync(default)).Should().Be("Tasks API 1.0, 5 tasks");
    }

    [Test]
    public async Task Summary_loads_after_the_warm_up_because_Kiota_retries_the_503s()
    {
        var watch = Stopwatch.StartNew();

        var summary = await Tasks.GetSummaryAsync(default);

        // Two 503s with Retry-After: 1, each waited out by Kiota's retry handler.
        watch.Elapsed.Should().BeGreaterThan(TimeSpan.FromSeconds(1.5));
        summary.Total.Should().Be(5);
        summary.Done.Should().Be(1);
        summary.Overdue.Should().Be(1);
        summary.ByPriority.Should().BeEquivalentTo(new Dictionary<string, int> { ["low"] = 1, ["normal"] = 2, ["high"] = 2 });
    }

    [Test]
    public async Task Summary_without_retries_shows_the_warm_up_503()
    {
        var act = () => Client.Api.Reports.Summary.GetAsync(config => config.Options.Add(new RetryHandlerOption { MaxRetry = 0 }));

        var error = (await act.Should().ThrowAsync<ApiException>()).Which;
        error.ResponseStatusCode.Should().Be(503);
        error.ResponseHeaders.Should().ContainKey("Retry-After").WhoseValue.Should().Equal("1");
    }
}

/// <summary>
/// Why the app registers with <see cref="TasksApiSetup.AddTasksApi"/> and not <c>AddKiotaClient</c>:
/// with <c>AddKiotaClient</c> the adapter only knows JSON (unoplatform/uno.extensions#2799). When
/// these tests start failing, the issue is fixed: switch the app back to <c>AddKiotaClient</c>.
/// </summary>
public class AddKiotaClient_limits
{
    private TasksApiHost _api = null!;

    [SetUp]
    public void SetUp() => _api = new TasksApiHost((context, services) =>
        services.AddKiotaClient<TasksApiClient>(context, options: new EndpointOptions { Url = "http://localhost" }));

    [TearDown]
    public void TearDown() => _api.Dispose();

    private TasksApiClient Client => _api.Services.GetRequiredService<TasksApiClient>();

    [Test]
    public async Task Json_works()
    {
        (await Client.Api.Tasks.GetAsync()).Should().HaveCount(5);
    }

    [Test]
    public async Task Text_plain_response_throws()
    {
        var act = () => Client.Api.Status.GetAsync();
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>().WithMessage("*expected a application/json content type*");
    }

    [Test]
    public async Task Problem_json_error_throws_instead_of_the_typed_error()
    {
        var act = () => Client.Api.Tasks.PostAsync(new Api.TaskInput { Title = "" });
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>().WithMessage("*expected a application/json content type*");
    }

    [Test]
    public async Task Multipart_upload_throws()
    {
        var body = new MultipartBody();
        body.AddOrReplacePart("file", "text/plain", new byte[] { 1 }, "x.txt");
        var act = () => Client.Api.Tasks[1].Attachments.PostAsync(body);
        // Throws before sending: the request body cannot even be serialized.
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>().WithMessage("*expected a application/json content type*");
    }
}

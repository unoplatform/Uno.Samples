# A Kiota-generated API client on Uno Platform, via Uno.Extensions

A small task manager whose every byte of HTTP goes through a client the **Kiota CLI generated**
from an OpenAPI contract, registered in **`Uno.Extensions.Http`** and wrapped in one app service.
MVUX, Uno Navigation, Uno Toolkit and the Simple theme, as `dotnet new unoapp -preset recommended`
sets them up.

The sample is **design-first**: [`Specs/TasksApi.yaml`](Specs/TasksApi.yaml) is the contract. The
server (`../Http.KiotaExtensionsDemo.Server`) implements it, the client in
`Content/Client/TasksApiClient` is generated from it, and nobody edits either by hand to match the
other. The contract covers the cases that trip a JSON-only client:

| Operation | What it exercises | Where in the app |
| --- | --- | --- |
| `GET /api/tasks?search&done&top&skip` | query parameters | the list, the search box and the All / Open / Done filter |
| `POST /api/tasks`, `PUT /api/tasks/{id}` | JSON bodies; a 400 `application/problem+json` mapped to the generated `ValidationProblem` | the edit page: each server message under its field |
| `GET /api/tasks/{id}` | path parameters; a 404 mapped to the generated `ApiError` | the details page, and its error state when the task is gone |
| `DELETE /api/tasks/{id}` | a 204 with no body | Delete on the details page, after a confirmation dialog |
| `GET` / `POST /api/tasks/{id}/attachments` | a `multipart/form-data` upload (file + description) | the details page: the list, and *Attach a file...* with the system picker |
| `GET /api/status` | a `text/plain` response | the footer of the main page |
| `GET /api/reports/summary` | 503 with `Retry-After` while warming up, retried by Kiota | the summary card, which shows progress for about two seconds after the server starts |

## The three pages

- **Tasks** (`Presentation/MainPage.xaml`, `MainModel.cs`): the summary card, the search box and
  done filter, the list (loading, empty, error with Retry), and the status line. The list reloads
  when the search or filter changes, after any write anywhere in the app, and on Retry.
- **Task** (`TaskDetailsPage.xaml`, `TaskDetailsModel.cs`): the server's current copy of the task
  (the navigation payload only identifies it), its attachments, an upload with an optional
  description, Edit and Delete.
- **New task / Edit task** (`TaskEditPage.xaml`, `TaskEditModel.cs`): one form for both. The app
  does no validation of its own: Save sends the draft, and the server's messages appear under the
  field they belong to (try an empty title, or one over 200 characters).

## How the code is arranged

- `Services/TasksApiSetup.cs`: **the registration.** `AddTasksApi` registers the generated
  `TasksApiClient` and `TasksService`; `App.xaml.cs` calls it inside `UseHttp`, and the tests call
  the same method. It is `AddKiotaClient<TasksApiClient>` with one difference: `AddKiotaClient`
  gives the request adapter Kiota's JSON factories only
  ([unoplatform/uno.extensions#2799](https://github.com/unoplatform/uno.extensions/issues/2799)),
  so the `text/plain` status, the multipart upload and the `problem+json` errors would all throw
  `ArgumentOutOfRangeException: expected a application/json content type`. `AddTasksApi` builds the
  adapter with no factories, so it uses Kiota's default registries (JSON, text, form and multipart,
  filled by the generated constructor), and attaches Kiota's middleware exactly as `AddKiotaClient`
  does, so the retry handler still waits out the summary's 503s. Uno's `AddClient` supplies the base
  address, the native handler and every registered `DelegatingHandler`. The address is passed as
  `options`, not bound from a section, because a bound `Url` is lost in trimmed builds
  ([unoplatform/uno.extensions#3266](https://github.com/unoplatform/uno.extensions/issues/3266)).
- `Services/TasksService.cs`: **the only code that sees Kiota types.** Each method calls the
  generated request builders, maps the generated models to the app's records, and turns the
  contract's error models into the app's exceptions: `ValidationProblem` (400) into
  `TaskValidationException`, `ApiError` (404) into `TaskNotFoundException`. Two Kiota details are
  handled there: a map schema (`ValidationProblem.errors`, `Summary.byPriority`) is generated as a
  class with only `AdditionalData`, whose values are untyped nodes (`UntypedArray` of
  `UntypedString`, untyped numbers); and the schema's `message` property is generated as
  `MessageEscaped`, which the exception's `Message` returns. After every write it raises
  `Changed`, a `Signal` every feed that reads tasks refreshes on.
- `Models/Tasks.cs`: the app's records (`TaskItem`, `TaskDraft`, `Attachment`, `TaskSummary`, ...)
  and exceptions. The generated namespace `Http.KiotaExtensionsDemo.Client` is deliberately not a
  global using, so a Model cannot reach a Kiota type by accident.
- `Services/FilePicker.cs`: the system file picker, shown on the UI thread through `IDispatcher`
  (MVUX commands run on a background thread).
- `App.xaml.cs`: the host, the routes (`Main`, and `TaskDetails` and `TaskEdit` as `DataViewMap`s
  taking a `TaskItem` and a `TaskEditRequest`), and `ApiUrl`, the API address per head:
  `ApiClient:Url` from appsettings (set in **both** files, so Release builds have it too), the
  page's origin on WebAssembly, `10.0.2.2` instead of `localhost` on the Android emulator.
- `Content/Client/TasksApiClient`: **generated**, with its `kiota-lock.json`. Regenerated, never
  edited.
- `../Http.KiotaExtensionsDemo.Server/Apis`: the server, minimal APIs over an in-memory store
  (`TasksApi.cs`, `TaskStore.cs`). It also hosts the WebAssembly head.
- `../Http.KiotaExtensionsDemo.Tests`: every operation through `AddTasksApi` against the server
  in-process, plus `AddKiotaClient_limits`, which shows what plain `AddKiotaClient` cannot do.

## Running it

The setup guide (versions, each head, the tests, the steps to change the API, troubleshooting) is
[../README.md](../README.md). The short version: start the server
(`dotnet run --project Http.KiotaExtensionsDemo.Server`), then run a head.

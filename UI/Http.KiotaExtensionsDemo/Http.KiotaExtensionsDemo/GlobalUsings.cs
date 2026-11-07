global using System.Collections.Immutable;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Localization;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using Http.KiotaExtensionsDemo.Models;
global using Http.KiotaExtensionsDemo.Presentation;
global using Http.KiotaExtensionsDemo.Services;
global using Http.KiotaExtensionsDemo.Services.Endpoints;
// The generated client's namespace (Http.KiotaExtensionsDemo.Client) is deliberately not global:
// only Services/ uses it, so Models and pages cannot reach a Kiota type by accident.
global using ApplicationExecutionState = Windows.ApplicationModel.Activation.ApplicationExecutionState;
[assembly: Uno.Extensions.Reactive.Config.BindableGenerationTool(3)]

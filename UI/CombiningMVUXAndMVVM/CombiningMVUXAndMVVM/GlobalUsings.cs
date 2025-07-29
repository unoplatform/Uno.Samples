global using System.Collections.Immutable;
global using CombiningMVUXAndMVVM.Client;
global using CombiningMVUXAndMVVM.DataContracts;
global using CombiningMVUXAndMVVM.DataContracts.Serialization;
global using CombiningMVUXAndMVVM.Models;
global using CombiningMVUXAndMVVM.Presentation;
global using CombiningMVUXAndMVVM.Services.Caching;
global using CombiningMVUXAndMVVM.Services.Endpoints;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Localization;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using Uno.Extensions.Http.Kiota;
global using ApplicationExecutionState = Windows.ApplicationModel.Activation.ApplicationExecutionState;
[assembly: Uno.Extensions.Reactive.Config.BindableGenerationTool(3)]

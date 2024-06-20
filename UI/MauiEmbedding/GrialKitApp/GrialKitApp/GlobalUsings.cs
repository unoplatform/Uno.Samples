global using System.Collections.Immutable;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Localization;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using GrialKitApp.Models;
global using GrialKitApp.Presentation;
global using GrialKitApp.Services.Endpoints;
#if MAUI_EMBEDDING
global using GrialKitApp.MauiControls;
#endif
global using ApplicationExecutionState = Windows.ApplicationModel.Activation.ApplicationExecutionState;

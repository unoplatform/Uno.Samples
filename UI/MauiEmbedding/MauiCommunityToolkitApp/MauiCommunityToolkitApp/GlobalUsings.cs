global using System.Collections.Immutable;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Localization;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using MauiCommunityToolkitApp.Models;
global using MauiCommunityToolkitApp.Presentation;
global using MauiCommunityToolkitApp.DataContracts;
global using MauiCommunityToolkitApp.DataContracts.Serialization;
global using MauiCommunityToolkitApp.Services.Caching;
global using MauiCommunityToolkitApp.Services.Endpoints;
#if MAUI_EMBEDDING
global using MauiCommunityToolkitApp.MauiControls;
#endif
global using ApplicationExecutionState = Windows.ApplicationModel.Activation.ApplicationExecutionState;

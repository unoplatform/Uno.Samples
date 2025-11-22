using MauiCommunityToolkitApp;
using Uno.UI.Hosting;
using App = MauiCommunityToolkitApp.App;

var host = UnoPlatformHostBuilder.Create()
	.App(() => new App())
	.UseAppleUIKit()
	.Build();

host.Run();

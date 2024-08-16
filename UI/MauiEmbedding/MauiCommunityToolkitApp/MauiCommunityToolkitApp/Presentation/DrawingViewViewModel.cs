using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiCommunityToolkitApp.Presentation;
partial class DrawingViewViewModel : ObservableObject
{
	[ObservableProperty]
	bool isMultiLineModeEnabled;

	[ObservableProperty]
	bool shouldCleanOnFinish;
}

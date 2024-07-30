using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiEmbeddingApp.ViewModels;

partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    bool isMultiLineModeEnabled;
    
    [ObservableProperty]
    bool shouldCleanOnFinish;
}

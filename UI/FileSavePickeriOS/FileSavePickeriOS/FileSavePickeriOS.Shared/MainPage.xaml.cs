using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace FileSavePickeriOS;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();
    }

    public async void SaveFile_Click(object sender, RoutedEventArgs e)
    {
        var savePicker = new FileSavePicker();
        savePicker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
        savePicker.FileTypeChoices.Add("Images", new[]
        {
                ".jpg",
                ".gif"
            });
        savePicker.FileTypeChoices.Add("Text", new[]
        {
                ".txt",
                ".docs"
            });

        var file = await savePicker.PickSaveFileAsync();
        await FileIO.WriteTextAsync(file, "Hello, world!");
    }
}

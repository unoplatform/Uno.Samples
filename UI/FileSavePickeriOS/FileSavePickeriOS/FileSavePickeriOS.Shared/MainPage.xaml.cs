using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace FileSavePickeriOS
{
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
}

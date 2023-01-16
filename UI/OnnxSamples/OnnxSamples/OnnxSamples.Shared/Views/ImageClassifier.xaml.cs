using Microsoft.UI.Xaml.Controls;

using OnnxSamples.Models;
using OnnxSamples.Views;

using System.Threading.Tasks;
using System;

using Uno.Toolkit.UI;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace OnnxSamples.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ImageClassifier : Page
    {
        OnnxImageClassifier _classifier;

        public string[] EmbeddedResources { get; } = typeof(MainPage).Assembly.GetManifestResourceNames();

        public ImageClassifier()
        {
            InitializeComponent();
            _classifier = new OnnxImageClassifier();
        }

        async Task RunInferenceAsync(string filename)
        {
            try
            {
                var sampleImage = await _classifier.GetSampleImageAsync(filename);
                var result = await _classifier.GetClassificationAsync(sampleImage, filename);

                var dialog = new ContentDialog();
                dialog.Content = result;
                dialog.CloseButtonText = "Done";

                var dialogResult = await dialog.ShowAsync();

            }
            catch (Exception exception)
            {

                var dialog = new ContentDialog();
                dialog.Content = $"ERROR:{exception.Message}";
                dialog.CloseButtonText = "Done";

                var dialogResult = await dialog.ShowAsync();
            }
        }

        private async void RunButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            var runButton = sender as Button;
            runButton.IsEnabled = false;
            await RunInferenceAsync("dogg.jpg");
            runButton.IsEnabled = true;
        }

        private async void LoadButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) => await RunInferenceAsync("chickenn.jpg");
    }
}

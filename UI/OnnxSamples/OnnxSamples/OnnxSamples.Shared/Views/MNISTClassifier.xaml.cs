// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

using Microsoft.UI.Xaml.Controls;

using System.Threading.Tasks;
using System;
using OnnxSamples.Models;

namespace OnnxSamples.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MNISTClassifier : Page
    {
        TensorflowMNISTClassifier _classifier;
        public string[] EmbeddedResources { get; } = typeof(MainPage).Assembly.GetManifestResourceNames();

        public MNISTClassifier()
        {
            InitializeComponent();
            _classifier = new TensorflowMNISTClassifier();
        }

        async Task RunInferenceAsync()
        {
            try
            {
                var sampleImage = _classifier.GetSampleImage();
                var result = await _classifier.GetPredictionAsync(sampleImage);

                var dialog = new ContentDialog();
                dialog.Content = $"The result is : {result}";
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
            await RunInferenceAsync();
            runButton.IsEnabled = true;
        }

    }
}

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;

using OnnxSamples.Models;

using System;
using System.IO;
using System.Threading.Tasks;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace OnnxSamples.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BertQnA : Page
    {
        PytorchBertQnA bertQnA;

        public string[] EmbeddedResources { get; } = typeof(MainPage).Assembly.GetManifestResourceNames();
        
        public BertQnA()
        {
            InitializeComponent();
            bertQnA = new PytorchBertQnA();
            foreach (var item in EmbeddedResources)
            {
                Console.WriteLine(item);
            }
        }
        
        async Task RunBertQnAAsync(string question , string context)
        {
            try
            {
                var result = await bertQnA.GetAnswerFromBert(question, context);
                var dialog = new ContentDialog();
                dialog.Title = $"Answer to {question}";
                dialog.Content = $"Answer is : {result.Item2}";
                dialog.CloseButtonText = "Done";

                var dialogResult = await dialog.ShowAsync();
            }
            catch (Exception exception)
            {

                var dialog = new ContentDialog();
                dialog.Content = $"ERROR:{exception.Message}";
                dialog.CloseButtonText = "Done";
                _ = await dialog.ShowAsync();
            }
        }


        private async void RunButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            var runButton = sender as Button;
            runButton.IsEnabled = false;
            await RunBertQnAAsync(QuestionTextBox.Text, ContextTextBox.Text);
            runButton.IsEnabled = true;
        }
    }
}

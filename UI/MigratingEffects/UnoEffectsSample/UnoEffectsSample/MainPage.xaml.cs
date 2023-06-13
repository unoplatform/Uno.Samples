
using System.ComponentModel;

namespace UnoEffectsSample
{
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        public MainPage()
        {
            this.InitializeComponent();

            ErrorText = "Ceci n'est pas une erreur!";
            //ErrorTextBox.SetErrorMessage("Ceci n'est pas une erreur!");

            PropertyChanged += MainPage_PropertyChanged;
        }

        private void MainPage_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.PropertyName);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ErrorText = null;
        }

        private string? errorText;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string? ErrorText
        {
            get => errorText; 
            set
            {
                if(errorText != value)
                {
                    errorText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ErrorText)));
                }
            }
        }
    }
}
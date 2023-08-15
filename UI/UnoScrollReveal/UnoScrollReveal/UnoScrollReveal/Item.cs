namespace UnoScrollReveal
{
    public partial class Item : ObservableObject
    {
        [ObservableProperty]
        private string title;

        [ObservableProperty]
        private string description;

        [ObservableProperty]
        private bool isEmpty;
    }
}
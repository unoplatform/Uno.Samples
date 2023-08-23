using System.Collections.ObjectModel;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace HomeIdeasUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public readonly ObservableCollection<Ideas> ideas;

        public void GenerateInfo()
        {
            // Home ideas list
            ideas.Add(new Ideas() { Percentage = "50%", PercentageColor = "#ffa500", Title = "Walking", Description = "How Many in May?", Amount = "24.08", AmountType = "km" });
            ideas.Add(new Ideas() { Percentage = "50%", PercentageColor = "#164734", Title = "Vacations in France", Description = "Budget with Emma", Amount = "72.000", AmountType = "USD" });
            ideas.Add(new Ideas() { Percentage = "50%", PercentageColor = "#01abf3", Title = "Reading", Description = "Goal: Read 16 books a year", Amount = "12", AmountType = "/16" });
            ideas.Add(new Ideas() { Percentage = "50%", PercentageColor = "#ff4582", Title = "Quit Smoking", Description = "Last cigare on 0420", Amount = "24.08", AmountType = "km" });
            ideas.Add(new Ideas() { Percentage = "50%", PercentageColor = "#ffa500", Title = "Walking", Description = "How Many in May?", Amount = "24.08", AmountType = "km" });
            ideas.Add(new Ideas() { Percentage = "50%", PercentageColor = "#164734", Title = "Vacations in France", Description = "Budget with Emma", Amount = "72.000", AmountType = "USD" });

        } 
        public MainPage()
        {
            this.InitializeComponent();
            ideas = new ObservableCollection<Ideas>();
            GenerateInfo();

        }

      
    }
}

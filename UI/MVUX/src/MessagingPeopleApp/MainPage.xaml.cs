namespace MessagingPeopleApp
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();                   
        }

        public MainPage(BindablePeopleModel peopleModel)
            : this()
        {
            this.DataContext = peopleModel;
        }
    }
}
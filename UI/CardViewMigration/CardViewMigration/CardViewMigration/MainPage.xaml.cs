using CommunityToolkit.Mvvm.Input;
using System.Reflection;

namespace CardViewMigration;

public sealed partial class MainPage : Page
{
    public ICommand NavigateCommand { get; set; }

    public MainPage()
    {
        this.InitializeComponent();

        NavigateCommand = new RelayCommand<string>((pageTypeName)=>
        {
            Type pageType = Type.GetType(pageTypeName, false, true);
            Frame.Navigate(pageType);
        });

        DataContext = this;
    }
    public Type GetType(string typeName)
    {
        return Assembly.GetCallingAssembly().GetType(typeName);
    }
}

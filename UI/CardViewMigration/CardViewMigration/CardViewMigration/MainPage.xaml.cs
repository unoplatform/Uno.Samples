using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Reflection;
using System.Windows.Input;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CardViewMigration;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CardViewMigration;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class CardViewXamlPage : Page
{
    public CardViewXamlPage()
    {
        InitializeComponent();
    }

    public void GoBack()
    {
        Frame.GoBack();
    }
}

using InTheHand.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnoCakesMobile.Services;
using UnoCakesMobile.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UnoCakesMobile.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CakeList : Page
    {
        public CakeListViewModel ViewModel { get; private set; }

        public CakeList()
        {
            this.InitializeComponent();
            ViewModel = DependencyService.Get<CakeListViewModel>();
            this.Loaded += CakeList_Loaded;
            
        }

        private void CakeList_Loaded(object sender, RoutedEventArgs e)
        {
            if(Frame.BackStackDepth > 0)
                Frame.BackStack.RemoveAt(0);
        }
    }
}

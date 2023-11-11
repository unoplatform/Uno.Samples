using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnoCakesMobile.Services;
using UnoCakesMobile.Views;

namespace UnoCakesMobile.ViewModels
{
    public partial class CakeListViewModel : ObservableObject
    {
        private INavigationService _navigationService;
        public CakeListViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public void ShowCake()
        {
            _navigationService.Navigate(typeof(CakeDetails), null, new DrillInNavigationTransitionInfo());
        }
    }
}

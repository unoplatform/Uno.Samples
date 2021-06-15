using DemoApp.ViewModels;

using PharmaSupply.Contracts.Pharmacy.ContractDefinition;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DemoApp.TabbedPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PatientPage : Page
    {
        #region Properties

        public PatientVM ViewModel { get; set; }

        #endregion

        public PatientPage()
        {
            this.InitializeComponent();
        }

        #region Method(s)

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (ViewModel == null)
            {
                ViewModel = DataContext as PatientVM;
            }
            ViewModel.SetUp = e.Parameter as SetUp;
            ViewModel.Bind();
        }


        
        private async void ConsumeButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            var fxn = new ConsumeFunction();
            fxn.TokenId = string.IsNullOrEmpty(tokenId.Text) || string.IsNullOrWhiteSpace(tokenId.Text) ? BigInteger.Parse(tokenId.Text) : new BigInteger(new Random().Next(10, 1000));
            fxn.PatientAddress = ViewModel.SetUp.Accounts.four.Address;
            fxn.Patient = Convert.ToByte(3);

            await ViewModel.ConsumptionCommand(fxn);
            await UpdateTracking();
        }

        private async Task UpdateTracking()
        {
            var track = await ViewModel.GetPreviousOwners();
            trackTypes.Children.Clear();
            foreach (var item in track._types)
            {
                trackTypes.Children.Add(item);
            }
            trackAddresses.Children.Clear();
            foreach (var item in track._addresses)
            {
                trackAddresses.Children.Add(item);
            }
            trackTokenIds.Children.Clear();
            foreach (var item in track._tokens)
            {
                trackTokenIds.Children.Add(item);
            }
        }

        #endregion
    }

}

using DemoApp.ViewModels;

using PharmaSupply.Contracts.Pharmacy.ContractDefinition;

using System;
using System.Numerics;
using System.Threading.Tasks;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DemoApp.TabbedPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PharmacyPage : Page
    {
        #region Properties

        public PharmacyVM ViewModel { get; set; }

        #endregion

        public PharmacyPage()
        {
            this.InitializeComponent();
        }

        #region Method(s)

        
        private async void DispatchButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            var fxn = new DispatchFunction();
            fxn.SourceAddress = ViewModel.SetUp.Accounts.three.Address;
            fxn.SourceType = 2;
            fxn.DestinationAddress = (string.IsNullOrEmpty(_accountParam.Text) || string.IsNullOrWhiteSpace(_accountParam.Text)) ? ViewModel.SetUp.Accounts.four.Address : _accountParam.Text;
            fxn.DestinationType = (string.IsNullOrEmpty(_typeParam.Text) || string.IsNullOrWhiteSpace(_typeParam.Text)) ? Convert.ToByte(1) : Convert.ToByte(_typeParam.Text);
            fxn.TokenId = string.IsNullOrEmpty(tokenId.Text) || string.IsNullOrWhiteSpace(tokenId.Text) ? BigInteger.Parse(tokenId.Text) : new BigInteger(new Random().Next(10, 1000));
            ViewModel = DataContext as PharmacyVM;
            await ViewModel.DispatchCommand(fxn);
            await UpdateTracking();
        }

        private async void DeployButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            var deployment = new PharmacyDeployment();
            deployment.Name = (string.IsNullOrEmpty(tokenName.Text) || string.IsNullOrWhiteSpace(tokenName.Text)) ? ViewModel.Name : tokenName.Text;
            deployment.Symbol = (string.IsNullOrEmpty(tokenSymbol.Text) || string.IsNullOrWhiteSpace(tokenSymbol.Text)) ? ViewModel.Symbol : tokenSymbol.Text;
            ViewModel = DataContext as PharmacyVM;
            await ViewModel.DeployCommand(deployment);
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

﻿using DemoApp.CanvasObjects;
using DemoApp.ViewModels;

using PharmaSupply.Contracts.DrugShipment;
using PharmaSupply.Contracts.DrugShipment.ContractDefinition;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DemoApp.TabbedPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ManufacturerPage : Page
    {
        #region Properties

        public ManufacturerVM ViewModel { get; set; }

        #endregion

        public ManufacturerPage()
        {
            this.InitializeComponent();
            
        }   

        #region Method(s)

       
        private async void DispatchButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            var fxn = new DispatchFunction();
            fxn.SourceAddress = ViewModel.SetUp.Accounts.one.Address;
            fxn.SourceType = 0;
            fxn.DestinationAddress = (string.IsNullOrEmpty(_accountParam.Text) || string.IsNullOrWhiteSpace(_accountParam.Text)) ? ViewModel.SetUp.Accounts.two.Address : _accountParam.Text;
            fxn.DestinationType = (string.IsNullOrEmpty(_typeParam.Text) || string.IsNullOrWhiteSpace(_typeParam.Text)) ? Convert.ToByte(1) : Convert.ToByte(_typeParam.Text);
            fxn.TokenId = string.IsNullOrEmpty(tokenId.Text) || string.IsNullOrWhiteSpace(tokenId.Text) ? BigInteger.Parse(tokenId.Text) : new BigInteger(new Random().Next(10, 1000));
            ViewModel = DataContext as ManufacturerVM;
            await ViewModel.DispatchCommand(fxn);
            await UpdateTracking();    
        }

        private async void DeployButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            var deployment = new DrugShipmentDeployment();
            deployment.Name = (string.IsNullOrEmpty(tokenName.Text) || string.IsNullOrWhiteSpace(tokenName.Text)) ?  ViewModel.Name : tokenName.Text;
            deployment.Symbol = (string.IsNullOrEmpty(tokenSymbol.Text) || string.IsNullOrWhiteSpace(tokenSymbol.Text)) ? ViewModel.Symbol : tokenSymbol.Text;
            ViewModel = DataContext as ManufacturerVM;
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

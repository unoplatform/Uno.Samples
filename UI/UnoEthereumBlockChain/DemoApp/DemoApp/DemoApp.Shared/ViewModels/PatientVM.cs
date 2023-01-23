using Microsoft.UI.Xaml.Controls;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;


using PharmaSupply.Contracts.Pharmacy;
using PharmaSupply.Contracts.Pharmacy.ContractDefinition;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.UI.Text;

namespace DemoApp.ViewModels
{
    public class PatientVM : BaseViewModel
    {
        #region Properties

        string contractHashes;
        string ownedTokens;

        string name;
        string symbol;

        TransactionReceipt consumptionReceipt;
        
        string shipmentMsgs;
        string errorMsgs;
        string transferMsgs;
        string approvalMsgs;


        public SetUp SetUp { get; internal set; }

        public string ContractHashes
        {
            get => contractHashes;
            set => SetField(ref contractHashes, value, "ContractHashes");
        }

        public string OwnedTokens
        {
            get => ownedTokens;
            set => SetField(ref ownedTokens, value, "OwnedTokens");
        }

        public string Name
        {
            get => name;
            set => SetField(ref name, value, "Name");
        }

        public string Symbol
        {
            get => symbol;
            set => SetField(ref symbol, value, "Symbol");
        }

        public TransactionReceipt ConsumptionReceipt
        {
            get => consumptionReceipt;
            set => SetField(ref consumptionReceipt, value, "ConsumptionReceipt");
        }
                
        public string ShipmentMsgs
        {
            get => shipmentMsgs;
            set => SetField(ref shipmentMsgs, value, "ShipmentMsgs");
        }

        public string ErrorMsgs
        {
            get => errorMsgs;
            set => SetField(ref errorMsgs, value, "ErrorMsgs");
        }

        public string TransferMsgs
        {
            get => transferMsgs;
            set => SetField(ref transferMsgs, value, "TransferMsgs");
        }

        public string ApprovalMsgs
        {
            get => approvalMsgs;
            set => SetField(ref approvalMsgs, value, "ApprovalMsgs");
        }

        public PharmacyService DrugShipmentService { get; set; }

        #endregion

        #region Constructor(s)

        public PatientVM()
        {
            Name = "Moderna";
            Symbol = "MOD";
        }

        #endregion

        #region Method(s)

        public void Bind()
        {
            ContractHashes = $"DrugShipment Address : {SetUp.DrugShipment},  Migration Address : {SetUp.Migrations}";
            DrugShipmentService = SetUp.Service;
        }
        
        public async Task ConsumptionCommand(ConsumeFunction fxn)
        {

            try
            {
                ConsumptionReceipt = await SetUp.Service.ConsumeRequestAndWaitForReceiptAsync(fxn);
                GetDispatchEvents();
            }
            catch (Exception exception)
            {

                Console.WriteLine(exception.Message);
            }
        }

        public async Task<(IEnumerable<TextBlock> _types, IEnumerable<TextBlock> _addresses, IEnumerable<TextBlock> _tokens)> GetPreviousOwners()
        {
            var owners = await SetUp.Service.GetPreviousOwnersQueryAsync();
            var _addresses = owners?.ReturnValue1.Select(address => new TextBlock { Text = address, FontWeight = FontWeights.ExtraBold });
            var _types = owners?.ReturnValue2.Select(_type => new TextBlock { Text = _type, FontWeight = FontWeights.ExtraBold });
            var _tokens = owners?.ReturnValue3.Select(_token => new TextBlock { Text = _token.ToString(), FontWeight = FontWeights.ExtraBold });
            return (_types != null ? _types : new List<TextBlock>(), _addresses != null ? _addresses : new List<TextBlock>(), _tokens != null ? _tokens : new List<TextBlock>());
        }

        public void GetDispatchEvents()
        {
            var logShipment = ConsumptionReceipt.DecodeAllEvents<LogShipmentEventDTO>().Select(log => $"{log.Event.Source} | {log.Event.Destination} | {log.Event.Message}");
            var logError = ConsumptionReceipt.DecodeAllEvents<LogErrorEventDTO>().Select(log => $"{log.Event.Source} | {log.Event.Message}");
            var transfer = ConsumptionReceipt.DecodeAllEvents<TransferEventDTO>().Select(log => $"{log.Event.From} | {log.Event.To} | {log.Event.TokenId}");
            var approval = ConsumptionReceipt.DecodeAllEvents<ApprovalEventDTO>().Select(log => $"{log.Event.Approved} | {log.Event.Owner} | {log.Event.TokenId}");

            ShipmentMsgs = $"LOG SHIPMENT EVENTS : {string.Join("\n", logShipment)}";
            ErrorMsgs = $"LOG ERROR EVENTS : {string.Join("\n", logError)}";
            TransferMsgs = $"LOG TRANSFERS : {string.Join("\n", transfer)}";
            ApprovalMsgs = $"LOG APPROVALS : {string.Join("\n", approval)}";
        }
        #endregion
    }
}

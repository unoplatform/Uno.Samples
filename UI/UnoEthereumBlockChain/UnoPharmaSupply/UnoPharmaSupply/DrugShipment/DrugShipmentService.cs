using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Web3;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.CQS;
using Nethereum.Contracts.ContractHandlers;
using Nethereum.Contracts;
using System.Threading;
using PharmaSupply.Contracts.DrugShipment.ContractDefinition;

namespace PharmaSupply.Contracts.DrugShipment
{
    public partial class DrugShipmentService
    {
        public static Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(Nethereum.Web3.Web3 web3, DrugShipmentDeployment drugShipmentDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            return web3.Eth.GetContractDeploymentHandler<DrugShipmentDeployment>().SendRequestAndWaitForReceiptAsync(drugShipmentDeployment, cancellationTokenSource);
        }

        public static Task<string> DeployContractAsync(Nethereum.Web3.Web3 web3, DrugShipmentDeployment drugShipmentDeployment)
        {
            return web3.Eth.GetContractDeploymentHandler<DrugShipmentDeployment>().SendRequestAsync(drugShipmentDeployment);
        }

        public static async Task<DrugShipmentService> DeployContractAndGetServiceAsync(Nethereum.Web3.Web3 web3, DrugShipmentDeployment drugShipmentDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            var receipt = await DeployContractAndWaitForReceiptAsync(web3, drugShipmentDeployment, cancellationTokenSource);
            return new DrugShipmentService(web3, receipt.ContractAddress);
        }

        protected Nethereum.Web3.Web3 Web3 { get; }

        public ContractHandler ContractHandler { get; }

        public DrugShipmentService(Nethereum.Web3.Web3 web3, string contractAddress)
        {
            Web3 = web3;
            ContractHandler = web3.Eth.GetContractHandler(contractAddress);
        }

        public Task<string> ConsumeRequestAsync(ConsumeFunction consumeFunction)
        {
            return ContractHandler.SendRequestAsync(consumeFunction);
        }

        public Task<TransactionReceipt> ConsumeRequestAndWaitForReceiptAsync(ConsumeFunction consumeFunction, CancellationTokenSource cancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(consumeFunction, cancellationToken);
        }

        public Task<string> ConsumeRequestAsync(byte patient, string patientAddress, BigInteger tokenId)
        {
            var consumeFunction = new ConsumeFunction();
            consumeFunction.Patient = patient;
            consumeFunction.PatientAddress = patientAddress;
            consumeFunction.TokenId = tokenId;

            return ContractHandler.SendRequestAsync(consumeFunction);
        }

        public Task<TransactionReceipt> ConsumeRequestAndWaitForReceiptAsync(byte patient, string patientAddress, BigInteger tokenId, CancellationTokenSource cancellationToken = null)
        {
            var consumeFunction = new ConsumeFunction();
            consumeFunction.Patient = patient;
            consumeFunction.PatientAddress = patientAddress;
            consumeFunction.TokenId = tokenId;

            return ContractHandler.SendRequestAndWaitForReceiptAsync(consumeFunction, cancellationToken);
        }

        public Task<string> DispatchRequestAsync(DispatchFunction dispatchFunction)
        {
            return ContractHandler.SendRequestAsync(dispatchFunction);
        }

        public Task<TransactionReceipt> DispatchRequestAndWaitForReceiptAsync(DispatchFunction dispatchFunction, CancellationTokenSource cancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(dispatchFunction, cancellationToken);
        }

        public Task<string> DispatchRequestAsync(byte sourceType, string sourceAddress, byte destinationType, string destinationAddress, BigInteger tokenId)
        {
            var dispatchFunction = new DispatchFunction();
            dispatchFunction.SourceType = sourceType;
            dispatchFunction.SourceAddress = sourceAddress;
            dispatchFunction.DestinationType = destinationType;
            dispatchFunction.DestinationAddress = destinationAddress;
            dispatchFunction.TokenId = tokenId;

            return ContractHandler.SendRequestAsync(dispatchFunction);
        }

        public Task<TransactionReceipt> DispatchRequestAndWaitForReceiptAsync(byte sourceType, string sourceAddress, byte destinationType, string destinationAddress, BigInteger tokenId, CancellationTokenSource cancellationToken = null)
        {
            var dispatchFunction = new DispatchFunction();
            dispatchFunction.SourceType = sourceType;
            dispatchFunction.SourceAddress = sourceAddress;
            dispatchFunction.DestinationType = destinationType;
            dispatchFunction.DestinationAddress = destinationAddress;
            dispatchFunction.TokenId = tokenId;

            return ContractHandler.SendRequestAndWaitForReceiptAsync(dispatchFunction, cancellationToken);
        }

        public Task<string> GetCurrentOwnerAddressQueryAsync(GetCurrentOwnerAddressFunction getCurrentOwnerAddressFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetCurrentOwnerAddressFunction, string>(getCurrentOwnerAddressFunction, blockParameter);
        }


        public Task<string> GetCurrentOwnerAddressQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetCurrentOwnerAddressFunction, string>(null, blockParameter);
        }

        public Task<string> GetCurrentOwnerTypeQueryAsync(GetCurrentOwnerTypeFunction getCurrentOwnerTypeFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetCurrentOwnerTypeFunction, string>(getCurrentOwnerTypeFunction, blockParameter);
        }


        public Task<string> GetCurrentOwnerTypeQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetCurrentOwnerTypeFunction, string>(null, blockParameter);
        }

        public Task<GetPreviousOwnersOutputDTO> GetPreviousOwnersQueryAsync(GetPreviousOwnersFunction getPreviousOwnersFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<GetPreviousOwnersFunction, GetPreviousOwnersOutputDTO>(getPreviousOwnersFunction, blockParameter);
        }

        public Task<GetPreviousOwnersOutputDTO> GetPreviousOwnersQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<GetPreviousOwnersFunction, GetPreviousOwnersOutputDTO>(null, blockParameter);
        }

        public Task<string> GetShipmentStatusQueryAsync(GetShipmentStatusFunction getShipmentStatusFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetShipmentStatusFunction, string>(getShipmentStatusFunction, blockParameter);
        }


        public Task<string> GetShipmentStatusQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetShipmentStatusFunction, string>(null, blockParameter);
        }

        public Task<string> ApproveRequestAsync(ApproveFunction approveFunction)
        {
            return ContractHandler.SendRequestAsync(approveFunction);
        }

        public Task<TransactionReceipt> ApproveRequestAndWaitForReceiptAsync(ApproveFunction approveFunction, CancellationTokenSource cancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(approveFunction, cancellationToken);
        }

        public Task<string> ApproveRequestAsync(string to, BigInteger tokenId)
        {
            var approveFunction = new ApproveFunction();
            approveFunction.To = to;
            approveFunction.TokenId = tokenId;

            return ContractHandler.SendRequestAsync(approveFunction);
        }

        public Task<TransactionReceipt> ApproveRequestAndWaitForReceiptAsync(string to, BigInteger tokenId, CancellationTokenSource cancellationToken = null)
        {
            var approveFunction = new ApproveFunction();
            approveFunction.To = to;
            approveFunction.TokenId = tokenId;

            return ContractHandler.SendRequestAndWaitForReceiptAsync(approveFunction, cancellationToken);
        }

        public Task<BigInteger> BalanceOfQueryAsync(BalanceOfFunction balanceOfFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<BalanceOfFunction, BigInteger>(balanceOfFunction, blockParameter);
        }


        public Task<BigInteger> BalanceOfQueryAsync(string owner, BlockParameter blockParameter = null)
        {
            var balanceOfFunction = new BalanceOfFunction();
            balanceOfFunction.Owner = owner;

            return ContractHandler.QueryAsync<BalanceOfFunction, BigInteger>(balanceOfFunction, blockParameter);
        }

        public Task<string> GetApprovedQueryAsync(GetApprovedFunction getApprovedFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetApprovedFunction, string>(getApprovedFunction, blockParameter);
        }


        public Task<string> GetApprovedQueryAsync(BigInteger tokenId, BlockParameter blockParameter = null)
        {
            var getApprovedFunction = new GetApprovedFunction();
            getApprovedFunction.TokenId = tokenId;

            return ContractHandler.QueryAsync<GetApprovedFunction, string>(getApprovedFunction, blockParameter);
        }

        public Task<bool> IsApprovedForAllQueryAsync(IsApprovedForAllFunction isApprovedForAllFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<IsApprovedForAllFunction, bool>(isApprovedForAllFunction, blockParameter);
        }


        public Task<bool> IsApprovedForAllQueryAsync(string owner, string _operator, BlockParameter blockParameter = null)
        {
            var isApprovedForAllFunction = new IsApprovedForAllFunction();
            isApprovedForAllFunction.Owner = owner;
            isApprovedForAllFunction.Operator = _operator;

            return ContractHandler.QueryAsync<IsApprovedForAllFunction, bool>(isApprovedForAllFunction, blockParameter);
        }

        public Task<string> NameQueryAsync(NameFunction nameFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<NameFunction, string>(nameFunction, blockParameter);
        }


        public Task<string> NameQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<NameFunction, string>(null, blockParameter);
        }

        public Task<string> OwnerOfQueryAsync(OwnerOfFunction ownerOfFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<OwnerOfFunction, string>(ownerOfFunction, blockParameter);
        }


        public Task<string> OwnerOfQueryAsync(BigInteger tokenId, BlockParameter blockParameter = null)
        {
            var ownerOfFunction = new OwnerOfFunction();
            ownerOfFunction.TokenId = tokenId;

            return ContractHandler.QueryAsync<OwnerOfFunction, string>(ownerOfFunction, blockParameter);
        }

        public Task<string> SafeTransferFromRequestAsync(SafeTransferFromFunction safeTransferFromFunction)
        {
            return ContractHandler.SendRequestAsync(safeTransferFromFunction);
        }

        public Task<TransactionReceipt> SafeTransferFromRequestAndWaitForReceiptAsync(SafeTransferFromFunction safeTransferFromFunction, CancellationTokenSource cancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(safeTransferFromFunction, cancellationToken);
        }

        public Task<string> SafeTransferFromRequestAsync(string from, string to, BigInteger tokenId)
        {
            var safeTransferFromFunction = new SafeTransferFromFunction();
            safeTransferFromFunction.From = from;
            safeTransferFromFunction.To = to;
            safeTransferFromFunction.TokenId = tokenId;

            return ContractHandler.SendRequestAsync(safeTransferFromFunction);
        }

        public Task<TransactionReceipt> SafeTransferFromRequestAndWaitForReceiptAsync(string from, string to, BigInteger tokenId, CancellationTokenSource cancellationToken = null)
        {
            var safeTransferFromFunction = new SafeTransferFromFunction();
            safeTransferFromFunction.From = from;
            safeTransferFromFunction.To = to;
            safeTransferFromFunction.TokenId = tokenId;

            return ContractHandler.SendRequestAndWaitForReceiptAsync(safeTransferFromFunction, cancellationToken);
        }

        public Task<string> SafeTransferFromRequestAsync(SafeTransferFromDataFunction safeTransferFromFunction)
        {
            return ContractHandler.SendRequestAsync(safeTransferFromFunction);
        }

        public Task<TransactionReceipt> SafeTransferFromRequestAndWaitForReceiptAsync(SafeTransferFromDataFunction safeTransferFromFunction, CancellationTokenSource cancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(safeTransferFromFunction, cancellationToken);
        }

        public Task<string> SafeTransferFromRequestAsync(string from, string to, BigInteger tokenId, byte[] data)
        {
            var safeTransferFromFunction = new SafeTransferFromDataFunction();
            safeTransferFromFunction.From = from;
            safeTransferFromFunction.To = to;
            safeTransferFromFunction.TokenId = tokenId;
            safeTransferFromFunction.Data = data;

            return ContractHandler.SendRequestAsync(safeTransferFromFunction);
        }

        public Task<TransactionReceipt> SafeTransferFromRequestAndWaitForReceiptAsync(string from, string to, BigInteger tokenId, byte[] data, CancellationTokenSource cancellationToken = null)
        {
            var safeTransferFromFunction = new SafeTransferFromDataFunction();
            safeTransferFromFunction.From = from;
            safeTransferFromFunction.To = to;
            safeTransferFromFunction.TokenId = tokenId;
            safeTransferFromFunction.Data = data;

            return ContractHandler.SendRequestAndWaitForReceiptAsync(safeTransferFromFunction, cancellationToken);
        }

        public Task<string> SetApprovalForAllRequestAsync(SetApprovalForAllFunction setApprovalForAllFunction)
        {
            return ContractHandler.SendRequestAsync(setApprovalForAllFunction);
        }

        public Task<TransactionReceipt> SetApprovalForAllRequestAndWaitForReceiptAsync(SetApprovalForAllFunction setApprovalForAllFunction, CancellationTokenSource cancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(setApprovalForAllFunction, cancellationToken);
        }

        public Task<string> SetApprovalForAllRequestAsync(string _operator, bool approved)
        {
            var setApprovalForAllFunction = new SetApprovalForAllFunction();
            setApprovalForAllFunction.Operator = _operator;
            setApprovalForAllFunction.Approved = approved;

            return ContractHandler.SendRequestAsync(setApprovalForAllFunction);
        }

        public Task<TransactionReceipt> SetApprovalForAllRequestAndWaitForReceiptAsync(string _operator, bool approved, CancellationTokenSource cancellationToken = null)
        {
            var setApprovalForAllFunction = new SetApprovalForAllFunction();
            setApprovalForAllFunction.Operator = _operator;
            setApprovalForAllFunction.Approved = approved;

            return ContractHandler.SendRequestAndWaitForReceiptAsync(setApprovalForAllFunction, cancellationToken);
        }

        public Task<bool> SupportsInterfaceQueryAsync(SupportsInterfaceFunction supportsInterfaceFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<SupportsInterfaceFunction, bool>(supportsInterfaceFunction, blockParameter);
        }


        public Task<bool> SupportsInterfaceQueryAsync(byte[] interfaceId, BlockParameter blockParameter = null)
        {
            var supportsInterfaceFunction = new SupportsInterfaceFunction();
            supportsInterfaceFunction.InterfaceId = interfaceId;

            return ContractHandler.QueryAsync<SupportsInterfaceFunction, bool>(supportsInterfaceFunction, blockParameter);
        }

        public Task<string> SymbolQueryAsync(SymbolFunction symbolFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<SymbolFunction, string>(symbolFunction, blockParameter);
        }


        public Task<string> SymbolQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<SymbolFunction, string>(null, blockParameter);
        }

        public Task<string> TokenURIQueryAsync(TokenURIFunction tokenURIFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<TokenURIFunction, string>(tokenURIFunction, blockParameter);
        }


        public Task<string> TokenURIQueryAsync(BigInteger tokenId, BlockParameter blockParameter = null)
        {
            var tokenURIFunction = new TokenURIFunction();
            tokenURIFunction.TokenId = tokenId;

            return ContractHandler.QueryAsync<TokenURIFunction, string>(tokenURIFunction, blockParameter);
        }

        public Task<string> TransferFromRequestAsync(TransferFromFunction transferFromFunction)
        {
            return ContractHandler.SendRequestAsync(transferFromFunction);
        }

        public Task<TransactionReceipt> TransferFromRequestAndWaitForReceiptAsync(TransferFromFunction transferFromFunction, CancellationTokenSource cancellationToken = null)
        {
            return ContractHandler.SendRequestAndWaitForReceiptAsync(transferFromFunction, cancellationToken);
        }

        public Task<string> TransferFromRequestAsync(string from, string to, BigInteger tokenId)
        {
            var transferFromFunction = new TransferFromFunction();
            transferFromFunction.From = from;
            transferFromFunction.To = to;
            transferFromFunction.TokenId = tokenId;

            return ContractHandler.SendRequestAsync(transferFromFunction);
        }

        public Task<TransactionReceipt> TransferFromRequestAndWaitForReceiptAsync(string from, string to, BigInteger tokenId, CancellationTokenSource cancellationToken = null)
        {
            var transferFromFunction = new TransferFromFunction();
            transferFromFunction.From = from;
            transferFromFunction.To = to;
            transferFromFunction.TokenId = tokenId;

            return ContractHandler.SendRequestAndWaitForReceiptAsync(transferFromFunction, cancellationToken);
        }
    }
}

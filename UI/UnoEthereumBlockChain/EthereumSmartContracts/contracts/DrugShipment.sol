// SPDX-License-Identifier: MIT
pragma solidity >=0.5.12<=0.8.4;

import "@openzeppelin/contracts/token/ERC721/ERC721.sol";


contract DrugShipment is ERC721
{
    enum UserType { Manufacturer, Wholesaler, Pharmacy, Patient }
    enum Shipment { Created, Dispatched, Delivered, Dispensed, Consumed }
    struct User
    {
        UserType userType;
        address payable userAddress;        
    }

    // Parameters
    User private currentOwner;
    Shipment private shipmentStatus;
    address[] private previousOwnersAddress =  new address[](1);
    string[] private previousOwnersType =  new string[](1);
    uint256[] private tokenIds =  new uint256[](1);

    // Events
    event LogShipment(address source, address destination, string message);
    event LogDispensed(address source, address destination, string message);
    event LogConsumed(address patient, string message);
    event LogError(address source, string message);
    
    // Constructor : Creates New Drug Shipment
    constructor (string memory name, string memory symbol) ERC721(name, symbol)
    {

    }

    function Dispatch(UserType sourceType, address payable sourceAddress, UserType destinationType, address payable destinationAddress, uint256 tokenId) public 
    {
        Create(sourceType, sourceAddress, tokenId);
        if (super.ownerOf(tokenId) == sourceAddress)
        {
            if (sourceType != UserType.Patient) 
            {
                if(destinationType != UserType.Patient)
                {
                    shipmentStatus = Shipment.Dispatched;

                    super.safeTransferFrom(sourceAddress, destinationAddress, tokenId);
                    shipmentStatus = Shipment.Delivered;
                    currentOwner  = User(destinationType, destinationAddress);
                    previousOwnersAddress.push(currentOwner.userAddress);
                    previousOwnersType.push(GetUserType(currentOwner.userType));
                    tokenIds.push(tokenId);
                    emit LogShipment(sourceAddress, destinationAddress, "Shipment Delivered");

                    if(destinationType == UserType.Patient && sourceType == UserType.Pharmacy)
                    {
                        Dispensed(sourceType, sourceAddress, destinationType, destinationAddress ,tokenId);
                    }
                }
                else
                {
                    if(destinationType == UserType.Patient && sourceType == UserType.Pharmacy)
                    {
                        Dispensed(sourceType, sourceAddress, destinationType, destinationAddress ,tokenId);
                    }
                    else
                    {
                         emit LogError(sourceAddress, "User can't dispatch directly to a Patient");
                    }
                   
                }
            }
            else 
            {
                emit LogError(sourceAddress, "User can't Dispatch drugs, you are a Patient");
            }
        }
        else 
        {
            emit LogError(sourceAddress, "User can not Dispatch a Drug shipment");
        }
    }

    function Create (UserType  userType, address payable userAddress, uint256 shipmentId) private 
    {
        if(userType != UserType.Patient)
        {
            currentOwner = User(userType, userAddress);
            shipmentStatus = Shipment.Created;    
            previousOwnersAddress.push(currentOwner.userAddress);
            previousOwnersType.push(GetUserType(currentOwner.userType));
            tokenIds.push(shipmentId);

            super._safeMint(msg.sender, shipmentId);
            emit LogShipment(userAddress, userAddress, "Shipment Created");

        }            
        else
        {
            emit LogError(userAddress, "User can not create a Drug shipment");
            revert();
        } 
    }
    function Dispensed(UserType sourceType, address payable sourceAddress, UserType destinationType, address payable destinationAddress, uint256 tokenId) private
    {
        if(destinationType == UserType.Patient && sourceType == UserType.Pharmacy)
        {
            super.safeTransferFrom(sourceAddress, destinationAddress, tokenId);
            shipmentStatus = Shipment.Dispensed;
            currentOwner  = User(destinationType, destinationAddress);
            previousOwnersAddress.push(currentOwner.userAddress);
            previousOwnersType.push(GetUserType(currentOwner.userType));
            tokenIds.push(tokenId);
            emit LogDispensed(sourceAddress, destinationAddress, "Drug has been Dispensed");
        }
    }

    //Patient only 
    function Consume(UserType patient, address payable patientAddress, uint256 tokenId) public
    {
        if(super.ownerOf(tokenId) == patientAddress)
        {
            if(patient == UserType.Patient)
            {
                super._burn(tokenId);
                emit LogConsumed(patientAddress, "Patient has consumed Drug");
            }
            else
            {
                emit LogError(patientAddress, "You shouldn't get high of your own supply. You aren't a patient");
            }
        }   
        else
        {
            emit LogError(msg.sender, "You can't consume what you don't own");
        }
    }

    function GetCurrentOwnerType() public view returns (string memory)
    {
        if (currentOwner.userType == UserType.Manufacturer) return "Manufacturer";
        if (currentOwner.userType == UserType.Wholesaler) return "Wholesaler";
        if (currentOwner.userType == UserType.Pharmacy) return "Pharmacy";
        if (currentOwner.userType == UserType.Patient) return "Patient";
        else return "No Current Owner";
    }

    function GetCurrentOwnerAddress() public view returns (address)
    {
        return currentOwner.userAddress;
    }

    function GetShipmentStatus() public view returns (string memory)
    {
        if(shipmentStatus == Shipment.Consumed) return "Consumed";
        if(shipmentStatus == Shipment.Created) return "Created";
        if(shipmentStatus == Shipment.Delivered) return "Delivered";
        if(shipmentStatus == Shipment.Dispatched) return "Dispatched";
        if(shipmentStatus == Shipment.Dispensed) return "Dispensed";
        else return "Post Consumption";
    }

    function GetPreviousOwners() public view returns (address[] memory, string[] memory, uint256[] memory)
    {
        return (previousOwnersAddress, previousOwnersType, tokenIds);
    }

    function GetUserType(UserType _userType) private pure returns (string memory)
    {
        if (_userType == UserType.Manufacturer) return "Manufacturer";
        if (_userType == UserType.Wholesaler) return "Wholesaler";
        if (_userType == UserType.Pharmacy) return "Pharmacy";
        if (_userType == UserType.Patient) return "Patient";
        else return "No Current Owner";
    }
}

contract Wholesaler is DrugShipment
{
    // Constructor : Creates New Drug Shipment
    constructor (string memory name, string memory symbol) DrugShipment(name, symbol)
    {

    }
}

contract Pharmacy is DrugShipment
{
    // Constructor : Creates New Drug Shipment
    constructor (string memory name, string memory symbol) DrugShipment(name, symbol)
    {

    }
}
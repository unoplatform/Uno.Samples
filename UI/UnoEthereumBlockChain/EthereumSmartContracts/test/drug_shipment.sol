// SPDX-License-Identifier: MIT
pragma solidity >=0.5.12<=0.8.4;

import 'truffle/Assert.sol';
import 'truffle/DeployedAddresses.sol';
import '../contracts/DrugShipment.sol';

contract TestDrugShipment
{
    event LogString(string value);
    event LogAddress(address value);
    event LogInt(uint256 value);
    event LogUser(DrugShipment.User value);

    //Already Deployed Contract
    function DTestCurrentOwnerType() public
    {
        assert(true);    
    }



    // Undeployed Contract
    function TestCurrentOwnerType() public 
    {
        
        address payable manufacturer = payable(address (0x0F734A24b517C44543691ba2561286371A08d7dc));

        DrugShipment drugShipment = new DrugShipment(DrugShipment.UserType.Manufacturer, manufacturer, 2, "Astrazenca", "ATZ");

        Assert.notEqual(drugShipment.GetCurrentOwnerType(), "", "Current Owner should be Manufacturer.");
    }
    
}
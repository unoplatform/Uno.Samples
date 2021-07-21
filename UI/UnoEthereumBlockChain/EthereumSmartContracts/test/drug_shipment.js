const DrugShipment = artifacts.require("DrugShipment");
const assert = require("chai").assert;
const truffleAssert = require('truffle-assertions');
/*
 * uncomment accounts to access the test accounts made available by the
 * Ethereum client
 * See docs: https://www.trufflesuite.com/docs/truffle/testing/writing-tests-in-javascript
 */
contract("DrugShipment", function (accounts) {
  it("should assert true", async function () {
    await DrugShipment.deployed();

    console.log(accounts);

    return assert.isTrue(true);
  });

  // Get owner of token id
  it("Should check if owner of token id current address", async() =>
  {
    const instance = await DrugShipment.deployed();
    const owner = await instance.ownerOf(0001);
    console.log(owner);
    console.log(accounts[0]);
    assert.isTrue(owner == accounts[1]);
  });

  
  // Tests Current Owner Type Fxn
  it("Should assert Current owner is not equal to 'No Current Owner'", async function () {
    const instance = await DrugShipment.deployed();
    const currentOwner = await instance.GetCurrentOwnerType();
    
    console.log("Current Owner : ", currentOwner);
    assert.isNotTrue(currentOwner == "No Current Owner", currentOwner);
  });

  // Tests Shipment Status Fxn
  it("Should assert Shipment Status is not equal to 'Post Consumption'", async function () {
    const instance = await DrugShipment.deployed();
    const status = await instance.GetShipmentStatus();

    console.log("Shipment Status: ", status);

    assert.isNotTrue(status == "Post Consumption", status);
  });

  // Tests Dispatch Fxn
  it("Should emit 3  events '", async function () {
    const instance = await DrugShipment.deployed();
    const previousOwner = await instance.GetCurrentOwnerType();
    const dispatch = await instance.Dispatch(0, accounts[0], 1, accounts[1], 0001);
    const currentOwner = await instance.GetCurrentOwnerType();

    console.log("Previous Owner : ", previousOwner);
    console.log("Current Owner : ", currentOwner);
    
    truffleAssert.eventEmitted(dispatch, 'LogShipment');
    truffleAssert.eventEmitted(dispatch, 'Transfer');
    truffleAssert.prettyPrintEmittedEvents(dispatch);
    assert.isNotTrue(previousOwner == currentOwner);
  });
});

// truffle test ./test/drug_shipment.js

//truffle deploy --reset --network development
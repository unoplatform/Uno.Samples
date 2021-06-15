const DrugShipment = artifacts.require("DrugShipment");
const Wholesaler = artifacts.require("Wholesaler");
const Pharmacy = artifacts.require("Pharmacy");

module.exports = function (deployer) {
    //deployer.deploy(DrugShipment, 0, '0xCA382917d5B7C676FEFdBa5A98c95da4F91127D2', 0001, 'mordena', 'MOD');
    deployer.deploy(DrugShipment, 'mordena', 'MOD');
    deployer.deploy(Wholesaler, 'mordena', 'MOD');
   deployer.deploy(Pharmacy, 'mordena', 'MOD');
};
//0x4242C526c42c34e6688c12ef3750EB3ac4cDa349
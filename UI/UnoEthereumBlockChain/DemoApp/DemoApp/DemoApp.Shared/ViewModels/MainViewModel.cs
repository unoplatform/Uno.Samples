using Nethereum.Web3;
using Nethereum.Web3.Accounts;

using PharmaSupply.Contracts.Pharmacy;

namespace DemoApp.ViewModels
{

    #region HelperClasses & Enums

    public enum UserType
    {
        Manufacturer, Wholesaler, Pharmacy, Patient
    }

    public class AccountAddresses
    {
        public string one { get; set; }
        public string two { get; set; }
        public string three { get; set; }
        public string four { get; set; }
    }

    public class AccountKeys
    {
        public string one { get; set; }
        public string two { get; set; }
        public string three { get; set; }
        public string four { get; set; }
    }

    public class Accounts
    {
        public Account one { get; set; }
        public Account two { get; set; }
        public Account three { get; set; }
        public Account four { get; set; }
    }

    public class Web3s
    {
        public Web3 one { get; set; }
        public Web3 two { get; set; }
        public Web3 three { get; set; }
        public Web3 four { get; set; }
    }

    public class SetUp
    {
        public string DrugShipment { get; set; }
        public string Migrations { get; set; }

        public Accounts Accounts { get; set; }

        public Web3s Web3s { get; set; }
        public PharmacyService Service { get; set; }
    }

    #endregion

    public class MainViewModel
    {
        #region Properties

        public string DrugShipment { get; set; }
        public string Migrations { get; set; }
        public string Url { get; set; }
        public string NetworkId { get; set; }

        public AccountAddresses AccountAddress { get; set; }
        public AccountKeys AccountKeys { get; set; }

        public Accounts Accounts { get; set; }

        public Web3s Web3s { get; set; }

        #endregion

        public MainViewModel()
        {
            Url = "HTTP://127.0.0.1:7545";
            NetworkId = "5777";
            DrugShipment = "N/A";
            Migrations = "N/A";
            AccountAddress = new AccountAddresses();
            AccountKeys = new AccountKeys();
            Accounts = new Accounts();
            Web3s = new Web3s();
        }

        public SetUp Setup()
        {
            Accounts.one = new Account(AccountKeys.one);
            Accounts.two = new Account(AccountKeys.two);
            Accounts.three = new Account(AccountKeys.three);
            Accounts.four = new Account(AccountKeys.four);           

            Web3s.one = new Web3(Accounts.one, Url);
            Web3s.two = new Web3(Accounts.two, Url);
            Web3s.three = new Web3(Accounts.three, Url);
            Web3s.four = new Web3(Accounts.four, Url);
            
            return new SetUp { Accounts = Accounts, DrugShipment = DrugShipment,  Migrations = Migrations, Web3s = Web3s };
        }

    }
}

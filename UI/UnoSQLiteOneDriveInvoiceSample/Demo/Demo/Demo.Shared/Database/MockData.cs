using Demo.Database.Entities;
using Demo.Database.Enums;
using Bogus;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Bogus.Extensions.UnitedKingdom;

namespace Demo.Database
{
    public class MockData
    {
        public MockData()
        {
            AddressFaker = new Faker<Address>()
            .RuleFor(address => address.AddressOne, faker => faker.Address.StreetAddress())
            .RuleFor(address => address.AddressTwo, faker => faker.Address.SecondaryAddress())
            .RuleFor(address => address.City, faker => faker.Address.City())
            .RuleFor(address => address.State, faker => faker.Address.State())
            .RuleFor(address => address.Country, faker => faker.Address.Country())
            .RuleFor(address => address.PostalCode, faker => faker.Address.ZipCode())
            .RuleFor(address => address.IsUser, false)
            .RuleFor(address => address.Type, faker => faker.PickRandom<AddressType>());

            AccountFaker = new Faker<Account>()
            .CustomInstantiator(faker => new Account())
            .RuleFor(account => account.IsUser, false)
            .RuleFor(account => account.BankName, faker => $"{faker.Company.CompanyName()} BANK")
            .RuleFor(account => account.Holder, faker => faker.Person.FullName)
            .RuleFor(account => account.Number, faker => faker.Finance.Account())
            .RuleFor(account => account.Iban, faker => faker.Finance.Iban())
            .RuleFor(account => account.RoutingNumber, faker => faker.Finance.RoutingNumber())
            .RuleFor(account => account.SwiftNumber, faker => faker.Finance.SortCode())
            .RuleFor(account => account.Currency, faker => faker.Finance.Currency().Code)
            .RuleFor(account => account.Address, AddressFaker.Generate());
            
            CommunicationFaker = new Faker<Communication>()
            .RuleFor(communication => communication.HomeEmail, faker => faker.Person.Email)
            .RuleFor(communication => communication.WorkEmail, faker => faker.Person.Email)
            .RuleFor(communication => communication.HomePhone, faker => faker.Person.Phone)
            .RuleFor(communication => communication.WorkPhone, faker => faker.Phone.PhoneNumber())
            .RuleFor(communication => communication.Website, faker => faker.Person.Website)
            .RuleFor(communication => communication.IsUser, false);

            ClientFaker = new Faker<Client>()
            .RuleFor(client => client.Type, faker => faker.PickRandom<ClientType>())
            .RuleFor(client => client.Name, faker => faker.Company.CompanyName());


            UserAddressFaker = new Faker<Address>()
            .RuleFor(address => address.AddressOne, faker => faker.Address.StreetAddress())
            .RuleFor(address => address.AddressTwo, faker => faker.Address.SecondaryAddress())
            .RuleFor(address => address.City, faker => faker.Address.City())
            .RuleFor(address => address.State, faker => faker.Address.State())
            .RuleFor(address => address.Country, faker => faker.Address.Country())
            .RuleFor(address => address.PostalCode, faker => faker.Address.ZipCode())
            .RuleFor(address => address.IsUser, true)
            .RuleFor(address => address.Type, faker => faker.PickRandom<AddressType>());

            UserAccountFaker = new Faker<Account>()
            .RuleFor(account => account.IsUser, true)
            .RuleFor(account => account.BankName, faker => $"{faker.Company.CompanyName()} BANK")
            .RuleFor(account => account.Holder, faker => faker.Person.FullName)
            .RuleFor(account => account.Number, faker => faker.Finance.Account())
            .RuleFor(account => account.Iban, faker => faker.Finance.Iban())
            .RuleFor(account => account.RoutingNumber, faker => faker.Finance.RoutingNumber())
            .RuleFor(account => account.SwiftNumber, faker => faker.Finance.SortCode())
            .RuleFor(account => account.Currency, faker => faker.Finance.Currency().Code)
            .RuleFor(account => account.Address, UserAddressFaker.Generate());

            UserCommunicationFaker = new Faker<Communication>()
           .RuleFor(communication => communication.HomeEmail, faker => faker.Person.Email)
           .RuleFor(communication => communication.WorkEmail, faker => faker.Person.Email)
           .RuleFor(communication => communication.HomePhone, faker => faker.Person.Phone)
           .RuleFor(communication => communication.WorkPhone, faker => faker.Phone.PhoneNumber())
           .RuleFor(communication => communication.Website, faker => faker.Person.Website)
           .RuleFor(communication => communication.IsUser, true);

            UserAccount = UserAccountFaker.Generate();

            UserCommunication = UserCommunicationFaker.Generate();

            InvoiceFaker = new Faker<Invoice>()
            .RuleFor(invoice => invoice.Currency, faker => faker.Finance.Currency().Code)
            .RuleFor(invoice => invoice.IssueDate, faker => faker.Date.Recent())
            .RuleFor(invoice => invoice.DueDate, faker => faker.Date.Soon(30))
            .RuleFor(invoice => invoice.Status, faker => faker.PickRandom<InvoiceStatus>());


            ItemBlobFaker = new Faker<ItemBlob>()
           .RuleFor(itemBlob => itemBlob.Description, faker => faker.Commerce.ProductDescription())
           .RuleFor(itemBlob => itemBlob.Price, faker => Double.Parse(faker.Commerce.Price()))
           .RuleFor(itemBlob => itemBlob.ItemType, faker => faker.Commerce.ProductAdjective());           

        }

        public  Faker<Address> AddressFaker { get; set; }
        public  Faker<Account> AccountFaker { get; set; }

        public  Faker<Address> UserAddressFaker { get; set; }

        public  Faker<Account> UserAccountFaker { get; set; }

        public  Faker<ItemBlob> ItemBlobFaker { get; set; }

        public  Faker<Client> ClientFaker { get; set; }
        public  Faker<Communication> CommunicationFaker { get; set; }

        public  Faker<Communication> UserCommunicationFaker { get; set; }

        public  Faker<Invoice> InvoiceFaker { get; set; }

        public  Account UserAccount { get; set; }

        public  Communication UserCommunication { get; set; }



    }
}

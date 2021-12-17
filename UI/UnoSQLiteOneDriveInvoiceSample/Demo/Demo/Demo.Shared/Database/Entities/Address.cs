using Demo.Database.Enums;

using SQLite;
using SQLiteNetExtensions.Attributes;

using System;
using System.Collections.Generic;
using System.Text;

using Windows.Media.Protection.PlayReady;

namespace Demo.Database.Entities
{
    [Table("Address")]
    public class Address
    {
        #region Column(s)

        /// <summary>
        /// Unique identifer of Address record. (Required)
        /// </summary>
        [PrimaryKey, AutoIncrement]
        [Column("id"), NotNull]
        public Guid Id { get; set; }

        /// <summary>
        /// First line of the Address record. (Required)
        /// </summary>
        [Column("address_one")]
        [MaxLength(255), NotNull]
        public string AddressOne { get; set; }

        /// <summary>
        /// Second line of the Address record.
        /// </summary>
        [Column("address_two")]
        [MaxLength(255)]
        public string AddressTwo { get; set; }

        /// <summary>
        /// City/Village of the Address record. (Required)
        /// </summary>
        [Column("city")]
        [MaxLength(255), NotNull]
        public string City { get; set; }

        /// <summary>
        /// State/County of the Address record. 
        /// </summary>
        [Column("state")]
        [MaxLength(255)]
        public string State { get; set; }

        /// <summary>
        /// Country of the Address record. (Required)
        /// </summary>
        [Column("country")]
        [MaxLength(255), NotNull]
        public string Country { get; set; }

        /// <summary>
        /// Postal/Zip code of the Address record. (Required)
        /// </summary>
        [Column("postal_code")]
        [MaxLength(20), NotNull]
        public string PostalCode { get; set; }

        /// <summary>
        /// Flag to denote if Address record is User or Client. (Default false)
        /// </summary>
        [Column("is_user")]
        public bool IsUser { get; set; }

        /// <summary>
        /// Address Type - Billing or Shipping. (Required)
        /// </summary>
        [Column("address_type"), NotNull]
        public AddressType Type { get; set; }

        #endregion

        #region ForeignKey(s)

        /// <summary>
        /// Account Id attached to the Address record.
        /// </summary>
        [Column("account_id")]
        [ForeignKey(typeof(Account))]
        public Guid AccountId { get; set; }

        /// <summary>
        /// Client Id attached to a Address record.
        /// </summary>
        [Column("client_id")]
        [ForeignKey(typeof(Client))]
        public Guid ClientId { get; set; }

        #endregion

        #region OneToOne Linking(s)

        /// <summary>
        /// Account record attached to this Address record.
        /// </summary>
        [OneToOne(ReadOnly = true)]
        public Account Account { get; set; }

        /// <summary>
        /// Client record attached to this Address record.
        /// </summary>
        [OneToOne(ReadOnly = true)]
        public Client Client { get; set; }

        #endregion
    }
}

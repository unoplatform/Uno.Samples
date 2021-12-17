using SQLite;

using SQLiteNetExtensions.Attributes;

using System;

namespace Demo.Database.Entities
{
    [Table("Account")]
    public class Account
    {
        #region Column(s)

        /// <summary>
        /// Unique Identifier.  (Required)
        /// </summary>
        [PrimaryKey, AutoIncrement]
        [Column("id"), NotNull]
        public Guid Id { get; set; }

        /// <summary>
        /// Bank name of the Account record. (Required)
        /// </summary>
        [Column("bank_name"), NotNull]
        public string BankName { get; set; }

        /// <summary>
        /// Name of holder of the Account record. (Required)
        /// </summary>
        [Column("account_holder"), NotNull]
        public string Holder { get; set; }

        /// <summary>
        /// Account number of the Account record. (Required)
        /// </summary>
        [Column("account_number")]
        public string Number { get; set; }

        /// <summary>
        /// IBAN of the Account record.
        /// </summary>
        [Column("iban")]
        [MaxLength(30)]
        public string Iban { get; set; }

        /// <summary>
        /// Routing Number of Account record.
        /// </summary>
        [Column("routing_number")]
        [MaxLength(9)]
        public string RoutingNumber { get; set; }

        /// <summary>
        /// Swift Number of the Account record.
        /// </summary>
        [Column("swift_number")]
        [MaxLength(11)]
        public string SwiftNumber { get; set; }

        /// <summary>
        /// Currency of the Account record.
        /// </summary>
        [Column("currency"), NotNull]
        [MaxLength(4)]
        public string Currency { get; set; }

        /// <summary>
        /// Flag denoting if the Account record belongs to User or CLient. (Default false)
        /// </summary>
        [Column("is_user")]
        public bool IsUser { get; set; }



        #endregion
        #region ForeignKey(s)
        /// <summary>
        /// Client id attached to the Account record.
        /// </summary>
        [Column("client_id")]
        [ForeignKey(typeof(Client))]
        public Guid ClientId { get; set; }


        #endregion
        #region OneToOne Linking(s)
        /// <summary>
        /// Address details of the Account record.
        /// </summary>
        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public Address Address { get; set; }

        /// <summary>
        /// Client details of the Account record.
        /// </summary>
        [OneToOne(ReadOnly = true)]
        public Client Client { get; set; }

        #endregion
    }

    
}

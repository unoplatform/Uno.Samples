using Demo.Database.Enums;

using SQLite;
using SQLiteNetExtensions.Attributes;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Demo.Database.Entities
{
    [Table("Invoice")]
    public class Invoice
    {
        #region Column(s)

        /// <summary>
        /// Unique Identifier of Invoice record. (Required)
        /// </summary>
        [PrimaryKey, AutoIncrement]
        [Column("id"), NotNull]
        public Guid Id { get; set; }

        /// <summary>
        /// Currency of the Invoice record. (Required)
        /// </summary>
        [Column("currency"), NotNull]
        [MaxLength(4)]
        public string Currency { get; set; }

        /// <summary>
        /// Date and time Invoice record was created. (Required)
        /// </summary>
        [Column("issue_date"), NotNull]
        public DateTime IssueDate { get; set; }

        /// <summary>
        /// Date and Time Invoice record is due to be paid. (Required)
        /// </summary>
        [Column("due_date"), NotNull]
        public DateTime DueDate { get; set; }

        /// <summary>
        /// Status of the Invoice record. (Required)
        /// </summary>
        [Column("invoice_status"), NotNull]
        public InvoiceStatus Status { get; set; }

        #endregion

        #region ForeignKey(s)

        /// <summary>
        /// User Id of Human attached to the Invoice record.
        /// </summary>
        [Column("user"), NotNull]
        public string FullName { get; set; }

        /// <summary>
        /// User Address Id of Human attached to the Invoice record.
        /// </summary>
        [Column("user_address")]
        [ForeignKey(typeof(Address)), NotNull]
        public Guid UserAddressId { get; set; }

        /// <summary>
        /// User Bank Account Id of Human attached to the Invoice record.
        /// </summary>
        [Column("bank_account")]
        [ForeignKey(typeof(Account)), NotNull]
        public Guid UserBankAccountId { get; set; }

        /// <summary>
        /// Client Id attached to the Invoice record.
        /// </summary>
        [Column("client")]
        [ForeignKey(typeof(Client)), NotNull]
        public Guid ClientId { get; set; }

        #endregion

        #region ManyToOne Linking(s)

        /// <summary>
        /// User Address details attached to the Invoice record.
        /// </summary>
        [ManyToOne]
        public Address UserAddress { get; set; }

        /// <summary>
        /// User bank account details attached to the Invoice record.
        /// </summary>
        [ManyToOne]
        public Account UserBankAccount { get; set; }

        /// <summary>
        /// Client details attached to the Invoice record.
        /// </summary>
        [ManyToOne]
        public Client Client { get; set; }

        #endregion

        #region TextBlob Linking(s)

        /// <summary>
        /// Collection of Items attached to the Invoice record.
        /// </summary>
        [TextBlob("ItemsBlob")]
        public ObservableCollection<ItemBlob> Items { get; set; }

        public string ItemsBlob { get; set; }
        #endregion

    }
}

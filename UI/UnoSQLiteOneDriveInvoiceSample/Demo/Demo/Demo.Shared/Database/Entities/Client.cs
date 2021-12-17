
using Demo.Database.Enums;

using SQLite;
using SQLiteNetExtensions.Attributes;

using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Database.Entities
{
    [Table("Client")]
    public class Client
    {
        #region Column(s)

        /// <summary>
        /// Unique Identifier of Client record. (Required)
        /// </summary>
        [PrimaryKey, AutoIncrement]
        [Column("id"), NotNull]
        public Guid Id { get; set; }

        /// <summary>
        /// Type of Client - Individual, Small, Medium, Large.  (Required)
        /// </summary>
        [Column("client_type"), NotNull]
        public ClientType Type { get; set; }

        /// <summary>
        /// Name of Client record.
        /// </summary>
        [Column("name"), NotNull]
        [MaxLength(255)]
        public string Name { get; set; }





        #endregion

        #region ForeignKey(s)
        #endregion

        #region OneToOne Linking(s)
        /// <summary>
        /// Communication details attached to this Client record.
        /// </summary>
        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public Communication Communication { get; set; }

        /// <summary>
        /// Account details attached to this Client record.
        /// </summary>
        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public Account BankAccount { get; set; }

        /// <summary>
        /// Billing Address details attached to this Client record.
        /// </summary>
        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public Address BillingAddress { get; set; }

        #endregion
    }
}

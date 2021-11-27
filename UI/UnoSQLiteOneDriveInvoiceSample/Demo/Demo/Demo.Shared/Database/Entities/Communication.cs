using SQLite;
using SQLiteNetExtensions.Attributes;

using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Database.Entities
{
    [Table("Communication")]
    public class Communication
    {
        #region Column(s)

        /// <summary>
        /// Unique Identifier. (Required)
        /// </summary>
        [PrimaryKey, AutoIncrement]
        [Column("id"), NotNull]
        public Guid Id { get; set; }

        /// <summary>
        /// Home email of Communication record.
        /// </summary>
        [Column("home_email")]
        [MaxLength(255)]
        public string HomeEmail { get; set; }

        /// <summary>
        /// Work email of Communication record.  (Required)
        /// </summary>
        [Column("work_email"), NotNull]
        [MaxLength(255)]
        public string WorkEmail { get; set; }

        /// <summary>
        /// Home phone of Communication record.
        /// </summary>
        [Column("home_phone")]
        [MaxLength(255)]
        public string HomePhone { get; set; }

        /// <summary>
        /// Work phone of Communication record.  (Required)
        /// </summary>
        [Column("work_phone"), NotNull]
        [MaxLength(255)]
        public string WorkPhone { get; set; }

        /// <summary>
        /// Website of Communication record.
        /// </summary>
        [Column("website")]
        [MaxLength(255)]
        public string Website { get; set; }

        /// <summary>
        /// Flag to denote if Communication record belongs to User(Human) or Client. (Default is false)
        /// </summary>
        [Column("is_user")]
        public bool IsUser { get; set; }

        #endregion

        #region ForeignKey(s)        

        /// <summary>
        /// Client unique identifier for Communication record.
        /// </summary>
        [Column("client_id")]
        [ForeignKey(typeof(Client))]
        public Guid ClientId { get; set; }



        #endregion

        #region OneToOne Linking(s)        

        /// <summary>
        /// Client owner of Communication record.
        /// </summary>
        [Column("client")]
        [OneToOne(ReadOnly = true)]
        public Client Client { get; set; }

        #endregion    
    }
}

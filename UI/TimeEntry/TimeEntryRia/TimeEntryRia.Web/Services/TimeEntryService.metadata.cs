
namespace TimeEntryRia.Web
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.ServiceModel.DomainServices.Hosting;
    using System.ServiceModel.DomainServices.Server;


    // The MetadataTypeAttribute identifies TimeEntryMetadata as the class
    // that carries additional metadata for the TimeEntry class.
    [MetadataTypeAttribute(typeof(TimeEntry.TimeEntryMetadata))]
    public partial class TimeEntry
    {

        // This class allows you to attach custom attributes to properties
        // of the TimeEntry class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class TimeEntryMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private TimeEntryMetadata()
            {
            }

            [Required]
            public DateTime Date { get; set; }

            [Required]
            [Range(0.5, 24.0)]
            public double Hours { get; set; }

            [Key]
            [Editable(false)]
            public int Id { get; set; }

            public byte[] LastUpdated { get; set; }

            public Project Project { get; set; }

            [Required]
            public int ProjectId { get; set; }

            public TimeEntryUser TimeEntryUser { get; set; }

            [Required]
            public int UserId { get; set; }
        }
    }
}

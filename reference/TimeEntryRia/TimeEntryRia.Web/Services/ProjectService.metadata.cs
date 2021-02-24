
namespace TimeEntryRia.Web
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Objects.DataClasses;
    using System.Linq;
    using System.ServiceModel.DomainServices.Hosting;
    using System.ServiceModel.DomainServices.Server;


    // The MetadataTypeAttribute identifies ProjectMetadata as the class
    // that carries additional metadata for the Project class.
    [MetadataTypeAttribute(typeof(Project.ProjectMetadata))]
    public partial class Project
    {

        // This class allows you to attach custom attributes to properties
        // of the Project class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class ProjectMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private ProjectMetadata()
            {
            }

            [Key]
            [Editable(false)]
            [Display(Name = "Project ID")]
            public int Id { get; set;} 

            [Editable(false)]
            public byte[] LastUpdated { get; set; }

            [Required]
            [StringLength(50)]
            [Display(Name="Project Name")]
            public string Name { get; set; }

            public EntityCollection<TimeEntry> TimeEntries { get; set; }
        }
    }
}

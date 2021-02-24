using System;
using System.Collections.Generic;

#nullable disable

namespace TimeEntryApi.Models
{
    public partial class Project
    {
        public Project()
        {
            TimeEntries = new HashSet<TimeEntry>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] LastUpdated { get; set; }

        public virtual ICollection<TimeEntry> TimeEntries { get; set; }
    }
}

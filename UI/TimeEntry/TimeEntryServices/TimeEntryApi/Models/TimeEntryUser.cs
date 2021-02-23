using System;
using System.Collections.Generic;

#nullable disable

namespace TimeEntryApi.Models
{
    public partial class TimeEntryUser
    {
        public TimeEntryUser()
        {
            TimeEntries = new HashSet<TimeEntry>();
        }

        public int Id { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public int RoleId { get; set; }
        public byte[] LastUpdated { get; set; }

        public virtual Role Role { get; set; }
        public virtual Security Security { get; set; }
        public virtual ICollection<TimeEntry> TimeEntries { get; set; }
    }
}

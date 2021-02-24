using System;
using System.Collections.Generic;

#nullable disable

namespace TimeEntryApi.Models
{
    public partial class Role
    {
        public Role()
        {
            TimeEntryUsers = new HashSet<TimeEntryUser>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] LastUpdated { get; set; }

        public virtual ICollection<TimeEntryUser> TimeEntryUsers { get; set; }
    }
}

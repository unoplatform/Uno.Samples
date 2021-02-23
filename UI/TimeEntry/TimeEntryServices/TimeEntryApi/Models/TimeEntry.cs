using System;
using System.Collections.Generic;

#nullable disable

namespace TimeEntryApi.Models
{
    public partial class TimeEntry
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProjectId { get; set; }
        public DateTime Date { get; set; }
        public double Hours { get; set; }
        public byte[] LastUpdated { get; set; }

        public virtual Project Project { get; set; }
        public virtual TimeEntryUser User { get; set; }
    }
}

using System;
using System.Collections.Generic;

#nullable disable

namespace TimeEntryApi.Models
{
    public partial class Security
    {
        public int UserId { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public byte[] LastUpdated { get; set; }

        public virtual TimeEntryUser User { get; set; }
    }
}

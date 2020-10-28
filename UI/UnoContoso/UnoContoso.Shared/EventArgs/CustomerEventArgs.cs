using System;
using System.Collections.Generic;
using System.Text;
using UnoContoso.Enums;
using UnoContoso.Models;

namespace UnoContoso.EventArgs
{
    public class CustomerEventArgs
    {
        public EntityChanges Changes { get; set; }

        public Customer Customer { get; set; }

        public Guid CustomerId { get; set; }
    }
}

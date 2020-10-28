using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;
using UnoContoso.EventArgs;

namespace UnoContoso.Events
{
    public class CustomerEvent : PubSubEvent<CustomerEventArgs>
    {
    }
}

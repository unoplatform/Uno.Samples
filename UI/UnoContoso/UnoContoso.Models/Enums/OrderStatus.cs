using System;
using System.Collections.Generic;
using System.Text;

namespace UnoContoso.Models
{
    /// <summary>
    /// Represents the status of an order.
    /// </summary>
    public enum OrderStatus
    {
        Open,
        Filled,
        Cancelled
    }
}

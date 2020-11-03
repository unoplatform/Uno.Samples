using System;
using System.Collections.Generic;
using System.Text;

namespace UnoContoso.Model
{
    /// <summary>
    /// Represents an exception that occurs when there's an error saving an order.
    /// </summary>
    public class OrderSavingException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the OrderSavingException class with a default error message.
        /// </summary>
        public OrderSavingException() : base("Error saving an order.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the OrderSavingException class with the specified error message.
        /// </summary>
        public OrderSavingException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the OrderSavingException class with 
        /// the specified error message and inner exception.
        /// </summary>
        public OrderSavingException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

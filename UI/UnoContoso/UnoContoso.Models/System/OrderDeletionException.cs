using System;
using System.Collections.Generic;
using System.Text;

namespace UnoContoso.Model
{
    /// <summary>
    /// Represents an exception that occurs when there's an error deleting an order.
    /// </summary>
    public class OrderDeletionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the OrderDeletionException class with a default error message.
        /// </summary>
        public OrderDeletionException() : base("Error deleting an order.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the OrderDeletionException class with the specified error message.
        /// </summary>
        public OrderDeletionException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the OrderDeletionException class with 
        /// the specified error message and inner exception.
        /// </summary>
        public OrderDeletionException(string message,
            Exception innerException) : base(message, innerException)
        {
        }
    }
}

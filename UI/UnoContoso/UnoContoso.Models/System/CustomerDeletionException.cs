using System;
using System.Collections.Generic;
using System.Text;

namespace UnoContoso.Models
{
    public class CustomerDeletionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the CustomerDeletionException class with a default error message.
        /// </summary>
        public CustomerDeletionException() : base("Error deleting a customer.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the CustomerDeletionException class with the specified error message.
        /// </summary>
        public CustomerDeletionException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CustomerDeletionException class with 
        /// the specified error message and inner exception.
        /// </summary>
        public CustomerDeletionException(string message,
            Exception innerException) : base(message, innerException)
        {
        }
    }
}

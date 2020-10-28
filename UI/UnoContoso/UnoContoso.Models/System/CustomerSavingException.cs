using System;
using System.Collections.Generic;
using System.Text;

namespace UnoContoso.Models
{
    public class CustomerSavingException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the CustomerSavingException class with a default error message.
        /// </summary>
        public CustomerSavingException() : base("Error saving a customer.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the CustomerSavingException class with the specified error message.
        /// </summary>
        public CustomerSavingException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CustomerSavingException class with 
        /// the specified error message and inner exception.
        /// </summary>
        public CustomerSavingException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;
using UnoContoso.Models;

namespace UnoContoso.Model
{
    /// <summary>
    /// Provides a bindable wrapper for the LineItem model class.
    /// </summary>
    public class LineItemWrapper : BindableBase
    {
        /// <summary>
        /// Initializes a new instance of the LineItemWrapper class that wraps a LineItem object.
        /// </summary>
        public LineItemWrapper(LineItem model = null) => Model = model ?? new LineItem();

        /// <summary>
        /// Gets the underlying LineItem object.
        /// </summary>
        public LineItem Model { get; }

        /// <summary>
        /// Gets or sets the product for the line item.
        /// </summary>
        public Product Product
        {
            get => Model.Product;
            set
            {
                if (Model.Product != value)
                {
                    Model.Product = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the product quantity for the line item.
        /// </summary>
        public int Quantity
        {
            get => Model.Quantity;
            set
            {
                if (Model.Quantity != value)
                {
                    Model.Quantity = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}

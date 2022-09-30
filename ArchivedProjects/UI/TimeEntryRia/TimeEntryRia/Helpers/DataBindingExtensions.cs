namespace TimeEntryRia
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// Provides extension methods for dealing with <see cref="Binding"/> objects
    /// </summary>
    public static class DataBindingExtensions
    {
        /// <summary>
        /// Creates a new <see cref="Binding"/> using <paramref name="bindingSource"/> as the <see cref="Binding.Source"/>
        /// and <paramref name="propertyPath"/> as the <see cref="Binding.Path"/>.
        /// </summary>
        /// <param name="bindingSource">The object to use as the new binding's <see cref="Binding.Source"/>.</param>
        /// <param name="propertyPath">The property path to use as the new binding's <see cref="Binding.Path"/>.</param>
        /// <returns>A new <see cref="Binding"/> object.</returns>
        public static Binding CreateOneWayBinding(this INotifyPropertyChanged bindingSource, string propertyPath)
        {
            return bindingSource.CreateOneWayBinding(propertyPath, null);
        }

        /// <summary>
        /// Creates a new <see cref="Binding"/> using <paramref name="bindingSource"/> as the <see cref="Binding.Source"/>,
        /// <paramref name="propertyPath"/> as the <see cref="Binding.Path"/>,
        /// and <paramref name="converter"/> as the <see cref="Binding.Converter"/>.
        /// </summary>
        /// <param name="bindingSource">The object to use as the new binding's <see cref="Binding.Source"/>.</param>
        /// <param name="propertyPath">The property path to use as the new binding's <see cref="Binding.Path"/>.</param>
        /// <param name="converter">The converter to use as the new binding's <see cref="Binding.Converter"/>.</param>
        /// <returns>A new <see cref="Binding"/> object.</returns>
        public static Binding CreateOneWayBinding(this INotifyPropertyChanged bindingSource, string propertyPath, IValueConverter converter)
        {
            Binding binding = new Binding();

            binding.Source = bindingSource;
            binding.Path = new PropertyPath(propertyPath);
            binding.Converter = converter;

            return binding;
        }

        /// <summary>
        /// Creates a new <see cref="Binding"/> object by copying all properties
        /// from another <see cref="Binding"/> object.
        /// </summary>
        /// <param name="binding"><see cref="Binding"/> from which property values will be copied</param>
        /// <returns>A new <see cref="Binding"/> object.</returns>
        public static Binding CreateCopy(this Binding binding)
        {
            if (binding == null)
            {
                throw new ArgumentNullException("binding");
            }

            Binding newBinding = new Binding()
            {
                BindsDirectlyToSource = binding.BindsDirectlyToSource,
                Converter = binding.Converter,
                ConverterParameter = binding.ConverterParameter,
                ConverterCulture = binding.ConverterCulture,
                Mode = binding.Mode,
                NotifyOnValidationError = binding.NotifyOnValidationError,
                Path = binding.Path,
                UpdateSourceTrigger = binding.UpdateSourceTrigger,
                ValidatesOnExceptions = binding.ValidatesOnExceptions
            };

            if (binding.ElementName != null)
            {
                newBinding.ElementName = binding.ElementName;
            }
            else if (binding.RelativeSource != null)
            {
                newBinding.RelativeSource = binding.RelativeSource;
            }
            else
            {
                newBinding.Source = binding.Source;
            }

            return newBinding;
        }
    }
}
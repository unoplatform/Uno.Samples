using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;

namespace TimeEntryRia.MVVM
{
    /// <summary>
    /// This class was presented in the book:
    /// Pro Business Applications with Silverlight 5 by Chris Anderson (Apress, 2012).
    /// https://www.apress.com/us/book/9781430235002
    /// </summary>
    public class ViewModelBase : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        private bool _isBusy;
        private Dictionary<string, List<string>> _validationErrors = new Dictionary<string, List<string>>();

        #region INotifyPropertyChanged
        /// <summary>
        /// Raise the PropertyChanged event for the 
        /// specified property.
        /// </summary>
        /// <param name="propertyName">
        /// A string representing the name of 
        /// the property that changed.</param>
        /// <remarks>
        /// Only raise the event if the value of the property 
        /// has changed from its previous value</remarks>
        protected void OnPropertyChanged(string propertyName)
        {
            // Validate the property name in debug builds
            VerifyProperty(propertyName);

            if (null != PropertyChanged)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Verifies whether the current class provides a property with a given
        /// name. This method is only invoked in debug builds, and results in
        /// a runtime exception if the <see cref="OnPropertyChanged"/> method
        /// is being invoked with an invalid property name. This may happen if
        /// a property's name was changed but not the parameter of the property's
        /// invocation of <see cref="OnPropertyChanged"/>.
        /// </summary>
        /// <param name="propertyName">The name of the changed property.</param>
        [System.Diagnostics.Conditional("DEBUG")]
        private void VerifyProperty(string propertyName)
        {
            Type type = this.GetType();

            // Look for a *public* property with the specified name
            System.Reflection.PropertyInfo pi = type.GetProperty(propertyName);
            if (pi == null)
            {
                // There is no matching property - notify the developer
                string msg = "OnPropertyChanged was invoked with invalid " +
                                "property name {0}. {0} is not a public " +
                                "property of {1}.";
                msg = String.Format(msg, propertyName, type.FullName);
                System.Diagnostics.Debug.Assert(1 != 1, msg);
            }
        }

        private string GetPropertyNameFromExpression<T>(Expression<Func<T>> property)
        {
            var lambda = (LambdaExpression)property;
            MemberExpression memberExpression;

            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = (UnaryExpression)lambda.Body;
                memberExpression = (MemberExpression)unaryExpression.Operand;
            }
            else
            {
                memberExpression = (MemberExpression)lambda.Body;
            }

            return memberExpression.Member.Name;
        }

        protected void OnPropertyChanged<T>(Expression<Func<T>> property)
        {
            OnPropertyChanged(GetPropertyNameFromExpression(property));
        }

        protected bool SetPropertyValue<T>(ref T backingField, T value, Expression<Func<T>> propertyDelegate)
        {
            if (Object.Equals(backingField, value))
            {
                return false;
            }
            backingField = value;

            ValidateProperty(propertyDelegate, value);
            OnPropertyChanged(propertyDelegate);

            return true;
        }
        #endregion

        public IEnumerable GetErrors(string propertyName)
        {
            List<string> errors = null;

            if (_validationErrors.ContainsKey(propertyName))
                errors = _validationErrors[propertyName];

            return errors;
        }

        public bool HasErrors
        {
            get { return _validationErrors.Count != 0; }
        }

        protected bool ValidateProperty<T>(Expression<Func<T>> property, object value)
        {
            return ValidateProperty(GetPropertyNameFromExpression(property), value);
        }

        protected virtual bool ValidateProperty(string propertyName)
        {
            PropertyInfo propInfo = this.GetType().GetProperty(propertyName);
            object value = propInfo.GetValue(this, null);

            return ValidateProperty(propertyName, value);
        }

        protected virtual bool ValidateProperty(string propertyName, object value)
        {
            // Validate a property based upon its validation attributes
            List<ValidationResult> validationResults = new List<ValidationResult>();

            ValidationContext validationContext = new ValidationContext(this, null, null);
            validationContext.MemberName = propertyName;

            bool isValid = Validator.TryValidateProperty(value, validationContext, validationResults);

            List<string> errors = new List<string>();

            foreach (ValidationResult result in validationResults)
                errors.Add(result.ErrorMessage);

            if (errors.Count == 0)
            {
                if (_validationErrors.ContainsKey(propertyName))
                    _validationErrors.Remove(propertyName);
            }
            else
            {
                _validationErrors[propertyName] = errors;
            }

            OnErrorsChanged(propertyName);

            return isValid;
        }

        public virtual bool Validate()
        {
            // Validate this object based upon its validation attributes
            ValidationContext validationContext = new ValidationContext(this, null, null);
            List<ValidationResult> validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(this, validationContext, validationResults, true);

            _validationErrors.Clear();

            foreach (ValidationResult result in validationResults)
            {
                foreach (string propertyName in result.MemberNames)
                {
                    List<string> errors = null;

                    if (_validationErrors.ContainsKey(propertyName))
                    {
                        errors = _validationErrors[propertyName];
                    }
                    else
                    {
                        errors = new List<string>();
                        _validationErrors[propertyName] = errors;
                    }

                    errors.Add(result.ErrorMessage);
                    OnErrorsChanged(propertyName);
                }
            }

            return isValid;
        }

        protected void OnErrorsChanged(string propertyName)
        {
            if (ErrorsChanged != null)
                ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                SetPropertyValue(ref _isBusy, value, () => IsBusy);
            }
        }

        public bool IsDesignTimeMode
        {
            get { return DesignerProperties.IsInDesignTool; }
        }

        public event EventHandler<ErrorEventArgs> NotifyError;

        protected void OnNotifyError(string errorMessage)
        {
            if (NotifyError != null)
                NotifyError(this, new ErrorEventArgs(errorMessage));
        }
    }

    public class ErrorEventArgs : EventArgs
    {
        public string ErrorMessage { get; set; }

        public ErrorEventArgs(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }
    }
}

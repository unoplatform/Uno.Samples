namespace TimeEntryRia.Web
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ServiceModel.DomainServices.Client;
    using System.ServiceModel.DomainServices.Client.ApplicationServices;
    using TimeEntryRia.Web.Resources;

    /// <summary>
    /// Extensions to provide client side custom validation and data binding to <see cref="RegistrationData"/>.
    /// </summary>
    public partial class RegistrationData
    {
        private OperationBase currentOperation;

        /// <summary>
        /// Gets or sets a function that returns the password.
        /// </summary>
        internal Func<string> PasswordAccessor { get; set; }

        /// <summary>
        /// Gets and sets the password.
        /// </summary>
        [Required(ErrorMessageResourceName = "ValidationErrorRequiredField", ErrorMessageResourceType = typeof(ValidationErrorResources))]
        [Display(Order = 3, Name = "PasswordLabel", Description = "PasswordDescription", ResourceType = typeof(RegistrationDataResources))]
        [RegularExpression("^.*[^a-zA-Z0-9].*$", ErrorMessageResourceName = "ValidationErrorBadPasswordStrength", ErrorMessageResourceType = typeof(ValidationErrorResources))]
        [StringLength(50, MinimumLength = 7, ErrorMessageResourceName = "ValidationErrorBadPasswordLength", ErrorMessageResourceType = typeof(ValidationErrorResources))]
        public string Password
        {
            get
            {
                return (this.PasswordAccessor == null) ? string.Empty : this.PasswordAccessor();
            }

            set
            {
                this.ValidateProperty("Password", value);
                this.CheckPasswordConfirmation();

                // Do not store the password in a private field as it should
                // not be stored in memory in plain-text.  Instead, the supplied
                // PasswordAccessor serves as the backing store for the value.

                this.RaisePropertyChanged("Password");
            }
        }

        /// <summary>
        /// Gets or sets a function that returns the password confirmation.
        /// </summary>
        internal Func<string> PasswordConfirmationAccessor { get; set; }

        /// <summary>
        /// Gets and sets the password confirmation string.
        /// </summary>
        [Required(ErrorMessageResourceName = "ValidationErrorRequiredField", ErrorMessageResourceType = typeof(ValidationErrorResources))]
        [Display(Order = 4, Name = "PasswordConfirmationLabel", ResourceType = typeof(RegistrationDataResources))]
        public string PasswordConfirmation
        {
            get
            {
                return (this.PasswordConfirmationAccessor == null) ? string.Empty : this.PasswordConfirmationAccessor();
            }

            set
            {
                this.ValidateProperty("PasswordConfirmation", value);
                this.CheckPasswordConfirmation();

                // Do not store the password in a private field as it should
                // not be stored in memory in plain-text.  Instead, the supplied
                // PasswordAccessor serves as the backing store for the value.

                this.RaisePropertyChanged("PasswordConfirmation");
            }
        }

        /// <summary>
        /// Gets or sets the current registration or login operation.
        /// </summary>
        internal OperationBase CurrentOperation
        {
            get
            {
                return this.currentOperation;
            }
            set
            {
                if (this.currentOperation != value)
                {
                    if (this.currentOperation != null)
                    {
                        this.currentOperation.Completed -= (s, e) => this.CurrentOperationChanged();
                    }

                    this.currentOperation = value;

                    if (this.currentOperation != null)
                    {
                        this.currentOperation.Completed += (s, e) => this.CurrentOperationChanged();
                    }

                    this.CurrentOperationChanged();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the user is presently being registered or logged in.
        /// </summary>
        [Display(AutoGenerateField = false)]
        public bool IsRegistering
        {
            get
            {
                return this.CurrentOperation != null && !this.CurrentOperation.IsComplete;
            }
        }

        /// <summary>
        /// Helper method for when the current operation changes.
        /// Used to raise appropriate property change notifications.
        /// </summary>
        private void CurrentOperationChanged()
        {
            this.RaisePropertyChanged("IsRegistering");
        }

        /// <summary>
        /// Checks to ensure the password and confirmation match.  If they don't match,
        /// then a validation error is added.
        /// </summary>
        private void CheckPasswordConfirmation()
        {
            // If either of the passwords has not yet been entered, then don't test for equality between
            // the fields.  The Required attribute will ensure a value has been entered for both fields.
            if (string.IsNullOrWhiteSpace(this.Password)
                || string.IsNullOrWhiteSpace(this.PasswordConfirmation))
            {
                return;
            }

            // If the values are different, then add a validation error with both members specified
            if (this.Password != this.PasswordConfirmation)
            {
                this.ValidationErrors.Add(new ValidationResult(ValidationErrorResources.ValidationErrorPasswordConfirmationMismatch, new string[] { "PasswordConfirmation", "Password" }));
            }
        }

        /// <summary>
        /// Perform logic after the UserName value has been entered
        /// </summary>
        /// <param name="userName">The user name value that was entered.</param>
        /// <remarks>
        /// Allow the form to indicate when the value has been completely entered, because
        /// using the OnUserNameChanged method can lead to a premature call before the user
        /// has finished entering the value in the form.
        /// </remarks>
        internal void UserNameEntered(string userName)
        {
            // Auto-Fill FriendlyName to match UserName for new entities when there is not a friendly name specified
            if ((this.EntityState == EntityState.New || this.EntityState == EntityState.Detached)
                && string.IsNullOrWhiteSpace(this.FriendlyName))
            {
                this.FriendlyName = userName;
            }
        }

        /// <summary>
        /// Creates a new <see cref="LoginParameters"/>
        /// initialized with this entity's data (IsPersistent will default to false)
        /// </summary>
        public LoginParameters ToLoginParameters()
        {
            return new LoginParameters(this.UserName, this.Password, false, null);
        }
    }
}

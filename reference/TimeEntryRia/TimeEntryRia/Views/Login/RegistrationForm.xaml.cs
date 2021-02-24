namespace TimeEntryRia.LoginUI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.ServiceModel.DomainServices.Client;
    using System.ServiceModel.DomainServices.Client.ApplicationServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using TimeEntryRia.Web;

    /// <summary>
    /// Form that presents the <see cref="RegistrationData"/> and performs the registration process.
    /// </summary>
    public partial class RegistrationForm : StackPanel
    {
        private LoginRegistrationWindow parentWindow;
        private RegistrationData registrationData = new RegistrationData();
        private UserRegistrationContext userRegistrationContext = new UserRegistrationContext();

        /// <summary>
        /// Creates a new <see cref="RegistrationForm"/> instance.
        /// </summary>
        public RegistrationForm()
        {
            InitializeComponent();

            // Set the DataContext of this control to the
            // Registration instance to allow for easy binding.
            this.DataContext = this.registrationData;
        }

        /// <summary>
        /// Sets the parent window for the current <see cref="RegistrationForm"/>.
        /// </summary>
        /// <param name="window">The window to use as the parent.</param>
        public void SetParentWindow(LoginRegistrationWindow window)
        {
            this.parentWindow = window;
        }

        /// <summary>
        /// Wire up the Password and PasswordConfirmation Accessors as the fields get generated.
        /// </summary>
        private void RegisterForm_AutoGeneratingField(object dataForm, DataFormAutoGeneratingFieldEventArgs e)
        {
            // Put all the fields in adding mode
            e.Field.Mode = DataFieldMode.AddNew;

            if (e.PropertyName == "Password")
            {
                PasswordBox passwordBox = (PasswordBox)e.Field.Content;
                this.registrationData.PasswordAccessor = () => passwordBox.Password;
            }
            else if (e.PropertyName == "PasswordConfirmation")
            {
                PasswordBox passwordConfirmationBox = (PasswordBox)e.Field.Content;
                this.registrationData.PasswordConfirmationAccessor = () => passwordConfirmationBox.Password;
            }
            else if (e.PropertyName == "UserName")
            {
                TextBox textBox = (TextBox)e.Field.Content;
                textBox.LostFocus += this.UserNameLostFocus;
            }
            else if (e.PropertyName == "Question")
            {
                // Create a ComboBox populated with security questions
                ComboBox comboBoxWithSecurityQuestions = RegistrationForm.CreateComboBoxWithSecurityQuestions();

                // Replace the control
                // Note: Since TextBox.Text treats empty text as string.Empty and ComboBox.SelectedItem
                // treats an empty selection as null, we need to use a converter on the binding
                e.Field.ReplaceTextBox(comboBoxWithSecurityQuestions, ComboBox.SelectedItemProperty, binding => binding.Converter = new TargetNullValueConverter());
            }
        }

        /// <summary>
        /// The callback for when the UserName TextBox loses focus.  Call into the
        /// registration data to allow logic to be processed, possibly setting
        /// the FriendlyName field.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        private void UserNameLostFocus(object sender, RoutedEventArgs e)
        {
            this.registrationData.UserNameEntered(((TextBox)sender).Text);
        }

        /// <summary>
        /// Returns a <see cref="ComboBox" /> object whose <see cref="ComboBox.ItemsSource" /> property
        /// is initialized with the resource strings defined in <see cref="SecurityQuestions" />.
        /// </summary>
        private static ComboBox CreateComboBoxWithSecurityQuestions()
        {
            // Use reflection to grab all the localized security questions
            IEnumerable<string> availableQuestions = from propertyInfo in typeof(SecurityQuestions).GetProperties()
                                                     where propertyInfo.PropertyType.Equals(typeof(string))
                                                     select (string)propertyInfo.GetValue(null, null);

            // Create and initialize the ComboBox object
            ComboBox comboBox = new ComboBox();
            comboBox.ItemsSource = availableQuestions;
            return comboBox;
        }

        /// <summary>
        /// Submit the new registration.
        /// </summary>
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            // We need to force validation since we are not using the standard OK
            // button from the DataForm.  Without ensuring the form is valid, we
            // would get an exception invoking the operation if the entity is invalid.
            if (this.registerForm.ValidateItem())
            {
                this.registrationData.CurrentOperation = this.userRegistrationContext.CreateUser(
                    this.registrationData,
                    this.registrationData.Password,
                    this.RegistrationOperation_Completed, null);

                this.parentWindow.AddPendingOperation(this.registrationData.CurrentOperation);
            }
        }

        /// <summary>
        /// Completion handler for the registration operation. If there was an error, an
        /// <see cref="ErrorWindow"/> is displayed to the user. Otherwise, this triggers
        /// a login operation that will automatically log in the just registered user.
        /// </summary>
        private void RegistrationOperation_Completed(InvokeOperation<CreateUserStatus> operation)
        {
            if (!operation.IsCanceled)
            {
                if (operation.HasError)
                {
                    ErrorWindow.CreateNew(operation.Error);
                    operation.MarkErrorAsHandled();
                }
                else if (operation.Value == CreateUserStatus.Success)
                {
                    this.registrationData.CurrentOperation = WebContext.Current.Authentication.Login(this.registrationData.ToLoginParameters(), this.LoginOperation_Completed, null);
                    this.parentWindow.AddPendingOperation(this.registrationData.CurrentOperation);
                }
                else if (operation.Value == CreateUserStatus.DuplicateUserName)
                {
                    this.registrationData.ValidationErrors.Add(new ValidationResult(ErrorResources.CreateUserStatusDuplicateUserName, new string[] { "UserName" }));
                }
                else if (operation.Value == CreateUserStatus.DuplicateEmail)
                {
                    this.registrationData.ValidationErrors.Add(new ValidationResult(ErrorResources.CreateUserStatusDuplicateEmail, new string[] { "Email" }));
                }
                else
                {
                    ErrorWindow.CreateNew(ErrorResources.ErrorWindowGenericError);
                }
            }
        }

        /// <summary>
        /// Completion handler for the login operation that occurs after a successful
        /// registration and login attempt.  This will close the window. On the unexpected
        /// event that this operation failed (the user was just added so why wouldn't it
        /// be authenticated successfully) an  <see cref="ErrorWindow"/> is created and
        /// will display the error message.
        /// </summary>
        /// <param name="loginOperation">The <see cref="LoginOperation"/> that has completed.</param>
        private void LoginOperation_Completed(LoginOperation loginOperation)
        {
            if (!loginOperation.IsCanceled)
            {
                this.parentWindow.Close();

                if (loginOperation.HasError)
                {
                    ErrorWindow.CreateNew(string.Format(System.Globalization.CultureInfo.CurrentUICulture, ErrorResources.ErrorLoginAfterRegistrationFailed, loginOperation.Error.Message));
                    loginOperation.MarkErrorAsHandled();
                }
                else if (loginOperation.LoginSuccess == false)
                {
                    // The operation was successful, but the actual login was not
                    ErrorWindow.CreateNew(string.Format(System.Globalization.CultureInfo.CurrentUICulture, ErrorResources.ErrorLoginAfterRegistrationFailed, ErrorResources.ErrorBadUserNameOrPassword));
                }
            }
        }

        /// <summary>
        /// Switches to the login window.
        /// </summary>
        private void BackToLogin_Click(object sender, RoutedEventArgs e)
        {
            this.parentWindow.NavigateToLogin();
        }

        /// <summary>
        /// If a registration or login operation is in progress and is cancellable, cancel it.
        /// Otherwise, close the window.
        /// </summary>
        private void CancelButton_Click(object sender, EventArgs e)
        {
            if (this.registrationData.CurrentOperation != null && this.registrationData.CurrentOperation.CanCancel)
            {
                this.registrationData.CurrentOperation.Cancel();
            }
            else
            {
                this.parentWindow.Close();
            }
        }
    }
}

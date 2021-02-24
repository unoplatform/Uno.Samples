namespace TimeEntryRia.LoginUI
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ServiceModel.DomainServices.Client;
    using System.ServiceModel.DomainServices.Client.ApplicationServices;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// <see cref="ChildWindow"/> class that controls the registration process.
    /// </summary>
    public partial class LoginRegistrationWindow : ChildWindow
    {
        private IList<OperationBase> possiblyPendingOperations = new List<OperationBase>();

        /// <summary>
        /// Creates a new <see cref="LoginRegistrationWindow"/> instance.
        /// </summary>
        public LoginRegistrationWindow()
        {
            InitializeComponent();

            this.registrationForm.SetParentWindow(this);
            this.loginForm.SetParentWindow(this);

            this.LayoutUpdated += this.GoToInitialState;
            this.LayoutUpdated += this.UpdateTitle;
        }

        /// <summary>
        /// Initializes the <see cref="VisualStateManager"/> for this component
        /// by putting it into the "AtLogin" state
        /// </summary>
        private void GoToInitialState(object sender, EventArgs eventArgs)
        {
            VisualStateManager.GoToState(this, "AtLogin", false);
            this.LayoutUpdated -= this.GoToInitialState;
        }

        /// <summary>
        /// Updates the window title according to which panel
        /// (registration / login) is currently being displayed
        /// </summary>
        private void UpdateTitle(object sender, EventArgs eventArgs)
        {
            this.Title = (this.registrationForm.Visibility == Visibility.Visible) ?
                ApplicationStrings.RegistrationWindowTitle :
                ApplicationStrings.LoginWindowTitle;
        }

        /// <summary>
        /// Notifies the <see cref="LoginRegistrationWindow"/> window that it can only close
        /// if <paramref name="operation"/> is finished or can be cancelled
        /// </summary>
        /// <param name="operation">The pending operation to monitor</param>
        public void AddPendingOperation(OperationBase operation)
        {
            this.possiblyPendingOperations.Add(operation);
        }

        /// <summary>
        /// Causes the <see cref="VisualStateManager"/> to change to the "AtLogin" state.
        /// </summary>
        public virtual void NavigateToLogin()
        {
            VisualStateManager.GoToState(this, "AtLogin", true);
        }

        /// <summary>
        /// Causes the <see cref="VisualStateManager"/> to change to the "AtRegistration" state.
        /// </summary>
        public virtual void NavigateToRegistration()
        {
            VisualStateManager.GoToState(this, "AtRegistration", true);
        }

        /// <summary>
        /// Prevents the window from closing while there are operations in progress
        /// </summary>
        private void LoginWindow_Closing(object sender, CancelEventArgs eventArgs)
        {
            foreach (OperationBase operation in this.possiblyPendingOperations)
            {
                if (!operation.IsComplete)
                {
                    if (operation.CanCancel)
                    {
                        operation.Cancel();
                    }
                    else
                    {
                        eventArgs.Cancel = true;
                    }
                }
            }
        }
    }
}
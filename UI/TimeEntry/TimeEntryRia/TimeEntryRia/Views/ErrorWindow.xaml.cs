namespace TimeEntryRia
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Net;

    /// <summary>
    /// Controls when a stack trace should be displayed on the
    /// Error Window
    /// 
    /// Defaults to <see cref="OnlyWhenDebuggingOrRunningLocally"/>
    /// </summary>
    public enum StackTracePolicy
    {
        /// <summary>
        ///   Stack trace is showed only when running with a debugger attached
        ///   or when running the app on the local machine. Use this to get
        ///   additional debug information you don't want the end users to see
        /// </summary>
        OnlyWhenDebuggingOrRunningLocally,

        /// <summary>
        /// Always show the stack trace, even if debugging
        /// </summary>
        Always,

        /// <summary>
        /// Never show the stack trace, even when debugging
        /// </summary>
        Never
    }

    /// <summary>
    /// <see cref="ChildWindow"/> class that displays errors to the user.
    /// </summary>
    public partial class ErrorWindow : ChildWindow
    {
        /// <summary>
        /// Creates a new <see cref="ErrorWindow"/> instance.
        /// </summary>
        /// <param name="message">The error message to display.</param>
        /// <param name="errorDetails">Extra information about the error.</param>
        protected ErrorWindow(string message, string errorDetails)
        {
            InitializeComponent();
            IntroductoryText.Text = message;
            ErrorTextBox.Text = errorDetails;
        }

        #region Factory Shortcut Methods
        /// <summary>
        /// Creates a new Error Window given an error message.
        /// Current stack trace will be displayed if app is running under
        /// debug or on the local machine.
        /// </summary>
        /// <param name="message">The message to display.</param>
        public static void CreateNew(string message)
        {
            CreateNew(message, StackTracePolicy.OnlyWhenDebuggingOrRunningLocally);
        }

        /// <summary>
        /// Creates a new Error Window given an exception.
        /// Current stack trace will be displayed if app is running under
        /// debug or on the local machine
        /// 
        /// The exception is converted onto a message using
        /// <see cref="ConvertExceptionToMessage"/>
        /// </summary>
        /// <param name="exception">The exception to display.</param>
        public static void CreateNew(Exception exception)
        {
            CreateNew(exception, StackTracePolicy.OnlyWhenDebuggingOrRunningLocally);
        }

        /// <summary>
        /// Creates a new Error Window given an exception. The exception is converted onto a message using
        /// <see cref="ConvertExceptionToMessage"/>
        /// </summary>    
        /// <param name="exception">The exception to display.</param>
        /// <param name="policy">When to display the stack trace, see <see cref="StackTracePolicy"/>.</param>
        public static void CreateNew(Exception exception, StackTracePolicy policy)
        {
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }

            string fullStackTrace = exception.StackTrace;

            // Account for nested exceptions
            Exception innerException = exception.InnerException;
            while (innerException != null)
            {
                fullStackTrace += "\nCaused by: " + exception.Message + "\n\n" + exception.StackTrace;
                innerException = innerException.InnerException;
            }

            CreateNew(ConvertExceptionToMessage(exception), fullStackTrace, policy);
        }

        /// <summary>
        /// Creates a new Error Window given an error message.
        /// </summary>   
        /// <param name="message">The message to display.</param>
        /// <param name="policy">When to display the stack trace, see <see cref="StackTracePolicy"/>.</param>
        public static void CreateNew(string message, StackTracePolicy policy)
        {
            CreateNew(message, new StackTrace().ToString(), policy);
        }
        #endregion

        #region Factory Methods
        /// <summary>
        /// All other factory methods will result in a call to this one
        /// </summary>
        /// 
        /// <param name="message">Which message to display</param>
        /// <param name="stackTrace">The associated stack trace</param>
        /// <param name="policy">In which situations the stack trace should be appended to the message</param>
        private static void CreateNew(string message, string stackTrace, StackTracePolicy policy)
        {
            string errorDetails = string.Empty;

            if (policy == StackTracePolicy.Always ||
                policy == StackTracePolicy.OnlyWhenDebuggingOrRunningLocally && IsRunningUnderDebugOrLocalhost)
            {
                errorDetails = stackTrace ?? string.Empty;
            }

            ErrorWindow window = new ErrorWindow(message, errorDetails);
            window.Show();
        }
        #endregion

        #region Factory Helpers
        /// <summary>
        /// Returns whether running under a dev environment, i.e., with a debugger attached or
        /// with the server hosted on localhost
        /// </summary>
        private static bool IsRunningUnderDebugOrLocalhost
        {
            get
            {
                if (Debugger.IsAttached)
                {
                    return true;
                }
                else
                {
                    string hostUrl = Application.Current.Host.Source.Host;
                    return hostUrl.Contains("::1") || hostUrl.Contains("localhost") || hostUrl.Contains("127.0.0.1");
                }
            }
        }

        /// <summary>
        /// Creates a user friendly message given an Exception. Currently this simply
        /// takes the Exception.Message value, optionally  but you might want to change this to treat
        /// some specific Exception classes differently
        /// </summary>
        /// <param name="e">The exception to convert.</param>
        private static string ConvertExceptionToMessage(Exception e)
        {
            return e.Message;
        }
        #endregion

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnoEffectsSample
{
    /// <summary>
    /// Provides an attached property to support the Android EditText Error property.
    /// </summary>
    public static class TextBoxErrorHelper
    {
        /// <summary>
        /// Attached property to set a custom error message for the TextBox
        /// </summary>
        public static DependencyProperty ErrorTextProperty { get; } = DependencyProperty.RegisterAttached("ErrorText", typeof(string), typeof(TextBoxErrorHelper), new PropertyMetadata(null, OnErrorTextChanged));

        public static void SetErrorText(DependencyObject d, string errorText)
        {
            d.SetValue(ErrorTextProperty, errorText);
        }

        public static string GetErrorText(DependencyObject d)
        {
            return (string)d.GetValue(ErrorTextProperty);
        }

        // Method to handle changes in the attached property
        private static void OnErrorTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBox t = (TextBox)d;
#if ANDROID
            var contentElement = (ContentControl)t.GetTemplateChild("ContentElement");
            if (contentElement != null)
            {
                // contentElement contains the native control as its Content
                var editText = (Android.Widget.EditText)contentElement.Content;
                // EditText.Error property sets an error message to display and toggles the error flag to the right of the control
                editText.Error = (string)e.NewValue;
            }
#endif
        }
    }
}

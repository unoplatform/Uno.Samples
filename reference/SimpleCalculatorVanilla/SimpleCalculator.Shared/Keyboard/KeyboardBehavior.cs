using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace SimpleCalculator.Keyboard
{
    public static class KeyboardBehavior
    {
        #region KeyUpCommand (Attached DP)
        public static DependencyProperty KeyUpCommandProperty { get; } = DependencyProperty.RegisterAttached(
            "KeyUpCommand", typeof(ICommand), typeof(KeyboardBehavior), new PropertyMetadata(default(ICommand), OnKeyUpCommandChanged));

        public static ICommand GetKeyUpCommand(UIElement element)
            => (ICommand)element.GetValue(KeyUpCommandProperty);

        public static void SetKeyUpCommand(UIElement element, ICommand command)
            => element.SetValue(KeyUpCommandProperty, command);

        private static void OnKeyUpCommandChanged(DependencyObject snd, DependencyPropertyChangedEventArgs args)
        {
            if (snd is UIElement elt)
            {
                elt.KeyUp -= OnKeyUp;
                if (args.NewValue is ICommand)
                {
                    elt.KeyUp += OnKeyUp;
                }
            }
        }
        private static void OnKeyUp(object snd, KeyRoutedEventArgs e)
        {
            if (snd is UIElement elt && GetKeyUpCommand(elt) is { } command
                && KeyValues.Keys.TryGetValue(e.Key.ToString(), out var key)
                && command.CanExecute(key))
            {
                command.Execute(key);
            }
        }
        #endregion
    }
}

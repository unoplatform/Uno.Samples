using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Uno.Extensions.Reactive;

namespace SimpleCalculator.Keyboard
{
    public static class KeyboardBehavior
    {
        #region KeyUpCommand (Attached DP)
        public static readonly DependencyProperty KeyUpCommandProperty = DependencyProperty.RegisterAttached(
            "KeyUpCommand", typeof(ICommand), typeof(UIElement), new PropertyMetadata(default(Command), OnKeyUpCommandChanged));
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
            if (snd is UIElement elt && GetKeyUpCommand(elt) is { } command && command.CanExecute(e.Key.ToString()))
            {
                command.Execute(e.Key.ToString());
            }
        }
        #endregion
    }
}

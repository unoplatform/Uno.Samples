using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace XamlControlLibrary
{
    public partial class MyTemplatedControl : Control
    {
        public MyTemplatedControl()
        {
            //this.DefaultStyleKey = typeof(MyTemplatedControl);
        }


        public int MyProperty
        {
            get { return (int)GetValue(MyPropertyProperty); }
            set { SetValue(MyPropertyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MyPropertyProperty =
            DependencyProperty.Register(
                "MyProperty",
                typeof(int),
                typeof(MyTemplatedControl),
                new PropertyMetadata(
                    0,
                    (s, e) => ((MyTemplatedControl)s)?.OnMyPropertyChanged(e))
            );


        private void OnMyPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            // System.Diagnostics.Debug.WriteLine($"MyProperty Changed from [{e.OldValue}] to [{e.NewValue}]");
        }
    }
}

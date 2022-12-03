using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace DemoApp.CanvasObjects
{
    public sealed partial class Node : StackPanel
    {
        public Node(string text, Visibility disableArrow = Visibility.Visible)
        {
            this.InitializeComponent();

            textBlock.Text = text;
            _line.Visibility = disableArrow;
        }
    }
}

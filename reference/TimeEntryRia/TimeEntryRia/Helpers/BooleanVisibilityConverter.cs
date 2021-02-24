using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace TimeEntryRia
{
    public class BooleanVisibilityConverter : BooleanConverter<Visibility>
    {
        public BooleanVisibilityConverter() :
            base(Visibility.Collapsed, Visibility.Visible) { }    
    }
}

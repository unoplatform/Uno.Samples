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

namespace TimeEntryRia.Models
{
    public class NameValuePair<T>
    {
        public NameValuePair(string name, T value)
        {
            this.Name = name;
            this.Value = value;
        }

        public string Name { get; set; }
        public T Value { get; set; }
    }
}

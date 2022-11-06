using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Text;

namespace InsertingSeparators
{
    public class MenuTemplateSelector : DataTemplateSelector
    {
        public DataTemplate NormalTemplate { get; set; }
        public DataTemplate SeparatorTemplate { get; set; }
        protected override DataTemplate SelectTemplateCore(object item)
        {
            if(item is Bird)
            {
                return NormalTemplate;
            }

            return base.SelectTemplateCore(item);
        }
    }
}

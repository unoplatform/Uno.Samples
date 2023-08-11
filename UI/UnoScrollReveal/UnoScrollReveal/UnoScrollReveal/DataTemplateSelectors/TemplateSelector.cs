using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnoScrollReveal.DataTemplateSelectors
{
    public class TemplateSelector : DataTemplateSelector
    {
        public DataTemplate ItemTemplate { get; set; }
        public DataTemplate DefaultTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            return ((Item)item)?.IsEmpty == false ? ItemTemplate : DefaultTemplate;
        }
    }
}

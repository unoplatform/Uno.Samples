using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ThreeLevelListDetailsSample.Data
{
    public class DataItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ListItemTemplate { get; set; }
        public DataTemplate DetailItemTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is IItem trackedItem)
            {
                return (trackedItem is WorkRequestItem) ? ListItemTemplate : DetailItemTemplate;
            }
            else
            {
                return null;
            }
        }
    }
}

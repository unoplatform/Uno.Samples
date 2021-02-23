using System;
using System.Windows.Data;
using System.Windows;

namespace TimeEntryRia
{
    /// <summary>
    /// Two way IValueConverter that lets you bind a property on a bindable object
    /// that returns the bold font weight if the value is true, otherwise normal
    /// </summary>    
    public class BooleanFontWeightConverter : BooleanConverter<FontWeight>
    {
        public BooleanFontWeightConverter() :
            base(FontWeights.Bold, FontWeights.Normal) { }
    }
}

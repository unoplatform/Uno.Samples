using System;
using System.Globalization;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace ChatUI.Converters
{
    /// <summary>
    /// Copied/adapted from https://github.com/unoplatform/uno/blob/master/src/Uno.UI.Tests/Converters/FromNullableBoolToVisibilityConverter.cs
    /// This converter will output a visibility based on if a nullable bool is set to true or otherwise.
    /// 
    /// VisibilityIfTrue (VisibilityIfTrue) : Determines the visibility value that will be returned if the value is true.
    /// 
    /// By default, VisibilityIfTrue is set to visible.
    /// 
    /// This converter may be used to display or hide some content based on a nullable boolean value.
    /// </summary>
    public class FromNullableBoolToVisibilityConverter : IValueConverter
    {
        public FromNullableBoolToVisibilityConverter()
        {
            VisibilityIfTrue = VisibilityIfTrue.Visible;
        }

        public VisibilityIfTrue VisibilityIfTrue { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
			return value switch
			{
				true => VisibilityIfTrue,
				null or bool when VisibilityIfTrue is VisibilityIfTrue.Collapsed => VisibilityIfTrue.Visible,
				_ => VisibilityIfTrue.Collapsed
			};
		}

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return null;
            }

			return value switch
			{
				false => VisibilityIfTrue,
				_ when VisibilityIfTrue is VisibilityIfTrue.Visible => VisibilityIfTrue.Collapsed,
				_ => VisibilityIfTrue.Visible
			};
		}
    }

    public enum VisibilityIfTrue
    {
        Visible,
        Collapsed
    }
}

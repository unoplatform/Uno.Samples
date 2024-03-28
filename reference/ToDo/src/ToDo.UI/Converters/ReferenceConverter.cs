namespace ToDo.Converters;

public class ReferenceConverter : IValueConverter
{
	public enum ReferenceConversionMode
	{
		IsNull,
		IsNotNull,
	}

	public ReferenceConversionMode ConversionMode { get; set; }

	public object Convert(object value, Type targetType, object parameter, string language)
		=> ConversionMode switch
		{
			ReferenceConversionMode.IsNull => value is null,
			ReferenceConversionMode.IsNotNull => value is not null,

			_ => throw new ArgumentOutOfRangeException($"Invalid ConversionMode: {ConversionMode}"),
		};


	public object ConvertBack(object value, Type targetType, object parameter, string language)
		 => throw new NotSupportedException("Only one-way conversion is supported.");
}

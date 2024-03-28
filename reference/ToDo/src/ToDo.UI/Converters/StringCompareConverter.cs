namespace ToDo.Converters;

/// <summary>
/// This converter allows for common string comparison operations on the value.
/// </summary>
/// <remarks>
/// Non-string value can be treated as string by setting the <see cref="ConvertToString" /> flag.
/// </remarks>
public class StringCompareConverter : IValueConverter
{
	public enum ComparisonMethod { IsNullOrEmpty, IsNullOrWhitespace, IsEqualToParameterValue }

	public ComparisonMethod Comparison { get; set; }

	/// <summary>
	/// Indicates whether <see cref="object.ToString()" /> should be used on the value when it is not a string.
	/// </summary>
	public bool ConvertToString { get; set; } = false;

	/// <summary>
	/// Indicates whether the final result should be inverted, returning <see cref="FalseValue"/> when the comparison is successful, and vice-versa.
	/// </summary>
	public bool InvertResult { get; set; } = false;

	/// <summary>
	/// Value to returned when the comparison is successful. The default value is literal true.
	/// </summary>
	public object TrueValue { get; set; } = true;
	
	/// <summary>
	/// Value to returned when the comparison is not successful. The default value is literal false.
	/// </summary>
	public object FalseValue { get; set; } = false;

	public object Convert(object value, Type targetType, object parameter, string language)
	{
		var str = value as string ?? (ConvertToString ? value?.ToString() : default);
		var result = Comparison switch
		{
			ComparisonMethod.IsEqualToParameterValue when parameter is string param => str?.Equals(param) == true,
			ComparisonMethod.IsNullOrEmpty => string.IsNullOrEmpty(str),
			ComparisonMethod.IsNullOrWhitespace => string.IsNullOrWhiteSpace(str),

			_ => throw new ArgumentOutOfRangeException($"Comparison: {Comparison}"),
		};
		result = InvertResult ? !result : result;

		return result ? TrueValue : FalseValue;
	}

	public object ConvertBack(object value, Type targetType, object parameter, string language)
		=> throw new NotSupportedException("Only one-way conversion is supported.");
}

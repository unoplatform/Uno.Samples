namespace UnoChatGPT.DataContracts
{
	/// <summary>
	/// A Weather Forecast for a specific date
	/// </summary>
	/// <param name="Date">Gets the Date of the Forecast.</param>
	/// <param name="TemperatureC">Gets the Forecast Temperature in Celsius.</param>
	/// <param name="Summary">Get a description of how the weather will feel.</param>
	public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
	{
		/// <summary>
		/// Gets the Forecast Temperature in Fahrenheit
		/// </summary>
		public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
	}
}
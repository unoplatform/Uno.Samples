using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace ToDo.Data;

public class DateTimeDataConverter : JsonConverter<DateTimeOffset?>
{
	/// <inheritdoc />
	public override DateTimeOffset? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType is JsonTokenType.StartObject)
		{
			var hasDate = false;
			var date = default(DateTime);
			var timeZone = default(TimeSpan);
			while (reader.Read() && reader.TokenType is JsonTokenType.PropertyName)
			{
				switch (reader.GetString()?.ToLowerInvariant())
				{
					case "datetime" when reader.Read() && reader.TokenType is JsonTokenType.String && reader.TryGetDateTime(out date):
						hasDate = true;
						break;

					case "timezone" when reader.Read() && reader.TokenType is JsonTokenType.String:
						try
						{
							var timeZoneCode = reader.GetString();
							if (timeZoneCode is not null && !RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
							{
								timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneCode).BaseUtcOffset;
							}
						}
						catch (Exception)
						{
							// As we use only the date, we can pretty safely ignore the timezone.
						}
						break;
				}
			}

			if (hasDate)
			{
				return new DateTimeOffset(date, timeZone);
			}
		}

		return default;
	}

	/// <inheritdoc />
	public override void Write(Utf8JsonWriter writer, DateTimeOffset? value, JsonSerializerOptions options)
	{
		if (value is null)
		{
			writer.WriteNullValue();
		}
		else
		{
			writer.WriteStartObject();
			writer.WritePropertyName("dateTime");
			writer.WriteStringValue(value.Value.Date.ToString("s"));
			writer.WritePropertyName("timeZone");
			writer.WriteStringValue("UTC");
			writer.WriteEndObject();
		}
	}
}

using System;

namespace ChatUI.Entities;

public class Message
{
	public Message(string text, string contactName = default, DateTimeOffset timestamp = default, bool isMyMessage = true)
	{
		Text = text;
		ContactName = contactName == default ? "Me" : contactName;
		Timestamp = timestamp == default ? DateTimeOffset.Now : timestamp;
		IsMyMessage = isMyMessage;
	}

	public string Text { get; init; }

	public string ContactName { get; init; }

	public DateTimeOffset Timestamp { get; init; }

	public bool IsMyMessage { get; init; }

	public bool IsLastInSequence { get; set; }

	public string UserFriendlyTimestamp => Timestamp.LocalDateTime.ToString("t");

	public static Message Empty => new Message(default);
}


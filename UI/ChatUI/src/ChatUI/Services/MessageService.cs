using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ChatUI.Models;
using Uno.Extensions;
using Uno.Extensions.Specialized;

namespace ChatUI.Services
{
    public class MessageService : IMessageService
	{
		private IList<Message> _messages = new List<Message>();

		public async ValueTask<IImmutableList<Message>> GetMessages(CancellationToken ct)
		{
			// To simulate network traffic
			await Task.Delay(TimeSpan.FromSeconds(2), ct);

			// Add an initial batch of messages if list is empty
			if(_messages.None())
			{
				_messages = Enumerable
					.Range(0, 100)
					.Select(i => new Message(
						text: $"Message {i}", 
						contactName: i % 2 == 0 ? "Me" : "Sender", 
						timestamp: DateTimeOffset.Now - TimeSpan.FromMinutes(30 * i), 
						isMyMessage: i % 2 == 0))
					.ToList();
			}

			return _messages.ToImmutableList();
		}

		public async ValueTask AddMessage(Message newMessage, CancellationToken ct)
		{
			// To simulate network traffic
			await Task.Delay(500);

			// TODO: Add service to update messages on the server...
		}
	}
}

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using WebRTC.Entities;

namespace WebRTC.SignalingServer.Hubs
{
	public class RtcHub : Hub
	{
		private readonly ILogger<RtcHub> _logger;

		public RtcHub(ILogger<RtcHub> logger)
		{
			_logger = logger;
		}

		private static readonly ConcurrentDictionary<string, (string roomName, string spdOffer)> _rooms = new();

		public override async Task OnConnectedAsync()
		{
			_logger.LogInformation($"New connection for client/room {Context.ConnectionId}.");

			// Send all rooms to all clients
			// Actually only the new connected client needs it...
			// Production code should take care of such details.
			await BroadcastRooms();
		}

		public override Task OnDisconnectedAsync(Exception? exception)
		{
			_logger.LogInformation($"Client {Context.ConnectionId} disconnected.");
			return RemoveRoom();
		}

		public async Task CreateRoom(string roomName, string sdpOffer)
		{
			_rooms[Context.ConnectionId] = (roomName, sdpOffer);

			_logger.LogInformation($"Creation of room \"{roomName}\" for client {Context.ConnectionId}.");

			await BroadcastRooms();
		}

		public async Task JoinRoom(string roomId, string spdAnswer)
		{
			_logger.LogInformation($"Client {Context.ConnectionId} asked to join room of client {roomId}");

			// The room id is the SignalR's ConnectionId of the client creating it.
			if (Clients.Client(roomId) is { } otherClient)
			{

				await otherClient.SendCoreAsync("Answer", new object[] {spdAnswer});
			}
		}

		public async Task RemoveRoom()
		{

			if (_rooms.TryRemove(Context.ConnectionId, out var room))
			{
				_logger.LogInformation($"Removing room {Context.ConnectionId} / {room.roomName}.");

			}

			await BroadcastRooms();
		}

		private async Task BroadcastRooms()
		{
			_logger.LogInformation($"Sending list of rooms to all clients. There is {_rooms.Count} waiting room(s).");


			var rooms = _rooms
				.Select(kvp => new RoomInfo(kvp.Key, kvp.Value.roomName, kvp.Value.spdOffer))
				.ToArray();

			var json = JsonSerializer.Serialize(rooms);

			await this.Clients.All.SendCoreAsync("Rooms", new object[] {json});
		}
	}
}

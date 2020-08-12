using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace UnoChat.Service.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(DateTimeOffset sentAt, Guid userId, string userName, Guid deviceTypeId, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", sentAt, DateTimeOffset.UtcNow, userId, userName, deviceTypeId, message);
        }
    }
}

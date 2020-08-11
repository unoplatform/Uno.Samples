using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;

namespace UnoChat.Client.Console
{
    using Console = System.Console;

    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hi! Err... who are you?");

            var name = Console.ReadLine();

            Console.WriteLine($"Ok {name} one second, we're going to connect to the SignalR server...");

            var connection = new HubConnectionBuilder()
                .WithUrl("https://unochatservice20200716114254.azurewebsites.net/ChatHub")
                .WithAutomaticReconnect()
                .Build();

            connection.On<string, string>("ReceiveMessage", (user, message) => Console.WriteLine($"{user}: {message}"));

            await connection.StartAsync();

            Console.WriteLine($"Aaaaaand we're connected. Enter a message and hit return to send it to other connected clients...");

            while (true)
            {
                var message = Console.ReadLine();

                await connection.InvokeAsync("SendMessage", name, message);
            }
        }
    }
}

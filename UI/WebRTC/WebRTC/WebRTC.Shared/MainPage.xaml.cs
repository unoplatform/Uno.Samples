using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using Uno.Extensions;
using Uno.Foundation;
using WebRTC.Entities;

namespace WebRTC
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		private HubConnection _signalingServer;

		public MainPage()
		{
			this.InitializeComponent();

			InitSignaling();
			InitEventHandlers();
		}

		private void InitSignaling()
		{
			_signalingServer = new HubConnectionBuilder()
				.WithUrl("http://localhost:6173/webrtc")
				.WithAutomaticReconnect()
				.Build();

			_signalingServer.On("Rooms", (Action<string>) OnRooms);
			_signalingServer.On("Answer", (Action<string>)OnAnswer);

			var t = _signalingServer.StartAsync();
		}

		private void InitEventHandlers()
		{
			this.RegisterHtmlEventHandler("Opened", OnConnectionOpened);
			this.RegisterHtmlCustomEventHandler("Message", OnConnectionMessage);
			this.RegisterHtmlCustomEventHandler("Error", OnConnectionError);
			this.RegisterHtmlEventHandler("Closed", OnConnectionClosed);
			this.RegisterHtmlCustomEventHandler("IceCandidate", OnConnectionIceCandidate, true);
		}

		private async void OnConnectionOpened(object sender, EventArgs e)
		{
			Show("Connection Opened");

			await _signalingServer.SendCoreAsync("RemoveRoom", new object[] { });
		}

		private void OnConnectionMessage(object sender, HtmlCustomEventArgs e)
		{
			Show(">> " + e.Detail, isMessage: true);
		}

		private void OnConnectionError(object sender, HtmlCustomEventArgs e)
		{
			Show("Connection Error " + e.Detail);
		}

		private void OnConnectionClosed(object sender, EventArgs e)
		{
			Show("Connection Closed");
		}

		private void OnConnectionIceCandidate(object sender, HtmlCustomEventArgs e)
		{
			Show("Ice Candidate ready");
			var tcs = Interlocked.Exchange(ref _iceTcs, null);
			tcs?.TrySetResult(e.Detail);
		}

		private static Brush LogColor = new SolidColorBrush(Colors.Gray);
		private static Brush MessageColor = new SolidColorBrush(Colors.Black);

		private void Show(string txt, bool isMessage = false)
		{
			var text = new TextBlock
			{
				Text = txt,
				Foreground = isMessage ? MessageColor : LogColor,
				FontWeight = isMessage ? FontWeights.Bold : FontWeights.Normal
			};
			received.Children.Add(text);
		}

		private void ClearDisplay()
		{
			received.Children.Clear();
		}

		private void OnRooms(string json)
		{
			var rooms = JsonConvert.DeserializeObject<List<RoomInfo>>(json);

			var allRoomsExceptMe = rooms
				.Where(r => r.RoomId != _signalingServer.ConnectionId)
				.ToArray();

			roomsList.ItemsSource = allRoomsExceptMe;
		}

		private void OnAnswer(string sdpAnswer)
		{
			Show("Received an answer from signaling server...");

			var js = $"element.UnoPeerConnection.SetAnswer({sdpAnswer});";

			this.ExecuteJavascriptAsync(js);
		}

		private TaskCompletionSource<string> _iceTcs;

		private async Task<string> InitPeerInitiatorConnection(string roomName = "")
		{
			var escapedRoomName = WebAssemblyRuntime.EscapeJs(roomName);
			var js = $@"
					(async () => {{
						if(element.UnoPeerConnection) {{
							element.UnoPeerConnection.Close();
						}}
						element.UnoPeerConnection = await Uno.WebRTC.PeerConnection.CreateInitiator(""{escapedRoomName}"", element);
					}})();";
			await this.ExecuteJavascriptAsync(js);

			_iceTcs = new TaskCompletionSource<string>();

			var offer = await _iceTcs.Task;

			return offer;
		}

		private async Task<string> InitPeerRemoteConnection(string spdOffer)
		{
			var js = $@"
					(async () => {{
						if(element.UnoPeerConnection) {{
							element.UnoPeerConnection.Close();
						}}
						element.UnoPeerConnection = await Uno.WebRTC.PeerConnection.CreateRemote(element, {spdOffer});
					}})();";
			await this.ExecuteJavascriptAsync(js);

			_iceTcs = new TaskCompletionSource<string>();

			var answer = await _iceTcs.Task;

			return answer;
		}

		private async void CreateRoom(object sender, RoutedEventArgs e)
		{
			ClearDisplay();

			Show($"Creating room {_signalingServer.ConnectionId}...");

			var name = this.roomName.Text;
			var iceOffer = await InitPeerInitiatorConnection(name);

			await _signalingServer.SendCoreAsync(
				"CreateRoom",
				new object[] { name, iceOffer });
		}

		private async void JoinRoom(object sender, RoutedEventArgs e)
		{
			var room = roomsList.SelectedItem;
			if (room is RoomInfo roomInfo)
			{
				ClearDisplay();

				Show($"Joining room {roomInfo.RoomName}/{roomInfo.RoomId}...");

				var spdOffer = roomInfo.SpdOffer;

				var iceAnswer = await InitPeerRemoteConnection(spdOffer);

				await _signalingServer.SendCoreAsync(
					"JoinRoom",
					new object[] { roomInfo.RoomId, iceAnswer });
			}
		}

		private void SendMessage(object sender, RoutedEventArgs e)
		{
			var message = WebAssemblyRuntime.EscapeJs(msgToSend.Text);
			var js = $"element.UnoPeerConnection.SendMessage(\"{message}\");";

			this.ExecuteJavascript(js);
		}
	}
}

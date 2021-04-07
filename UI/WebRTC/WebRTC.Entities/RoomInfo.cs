namespace WebRTC.Entities
{
	public class RoomInfo
	{
		public RoomInfo(string roomId, string roomName, string spdOffer)
		{
			RoomId = roomId;
			RoomName = roomName;
			SpdOffer = spdOffer;
		}

		public string RoomId { get; }
		public string RoomName { get; }
		public string SpdOffer { get; }
	}
}

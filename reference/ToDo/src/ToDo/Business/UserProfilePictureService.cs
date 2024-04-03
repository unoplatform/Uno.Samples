namespace ToDo.Business;

public class UserProfilePictureService : IUserProfilePictureService
{
	private readonly IUserProfilePictureEndpoint _client;

	private byte[]? _profilePicture;
	private string? _userEmailAddress;

	public UserProfilePictureService(IUserProfilePictureEndpoint client)
	{
		_client = client;
	}

	public async ValueTask<byte[]> GetAsync(UserContext? user, CancellationToken ct)
	{
		if (_profilePicture is null ||
			_profilePicture.Length == 0 ||
			user?.Email != _userEmailAddress)
		{
			var content = await _client.GetAsync(ct);

			_profilePicture = await content.ReadAsByteArrayAsync();
			_userEmailAddress = user?.Email;
		}
		return _profilePicture;
	}
}

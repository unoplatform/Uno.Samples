namespace ToDo.Data.Mock;

internal class MockUserProfilePictureEndpoint : IUserProfilePictureEndpoint
{
	private const string ProfilePictureDataFile = "Mock/profilePicture.json";

	private readonly ISerializer<string> _profilePictureSerializer;
	private readonly IStorage _dataService;

	public MockUserProfilePictureEndpoint(
				ISerializer<string> profilePictureSerializer,
				IStorage dataService)
	{
		_profilePictureSerializer = profilePictureSerializer;
		_dataService = dataService;
	}

	public async Task<HttpContent> GetAsync(CancellationToken ct)
	{
		var base64 =
			await _dataService.ReadPackageFileAsync<string>(_profilePictureSerializer,
				ProfilePictureDataFile) ?? throw new Exception("Unable to find mock profile picture");

		var bytes = Convert.FromBase64String(base64);
		var contents = new StreamContent(new MemoryStream(bytes));

		return contents;
	}
}

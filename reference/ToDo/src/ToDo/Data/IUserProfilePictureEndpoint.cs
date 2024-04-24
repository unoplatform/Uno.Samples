namespace ToDo.Data;

[Headers("Authorization: Bearer")]
public interface IUserProfilePictureEndpoint
{
	[Get("/photo/$value")]
	[Headers("Content-Type: image/jpg")]
	Task<HttpContent> GetAsync(CancellationToken ct);
}

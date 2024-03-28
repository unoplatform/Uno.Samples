
namespace ToDo.Business;

public interface IUserProfilePictureService
{
	ValueTask<byte[]> GetAsync(UserContext? user, CancellationToken cancellationToken);
}

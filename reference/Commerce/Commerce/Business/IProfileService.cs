namespace Commerce.Business;
using Commerce.Business.Models;

public interface IProfileService
{
	ValueTask<Profile> GetProfile(CancellationToken ct);
}

namespace Commerce.Business;

public interface IProfileService
{
    ValueTask<Profile> GetProfile(CancellationToken ct);
}

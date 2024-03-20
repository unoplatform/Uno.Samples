namespace Commerce.Business;

public class ProfileService : IProfileService
{
    public async ValueTask<Profile> GetProfile(CancellationToken ct)
    {
        await Task.Delay(1, ct);
        var data = new ProfileData
        {
            FirstName = "Michael",
            LastName = "Scott",
            Avatar = "https://loremflickr.com/360/360/face"
        };

        return new Profile(data);
    }
}

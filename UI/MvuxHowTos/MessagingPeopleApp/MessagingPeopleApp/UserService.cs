using System.Collections.Immutable;

namespace MessagingPeopleApp;

public interface IUserService
{
    ValueTask<IImmutableList<User>> GetUsersAsync(CancellationToken ct);
}

public class UserService : IUserService
{
    public async ValueTask<IImmutableList<User>> GetUsersAsync(CancellationToken ct)
    {
        await Task.Delay(500, ct);

        return _users.ToImmutableList();
    }

    private HashSet<User> _users = new()
    {
        new("Johnny"),
        new("Charlie")
    };
}

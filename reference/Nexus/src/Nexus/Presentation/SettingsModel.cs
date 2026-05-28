using Nexus.Services;
using Uno.Extensions.Reactive;

namespace Nexus.Presentation;

/// <summary>MVUX model for the Settings page; data sourced from <see cref="INexusService"/>.</summary>
public partial record SettingsModel(INexusService Nexus)
{
    public IFeed<SystemSettings> Settings => Feed.Async(Nexus.GetSettingsAsync);

    public IListFeed<User> Users => ListFeed.Async(Nexus.GetUsersAsync);
}

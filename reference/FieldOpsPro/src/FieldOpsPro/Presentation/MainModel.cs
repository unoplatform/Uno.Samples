using Uno.Extensions.Reactive;

namespace FieldOpsPro.Presentation;

public partial record MainModel
{
    private readonly IFieldOpsService _fieldOpsService;

    public MainModel(IFieldOpsService fieldOpsService)
    {
        _fieldOpsService = fieldOpsService;
    }

    /// <summary>Current operator profile, surfaced into the Shell chrome (sidebar user card).</summary>
    public IFeed<UserProfile> CurrentUser => Feed.Async(_fieldOpsService.GetCurrentUserAsync);
}

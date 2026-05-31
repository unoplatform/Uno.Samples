using Uno.Extensions.Reactive;

namespace FieldOpsPro.Presentation;

public partial record TeamModel
{
    private readonly IFieldOpsService _fieldOpsService;

    public TeamModel(IFieldOpsService fieldOpsService)
    {
        _fieldOpsService = fieldOpsService;
    }

    public IListFeed<TeamMember> Members => ListFeed.Async(_fieldOpsService.GetTeamMembersAsync);
}

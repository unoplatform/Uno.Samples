using Uno.Extensions.Reactive;

namespace FieldOpsPro.Presentation;

public partial record MapModel
{
    private readonly IFieldOpsService _fieldOpsService;

    public MapModel(IFieldOpsService fieldOpsService)
    {
        _fieldOpsService = fieldOpsService;
    }

    public IListFeed<Agent> Agents => ListFeed.Async(_fieldOpsService.GetAgentsAsync);
}

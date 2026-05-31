using Uno.Extensions.Reactive;

namespace FieldOpsPro.Presentation;

public partial record WorkOrdersModel
{
    private readonly IFieldOpsService _fieldOpsService;

    public WorkOrdersModel(IFieldOpsService fieldOpsService)
    {
        _fieldOpsService = fieldOpsService;
    }

    public IListFeed<TaskItem> PriorityTasks => ListFeed.Async(_fieldOpsService.GetPriorityTasksAsync);

    public IListFeed<TaskItem> TodayTasks => ListFeed.Async(_fieldOpsService.GetTodayTasksAsync);
}

using Uno.Extensions.Reactive;

namespace TaskFlow.Presentation;

public partial record CategoriesModel(ITaskService TaskService)
{
	public IListFeed<CategoryCount> Categories => ListFeed.AsyncEnumerable(TaskService.ObserveCategoryCountsAsync);
}

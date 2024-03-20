namespace ToDo.Presentation;

public partial class HomeViewModel
{
	private readonly INavigator _navigator;
	private readonly IAuthenticationService _authSvc;
	private readonly IUserProfilePictureService _userSvc;
	private readonly IStringLocalizer _localizer;
	private readonly ITaskListService _listSvc;
	private readonly IWritableOptions<ToDoApp> _appSettings;

	private HomeViewModel(
		INavigator navigator,
		IStringLocalizer localizer,
		IAuthenticationService authSvc,
		IUserProfilePictureService userSvc,
		ITaskListService listSvc,
		IMessenger messenger,
		IWritableOptions<ToDoApp> appSettings)
	{
		_navigator = navigator;
		_localizer = localizer;
		_navigator = navigator;
		_authSvc = authSvc;
		_userSvc = userSvc;
		_listSvc = listSvc;
		_appSettings = appSettings;

		messenger.Observe(Lists, list => list.Id);

		WellKnownLists = new TaskList[]
		{
			new(TaskList.WellknownListNames.Important, _localizer["HomePage_ImportantTaskListLabel"]),
			new(TaskList.WellknownListNames.Tasks, _localizer["HomePage_CommonTaskListLabel"]),
		};

	}

	public IFeed<UserContext?> CurrentUser => Feed<UserContext?>.Async(async ct => await _authSvc.GetCurrentUserAsync());
	public IFeed<byte[]?> ProfilePicture => Feed<byte[]?>.Async(async ct => await _userSvc.GetAsync(await CurrentUser, ct));

	private IListState<TaskList> Lists => ListState<TaskList>.Async(this, _listSvc.GetAllAsync);

	public TaskList[] WellKnownLists { get; }

	public IFeed<TaskList> SelectedList => Lists.AsFeed().Select(lists =>
	{
		var previousListId = _appSettings.Value?.LastTaskList;
		return lists.FirstOrDefault(x => x.Id == previousListId) ?? WellKnownLists[0];
	});

	public IListFeed<TaskList> CustomLists => Lists.Where(list => list.IsCustom);

	public async ValueTask SelectedListChanged(TaskList selectedTaskList, CancellationToken ct)
	{
		await _appSettings.UpdateAsync(x => x with { LastTaskList = selectedTaskList?.Id });
	}

	public async ValueTask CreateTaskList(CancellationToken ct)
	{
		var listName = await _navigator.GetDataAsync<AddListViewModel, string>(this, qualifier: Qualifiers.Dialog, cancellation: ct);

		if (listName is not null)
		{
			await _listSvc.CreateAsync(listName, ct);
		}
	}
}

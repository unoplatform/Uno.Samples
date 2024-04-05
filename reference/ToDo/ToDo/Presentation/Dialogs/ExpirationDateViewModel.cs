namespace ToDo.Presentation.Dialogs;

public partial record PickedDate(DateTimeOffset? Date);

public partial class ExpirationDateViewModel
{
	private readonly INavigator _navigator;

	private ExpirationDateViewModel(INavigator navigator, PickedDate entity)
	{
		_navigator = navigator;

		Entity = State.Value(this, () => entity);
		Entity.Execute(async (date, ct) =>
		{
			if (date?.Date is not null)
			{
				await _navigator.NavigateBackWithResultAsync(this, data: date, cancellation: ct);
			}
		});
	}

	public IState<PickedDate> Entity { get; }

	public async ValueTask SelectToday(CancellationToken ct)
		=> await _navigator.NavigateBackWithResultAsync(this, data: new PickedDate(DateTime.Today), cancellation: ct);

	public async ValueTask SelectTomorrow(CancellationToken ct)
		=> await _navigator.NavigateBackWithResultAsync(this, data: new PickedDate(DateTime.Today.AddDays(1)), cancellation: ct);

	public async ValueTask SelectNextWeek(CancellationToken ct)
		=> await _navigator.NavigateBackWithResultAsync(this, data: new PickedDate(DateTime.Today.AddDays(GetDaysToNextWeek())), cancellation: ct);

	/// <summary>
	/// Gets the number of days before the next monday
	/// </summary>
	private int GetDaysToNextWeek()
		=> (7 - (int)CultureInfo.CurrentUICulture.Calendar.GetDayOfWeek(DateTime.Today)) + 1;
}

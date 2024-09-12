namespace MVUX.Presentation.UpdateStateSample;

public partial record UpdateStateModel
{
	public IState<int> Number => State.Value(this, GetRandomNumber);

	// Update State
	public ValueTask ChangeValue(CancellationToken ct)
		=> Number.UpdateAsync(_ => GetRandomNumber(), ct);

	// Update State with item
	public ValueTask ChangeValueWithItem(string item, CancellationToken ct)
		=> Number.UpdateAsync(_ => Convert.ToInt32(item), ct);

	// Updating state from a command w/ accessing previous value
	public ValueTask ChangeValueIfGreaterThan(CancellationToken ct)
		=> Number.UpdateAsync(number =>
			{
				var newValue = GetRandomNumber();
				return newValue > number ? newValue : number;
			}, ct); 

	private readonly Random _random = new();

	private int GetRandomNumber()
		=> _random.Next(0, 100);
}

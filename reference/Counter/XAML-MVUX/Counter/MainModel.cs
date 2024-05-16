namespace Counter;

internal partial record MainModel
{
    public IState<Countable> Countable => State.Value(this, () => new Countable(0, 1));

    public ValueTask IncrementCommand() => 
        Countable.Update(c => c?.Increment(), CancellationToken.None);
}

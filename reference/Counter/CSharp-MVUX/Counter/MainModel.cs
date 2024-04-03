namespace Counter;

internal partial record MainModel
{
    public IState<int> Count => State.Value(this, () => 0);

    public IState<int> Step => State.Value(this, () => 1);
    
    public ValueTask IncrementCommand(int Step)
            => Count.Update(c => c + Step, CancellationToken.None);
}

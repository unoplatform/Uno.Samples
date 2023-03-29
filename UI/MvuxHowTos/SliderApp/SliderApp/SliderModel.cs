using Uno.Extensions.Reactive;

namespace SliderApp;

public partial record SliderModel
{
    public IState<double> SliderValue => State.Value(this, () => Random.Shared.NextDouble() * 100);
}

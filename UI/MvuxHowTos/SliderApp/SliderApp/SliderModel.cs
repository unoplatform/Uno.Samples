using System.Collections.Immutable;
using Uno.Extensions.Reactive;

namespace SliderApp;

public partial record SliderModel
{
    public IState<double> SliderValue => State.Value(this, () => Random.Shared.NextDouble() * 100);

    public async ValueTask ResetSlider(CancellationToken ct = default)
    {
        await SliderValue.Update(updater: currentValue => 0, ct);
    }
}

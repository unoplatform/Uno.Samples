using System.Collections.Immutable;
using Uno.Extensions.Reactive;
using Windows.Security.Cryptography.Core;

namespace SliderApp;

public partial record SliderModel
{
    public IState<double> SliderValue => State.Value(this, () => Random.Shared.NextDouble() * 100);

    public async ValueTask ResetSlider(CancellationToken ct = default)
    {
        await SliderValue.Set(value: 0, ct);
    }

    public async ValueTask IncrementSlider(CancellationToken ct = default)
    {
        static double incrementValue(double currentValue) =>
            currentValue <= 99
            ? currentValue + 1
            : 1;

        await SliderValue.Update(updater: incrementValue, ct);
    }
}

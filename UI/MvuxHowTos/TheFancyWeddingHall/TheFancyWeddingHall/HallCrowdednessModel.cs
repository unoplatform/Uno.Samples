using Uno.Extensions.Reactive;

namespace TheFancyWeddingHall;

public partial record HallCrowdednessModel(HallCrowdednessService HallCrowdednessService)
{
    public IState<HallCrowdedness> HallCrowdedness => State.Async(this, HallCrowdednessService.GetHallCrowdednessAsync);

    public async ValueTask Save(CancellationToken ct)
    {
        var updatedCrowdedness = await HallCrowdedness;

        await HallCrowdednessService.SetHallCrowdednessAsync(updatedCrowdedness!, ct);
    }
}

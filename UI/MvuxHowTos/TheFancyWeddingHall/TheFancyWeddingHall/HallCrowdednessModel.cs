using Uno.Extensions.Reactive;

namespace TheFancyWeddingHall;

public partial record HallCrowdednessModel(HallCrowdednessService HallCrowdednessService)
{
    public IState<HallCrowdedness> HallCrowdedness => State.Async(this, HallCrowdednessService.GetHallCrowdedness);

    public async ValueTask Save(CancellationToken ct)
    {
        var updatedCrowdedness = await HallCrowdedness;

        await HallCrowdednessService.SetHallCrowdedness(updatedCrowdedness!, ct);
    }
}

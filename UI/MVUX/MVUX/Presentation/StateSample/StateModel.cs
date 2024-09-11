using Uno.Extensions.Reactive;

namespace MVUX.Presentation.StateSample;
public partial record StateModel (IStateService StateService)
{
	public IState<HallCrowdedness> HallCrowdedness => State.Async(this, StateService.GetHallCrowdedness);

	public async ValueTask Save(CancellationToken ct)
	{
		var updatedCrowdedness = await HallCrowdedness;

		await StateService.SetHallCrowdedness(updatedCrowdedness!, ct);
	}
}

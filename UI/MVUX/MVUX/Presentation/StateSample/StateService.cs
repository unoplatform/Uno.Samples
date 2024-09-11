namespace MVUX.Presentation.StateSample;

public partial record HallCrowdedness(int NumberOfPeopleInHall);

public interface IStateService
{
	ValueTask<HallCrowdedness> GetHallCrowdedness(CancellationToken ct);
	ValueTask SetHallCrowdedness(HallCrowdedness crowdedness, CancellationToken ct);
}

public class StateService : IStateService
{
	// a service is normally stateless
	// the local field is for the purpose of this demo 
	private int _numberOfPeopleInHall = 5;

	public async ValueTask<HallCrowdedness> GetHallCrowdedness(CancellationToken ct)
	{
		// fake "loading from server"
		await Task.Delay(TimeSpan.FromSeconds(1));
		var result = new HallCrowdedness(_numberOfPeopleInHall);

		return result;
	}

	public async ValueTask SetHallCrowdedness(HallCrowdedness crowdedness, CancellationToken ct)
	{
		// fake "updating server"
		await Task.Delay(TimeSpan.FromSeconds(1));
		_numberOfPeopleInHall = crowdedness.NumberOfPeopleInHall;
	}
}

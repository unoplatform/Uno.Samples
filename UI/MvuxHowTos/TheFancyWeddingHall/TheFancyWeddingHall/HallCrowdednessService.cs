namespace TheFancyWeddingHall;

public partial record HallCrowdedness(int NumberOfPeopleInHall);

public class HallCrowdednessService
{
    // a service is normally stateless
    // the local field is for the purpose of this demo 
    private int _numberOfPeopleInHall = 5;

    public ValueTask<HallCrowdedness> GetHallCrowdednessAsync(CancellationToken ct)
    {
        // fake "loading from server"
        var result = new HallCrowdedness(_numberOfPeopleInHall);

        return ValueTask.FromResult(result);
    }

    public ValueTask SetHallCrowdednessAsync(HallCrowdedness crowdedness, CancellationToken ct)
    {
        // fake "updating server"
        _numberOfPeopleInHall = crowdedness.NumberOfPeopleInHall;        

        return ValueTask.CompletedTask;
    }
}

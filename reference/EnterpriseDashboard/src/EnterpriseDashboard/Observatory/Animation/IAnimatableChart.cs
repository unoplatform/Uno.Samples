namespace EnterpriseDashboard.Observatory.Animation;

public interface IAnimatableChart
{
    void Play();
    void Reset();
    bool IsPlaying { get; }
}

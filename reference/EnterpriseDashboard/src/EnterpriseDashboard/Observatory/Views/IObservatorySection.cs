namespace EnterpriseDashboard.Observatory.Views;

// Implemented by each Observatory section page so the shell's persistent top bar
// (refresh / date-range) can replay the currently-shown page's charts.
public interface IObservatorySection
{
    void Refresh();
}

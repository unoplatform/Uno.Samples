
namespace TimeEntryRia.Web
{
    public partial class TimeEntry
    {
        // A fix for a bug in RIA Services that marks TimeStamp fields as required
        partial void OnCreated()
        {
            _lastUpdated = new byte[] { 0 };
        }
    }
}

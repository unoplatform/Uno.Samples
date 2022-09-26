namespace TimeEntryRia.Web
{
    public partial class Project
    {
        // A fix for a bug in RIA Services that marks TimeStamp fields as required
        partial void OnCreated()
        {
            _lastUpdated = new byte[] { 0 };
        }
    }
}
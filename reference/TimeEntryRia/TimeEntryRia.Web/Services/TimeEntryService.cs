
namespace TimeEntryRia.Web.Services
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Data;
    using System.Linq;
    using System.ServiceModel.DomainServices.EntityFramework;
    using System.ServiceModel.DomainServices.Hosting;
    using System.ServiceModel.DomainServices.Server;
    using TimeEntryRia.Web;


    // Implements application logic using the TimeEntryEntities context.
    // TODO: Add your application logic to these methods or in additional methods.
    // TODO: Wire up authentication (Windows/ASP.NET Forms) and uncomment the following to disable anonymous access
    // Also consider adding roles to restrict access as appropriate.
    // [RequiresAuthentication]
    [EnableClientAccess()]
    [RequiresAuthentication]
    public class TimeEntryService : LinqToEntitiesDomainService<TimeEntryEntities>
    {

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'TimeEntries' query.
        public IQueryable<TimeEntry> GetTimeEntries()
        {
            return this.ObjectContext.TimeEntries;
        }

        public void InsertTimeEntry(TimeEntry timeEntry)
        {
            if ((timeEntry.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(timeEntry, EntityState.Added);
            }
            else
            {
                this.ObjectContext.TimeEntries.AddObject(timeEntry);
            }
        }

        public void UpdateTimeEntry(TimeEntry currentTimeEntry)
        {
            this.ObjectContext.TimeEntries.AttachAsModified(currentTimeEntry, this.ChangeSet.GetOriginal(currentTimeEntry));
        }

        public void DeleteTimeEntry(TimeEntry timeEntry)
        {
            if ((timeEntry.EntityState == EntityState.Detached))
            {
                this.ObjectContext.TimeEntries.Attach(timeEntry);
            }
            this.ObjectContext.TimeEntries.DeleteObject(timeEntry);
        }
    }
}



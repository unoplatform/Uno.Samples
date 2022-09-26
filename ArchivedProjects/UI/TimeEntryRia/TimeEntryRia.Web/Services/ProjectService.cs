
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
    public class ProjectService : LinqToEntitiesDomainService<TimeEntryEntities>
    {

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'Projects' query.
        public IQueryable<Project> GetProjects()
        {
            return this.ObjectContext.Projects;
        }

        public Project GetProject(int projectId)
        {
            return this.ObjectContext.Projects.FirstOrDefault(p => p.Id == projectId);
        }

        public void InsertProject(Project project)
        {
            if ((project.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(project, EntityState.Added);
            }
            else
            {
                this.ObjectContext.Projects.AddObject(project);
            }
        }

        public void UpdateProject(Project currentProject)
        {
            this.ObjectContext.Projects.AttachAsModified(currentProject, this.ChangeSet.GetOriginal(currentProject));
        }

        public void DeleteProject(Project project)
        {
            if ((project.EntityState == EntityState.Detached))
            {
                this.ObjectContext.Projects.Attach(project);
            }
            this.ObjectContext.Projects.DeleteObject(project);
        }
    }
}



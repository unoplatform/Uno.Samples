using System;
using System.Collections.ObjectModel;
using System.Net;
using System.ServiceModel.DomainServices.Client;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using TimeEntryRia.Models;
using TimeEntryRia.MVVM;
using TimeEntryRia.Web;
using TimeEntryRia.Web.Services;

namespace TimeEntryRia.ViewModels
{
    public class AddNewTimeEntryViewModel : ViewModelBase
    {
        private TimeEntry _newTimeEntry;
        public TimeEntry NewTimeEntry
        {
            get
            {
                return _newTimeEntry;
            }
            set
            {
                SetPropertyValue(ref _newTimeEntry, value, () => NewTimeEntry);
            }
        }

        private ObservableCollection<NameValuePair<int>> _projects;
        public ObservableCollection<NameValuePair<int>> Projects
        {
            get
            {
                return _projects;
            }
            set
            {
                SetPropertyValue(ref _projects, value, () => Projects);
            }
        }

        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }

        public event EventHandler CloseView;

        public AddNewTimeEntryViewModel()
        {
            LoadProjects();

            _newTimeEntry = new TimeEntry()
            {
                UserId = WebContext.Current.User.Id
            };
            _newTimeEntry.PropertyChanged += (s, e) => SaveCommand.CanExecute(null);

            SaveCommand = new DelegateCommand(o => this.Save(), o => this.IsTimeEntryValid());
            CancelCommand = new DelegateCommand(o => this.Cancel(), o => true);
        }

        private void LoadProjects()
        {
            var projectContext = new ProjectContext();
            var allProjectsQry = projectContext.GetProjectsQuery();
            var loadOperation = projectContext.Load(allProjectsQry, ProjectsLoaded, null);
        }

        private void ProjectsLoaded(System.ServiceModel.DomainServices.Client.LoadOperation<Project> result)
        {
            if (result.HasError)
            {
                // Handle here...
            }
            else
            {
                Projects = new ObservableCollection<NameValuePair<int>>();
                foreach (var item in result.Entities)
                {
                    Projects.Add(new NameValuePair<int>(item.Name, item.Id));
                }
            }
        }

        private bool IsTimeEntryValid()
        {
            return !this.NewTimeEntry.HasValidationErrors;
        }


        // Navigation methods

        private void Save()
        {
            var timeEntryContext = new TimeEntryContext();
            timeEntryContext.TimeEntries.Add(NewTimeEntry);
            timeEntryContext.SubmitChanges(ChangesSubmitted, null);
        }

        private void ChangesSubmitted(System.ServiceModel.DomainServices.Client.SubmitOperation result)
        {
            if (result.HasError)
            {
                // Handle here...
                var error = result.Error as DomainOperationException;
                switch (error.Status)
                {
                    case OperationErrorStatus.ValidationFailed:
                        result.MarkErrorAsHandled();
                        return;
                    default:
                        break;
                }
            }
            else
            {
                if (CloseView != null)
                {
                    CloseView(this, EventArgs.Empty);
                }
            }
        }

        private void Cancel()
        {
            if (CloseView != null)
            {
                CloseView(this, EventArgs.Empty);
            }
        }


    }
}

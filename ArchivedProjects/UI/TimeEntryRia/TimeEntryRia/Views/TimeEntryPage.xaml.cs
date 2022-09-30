using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Navigation;
using TimeEntryRia.Web;
using TimeEntryRia.Services;
using TimeEntryRia.Helpers;

namespace TimeEntryRia.Views
{
    // An example of coding in the code-behind vs ViewModel
    public partial class TimeEntryPage : Page
    {
        TimesheetSummaryServiceClient _service;
        DateTime? _lastSelectedDate;
        const string SelectedDateKey = "SelectedDateKey";
        private TimeEntryRia.Controls.BusyIndicator _busyIndicator;

        public TimeEntryPage()
        {
            //this.NavigationCacheMode = System.Windows.Navigation.NavigationCacheMode.Required;
            InitializeComponent();
            this.Title = ApplicationStrings.TimeEntryPageTitle;

            _service = new TimesheetSummaryServiceClient();
            _service.GetWeekSummaryCompleted += new EventHandler<GetWeekSummaryCompletedEventArgs>(service_GetWeekSummaryCompleted);
            _busyIndicator = Application.Current.RootVisual as TimeEntryRia.Controls.BusyIndicator;
        }

        void service_GetWeekSummaryCompleted(object sender, GetWeekSummaryCompletedEventArgs e)
        {
            try
            {
                if (!e.Cancelled && e.Error == null)
                {
                    WeekSummaryDataGrid.ItemsSource = e.Result;
                }
                else
                {
                    _busyIndicator.IsBusy = false;
                    MessageBox.Show(e.Error.Message);
                }
            }
            finally
            {
                _busyIndicator.IsBusy = false;
            }
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!WebContext.Current.Authentication.User.Identity.IsAuthenticated)
            {
                return;
            }

            if (_lastSelectedDate.HasValue)
            {
                RetrieveSummaryData(_lastSelectedDate.Value);
            }
            else
            {
                weekStartingDatePicker.SelectedDate = DateTime.Now;
            }
        }

        private void weekStartingDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is DateTime)
            {
                var selectedDate = (DateTime)e.AddedItems[0];
                if (_lastSelectedDate == selectedDate)
                {
                    return;
                }

                if (selectedDate.DayOfWeek != DayOfWeek.Monday)
                {
                    var dayOffset = DayOfWeek.Monday - selectedDate.DayOfWeek;
                    selectedDate = selectedDate + TimeSpan.FromDays(dayOffset);
                    weekStartingDatePicker.SelectedDate = selectedDate;
                }

                _lastSelectedDate = selectedDate;
                SettingsHelper.SaveSetting(SelectedDateKey, selectedDate);
                RetrieveSummaryData(selectedDate);
            }
        }

        private void RetrieveSummaryData(DateTime date)
        {
            _busyIndicator.IsBusy = true;
            WeekOfLabel.Text = string.Format("Week {0} of {1}",
                TimeEntry.GetIso8601WeekOfYear(date), date.Year);

            var user = WebContext.Current.User;
            _service.GetWeekSummaryAsync(user.Id, date);
        }

        private void DecreaseWeek_Click(object sender, RoutedEventArgs e)
        {
            weekStartingDatePicker.SelectedDate = weekStartingDatePicker.SelectedDate - TimeSpan.FromDays(7);
        }

        private void IncreaseWeek_Click(object sender, RoutedEventArgs e)
        {
            weekStartingDatePicker.SelectedDate = weekStartingDatePicker.SelectedDate + TimeSpan.FromDays(7);
        }

        private void AddTimeEntryButton_Click(object sender, RoutedEventArgs e)
        {
            var encodeDate =  Uri.EscapeDataString(_lastSelectedDate.Value.ToShortDateString());
            this.NavigationService.Navigate(new Uri("/AddTimeEntry/" + encodeDate, UriKind.Relative));
        }
    }
}

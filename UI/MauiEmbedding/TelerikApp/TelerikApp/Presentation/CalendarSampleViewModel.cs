using System.Collections.ObjectModel;

namespace TelerikApp.Presentation;

internal partial class CalendarSampleViewModel : ObservableObject
{
    private Random rnd;
    private List<string> titles;

    public CalendarSampleViewModel()
    {
        FilteredRemindersList = new ObservableCollection<Reminder>();
        titles = new List<string>()
        {
            "MAUI Dev Team Sync",
            ".NET MAUI Weekly Planning",
            "Unit Product Visibility",
            "Knowledge Sharing Session",
            "Reporting Product Team Sync",
            "UX Growth Series n",
            "DevTools Monthly Series",
            "MAUI Product Team Sync",
            "AllSpark Results Discussion",
            "Design Team Retrospective"
        };
        rnd = new Random();

        SelectedDate = DateTime.Today;
    }

    [ObservableProperty]
    private DateTime selectedDate;

    [ObservableProperty]
    private bool isEmptyMessageVisible;

    public ObservableCollection<Reminder> FilteredRemindersList { get; }

    partial void OnSelectedDateChanged(DateTime value)
    {
        GenerateReminders();
    }

    public void GenerateReminders()
    {
        var dayOfWeek = SelectedDate.DayOfWeek;
        if (dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday)
        {
            if (FilteredRemindersList.Count > 0)
            {
                FilteredRemindersList.Clear();
            }

            IsEmptyMessageVisible = true;

            return;
        }

        var count = rnd.Next(1, titles.Count - 1);

        var copyTitles = new List<string>(this.titles);
        var reminders = new List<Reminder>();
        for (int i = 0; i < count; i++)
        {
            var index = rnd.Next(0, copyTitles.Count - 1);
            var title = copyTitles[index];

            var reminder = new Reminder() { Date = SelectedDate, Title = title };
            reminders.Add(reminder);
            copyTitles.RemoveAt(index);
        }

        FilteredRemindersList.ReplaceWith(reminders);
        IsEmptyMessageVisible = false;
    }
}

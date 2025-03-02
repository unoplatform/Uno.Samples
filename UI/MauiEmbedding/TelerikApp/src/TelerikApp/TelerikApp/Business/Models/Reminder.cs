namespace TelerikApp.Business.Models;

public partial class Reminder : ObservableObject
{
    [ObservableProperty]
    private DateTime date;

    [ObservableProperty]
    private string? title;
}

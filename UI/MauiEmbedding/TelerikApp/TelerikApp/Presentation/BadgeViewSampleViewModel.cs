using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Maui.Controls.Compatibility.Primitives;

namespace TelerikApp.Presentation;

internal class BadgeViewSampleViewModel : ObservableObject
{
    public BadgeViewSampleViewModel()
    {
        Users = new ObservableCollection<User>
        {
            new User() { Name = "Abbie Hunter", LastMessageReceived = "Thanks", ImageSourcePath = "person_1.png", ActivityStatus = BadgeType.DoNotDisturb, LastMessageReceivedDate = "Yesterday" },
            new User() { Name = "Archie Wilson", LastMessageReceived = "See you tomorrow", ImageSourcePath = "person_2.png", ActivityStatus = BadgeType.Available, UnreadMessagesText = "1", LastMessageReceivedDate = "10:20 AM" },
            new User() { Name = "Blake Richardson", LastMessageReceived = "File uploaded", ImageSourcePath = "person_3.png", ActivityStatus = BadgeType.Away, LastMessageReceivedDate = "09:42 AM" },
            new User() { Name = "Bree Conner", LastMessageReceived = "ok", ImageSourcePath = "person_4.png", ActivityStatus = BadgeType.OutOfOffice, LastMessageReceivedDate = "Yesterday" },
            new User() { Name = "Cody Fleming", LastMessageReceived = "Please see issue #228", ImageSourcePath = "person_5.png", ActivityStatus = BadgeType.Available, UnreadMessagesText = "22", LastMessageReceivedDate = "2:30 PM" },
            new User() { Name = "Dallas Ruiz", LastMessageReceived = "Thanks", ImageSourcePath = "person_6.png", ActivityStatus = BadgeType.Add, LastMessageReceivedDate = "Wednesday" },
            new User() { Name = "Lola Hall", LastMessageReceived = "Need help with a ticket", ImageSourcePath = "person_7.png", ActivityStatus = BadgeType.Remove, LastMessageReceivedDate = "09:36 AM" },
            new User() { Name = "Madeleine Haynes", LastMessageReceived = "What do you think about the new des...", ImageSourcePath = "person_8.png", ActivityStatus = BadgeType.Rejected, LastMessageReceivedDate = "10:39 AM" },
            new User() { Name = "Poppy Mills", LastMessageReceived = "George will do it", ImageSourcePath = "person_9.png", ActivityStatus = BadgeType.Offline, LastMessageReceivedDate = "Saturday" },
        };
    }

    public ObservableCollection<User> Users { get; }
}

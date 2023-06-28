using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessagingPeopleApp
{
    public record User(string Name);
    public record ChatMessage(DateTime Sent, User Sender, User Recipient, string content);
}

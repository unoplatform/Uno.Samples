using System;

namespace UnoChat.Models;

public class Sender
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public Guid DeviceTypeId { get; set; }

    public bool IsMe { get; set; }
}

public class Model
{
    public Sender Sender { get; set; }

    public string Text { get; set; }

    public DateTimeOffset SentAt { get; set; }

    public DateTimeOffset RelayedAt { get; set; }

    public DateTimeOffset ReceivedAt { get; set; }
}

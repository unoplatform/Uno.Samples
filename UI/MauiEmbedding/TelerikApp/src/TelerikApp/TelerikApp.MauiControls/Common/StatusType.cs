namespace TelerikApp.MauiControls.Common;

public enum StatusType
{
    Normal,
    New,
    Beta,
    Updated
}

public class Example
{
    public string Name { get; set; }

    public string DisplayName { get; set; }

    public string Icon { get; set; }

    public string CodeUrl { get; set; }

    public string Page { get; set; }

    public string Description { get; set; }

    public string ExcludeFrom { get; set; }

    public string ControlName { get; set; }

    public bool IsConfigurable { get; set; }

    public StatusType Status { get; set; }
}

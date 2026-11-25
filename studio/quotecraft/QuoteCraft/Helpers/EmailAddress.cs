namespace QuoteCraft.Helpers;

/// <summary>The email check the onboarding form and the client dialog share.</summary>
public static class EmailAddress
{
    /// <summary>True for an empty field (the address is optional) or an address with an @ and a dot.</summary>
    public static bool IsValidOrEmpty(string? email) =>
        string.IsNullOrWhiteSpace(email) || (email.Contains('@') && email.Contains('.'));
}

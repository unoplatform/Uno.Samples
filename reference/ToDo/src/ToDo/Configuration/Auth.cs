namespace ToDo.Configuration;

// <see href="https://platform.uno/docs/articles/external/uno.extensions/doc/Learn/Serialization/HowTo-Serialization.html" />
public partial record Auth
{
    public string? ApplicationId { get; init; }
    public string[]? Scopes { get; init; }
    public string? RedirectUri { get; init; }
    public string? KeychainSecurityGroup { get; init; }
}

[JsonSerializable(typeof(Auth))]
internal partial class AuthJsonContext : JsonSerializerContext
{ }

using System.Text.Json.Serialization;
using Refit;

namespace Authentication.OidcExtensionsDemo.Services.Endpoints;

/// <summary>
/// The Duende demo server's protected API. <c>GET /api/test</c> echoes the claims of the access
/// token it receives, which makes it a convenient way to prove the token was sent and accepted.
/// </summary>
/// <remarks>
/// Registered with <c>services.AddRefitClient&lt;IDemoApi&gt;(context)</c> inside <c>UseHttp</c>
/// in <c>App.xaml.cs</c>. That registration is what chains the authentication handler into the
/// client: the access token in <c>ITokenCache</c> is added as an <c>Authorization: Bearer</c>
/// header on every call, and a 401 triggers a token refresh and one retry. The base address
/// comes from the <c>DemoApi</c> section of appsettings (the interface name without its leading
/// <c>I</c>).
/// </remarks>
[Headers("Accept: application/json")]
public interface IDemoApi
{
    [Get("/api/test")]
    Task<DemoClaim[]> GetTestAsync(CancellationToken ct);
}

/// <summary>One claim as the demo API returns it.</summary>
public sealed record DemoClaim(
    [property: JsonPropertyName("type")] string? Type,
    [property: JsonPropertyName("value")] string? Value);

/// <summary>
/// Source-generated serializer metadata for the API's payload, so deserialization is
/// trim- and AOT-safe on the WebAssembly and mobile heads.
/// </summary>
[JsonSerializable(typeof(DemoClaim[]))]
[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
public partial class DemoApiJsonContext : JsonSerializerContext
{
}

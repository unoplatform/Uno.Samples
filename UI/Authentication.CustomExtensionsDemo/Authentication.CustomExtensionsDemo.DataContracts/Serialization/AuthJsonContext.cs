using System.Text.Json.Serialization;

namespace Authentication.CustomExtensionsDemo.DataContracts.Serialization;

/// <summary>
/// Source-generated serializer metadata for the server's payloads, so its minimal APIs serialize
/// without reflection.
/// </summary>
[JsonSerializable(typeof(LoginRequest))]
[JsonSerializable(typeof(RefreshRequest))]
[JsonSerializable(typeof(TokenResponse))]
[JsonSerializable(typeof(Profile))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public partial class AuthJsonContext : JsonSerializerContext
{
}

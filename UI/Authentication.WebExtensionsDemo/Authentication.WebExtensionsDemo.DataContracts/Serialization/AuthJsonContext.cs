using System.Text.Json.Serialization;

namespace Authentication.WebExtensionsDemo.DataContracts.Serialization;

/// <summary>
/// Source-generated serializer metadata for the auth server's payloads, shared by the server and
/// the app so both sides agree on the JSON and deserialization stays trim-safe on WebAssembly
/// and mobile.
/// </summary>
[JsonSerializable(typeof(RefreshRequest))]
[JsonSerializable(typeof(TokenResponse))]
[JsonSerializable(typeof(Profile))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public partial class AuthJsonContext : JsonSerializerContext
{
}

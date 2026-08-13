using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Authentication.MsalDemo.Services;

/// <summary>Outcome of a Microsoft Graph call.</summary>
internal sealed record GraphResult(bool IsSuccess, int StatusCode, string Body, string? DisplayName, string? UserPrincipalName, string? Mail, string? JobTitle);

/// <summary>
/// Calls Microsoft Graph with the access token MSAL produced, to prove the token actually works
/// against a real API.
/// </summary>
/// <remarks>
/// Deliberately a plain <see cref="HttpClient"/> rather than the Graph SDK: it keeps the
/// dependency list to MSAL only, and it shows the one thing that matters - putting the access
/// token in an <c>Authorization: Bearer</c> header.
/// </remarks>
internal static class GraphClient
{
    private const string MeEndpoint = "https://graph.microsoft.com/v1.0/me";

    private static readonly HttpClient Client = new();

    public static async Task<GraphResult> GetMeAsync(string accessToken, CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, MeEndpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using var response = await Client.SendAsync(request, ct);
        var body = await response.Content.ReadAsStringAsync(ct);

        var status = (int)response.StatusCode;

        if (!response.IsSuccessStatusCode)
        {
            return new GraphResult(false, status, Prettify(body), null, null, null, null);
        }

        string? displayName = null, upn = null, mail = null, jobTitle = null;

        try
        {
            using var document = JsonDocument.Parse(body);
            var root = document.RootElement;
            displayName = ReadString(root, "displayName");
            upn = ReadString(root, "userPrincipalName");
            mail = ReadString(root, "mail");
            jobTitle = ReadString(root, "jobTitle");
        }
        catch (JsonException)
        {
            // Fall through - the raw body is still shown.
        }

        return new GraphResult(true, status, Prettify(body), displayName, upn, mail, jobTitle);
    }

    private static string? ReadString(JsonElement element, string property) =>
        element.TryGetProperty(property, out var value) && value.ValueKind == JsonValueKind.String
            ? value.GetString()
            : null;

    /// <summary>
    /// Re-indents the response for display. Uses <see cref="Utf8JsonWriter"/> rather than
    /// <c>JsonSerializer</c> so no reflection is involved and the code stays trim- and AOT-safe,
    /// which matters for the WebAssembly and mobile heads.
    /// </summary>
    private static string Prettify(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return "(empty response)";
        }

        try
        {
            using var document = JsonDocument.Parse(json);
            using var buffer = new MemoryStream();

            using (var writer = new Utf8JsonWriter(buffer, new JsonWriterOptions { Indented = true }))
            {
                document.WriteTo(writer);
            }

            return Encoding.UTF8.GetString(buffer.ToArray());
        }
        catch (JsonException)
        {
            return json;
        }
    }
}

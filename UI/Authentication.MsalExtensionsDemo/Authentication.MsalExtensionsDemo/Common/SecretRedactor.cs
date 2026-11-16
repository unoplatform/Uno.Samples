using System.Text.Json;
using System.Text.RegularExpressions;
using Windows.Storage;

namespace Authentication.MsalExtensionsDemo.Common;

/// <summary>
/// Demo aid: hides the values that should not end up in a screen recording - the client and
/// tenant IDs, the signed-in account, anything token-shaped - without changing what the app
/// actually does.
/// </summary>
/// <remarks>
/// <para>
/// Redaction happens at display time, not at capture time: the view models keep the real values
/// and pass them through <see cref="Apply"/> on the way to the UI, so the switch takes effect
/// immediately in both directions, including on flow-log entries that were written before it was
/// flipped.
/// </para>
/// <para>
/// Two layers, because a demo is a bad place to discover a gap. Values the app knows are secret
/// (<see cref="Remember"/>) are replaced wherever they appear, even mid-string - which is what
/// catches the client ID inside the Android redirect URI. Everything else goes through the
/// pattern sweep below, so a token, GUID or address that reaches the screen by a path nobody
/// thought about is still covered.
/// </para>
/// <para>
/// What it cannot cover: the sign-in UI itself. That runs in the system browser or an
/// ASWebAuthenticationSession, outside the app, so the account picker shows real addresses. Use a
/// throwaway account for the recording, or cut that part.
/// </para>
/// </remarks>
public sealed partial class SecretRedactor : ObservableObject
{
    private const string SettingKey = "Demo.RedactSecrets";

    /// <summary>Registered secrets, longest first so an overlapping value cannot be half-replaced.</summary>
    private readonly List<KeyValuePair<string, string>> _known = [];

    private bool _isEnabled;

    public SecretRedactor()
    {
        _isEnabled = LoadPersisted();
    }

    /// <summary>Raised whenever redaction is turned on or off, so views can re-render.</summary>
    public event EventHandler? Changed;

    /// <summary>Whether values are being hidden right now.</summary>
    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            if (_isEnabled == value)
            {
                return;
            }

            _isEnabled = value;
            Persist(value);

            Raise();
            Raise(nameof(ToggleLabel));
            Changed?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>Caption on the header switch, so the current mode is readable on camera.</summary>
    public string ToggleLabel => _isEnabled ? "Redacting" : "Redact";

    /// <summary>
    /// Registers a value to hide wherever it appears. Ignores anything too short or too common to
    /// replace safely (an empty client ID, a tenant of "consumers", a display name of "Me").
    /// </summary>
    /// <param name="value">The secret, or null.</param>
    /// <param name="label">What to show instead, for example "client ID".</param>
    public void Remember(string? value, string label)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length < 6)
        {
            return;
        }

        if (_known.Any(pair => string.Equals(pair.Key, value, StringComparison.OrdinalIgnoreCase)))
        {
            return;
        }

        _known.Add(new(value, $"[{label} hidden]"));
        _known.Sort(static (left, right) => right.Key.Length.CompareTo(left.Key.Length));
    }

    /// <summary>
    /// The display form of <paramref name="text"/>: unchanged when redaction is off, scrubbed
    /// when it is on.
    /// </summary>
    public string? Apply(string? text)
    {
        if (!_isEnabled || string.IsNullOrEmpty(text))
        {
            return text;
        }

        foreach (var (secret, mask) in _known)
        {
            text = text.Replace(secret, mask, StringComparison.OrdinalIgnoreCase);
        }

        // Order matters: a JWT contains base64url runs, and an address contains no GUID, so the
        // most specific pattern has to claim its text first.
        text = JwtPattern().Replace(text, "[token hidden]");
        text = GuidPattern().Replace(text, "[id hidden]");
        text = EmailPattern().Replace(text, "[account hidden]");
        text = OpaqueRunPattern().Replace(text, "[secret hidden]");

        return text;
    }

    /// <summary>
    /// The display form of a JSON document: every string value is replaced, so the response keeps
    /// the shape that proves the call worked while giving nothing away. Keys are left alone -
    /// they are Microsoft Graph's, not the user's.
    /// </summary>
    public string ApplyToJson(string json)
    {
        if (!_isEnabled || string.IsNullOrWhiteSpace(json))
        {
            return json;
        }

        try
        {
            using var document = JsonDocument.Parse(json);
            using var buffer = new MemoryStream();

            using (var writer = new Utf8JsonWriter(buffer, new JsonWriterOptions { Indented = true }))
            {
                WriteRedacted(document.RootElement, writer);
            }

            return System.Text.Encoding.UTF8.GetString(buffer.ToArray());
        }
        catch (JsonException)
        {
            // Not JSON (an error body, an exception message): the text sweep still applies.
            return Apply(json) ?? string.Empty;
        }
    }

    private void WriteRedacted(JsonElement element, Utf8JsonWriter writer)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                writer.WriteStartObject();
                foreach (var property in element.EnumerateObject())
                {
                    writer.WritePropertyName(property.Name);
                    WriteRedacted(property.Value, writer);
                }
                writer.WriteEndObject();
                break;

            case JsonValueKind.Array:
                writer.WriteStartArray();
                foreach (var item in element.EnumerateArray())
                {
                    WriteRedacted(item, writer);
                }
                writer.WriteEndArray();
                break;

            case JsonValueKind.String:
                // The context URL is Graph's own endpoint, so it is worth keeping visible.
                writer.WriteStringValue(
                    element.GetString() is { } value && value.StartsWith("https://graph.microsoft.com", StringComparison.OrdinalIgnoreCase)
                        ? value
                        : "[hidden]");
                break;

            default:
                element.WriteTo(writer);
                break;
        }
    }

    private static bool LoadPersisted()
    {
        try
        {
            return ApplicationData.Current.LocalSettings.Values[SettingKey] is bool stored && stored;
        }
        catch (Exception)
        {
            // No local settings on this head (or none yet): default to showing real values.
            return false;
        }
    }

    private static void Persist(bool value)
    {
        try
        {
            ApplicationData.Current.LocalSettings.Values[SettingKey] = value;
        }
        catch (Exception)
        {
            // Not fatal - the switch just will not survive a restart on this head.
        }
    }

    // Source-generated so no regex is compiled at runtime, which keeps the WebAssembly and mobile
    // heads trim- and AOT-safe.
    [GeneratedRegex(@"eyJ[A-Za-z0-9_-]{6,}\.[A-Za-z0-9_-]+(?:\.[A-Za-z0-9_-]+)?")]
    private static partial Regex JwtPattern();

    [GeneratedRegex(@"\b[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}\b")]
    private static partial Regex GuidPattern();

    [GeneratedRegex(@"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}\b")]
    private static partial Regex EmailPattern();

    // A long unbroken run of token characters is never prose, so whatever produced it is a secret
    // this class has not been told about.
    [GeneratedRegex(@"\b[A-Za-z0-9_-]{40,}\b")]
    private static partial Regex OpaqueRunPattern();
}
